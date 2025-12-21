using EP._6._2A_Assignment.Factories;
using EP._6._2A_Assignment.Interfaces;
using EP._6._2A_Assignment.Models;
using EP._6._2A_Assignment.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using DataAccess.Context; // Добавлено для доступа к ApplicationDbContext

namespace EP._6._2A_Assignment.Controllers
{
    public class BulkImportController : Controller
    {
        private readonly ImportItemFactory _factory;
        private readonly ApplicationDbContext _context; // Добавлен контекст базы данных

        // Обновленный конструктор: теперь принимает и фабрику, и контекст
        public BulkImportController(ImportItemFactory factory, ApplicationDbContext context)
        {
            _factory = factory;
            _context = context;
        }

        // GET: show uploading page
        public IActionResult BulkImport()
        {
            return View();
        }

        // POST: Json processing
        [HttpPost]
        public IActionResult BulkImport(IFormFile file, string jsonInput, [FromServices] ItemsInMemoryRepository tempRepo)
        {
            string json = jsonInput;

            // If file not empty - read
            if (file != null)
            {
                using var reader = new StreamReader(file.OpenReadStream());
                json = reader.ReadToEnd();
            }

            // If file is empty - use appendix
            if (string.IsNullOrWhiteSpace(json))
            {
                json = @"[...]"; // Твой JSON (оставь как был)
            }

            // Convert Json in object list IItemValidating via fabric
            List<IItemValidating> items = _factory.Create(json);

            // Temporary storage saving 
            tempRepo.Save(items);

            // Transmit list in view for preview 
            return View("Preview", items);
        }

        // POST: Commit — saving from in-memory in DB
        [HttpPost]
        public IActionResult Commit(IFormFile zipFile, [FromServices] ItemsInMemoryRepository tempRepo, [FromServices] ItemsDbRepository dbRepo)
        {
            // 1. Getting earlier loaded elements
            var items = tempRepo.GetAll();

            // 2. Saving each element one by one
            dbRepo.Save(items);

            // Further BulkimageUploadPart
            if (zipFile != null && zipFile.Length > 0)
            {
                // Path for image storage
                var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                if (!Directory.Exists(imagesPath))
                    Directory.CreateDirectory(imagesPath);

                // Copy zip into memory
                using var zipStream = new MemoryStream();
                zipFile.CopyTo(zipStream);

                using var archive = new ZipArchive(zipStream);

                foreach (var entry in archive.Entries)
                {
                    // Skip folders
                    if (string.IsNullOrEmpty(entry.Name))
                        continue;

                    var filePath = Path.Combine(imagesPath, entry.Name);

                    using var entryStream = entry.Open();
                    using var fileStream = new FileStream(filePath, FileMode.Create);

                    entryStream.CopyTo(fileStream);
                }
            }

            // 3. Clearing storage
            tempRepo.Clear();

            return RedirectToAction("Index", "Catalog");
        }

        // NEW METHOD: Generation of ZIP for download
        [HttpGet]
        public IActionResult DownloadGeneratedZip([FromServices] ItemsInMemoryRepository tempRepo)
        {
            var items = tempRepo.GetAll().OfType<MenuItem>().ToList();

            if (!items.Any())
            {
                return Content("No menu items found in preview. Please upload JSON first.");
            }

            using (var ms = new MemoryStream())
            {
                using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    foreach (var item in items)
                    {
                        var entry = archive.CreateEntry($"{item.Title}/default.jpg");
                        var defaultImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "default.jpg");

                        if (System.IO.File.Exists(defaultImagePath))
                        {
                            using (var entryStream = entry.Open())
                            using (var fileStream = new FileStream(defaultImagePath, FileMode.Open, FileAccess.Read))
                            {
                                fileStream.CopyTo(entryStream);
                            }
                        }
                    }
                }
                return File(ms.ToArray(), "application/zip", "Menu_Structure.zip");
            }
        }
    }
}