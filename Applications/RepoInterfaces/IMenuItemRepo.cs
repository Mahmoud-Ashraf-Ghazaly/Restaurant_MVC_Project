using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.RepoInterfaces
{
   public interface IMenuItemRepo
    {
        Task<MenuItem> GetByIdAsync(int id);
        Task<IEnumerable<MenuItem>> GetAvailableMenuItemsAsync();
        Task UpdateAsync(MenuItem menuItem);
        public Task<List<MenuItem>> GetAll();
        public Task<MenuItem?> GetById(int itemId);
        public Task<MenuItem> GetByName(string name);
        //public  Task<List<MenuItem>> GetListByName(string name);
        public Task Create(MenuItem item);
        public Task Update(MenuItem item);
        public Task Delete(int itemId);
        public Task Save();

    }
}
