using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.RepoInterfaces
{
    public interface IMenucateguryRepo
    {
        public Task<List<MenuCategory>> GetAll();
        public Task<MenuCategory?> GetById(int categoryId);
        public Task<MenuCategory?> GetByName(string name);
        public Task<bool> GetByName1(string name);
        public Task Create(MenuCategory category);
        public Task Update(MenuCategory category);
        public Task Delete(int categoryId);
        public Task Save();
    }
}
