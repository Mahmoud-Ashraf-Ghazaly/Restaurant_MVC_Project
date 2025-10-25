using Applications.MenuCategory_servic;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Restaurant.Controllers
{
    public class MenuCategoryController : Controller
    {
        private readonly IMenuCategoryservis menuCategoryservis;
        public MenuCategoryController(IMenuCategoryservis menuCategoryservis)
        {
            this.menuCategoryservis = menuCategoryservis;
        }

        public async Task<IActionResult> Getall()
        {
            var result = await menuCategoryservis.GetAll();
            return View(result);
        }


        [HttpGet]
        public async Task<IActionResult> New()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> New(MenuCategory menuCategory)
        {
            if (ModelState.IsValid)
            {
                await menuCategoryservis.Create(menuCategory);
                return RedirectToAction("Getall");
            }
            return View(menuCategory);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await menuCategoryservis.GetById(id);
            if (result == null)
            {
                return NotFound();
            }
            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(MenuCategory menuCategory)
        {
            if (ModelState.IsValid)
            {
                await menuCategoryservis.Update(menuCategory);

                return RedirectToAction("GetAll");
            }
            return View(menuCategory);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await menuCategoryservis.GetById(id);
            if (result == null)
            {
                return NotFound();
            }
            //await menuCategoryservis.Delete(id);
            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await menuCategoryservis.Delete(id);
            return RedirectToAction("GetAll");
        }


        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsCategoryNameInUse(string name,int id)
        {

            var existingCategory = await menuCategoryservis.GetById(id);
            if (existingCategory != null && existingCategory.Name == name)
            {
                return Json(true);
            }

            var categoryExists = await menuCategoryservis.GetByName1(name);
            if (categoryExists)
            {
                return Json($"Category name {name} is already in use.");
            }
            return Json(true);

        }

    }
}
