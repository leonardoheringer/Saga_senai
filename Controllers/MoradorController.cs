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

        public MoradorController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var morador = await _context.Moradores.FirstOrDefaultAsync(m => m.UserId == userId);
            
            if (morador == null)
            {
                return RedirectToAction("Create");
            }
            
            return View(morador);
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
                
                _context.Add(morador);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
                return RedirectToAction(nameof(Index));
            }
            return View(morador);
        }

        public async Task<IActionResult> Avisos()
        {
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

