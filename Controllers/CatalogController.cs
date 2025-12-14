using DataAccess.Context;
using EP._6._2A_Assignment.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EP._6._2A_Assignment.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CatalogController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class AdminController : Controller
        {
            private readonly ItemsDbRepository _dbRepo;
            private readonly ApplicationDbContext _context;
            private readonly UserManager<IdentityUser> _userManager;
            private readonly string _siteAdminEmail = "admin@example.com"; // admin

            public AdminController(ItemsDbRepository dbRepo, ApplicationDbContext context, UserManager<IdentityUser> userManager)
            {
                _dbRepo = dbRepo;
                _context = context;
                _userManager = userManager;
            }

            public async Task<IActionResult> Verification()
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Challenge(); // if not authorized

                if (user.Email == _siteAdminEmail)
                {
                    // Admin sees all pendings 
                    var pendingRestaurants = _context.Restaurants.Where(r => r.Status == "Pending").ToList();
                    return View("AdminPendingRestaurants", pendingRestaurants);
                }
                else
                {
                    // Owners see their restaurants
                    var ownedRestaurants = _context.Restaurants.Where(r => r.ownerEmailAdress == user.Email).ToList();
                    return View("OwnerPendingMenuItems", ownedRestaurants);
                }
            }
            [HttpPost]
            [Authorize] // only authorized users
            [TypeFilter(typeof(ValidateUserCanApproveAttribute))] // our ActionFilter for the rights
            public IActionResult Approve(Guid[] selectedIds)
            {
                _dbRepo.Approve(selectedIds); // rpository method updates status in DB
                return RedirectToAction("Verification");
            }
        }

        // type = restaurants | menu
        // view = card | list
        public IActionResult Index(string view = "card", bool forApproval = false, string type = "restaurant")
        {
            if (type == "restaurant")
            {
                var restaurants = _context.Restaurants.AsQueryable();

                if (forApproval)
                {
                    restaurants = restaurants.Where(r => r.Status == "Pending");
                }
                return View("CatalogRestaurants", restaurants.ToList());
            }
            else if (type == "menuitem")
            {
                var menuItems = _context.MenuItems.AsQueryable();

                if (forApproval)
                {
                    menuItems = menuItems.Where(m => m.Status == "Pending");
                }
                return View("CatalogMenu", menuItems.ToList());
            }

            return View();
        }

        [HttpPost]
        public IActionResult ApproveSelected(Guid[] selectedIds)
        {
            foreach (var id in selectedIds)
            {
                var rest = _context.Restaurants.Find(id);
                if (rest != null)
                {
                    rest.Status = "Approved";
                }
                var menu = _context.MenuItems.Find(id);
                if (menu != null)
                {
                    menu.Status = "Approved";
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Index", new { forApproval = true });
        }
    }
}
