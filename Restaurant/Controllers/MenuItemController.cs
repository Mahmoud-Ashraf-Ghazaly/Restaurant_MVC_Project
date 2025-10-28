using Applications.MenuCategory_servic;
using Applications.MenuItem_servic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration.UserSecrets;
using Models;

namespace Restaurant.Controllers
{
    public class MenuItemController : Controller
    {
        private readonly IMenuItem MenuItemserves;
        private readonly IMenuCategoryservis menuCategoryservis;
        public MenuItemController(IMenuItem menuItem, IMenuCategoryservis menu)
        {
            MenuItemserves = menuItem;
            menuCategoryservis = menu;
        }
        public async Task<IActionResult> Getall()
        {
            ViewBag.Categories = await menuCategoryservis.GetAll();
            var result = await MenuItemserves.GetAll();
            return View(result);
        }
        [HttpGet]
        public async Task<IActionResult> New()
        {
            ViewBag.Categories = await menuCategoryservis.GetAll();
            return View();
        }
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> New(MenuItem item)
        {
            ViewBag.Category = new SelectList(await menuCategoryservis.GetAll(), "Id", "Name");

            if (ModelState.IsValid)
            {
                if (item.ImageFile != null && item.ImageFile.Length > 0)
                {
                    var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    if (!Directory.Exists(uploadDir))
                        Directory.CreateDirectory(uploadDir);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(item.ImageFile.FileName);
                    var filePath = Path.Combine(uploadDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await item.ImageFile.CopyToAsync(stream);
                    }

                    item.ImageUrl =  fileName;
                }

                await MenuItemserves.Create(item);
                return RedirectToAction("GetAll");
            }

            return View(item);
        }

        private void useing(MemoryStream memoryStream, object value)
        {
            throw new NotImplementedException();
        }

        private void useing(object var, object value)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Categories = await menuCategoryservis.GetAll();
            var result = await MenuItemserves.GetById(id);
            if (result == null)
            {
                return NotFound();
            }
            return View(result);
        }
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Edit(MenuItem item)
        {
            ViewBag.Categories = new SelectList(await menuCategoryservis.GetAll(), "Id", "Name");

            if (ModelState.IsValid)
            {
                var existingItem = await MenuItemserves.GetById(item.Id);
                if (existingItem == null)
                    return NotFound();

               
                existingItem.Name = item.Name;
                existingItem.Description = item.Description;
                existingItem.Price = item.Price;
                existingItem.quanty = item.quanty;
                existingItem.PreparationTimeMinutes = item.PreparationTimeMinutes;
                existingItem.CategoryId = item.CategoryId;

                if (item.ImageFile != null && item.ImageFile.Length > 0)
                {
                    var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                    if (!Directory.Exists(uploadDir))
                        Directory.CreateDirectory(uploadDir);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(item.ImageFile.FileName);
                    var filePath = Path.Combine(uploadDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await item.ImageFile.CopyToAsync(stream);
                    }

                    if (!string.IsNullOrEmpty(existingItem.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingItem.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);
                    }

                    existingItem.ImageUrl =   fileName;
                }

                await MenuItemserves.Update(existingItem);
                return RedirectToAction("Getall");
            }

            return View(item);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await MenuItemserves.GetById(id);
            ViewBag.Categories = await menuCategoryservis.GetAll();
            if (result == null)
            {
                return NotFound();
            }
            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await MenuItemserves.Delete(id);
            return RedirectToAction("GetAll");
        }

       

        
        public async Task<IActionResult> Details(int id)
        {
            ViewBag.categories = await menuCategoryservis.GetAll();
            var result = await MenuItemserves.GetById(id);
            if (result == null)
            {
                return NotFound();
            }
            return View(result);
        }
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsMenuItemNameInUse(string name, int id)
        {

            var existingMenuItem = await MenuItemserves.GetById(id);
            if (existingMenuItem != null && existingMenuItem.Name == name)
            {
                return Json(true);
            }

            var existingMenuItem1 = await MenuItemserves.GetByName(name);
            if (existingMenuItem1!=null)
            {
                return Json($"MenuItem name {name} is already in use.");
            }

            return Json(true);

        }

    }
}
