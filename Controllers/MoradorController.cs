using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPetrobras.Data;
using SistemaPetrobras.Models;

namespace SistemaPetrobras.Controllers
{
    [Authorize(Roles = "Morador")]
    public class MoradorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<MoradorController> _logger;

        public MoradorController(ApplicationDbContext context, UserManager<IdentityUser> userManager, ILogger<MoradorController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var morador = await _context.Moradores.FirstOrDefaultAsync(m => m.UserId == userId);
            
            if (morador == null)
            {
                // ✅ Se não há registro de morador, vai para cadastro
                return RedirectToAction("Create");
            }
            
            // ✅ CORREÇÃO: Se morador existe, redireciona para o Perfil
            return RedirectToAction("Perfil");
        }

        // ✅ Novo método para acessar o perfil diretamente
        public async Task<IActionResult> Perfil()
        {
            var userId = _userManager.GetUserId(User);
            var morador = await _context.Moradores.FirstOrDefaultAsync(m => m.UserId == userId);
            
            if (morador == null)
            {
                return RedirectToAction("Create");
            }
            
            return View("Index", morador); // Usa a mesma view do Index original
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Morador morador)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                morador.UserId = userId;
                
                _logger.LogInformation($"Tentando adicionar morador: {morador.Nome}, CPF: {morador.CPF}, Email: {morador.Email}");
                try
                {
                    _context.Add(morador);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Morador adicionado com sucesso!");
                    
                    // ✅ CORREÇÃO: Após criar morador, redireciona para avisos
                    TempData["Sucesso"] = "Cadastro realizado com sucesso! Bem-vindo ao sistema.";
                    return RedirectToAction("Avisos");
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Erro ao salvar morador no banco de dados.");
                    ModelState.AddModelError("", "Erro ao salvar morador. Verifique se o CPF ou Email já estão cadastrados.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro inesperado ao criar morador.");
                    ModelState.AddModelError("", "Erro inesperado ao criar morador.");
                }
            }
            else
            {
                foreach (var modelStateEntry in ModelState.Values)
                {
                    foreach (var error in modelStateEntry.Errors)
                    {
                        _logger.LogError($"Erro de validação: {error.ErrorMessage}");
                    }
                }
            }
            return View(morador);
        }

        public async Task<IActionResult> Edit()
        {
            var userId = _userManager.GetUserId(User);
            var morador = await _context.Moradores.FirstOrDefaultAsync(m => m.UserId == userId);
            
            if (morador == null)
            {
                return NotFound();
            }
            
            return View(morador);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Morador morador)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                morador.UserId = userId;
                
                _context.Update(morador);
                await _context.SaveChangesAsync();
                
                // ✅ CORREÇÃO: Após editar, pode voltar para avisos ou perfil
                TempData["Sucesso"] = "Perfil atualizado com sucesso!";
                return RedirectToAction("Perfil");
            }
            return View(morador);
        }

        public async Task<IActionResult> Avisos()
        {
            // ✅ Verificar se o morador está cadastrado antes de mostrar avisos
            var userId = _userManager.GetUserId(User);
            var morador = await _context.Moradores.FirstOrDefaultAsync(m => m.UserId == userId);
            
            if (morador == null)
            {
                TempData["Info"] = "Complete seu cadastro para visualizar os avisos.";
                return RedirectToAction("Create");
            }
            
            var avisos = await _context.Avisos
                .Where(a => a.Ativo)
                .OrderByDescending(a => a.DataPublicacao)
                .ToListAsync();
            
            return View(avisos);
        }

        public IActionResult EnviarFeedback()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnviarFeedback(Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                feedback.MoradorId = userId;
                
                _context.Add(feedback);
                await _context.SaveChangesAsync();
                
                TempData["Sucesso"] = "Feedback enviado com sucesso!";
                return RedirectToAction("MeusFeedbacks");
            }
            return View(feedback);
        }

        public async Task<IActionResult> MeusFeedbacks()
        {
            var userId = _userManager.GetUserId(User);
            var feedbacks = await _context.Feedbacks
                .Where(f => f.MoradorId == userId)
                .OrderByDescending(f => f.DataEnvio)
                .ToListAsync();
            
            return View(feedbacks);
        }
    }
}

    