﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlbumDeFotos.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;

namespace AlbumDeFotos.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly Contexto _context;
        private readonly IWebHostEnvironment _environment;

        public AlbumsController(Contexto context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Albums
        public async Task<IActionResult> Index()
        {
            return View(await _context.Albums.ToListAsync());
        }

        // GET: Albums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Albums
                .FirstOrDefaultAsync(m => m.AlbumId == id);
            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // GET: Albums/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Albums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlbumId,Destino,FotoTopo,Inicio,Fim")] Album album, IFormFile arquivo)
        {
            if (ModelState.IsValid)
            {
                //Criar e Salvar imagem
                var linkUpload = Path.Combine(_environment.WebRootPath, "Imagens");

                if (arquivo != null)
                {
                    using var fileStream = new FileStream(Path.Combine(linkUpload, arquivo.FileName), FileMode.Create);
                    await arquivo.CopyToAsync(fileStream);
                    album.FotoTopo = "~/Imagens/" + arquivo.FileName;
                }

                _context.Add(album);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(album);
        }

        // GET: Albums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Albums.FindAsync(id);
            if (album == null)
            {
                return NotFound();
            }

            TempData["FotoTopo"] = album.FotoTopo;

            return View(album);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlbumId,Destino,FotoTopo,Inicio,Fim")] Album album, IFormFile arquivo)
        {
            if (id != album.AlbumId)
            {
                return NotFound();
            }

            //Se não tiver imagem, pega do TempData
            if (String.IsNullOrEmpty(album.FotoTopo))
                album.FotoTopo = TempData["FotoTopo"].ToString();

            if (ModelState.IsValid)
            {
                try
                {

                    //Upload da Imagem
                    var linkUpload = Path.Combine(_environment.WebRootPath, "Imagens");

                    if(arquivo != null)
                    {
                        using var fileStream = new FileStream(Path.Combine(linkUpload, arquivo.FileName), FileMode.Create);
                        await arquivo.CopyToAsync(fileStream);
                        album.FotoTopo = "~/Imagens/" + arquivo.FileName;
                    }

                    _context.Update(album);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumExists(album.AlbumId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(album);
        }


        // POST: Albums/Delete/5
        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            var album = await _context.Albums.FindAsync(id);

            //REMOVER IMAGENS AO EXCLUIR ALBUM

            //Guarda todos os links em uma variavel
            IEnumerable<string> links = _context.Imagens.Where(i => i.AlbumId == id).Select(i => i.Link);
            
            /*
             * Percorre todos os itens dos ALBUNS;
             * Altera o diretorio
             * Paga os arquivos
             */
            foreach(var item in links)
            {
                var linkImagem = item.Replace("~", "wwwroot");
                System.IO.File.Delete(linkImagem);
            }

            //Apagar os registros do banco de dados
            _context.Imagens.RemoveRange(_context.Imagens.Where(x => x.AlbumId == id));

            //Apagar imagem de topo do album
            var linkFotoAlbum = album.FotoTopo;
            linkFotoAlbum = linkFotoAlbum.Replace("~", "wwwroot");
            System.IO.File.Delete(linkFotoAlbum);

            //Salva as exclusões
            _context.Albums.Remove(album);
            await _context.SaveChangesAsync();
            return Json("Album excluido com sucesso!");
        }

        private bool AlbumExists(int id)
        {
            return _context.Albums.Any(e => e.AlbumId == id);
        }
    }
}
