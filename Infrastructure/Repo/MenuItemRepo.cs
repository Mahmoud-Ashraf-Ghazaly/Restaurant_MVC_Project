using Applications.RepoInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repo
{
    public class MenuItemRepo: IMenuItemRepo
    {
        private readonly Restaurantdb context;

        public MenuItemRepo(Restaurantdb context)
        {
            this.context = context;
        }

        public async Task<List<MenuItem>> GetAll()
        {
            var items = await context.MenuItems.ToListAsync();
            return items;
        }
        public async Task<MenuItem?> GetById(int itemId)
        {
            var item = await context.MenuItems.FirstOrDefaultAsync(i => i.Id == itemId);
            return item;
        }
        public async Task<MenuItem> GetByName(string name)
        {
            var item = await context.MenuItems.FirstOrDefaultAsync(c => c.Name==name);
            return item;
        }
        //public async Task<List<MenuItem>> GetListByName(string name)
        //{
        //    var items = await context.MenuItems.Where(i => i.Name.Contains(name)).ToListAsync();
        //    return items;
        //}
        public async Task Create(MenuItem item)
        {
            await context.MenuItems.AddAsync(item);
        }

        public async Task Update(MenuItem item)
        {
            context.MenuItems.Update(item);
        }

        public async Task<MenuItem> GetByIdAsync(int id)
        {
            return await context.MenuItems.FindAsync(id);
        }

        public async Task<IEnumerable<MenuItem>> GetAvailableMenuItemsAsync()
        {
            return await context.MenuItems
                .Where(m => m.quanty>0)
                .ToListAsync();
        }

        public async Task UpdateAsync(MenuItem menuItem)
        {
            context.MenuItems.Update(menuItem);
            await context.SaveChangesAsync();
        }
        public async Task Delete(int itemId)
        {
            var item = await context.MenuItems.FindAsync(itemId);
            if (item != null)
            {
                item.IsDeleted = true;
            }
        }
        public async Task Save()
        {
            await context.SaveChangesAsync();
        }
    }
}
