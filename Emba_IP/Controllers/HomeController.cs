using System.Diagnostics;
using Emba_IP.Models;
using Emba_IP.Services;
using Emba_IP.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Emba_IP.Controllers
{
    public class HomeController : Controller
    {
        private readonly IpFileService _service;

        public HomeController(IpFileService service)
        {
            _service = service;
        }

        public IActionResult Index(string? searchTerm, int page = 1, int pageSize = 20)
        {
            var trimmedSearchTerm = string.IsNullOrWhiteSpace(searchTerm) ? "" : searchTerm.Trim();

            var allIps = _service.GetAll()
               .Where(ip => string.IsNullOrWhiteSpace(trimmedSearchTerm) ||
                            ip.IpAddress.Replace(" ", "").Contains(trimmedSearchTerm.Replace(" ", ""), StringComparison.OrdinalIgnoreCase))
               .ToList();
            var totalCount = allIps.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            // Səhifələmə
            var pagedIps = allIps
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // ViewModel-i hazırla
            var model = new IpListViewModel
            {
                SearchTerm = searchTerm,
                IpList = pagedIps,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(allIps.Count / (double)pageSize),
                PageSize = pageSize,
                PageSizeOptions = new List<int> { 20, 50, 100 }
            };

            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(string ipAddress)
        {
            if (!string.IsNullOrWhiteSpace(ipAddress))
            {
                _service.Add(ipAddress.Trim());
            }
            return RedirectToAction("Index");
        }

        public IActionResult Edit(string ip)
        {
            if (string.IsNullOrEmpty(ip))
                return NotFound();

            var item = _service.GetAll().FirstOrDefault(x => x.IpAddress == ip);
            if (item == null)
                return NotFound();

            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(string oldIp, string newIp)
        {
            if (string.IsNullOrWhiteSpace(oldIp) || string.IsNullOrWhiteSpace(newIp))
                return BadRequest();

            _service.Update(oldIp.Trim(), newIp.Trim());
            return RedirectToAction("Index");
        }

        public IActionResult Delete(string ip)
        {
            if (string.IsNullOrEmpty(ip))
                return NotFound();

            var item = _service.GetAll().FirstOrDefault(x => x.IpAddress == ip);
            if (item == null)
            {
                return View("Delete");
            }

            return View(item);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(string ip)
        {
            if (string.IsNullOrEmpty(ip))
                return NotFound();

            bool deleted = _service.Delete(ip.Trim());

            if (!deleted)
            {
                // IP tapılmadı, istifadəçiyə xəbər ver
                ModelState.AddModelError("", "Bu IP artıq mövcud deyil.");
                var item = _service.GetAll().FirstOrDefault(x => x.IpAddress == ip);
                return View("Delete", item);
            }

            return RedirectToAction("Index");
        }
    }
}
