using Applications.Dtos;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.MenuCategory_servic
{
    public interface IOrderServes
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderDetailsAsync(int id);
        Task<IEnumerable<MenuItem>> GetAvailableMenuItemsAsync();
        Task<Order> CreateOrderAsync(Order order, int[] menuItemIds, int[] quantities);
        Task<bool> CancelOrderAsync(int id);
        Task<bool> CanCancelOrderAsync(int id);
        public  Task<List<OrderStatus>> GetAvailableStatusTransitionsAsync(int id);
        public Task UpdateOrderStatusAutomatically(Order order);
        public Task<bool> CanUpdateOrderStatusAsync(int id);
        public Task<bool> UpdateOrderStatusAsync(int id, OrderStatus newStatus);


    }
}
