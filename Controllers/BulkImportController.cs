using EP._6._2A_Assignment.Factories;
using EP._6._2A_Assignment.Interfaces;
using EP._6._2A_Assignment.Models;
using EP._6._2A_Assignment.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EP._6._2A_Assignment.Controllers
{
    public class BulkImportController : Controller
    {
        private readonly ImportItemFactory _factory;

        public BulkImportController(ImportItemFactory factory)
        {
            _factory = factory;
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
                json = @"[
                 {
                 ""type"": ""restaurant"",
                 ""id"": ""R-1001"",
                 ""name"": ""Trattoria Luca"",
                 ""ownerEmailAddress"": ""luca.owner@example.com"",
                 ""address"": ""123 Harbor Road, Valletta"",
                 ""phone"": ""+356 1234 5678""
                 },
                 {
                 ""type"": ""restaurant"",
                 ""id"": ""R-1002"",
                 ""name"": ""Sushi Wave"",
                 ""ownerEmailAddress"": ""hana.owner@example.com"",
                 ""address"": ""45 Marina Street, Sliema"",
                 ""phone"": ""+356 9876 5432""
                 },
                 {
                 ""type"": ""menuItem"",
                 ""id"": ""M-2001"",
                 ""title"": ""Tagliatelle al Ragù"",
                 ""price"": 11.50,
                 ""currency"": ""EUR"",
                 ""restaurantId"": ""R-1001""
                 },
                 {
                 ""type"": ""menuItem"",
                 ""id"": ""M-2002"",
                 ""title"": ""Ribeye 300g"",
                 ""price"": 24.00,
                 ""currency"": ""EUR"",
                 "" restaurantId "": ""R-1001""
                 }
                ]";
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
        [HttpPost]
        public IActionResult Commit(IFormFile zipFile, [FromServices] ItemsInMemoryRepository tempRepo, [FromServices] ItemsDbRepository dbRepo)
        {
            // 1. Getting earlier loaded elements
            var items = tempRepo.GetAll();

            // 2. Saving each element one by one
            dbRepo.Save(items);

            // 3. Clearing storage
            tempRepo.Clear();

            return RedirectToAction("Index", "Catalog");
        }


    }
}
