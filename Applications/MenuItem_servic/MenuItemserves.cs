using Applications.MenuCategory_servic;
using Applications.RepoInterfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.MenuItem_servic
{
    public class MenuItemserves:IMenuItem
    {
        private readonly IMenuItemRepo itemRepo;
        private readonly IMenuCategoryservis categoryservis;

        public MenuItemserves(IMenuItemRepo itemRepo)
        {
            this.itemRepo = itemRepo;
        }
        public async Task<List<MenuItem>> GetAll()
        {
            var items = await itemRepo.GetAll();
            return items;
        }
        public async Task<MenuItem?> GetById(int id)
        {
            var menuItem = await itemRepo.GetById(id);
            if (menuItem == null) return null;
         
            return menuItem;
        }
        public async Task<MenuItem> GetByName(string name)
        {
            var item = await itemRepo.GetByName(name);
            if (item == null) return null;
            

            return item;
        }
        public async Task Create(MenuItem newItem)
        {
            if (newItem == null)
            {
                return;
            }
            var menuitem = new MenuItem
            {
                Name = newItem.Name,
                Description = newItem.Description,
                Price = newItem.Price,
                quanty = newItem.quanty,
                ImageUrl = newItem.ImageUrl,
                CategoryId = newItem.CategoryId,
               PreparationTimeMinutes = newItem.PreparationTimeMinutes
            };
            await itemRepo.Create(menuitem);
            await itemRepo.Save();
        }
        public async Task Update(MenuItem newItem)
        {
            var item = await itemRepo.GetById(newItem.Id);
            if (item == null)
            {
                return;
            }
            item.Name = newItem.Name;
            item.Description = newItem.Description;
            item.Price = newItem.Price;
            item.quanty = newItem.quanty;
            item.PreparationTimeMinutes=newItem.PreparationTimeMinutes;
            item.ImageUrl = newItem.ImageUrl;
            item.CategoryId = newItem.CategoryId;

            await itemRepo.Update(item);
            await itemRepo.Save();
        }
        public async Task Delete(int id)
        {
            var item = await itemRepo.GetById(id);
            if (item == null)
            {
                return;
            }
            await itemRepo.Delete(id);
            await itemRepo.Save();
        }
    }
}
