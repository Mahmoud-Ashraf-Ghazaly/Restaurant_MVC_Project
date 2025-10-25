using Applications.RepoInterfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.MenuCategory_servic
{
    public class MenuCategoryserves : IMenuCategoryservis
    {
       
        private readonly IMenucateguryRepo categoryRepo;

        public MenuCategoryserves(IMenucateguryRepo categoryRepo)
        {
            this.categoryRepo = categoryRepo;

        }

        public async Task<List<MenuCategory>> GetAll()
        {
            var categories = await categoryRepo.GetAll();
            //var categoryDtos = categories.Select(category => new MenuCategory
            //{
            //    Id = category.Id,
            //    Name = category.Name,
            //    Description = category.Description,
            //    MenuItems = category.MenuItems?.Select(item => new MenuItem
            //    {
            //        Id = item.Id,
            //        Name = item.Name,
            //        Description = item.Description,
            //        Price = item.Price,
            //        ImageUrl = item.ImageUrl,
            //        CategoryId = item.CategoryId
            //    }).ToList() ?? new List<MenuItem>()
            //}).ToList();
            return categories;
        }
        public async Task<MenuCategory?> GetById(int id)
        {
            var menuCategory = await categoryRepo.GetById(id);
            if (menuCategory == null)
            {
                return null;
            }
            //var categoryDto = new MenuCategory()
            //{
            //    Id = menuCategory.Id,
            //    Name = menuCategory.Name,
            //    Description = menuCategory.Description,
            //    MenuItems = menuCategory.MenuItems?.Select(item => new MenuItem
            //    {
            //        Id = item.Id,
            //        Name = item.Name,
            //        Description = item.Description,
            //        Price = item.Price,
            //        ImageUrl = item.ImageUrl,
            //        CategoryId = item.CategoryId
            //    }).ToList() ?? new List<MenuItem>()
            //};
            return menuCategory;
        }

        public async Task<MenuCategory> GetByName(string name)
        {
            var menuCategory = await categoryRepo.GetByName(name);
            if (menuCategory == null)
            {
                return null;
            }
            
            return menuCategory;
        }
        public async Task Create(MenuCategory newCategory)
        {
            if (newCategory == null)
            {
                return;
            }
            var category = new MenuCategory()
            {
                Name = newCategory.Name,
                Description = newCategory.Description,
            };
            await categoryRepo.Create(category);
            await categoryRepo.Save();
        }
        public async Task Update(MenuCategory newCategory)
        {
            var category = await categoryRepo.GetById(newCategory.Id);
            if (category == null)
            {
                return;
            }
            category.Name = newCategory.Name;
            category.Description = newCategory.Description;
            await categoryRepo.Update(category);
            await categoryRepo.Save();
        }
        public async Task Delete(int id)
        {
            var category = await categoryRepo.GetById(id);
            if (category == null)
            {
                return;
            }
            await categoryRepo.Delete(id);
            await categoryRepo.Save();
        }

        public async Task<bool> GetByName1(string name)
        {
            var category = categoryRepo.GetByName1(name);
            if (category == null)
            {
                return false;
            }
            return await category;
        }



    }
}
