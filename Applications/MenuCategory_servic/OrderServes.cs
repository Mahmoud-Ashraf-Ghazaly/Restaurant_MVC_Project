
using Applications.Dtos;
using Applications.RepoInterfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.MenuCategory_servic
{
    public class OrderServes : IOrderServes
    {
        private readonly IOrderRepo _orderRepository;
        private readonly IMenuItemRepo _menuItemRepository;
        private const decimal TAX_RATE = 0.085m;
        private const decimal HAPPY_HOUR_DISCOUNT = 0.2m;
        private const decimal BULK_DISCOUNT = 0.1m;
        private const decimal BULK_THRESHOLD = 100m;

        public OrderServes(IOrderRepo orderRepository, IMenuItemRepo menuItemRepository)
        {
            _orderRepository = orderRepository;
            _menuItemRepository = menuItemRepository;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllWithDetailsAsync();
            
            foreach (var order in orders)
            {
                await UpdateOrderStatusAutomatically(order);
            }
            
            return orders;
        }

        public async Task<Order> GetOrderDetailsAsync(int id)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(id);
            if (order != null)
            {
                await UpdateOrderStatusAutomatically(order);
            }
            return order;
        }

        public async Task<IEnumerable<MenuItem>> GetAvailableMenuItemsAsync()
        {
            return await _menuItemRepository.GetAvailableMenuItemsAsync();
        }

        public async Task<Order> CreateOrderAsync(Order order, int[] menuItemIds, int[] quantities)
        {
            if (menuItemIds == null || menuItemIds.Length == 0)
                throw new ArgumentException("Please select at least one menu item.");

            if (quantities == null || quantities.Length == 0)
                throw new ArgumentException("Quantity information is missing or invalid.");

            order.OrderItems = new List<OrderItem>();
            decimal subtotal = 0;
            int maxPreparationTime = 0;
            

            for (int i = 0; i < menuItemIds.Length; i++)
            {
                var menuItem = await _menuItemRepository.GetByIdAsync(menuItemIds[i]);
                if (menuItem == null || menuItem.quanty == 0)
                    continue;

                var quantity = quantities[i];
                if (quantity <= 0)
                    continue;

                var itemSubtotal = menuItem.Price * quantity;
                if(menuItem.quanty>=quantity)
                menuItem.quanty-=quantity;
                else
                    throw new ArgumentException("Sorry Amount is not available");



                order.OrderItems.Add(new OrderItem
                {
                    MenuItemId = menuItem.Id,
                    Quantity = quantity,
                    UnitPrice = menuItem.Price,
                    Subtotal = itemSubtotal
                        
                });

                
                if (menuItem.PreparationTimeMinutes > maxPreparationTime)
                    maxPreparationTime = menuItem.PreparationTimeMinutes;

               
                menuItem.DailyOrderCount += quantity;
                
                
                if (menuItem.DailyOrderCount >= 50)
                {
                    menuItem.quanty = 0; 
                }

                
                await _menuItemRepository.Update(menuItem);

                subtotal += itemSubtotal;
            }

            if (order.OrderItems.Count == 0)
                throw new InvalidOperationException("No valid menu items were selected.");

           
            if (order.OrderType == OrderType.Delivery && string.IsNullOrWhiteSpace(order.DeliveryAddress))
                throw new ArgumentException("Delivery address is required for delivery orders.");

            
            decimal tax = subtotal * TAX_RATE;

          
            decimal discount = 0;
            var now = DateTime.Now;

         
            bool hasHappyHour = now.Hour >= 15 && now.Hour < 17;
            if (hasHappyHour)
                discount = subtotal * HAPPY_HOUR_DISCOUNT;

           
            bool hasBulkDiscount = subtotal > BULK_THRESHOLD;
            if (hasBulkDiscount && discount < subtotal * BULK_DISCOUNT)
                discount = subtotal * BULK_DISCOUNT;

            order.Subtotal = subtotal;
            order.TaxAmount = tax;
            order.DiscountAmount = discount;
            order.TotalAmount = subtotal + tax - discount;
          
            order.OrderDate = DateTime.Now;
            order.OrderStatus = OrderStatus.Pending;
            order.UpdatedAt = DateTime.Now;
            //order.userId = 

            if (order.OrderType == OrderType.Delivery)
                order.EstimatedDeliveryTime = DateTime.Now.AddMinutes(maxPreparationTime + 30);
            else if (order.OrderType == OrderType.Takeout)
                order.EstimatedDeliveryTime = DateTime.Now.AddMinutes(maxPreparationTime);
            else 
                order.EstimatedDeliveryTime = DateTime.Now.AddMinutes(maxPreparationTime);

            await _orderRepository.AddAsync(order);
            return order;
        }

      
        public async Task UpdateOrderStatusAutomatically(Order order)
        {
            if (order.OrderStatus == OrderStatus.Cancelled || 
                order.OrderStatus == OrderStatus.Delivered)
                return;

            bool statusChanged = false;
            var now = DateTime.Now;
            var minutesSinceOrder = (now - order.OrderDate).TotalMinutes;
            var minutesSinceStatusChange = order.UpdatedAt.HasValue 
                ? (now - order.UpdatedAt.Value).TotalMinutes 
                : minutesSinceOrder;

            if (order.OrderStatus == OrderStatus.Pending && minutesSinceOrder >= 5)
            {
                order.OrderStatus = OrderStatus.Preparing;
                order.UpdatedAt = now;
                statusChanged = true;
            }
            
            else if (order.OrderStatus == OrderStatus.Preparing)
            {
                int maxPrepTime = 15; 
                if (order.OrderItems != null && order.OrderItems.Any())
                {
                    maxPrepTime = order.OrderItems.Max(i => i.MenuItem?.PreparationTimeMinutes ?? 15);
                }

                if (minutesSinceStatusChange >= maxPrepTime)
                {
                    order.OrderStatus = OrderStatus.Ready;
                    order.UpdatedAt = now;
                    statusChanged = true;
                }
            }

            if (statusChanged)
            {
                await _orderRepository.UpdateAsync(order);
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int id, OrderStatus newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                return false;

            if (order.OrderStatus == OrderStatus.Delivered || 
                order.OrderStatus == OrderStatus.Cancelled)
                return false;

            if (!IsValidStatusTransition(order.OrderStatus, newStatus))
                return false;

            order.OrderStatus = newStatus;
            order.UpdatedAt = DateTime.Now;
            await _orderRepository.UpdateAsync(order);
            return true;
        }

        private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            if (currentStatus == OrderStatus.Delivered || currentStatus == OrderStatus.Cancelled)
                return false;

            switch (currentStatus)
            {
                case OrderStatus.Pending:
                    return newStatus == OrderStatus.Preparing || 
                           newStatus == OrderStatus.Cancelled;
                
                case OrderStatus.Preparing:
                    return newStatus == OrderStatus.Ready || 
                           newStatus == OrderStatus.Cancelled;
                
                case OrderStatus.Ready:
                    return newStatus == OrderStatus.Delivered;
                
                default:
                    return false;
            }
        }

        public async Task<bool> CancelOrderAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                return false;

            if (order.OrderStatus == OrderStatus.Ready ||
                order.OrderStatus == OrderStatus.Delivered ||
                order.OrderStatus == OrderStatus.Cancelled)
                return false;

            order.OrderStatus = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.Now;
            await _orderRepository.UpdateAsync(order);
            return true;
        }

        public async Task<bool> CanCancelOrderAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                return false;

            return order.OrderStatus != OrderStatus.Ready &&
                   order.OrderStatus != OrderStatus.Delivered &&
                   order.OrderStatus != OrderStatus.Cancelled;
        }

        public async Task<bool> CanUpdateOrderStatusAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                return false;

            // Can update if not Ready or Delivered
            return order.OrderStatus != OrderStatus.Ready &&
                   order.OrderStatus != OrderStatus.Delivered &&
                   order.OrderStatus != OrderStatus.Cancelled;
        }

        public async Task<List<OrderStatus>> GetAvailableStatusTransitionsAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                return new List<OrderStatus>();

            var availableStatuses = new List<OrderStatus>();

            switch (order.OrderStatus)
            {
                case OrderStatus.Pending:
                    availableStatuses.Add(OrderStatus.Preparing);
                    availableStatuses.Add(OrderStatus.Cancelled);
                    break;
                
                case OrderStatus.Preparing:
                    availableStatuses.Add(OrderStatus.Ready);
                    availableStatuses.Add(OrderStatus.Cancelled);
                    break;
                
                case OrderStatus.Ready:
                    availableStatuses.Add(OrderStatus.Delivered);
                    break;
            }

            return availableStatuses;
        }
    }
}

