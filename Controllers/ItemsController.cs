using EP._6._2A_Assignment.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EP._6._2A_Assignment.Controllers
{
    public class ItemsController : Controller
    {
        private readonly IItemsRepository _repo;

        public ItemsController(IItemsRepository repo)
        {
            _repo = repo;
        }
        // Restaurant list
        public IActionResult Catalog()
        {
            var restaurants = _repo.GetApprovedRestaurants();
            return View(restaurants);
        }

        // Restaurant's menu
        public IActionResult Restaurant(int id)
        {
            var menuItems = _repo.GetApprovedMenuItems(id);
            return View(menuItems);
        }
    }

}
