using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.MenuCategory_servic
{
    public interface IMenuCategoryservis
    {
        public Task<List<MenuCategory>> GetAll();
        public Task<MenuCategory?> GetById(int id);
       // public Task<MenuCategory?> GetByName(string name);
        public Task<bool> GetByName1(string name);
        public Task Create(MenuCategory menuCategory);
        public Task Update(MenuCategory menuCategory);
        public Task Delete(int id);
    }
}
