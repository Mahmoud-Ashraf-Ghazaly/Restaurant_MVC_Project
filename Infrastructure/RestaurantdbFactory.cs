using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class RestaurantdbFactory : IDesignTimeDbContextFactory<Restaurantdb>
    {
        public Restaurantdb CreateDbContext(string[] args)
        {   
            var optionsBuilder = new DbContextOptionsBuilder<Restaurantdb>();

            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog= Restaurant_MVC_DBV2  ;Integrated Security=True;Encrypt=False;Trust Server Certificate=True");

            return new Restaurantdb(optionsBuilder.Options);
        }
    }
}
