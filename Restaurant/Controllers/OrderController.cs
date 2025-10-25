using Applications.Dtos;
using Applications.MenuCategory_servic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Security.Claims;

namespace Restaurant.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderServes _orderService;

        public OrderController(IOrderServes orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Order> orders;
        


            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

           
            if (User.IsInRole("Admin"))
            {
              
                orders = await _orderService.GetAllOrdersAsync();
                ViewBag.IsAdminView = true;


                var now = DateTime.Now;
                var monthlyRevenue = orders
                    .Where(o => o.OrderDate.Month == now.Month && o.OrderDate.Year == now.Year)
                    .Sum(o => o.TotalAmount);

                ViewBag.MonthlyRevenue = monthlyRevenue;

            }
            else
            {

                var allOrders = await _orderService.GetAllOrdersAsync();
                orders = allOrders
                    .Where(o => o.userId == userId)
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();

                ViewBag.IsAdminView = false;
                var now = DateTime.Now;
                var userMonthlyRevenue = orders
                    .Where(o => o.OrderDate.Month == now.Month && o.OrderDate.Year == now.Year)
                    .Sum(o => o.TotalAmount);

                ViewBag.UserMonthlyRevenue = userMonthlyRevenue;


            }

            return View(orders);
        }
      
        public async Task<IActionResult> Create()
        {
            var menuItems = await _orderService.GetAvailableMenuItemsAsync();
            ViewBag.MenuItems = menuItems;
            


            var currentHour = DateTime.Now.Hour;
            if (currentHour >= 15 && currentHour < 17)
            {
                ViewBag.HappyHourMessage = "Happy Hour! Get 20% off your order (3 PM - 5 PM)";
            }

            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationUser user, Order order, int[] menuItemIds, int[]? quantities)
        {
            if (menuItemIds == null || menuItemIds.Length == 0)
            {
                ModelState.AddModelError("", "Please select at least one menu item.");
            }

            if (quantities == null || quantities.Length == 0)
            {
                ModelState.AddModelError("", "Quantity information is missing or invalid.");
            }

          
            if (order.OrderType == OrderType.Delivery && string.IsNullOrWhiteSpace(order.DeliveryAddress))
            {
                ModelState.AddModelError("DeliveryAddress", "Delivery address is required for delivery orders.");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            user.Id = userId;
            order.userId = user.Id;
                
            ModelState.Remove("user");
            ModelState.Remove("userId");
            ModelState.Remove("OrderItems");

            if (ModelState.IsValid)
            {
                try
                {
                    await _orderService.CreateOrderAsync(order, menuItemIds, quantities);
                    TempData["Success"] = "Order created successfully!";
                    TempData["OrderId"] = order.Id;
                    return RedirectToAction(nameof(Details), new { id = order.Id });
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred while creating the order: {ex.Message}");
                }
            }
            

            ViewBag.MenuItems = await _orderService.GetAvailableMenuItemsAsync();

            var currentHour = DateTime.Now.Hour;
            if (currentHour >= 15 && currentHour < 17)
            {
                ViewBag.HappyHourMessage = "Happy Hour! Get 20% off your order (3 PM - 5 PM)";
            }

            return View(order);
        }

        
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderDetailsAsync(id);
            if (order == null)
                return NotFound();

          
            var availableStatuses = await _orderService.GetAvailableStatusTransitionsAsync(id);
            ViewBag.AvailableStatuses = availableStatuses;

           
            var canUpdate = await _orderService.CanUpdateOrderStatusAsync(id);
            ViewBag.CanUpdateStatus = canUpdate;

            
            var canCancel = await _orderService.CanCancelOrderAsync(id);
            ViewBag.CanCancel = canCancel;

           
            switch (order.OrderStatus)
            {
                case OrderStatus.Pending:
                    ViewBag.StatusMessage = "Your order is pending. It will start preparing in a few minutes.";
                    ViewBag.StatusClass = "info";
                    break;
                case OrderStatus.Preparing:
                    ViewBag.StatusMessage = "Your order is being prepared. Hang tight!";
                    ViewBag.StatusClass = "warning";
                    break;
                case OrderStatus.Ready:
                    ViewBag.StatusMessage = "Your order is ready for pickup/delivery!";
                    ViewBag.StatusClass = "success";
                    break;
                case OrderStatus.Delivered:
                    ViewBag.StatusMessage = "Your order has been delivered. Enjoy your meal!";
                    ViewBag.StatusClass = "success";
                    break;
                case OrderStatus.Cancelled:
                    ViewBag.StatusMessage = "This order has been cancelled.";
                    ViewBag.StatusClass = "danger";
                    break;
            }

            return View(order);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus newStatus)
        {
            var canUpdate = await _orderService.CanUpdateOrderStatusAsync(id);
            if (!canUpdate)
            {
                TempData["Error"] = "Cannot update status for this order (already Ready, Delivered, or Cancelled).";
                return RedirectToAction(nameof(Details), new { id });
            }

            var result = await _orderService.UpdateOrderStatusAsync(id, newStatus);
            if (result)
            {
                TempData["Success"] = $"Order status updated to {newStatus} successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to update order status. Invalid status transition.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var canCancel = await _orderService.CanCancelOrderAsync(id);
            if (!canCancel)
            {
                TempData["Error"] = "Cannot cancel this order. Orders that are Ready or Delivered cannot be cancelled.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var result = await _orderService.CancelOrderAsync(id);
            if (result)
            {
                TempData["Success"] = "Order cancelled successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to cancel order.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
