using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.MenuItem_servic
{
    public interface IMenuItem
    {
        public Task<List<MenuItem>> GetAll();
        public Task<MenuItem?> GetById(int id);
        public Task<MenuItem> GetByName(string name);
        public Task Create(MenuItem newItem);
        public Task Update(MenuItem newItem);
        public Task Delete(int id);
    }
}
