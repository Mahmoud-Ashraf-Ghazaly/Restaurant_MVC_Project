using Applications.RepoInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.Repo.MenuCategoryRepo;

namespace Infrastructure.Repo
{
    public class MenuCategoryRepo : IMenucateguryRepo
    {
        private readonly Restaurantdb context;

        public MenuCategoryRepo(Restaurantdb context)
        {
            this.context = context;
        }

        public async Task<List<MenuCategory>> GetAll()
        {
            var categories = await context.MenuCategories.Include(i => i.MenuItems).ToListAsync();
            return categories;
        }
        public async Task<MenuCategory?> GetById(int categoryId)
        {
            var category = await context.MenuCategories.Include(i => i.MenuItems).FirstOrDefaultAsync(i => i.Id == categoryId);
            return category;
        }
        public async Task Create(MenuCategory category)
        {
            await context.MenuCategories.AddAsync(category);
        }

        public async Task Update(MenuCategory category)
        {
            context.MenuCategories.Update(category);
            
        }

        public async Task Delete(int categoryId)
        {
            var category = await context.MenuCategories.FindAsync(categoryId);
            if (category != null)
            {
                category.IsDeleted = true;
                foreach (var item in category.MenuItems)
                {
                    item.IsDeleted = true;
                }
            }
        }
        public async Task Save()
        {
            await context.SaveChangesAsync();
        }

        public async Task<MenuCategory> GetByName(string name)
        {
            var category = await context.MenuCategories.FirstOrDefaultAsync(c => c.Name==name);
            return category;
        }

        public async Task<bool> GetByName1(string name)
        {
            var category = await context.MenuCategories.AnyAsync(c => c.Name==name);
            return category;
        }
    }
}
