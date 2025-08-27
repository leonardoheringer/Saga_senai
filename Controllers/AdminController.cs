using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPetrobras.Data;
using SistemaPetrobras.Models;
using System.Globalization;
using System.Text;
using CsvHelper;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace SistemaPetrobras.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ICompositeViewEngine _viewEngine;

        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager, ICompositeViewEngine viewEngine)
        {
            _context = context;
            _userManager = userManager;
            _viewEngine = viewEngine;
        }

        // Método auxiliar para renderizar partial views como string
        private async Task<string> RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.ActionDescriptor.ActionName;

            ViewData.Model = model;

            using var writer = new StringWriter();
            var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);

            if (!viewResult.Success)
            {
                return $"Erro: View '{viewName}' não encontrada.";
            }

            var viewContext = new ViewContext(
                ControllerContext,
                viewResult.View,
                ViewData,
                TempData,
                writer,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);
            return writer.GetStringBuilder().ToString();
        }

        public async Task<IActionResult> Dashboard(string? regiao, string? escolaridade, DateTime? dataInicio, DateTime? dataFim)
        {
            var query = _context.Moradores.AsQueryable();

            if (!string.IsNullOrEmpty(regiao))
                query = query.Where(m => m.Regiao == regiao);

            if (!string.IsNullOrEmpty(escolaridade))
                query = query.Where(m => m.Escolaridade == escolaridade);

            if (dataInicio.HasValue)
                query = query.Where(m => m.DataCadastro >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(m => m.DataCadastro <= dataFim.Value);

            var moradores = await query.ToListAsync();

            // Métricas básicas
            ViewBag.TotalMoradores = moradores.Count;
            
            // Métricas adicionais
            ViewBag.FeedbacksPendentes = await _context.Feedbacks
                .Where(f => f.Status == StatusFeedback.Pendente)
                .CountAsync();
                
            ViewBag.AvisosAtivos = await _context.Avisos
                .Where(a => a.Ativo)
                .CountAsync();
                
            var dataLimite = DateTime.Now.AddDays(-30);
            ViewBag.NovosCadastros = await _context.Moradores
                .Where(m => m.DataCadastro >= dataLimite)
                .CountAsync();
            
            ViewBag.Regioes = await _context.Moradores.Select(m => m.Regiao).Distinct().ToListAsync();
            ViewBag.Escolaridades = await _context.Moradores.Select(m => m.Escolaridade).Distinct().ToListAsync();
            ViewBag.FiltroRegiao = regiao;
            ViewBag.FiltroEscolaridade = escolaridade;
            ViewBag.FiltroDataInicio = dataInicio?.ToString("yyyy-MM-dd");
            ViewBag.FiltroDataFim = dataFim?.ToString("yyyy-MM-dd");

            return View(moradores);
        }

        [HttpGet]
        public async Task<IActionResult> GetDadosGraficos(string? regiao, string? escolaridade, DateTime? dataInicio, DateTime? dataFim)
        {
            var query = _context.Moradores.AsQueryable();

            if (!string.IsNullOrEmpty(regiao))
                query = query.Where(m => m.Regiao == regiao);

            if (!string.IsNullOrEmpty(escolaridade))
                query = query.Where(m => m.Escolaridade == escolaridade);

            if (dataInicio.HasValue)
                query = query.Where(m => m.DataCadastro >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(m => m.DataCadastro <= dataFim.Value);

            var moradores = await query.ToListAsync();

            var dadosPorRegiao = moradores
                .GroupBy(m => m.Regiao)
                .Select(g => new { Regiao = g.Key, Quantidade = g.Count() })
                .ToList();

            var dadosPorEscolaridade = moradores
                .GroupBy(m => m.Escolaridade)
                .Select(g => new { Escolaridade = g.Key, Quantidade = g.Count() })
                .ToList();

            return Json(new { dadosPorRegiao, dadosPorEscolaridade });
        }

        public async Task<IActionResult> Feedbacks()
        {
            var feedbacks = await _context.Feedbacks
                .OrderByDescending(f => f.DataEnvio)
                .ToListAsync();

            return View(feedbacks);
        }

        [HttpPost]
        public async Task<IActionResult> AprovarFeedback(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback != null)
            {
                feedback.Status = StatusFeedback.Aprovado;
                feedback.AdminId = _userManager.GetUserId(User);
                feedback.DataResposta = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Feedbacks");
        }

        [HttpPost]
        public async Task<IActionResult> RejeitarFeedback(int id, string resposta)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback != null)
            {
                feedback.Status = StatusFeedback.Rejeitado;
                feedback.Resposta = resposta;
                feedback.AdminId = _userManager.GetUserId(User);
                feedback.DataResposta = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Feedbacks");
        }

        public async Task<IActionResult> ExportarCSV(string? regiao, string? escolaridade, DateTime? dataInicio, DateTime? dataFim)
        {
            var query = _context.Moradores.AsQueryable();

            if (!string.IsNullOrEmpty(regiao))
                query = query.Where(m => m.Regiao == regiao);

            if (!string.IsNullOrEmpty(escolaridade))
                query = query.Where(m => m.Escolaridade == escolaridade);

            if (dataInicio.HasValue)
                query = query.Where(m => m.DataCadastro >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(m => m.DataCadastro <= dataFim.Value);

            var moradores = await query.ToListAsync();

            using var writer = new StringWriter();
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            
            csv.WriteRecords(moradores);
            var csvContent = writer.ToString();

            return File(Encoding.UTF8.GetBytes(csvContent), "text/csv", "relatorio_moradores.csv");
        }

        public async Task<IActionResult> ExportarPDF(string? regiao, string? escolaridade, DateTime? dataInicio, DateTime? dataFim)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            
            var query = _context.Moradores.AsQueryable();

            if (!string.IsNullOrEmpty(regiao))
                query = query.Where(m => m.Regiao == regiao);

            if (!string.IsNullOrEmpty(escolaridade))
                query = query.Where(m => m.Escolaridade == escolaridade);

            if (dataInicio.HasValue)
                query = query.Where(m => m.DataCadastro >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(m => m.DataCadastro <= dataFim.Value);

            var moradores = await query.ToListAsync();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text("Relatório de Moradores - Sistema Petrobras")
                        .SemiBold().FontSize(16).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Spacing(20);

                            x.Item().Text($"Total de moradores: {moradores.Count}");
                            x.Item().Text($"Data do relatório: {DateTime.Now:dd/MM/yyyy HH:mm}");

                            x.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("Nome");
                                    header.Cell().Element(CellStyle).Text("CPF");
                                    header.Cell().Element(CellStyle).Text("Região");
                                    header.Cell().Element(CellStyle).Text("Escolaridade");
                                    header.Cell().Element(CellStyle).Text("Data Cadastro");

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                                    }
                                });

                                foreach (var morador in moradores)
                                {
                                    table.Cell().Element(CellStyle).Text(morador.Nome);
                                    table.Cell().Element(CellStyle).Text(morador.CPF);
                                    table.Cell().Element(CellStyle).Text(morador.Regiao);
                                    table.Cell().Element(CellStyle).Text(morador.Escolaridade);
                                    table.Cell().Element(CellStyle).Text(morador.DataCadastro.ToString("dd/MM/yyyy"));

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                                    }
                                }
                            });
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                        });
                });
            });

            var pdfBytes = document.GeneratePdf();
            return File(pdfBytes, "application/pdf", "relatorio_moradores.pdf");
        }

        // CRUD para Avisos
        public async Task<IActionResult> Avisos()
        {
            var avisos = await _context.Avisos.OrderByDescending(a => a.DataPublicacao).ToListAsync();
            return View(avisos);
        }

        public IActionResult CriarAviso()
        {
            try
            {
                // Se for AJAX, retornar PartialView (sem _Layout) para inserir no modal
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return PartialView("CriarAviso", new Aviso());

                return View(new Aviso());
            }
            catch (Exception ex)
            {
                // Log simples no console do servidor
                Console.Error.WriteLine(ex);

                // Se for AJAX, retornar o texto do erro para facilitar debug (temporário)
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return StatusCode(500, ex.ToString());

                TempData["Erro"] = "Erro ao abrir formulário de aviso: " + ex.Message;
                return RedirectToAction("Avisos");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarAviso(Aviso aviso)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    aviso.AutorId = _userManager.GetUserId(User);
                    aviso.DataPublicacao = DateTime.Now;
                    _context.Add(aviso);
                    await _context.SaveChangesAsync();

                    // ✅ CORREÇÃO: Verificar se é requisição AJAX
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { 
                            success = true, 
                            redirectUrl = Url.Action("Avisos"),
                            message = "Aviso criado com sucesso!"
                        });
                    }

                    TempData["Sucesso"] = "Aviso criado com sucesso!";
                    return RedirectToAction("Avisos");
                }

                // ✅ CORREÇÃO: Se há erros de validação e é AJAX
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var htmlContent = await RenderPartialViewToString("CriarAviso", aviso);
                    return Json(new { 
                        success = false, 
                        html = htmlContent,
                        message = "Por favor, corrija os erros no formulário."
                    });
                }

                return View(aviso);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro ao criar aviso: {ex}");

                // ✅ CORREÇÃO: Tratamento de erro para AJAX
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { 
                        success = false, 
                        message = "Erro interno do servidor. Tente novamente.",
                        error = ex.Message
                    });
                }

                TempData["Erro"] = "Erro ao criar aviso: " + ex.Message;
                return RedirectToAction("Avisos");
            }
        }

        public async Task<IActionResult> EditarAviso(int id)
        {
            var aviso = await _context.Avisos.FindAsync(id);
            if (aviso == null)
                return NotFound();

            return View(aviso);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarAviso(Aviso aviso)
        {
            if (ModelState.IsValid)
            {
                _context.Update(aviso);
                await _context.SaveChangesAsync();
                return RedirectToAction("Avisos");
            }
            return View(aviso);
        }

        [HttpPost]
        public async Task<IActionResult> ExcluirAviso(int id)
        {
            var aviso = await _context.Avisos.FindAsync(id);
            if (aviso != null)
            {
                _context.Avisos.Remove(aviso);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Avisos");
        }
    }
}

