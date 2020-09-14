using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AlbumDeFotos.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace AlbumDeFotos.Controllers
{
    public class ImagensController : Controller
    {
        private readonly Contexto _context;
        private readonly IWebHostEnvironment _environment;

        public ImagensController(Contexto context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Imagens/Create
        public IActionResult Create(int id)
        {
            ViewBag.Destinos = _context.Albums.FirstOrDefault(x => x.AlbumId == id);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ImagemId,Link,AlbumId")] Imagem imagem, IFormFile arquivo)
        {
            if (ModelState.IsValid)
            {
                var linkUpload = Path.Combine(_environment.WebRootPath, "Imagens");

                if (arquivo != null)
                {
                    //Salva imagem
                    using (var fileStream = new FileStream(Path.Combine(linkUpload, arquivo.FileName), FileMode.Create))
                    {
                        await arquivo.CopyToAsync(fileStream);
                        imagem.Link = "~/Imagens/" + arquivo.FileName;
                    }
                }

                _context.Add(imagem);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Albums", new { id = imagem.AlbumId });
            }
            ViewData["AlbumId"] = new SelectList(_context.Albums, "AlbumId", "Destino", imagem.AlbumId);
            return View(imagem);
        }

    }
}
