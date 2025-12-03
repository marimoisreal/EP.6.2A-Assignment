using DataAccess.Context;
using EP._6._2A_Assignment.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace EP._6._2A_Assignment.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CatalogController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string view = "card", bool forApproval = false)
        {

            var restaurants = _context.Restaurants.Cast<IItemValidating>().ToList();
            var menuItems = _context.MenuItems.Cast<IItemValidating>().ToList();


            if (forApproval)
            {
                restaurants = restaurants.Where(r => r.Status == "Pending").ToList();
                menuItems = menuItems.Where(m => m.Status == "Pending").ToList();
            }


            var items = restaurants.Concat(menuItems).ToList();

            ViewBag.ViewType = view; // card or row
            ViewBag.ForApproval = forApproval;

            return View("Catalog", items);
        }


        [HttpPost]
        public IActionResult ApproveSelected(Guid[] selectedIds)
        {
            foreach (var id in selectedIds)
            {
                var restaurant = _context.Restaurants.Find(id);
                if (restaurant != null) restaurant.Status = "Approved";

                var menuItem = _context.MenuItems.Find(id);
                if (menuItem != null) menuItem.Status = "Approved";
            }

            _context.SaveChanges();
            return RedirectToAction("Index", new { forApproval = true });
        }

    }

}