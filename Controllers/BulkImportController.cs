using EP._6._2A_Assignment.Factories;
using EP._6._2A_Assignment.Interfaces;
using EP._6._2A_Assignment.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EP._6._2A_Assignment.Controllers
{
    public class BulkImportController : Controller
    {
        private readonly ImportItemFactory _factory = new ImportItemFactory();

        public IActionResult BulkImport() => View();


        [HttpPost]
        public IActionResult BulkImport(IFormFile file, string jsonInput)
        {
            string tempJson = @"
            [
                {
                 ""type"": ""restaurant"",
                 ""id"": ""R-1001"",
                 ""name"": ""Trattoria Luca"",
                 ""description"": ""Pasta & grill with fresh daily specials"",
                 ""ownerEmailAddress"": ""luca.owner@example.com"",
                 ""address"": ""123 Harbor Road, Valletta"",
                 ""phone"": ""+356 1234 5678""
                 },
                 {
                 ""type"": ""restaurant"",
                 ""id"": ""R-1002"",
                 ""name"": ""Sushi Wave"",
                 ""description"": ""Classic nigiri and creative rolls"",
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

            string json = jsonInput ?? tempJson; // if entered - we use it, otherwise tempJson 

            if (file != null)
            {
                using var reader = new StreamReader(file.OpenReadStream());
                json = reader.ReadToEnd();
            }

            List<IItemValidating> items = _factory.Create(json);

            return View("Preview", items);
        }

        [HttpPost]
        public IActionResult BulkImport(IFormFile file, string jsonInput, [FromServices] ItemsInMemoryRepository tempRepo) // inject in-memory repo
        {
            string tempJson = "..."; 

            string json = jsonInput ?? tempJson;

            if (file != null)
            {
                using var reader = new StreamReader(file.OpenReadStream());
                json = reader.ReadToEnd();
            }

            List<IItemValidating> items = _factory.Create(json);

            tempRepo.Save(items); // temporary saving in memory 

            return View("Preview", items);
        }
        public IActionResult Commit([FromServices] ItemsInMemoryRepository tempRepo,
                            [FromServices] ItemsDbRepository dbRepo)
        {
            var items = tempRepo.GetAll(); // take all from the memory 
            dbRepo.Save(items);            // saving into db
            tempRepo.Clear();              // clean temp data

            return RedirectToAction("Index", "Catalog");
        }

    }
}
