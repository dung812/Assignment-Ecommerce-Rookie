using ShoesShop.Data;
using ShoesShop.DTO;
using ShoesShop.Domain;
using ShoesShop.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Linq;

namespace ShoesShop.Service
{
    public interface IOrderService
    {
        public bool CreateNewOrder(OrderViewModel orderViewModel, int customerId, int paymentId);
        public bool CreateOrderDetail(string orderId, List<CartViewModel> listCart);
        public List<OrderViewModel> GetOrderListByStatus(OrderStatus status);
        public List<OrderViewModel> GetListOrderByCustomerId(int customerId);
        public OrderViewModel GetOrderDetailById(string orderId);
        public List<OrderDetailViewModel> GetItemOfOderById(string orderId);
        public bool DeleteOrderByCustomer(int customerId, string orderId);

        public bool CheckedOrder(string orderId);
        public bool SuccessDeliveryOrder(string orderId);
        public bool CancellationOrder(string orderId);
        public bool DeleteOrderByAdmin(string orderId);
        public List<OrderViewModel> GetRecentOrders();
        public List<OrderStatisticViewModel> GetStatisticOrder();
        public List<OrderViewModel> GetOrderListByStatusFilter(OrderStatus status, DateTime? FromDate, DateTime? ToDate);
        public List<OrderViewModel> GetOrderListOfCustomerId(int customerId, OrderStatus status);
        public List<OutOfStockOrder> CheckOrderCanChecked(string orderId);
    }
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        public OrderService(IMapper mapper, ApplicationDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        public bool CreateNewOrder(OrderViewModel orderViewModel, int customerId, int paymentId)
        {
            Order order = new Order()
            {
                OrderId = orderViewModel.OrderId,
                OrderDate = orderViewModel.OrderDate,
                OrderStatus = OrderStatus.NewOrder,
                OrderName = orderViewModel.OrderName,
                Address = orderViewModel.Address,
                Phone = orderViewModel.Phone,
                Note = orderViewModel.Note,
                TotalMoney = orderViewModel.TotalMoney,
                TotalDiscounted = orderViewModel.TotalDiscounted,
                CustomerId = customerId,
                PaymentId = paymentId
            };
            _context.Orders.Add(order);
            _context.SaveChanges();
            return true;
        }
        public bool CreateOrderDetail(string orderId, List<CartViewModel> listCart)
        {
            try
            {
                foreach (var item in listCart)
                {
                    OrderDetail orderDetail = new OrderDetail();
                    orderDetail.OrderId = orderId;
                    orderDetail.ProductId = item.ProductId;
                    orderDetail.AttributeValueId = item.AttributeId;
                    orderDetail.Quantity = item.Quantity;
                    orderDetail.UnitPrice = item.UnitPrice;
                    orderDetail.DiscountedPrice = item.CurrentPriceItem;
                    orderDetail.PromotionPercent = item.PromotionPercent;
                    orderDetail.TotalDiscounted = item.TotalDiscountedPrice;
                    orderDetail.TotalMoney = (int)item.TotalPrice;

                    _context.OrderDetails.Add(orderDetail);
                }
                _context.SaveChanges();
                return true;
            } catch (Exception)
            {
                return false;
            }
        }

        public List<OrderViewModel> GetOrderListByStatus(OrderStatus status)
        {
            var orders = _context.Orders
                .Where(m => m.OrderStatus == status)
                .Include(m => m.Payment)
                .OrderByDescending(m => m.OrderDate)
                .ToList();

            var ordersDTO = _mapper.Map<List<OrderViewModel>>(orders);
            return ordersDTO;
        }        
        
        public List<OrderViewModel> GetOrderListByStatusFilter(OrderStatus status, DateTime? FromDate, DateTime? ToDate)
        {
            var orders = _context.Orders
                         .Include(m => m.Customer)
                        .Include(m => m.Payment)
                        .Where(m => m.OrderId != null);
            if (status != OrderStatus.All)
            {
                orders = orders.Where(m => m.OrderStatus == status);
            }

            if (FromDate != null && ToDate != null)
            {
                DateTime fromdate = (DateTime)FromDate;
                DateTime todate = (DateTime)ToDate;
                todate = todate.AddDays(1);
                orders = orders.Where(c => c.OrderDate > fromdate && c.OrderDate < todate).OrderBy(c => c.OrderDate);
            }
            var ordersDTO = _mapper.Map<List<OrderViewModel>>(orders.ToList());
            return ordersDTO;
        }        
        
        public List<OrderViewModel> GetOrderListOfCustomerId(int customerId, OrderStatus status)
        {
            var orders = _context.Orders
                        .Include(m => m.Customer)
                        .Include(m => m.Payment)
                        .Where(m => m.CustomerId == customerId);
            if (status != OrderStatus.All)
            {
                orders = orders.Where(m => m.OrderStatus == status);
            }
            var ordersDTO = _mapper.Map<List<OrderViewModel>>(orders.ToList());
            return ordersDTO;
        }

        public List<OrderViewModel> GetListOrderByCustomerId(int customerId)
        {
            List<OrderViewModel> orders = new List<OrderViewModel>();
            orders = _context.Orders
                                .Where(m => m.CustomerId == customerId)
                                .Include(m => m.Payment)
                                .OrderByDescending(m => m.OrderDate)
                                .Select(m => new OrderViewModel
                                {
                                    OrderId = m.OrderId,
                                    OrderDate = m.OrderDate,
                                    OrderStatus = m.OrderStatus,
                                    DeliveryDate = m.DeliveryDate,
                                    OrderName = m.OrderName,
                                    Address = m.Address,
                                    Phone = m.Phone,
                                    Note = m.Note,
                                    PaymentName = m.Payment.PaymentName
                                }).ToList();
            return orders;
        }

        public OrderViewModel GetOrderDetailById(string orderId)
        {
            var orderDetail = new OrderViewModel();
            orderDetail = _context.Orders
                 .Where(m => m.OrderId == orderId)
                 .Include(m => m.Payment)
                 .OrderByDescending(m => m.OrderDate)
                 .Select(m => new OrderViewModel
                 {
                     OrderId = m.OrderId,
                     OrderDate = m.OrderDate,
                     OrderStatus = m.OrderStatus,
                     DeliveryDate = m.DeliveryDate,
                     OrderName = m.OrderName,
                     Address = m.Address,
                     Phone = m.Phone,
                     Note = m.Note,
                     PaymentName = m.Payment.PaymentName
                 }).FirstOrDefault();
            return orderDetail;
        }

        public List<OrderDetailViewModel> GetItemOfOderById(string orderId)
        {
            List<OrderDetailViewModel> orderDetail = new List<OrderDetailViewModel>();
            orderDetail = _context.OrderDetails
                .Where(m => m.OrderId == orderId)
                .Include(m => m.Product)
                .Include(m => m.AttributeValue)
                .Select(m => new OrderDetailViewModel
                {
                    OrderId = m.OrderId,
                    ProductName = m.Product.ProductName,
                    ProductImage = m.Product.ImageFileName,
                    AttributeName = m.AttributeValue.Name,
                    Quantity = m.Quantity,

                    PromotionPercent = m.PromotionPercent,
                    UnitPrice = m.UnitPrice,
                    DiscountedPrice = m.DiscountedPrice,
                    TotalDiscounted = m.TotalDiscounted,
                    TotalMoney = m.TotalMoney,
                }).ToList();
            return orderDetail;
        }

        public bool DeleteOrderByCustomer(int customerId, string orderId)
        {
            var order = _context.Orders.FirstOrDefault(m => m.CustomerId == customerId && m.OrderId == orderId);
            if (order != null)
            {
                // Delete order detail
                //_context.Database.ExecuteSqlRaw("DELETE FROM OrderDetailS WHERE OrderId = '" + orderId + "'");
                order.OrderStatus = OrderStatus.Cancelled;

                // Delete order
                //_context.Orders.Remove(order);
                _context.SaveChanges();
                return true;
            }
            return false;
        }


        public List<OutOfStockOrder> CheckOrderCanChecked(string orderId)
        {
            List<OutOfStockOrder> lists = new List<OutOfStockOrder>();
            var listOrderProduct = _context.OrderDetails
                                    .Where(m => m.OrderId == orderId)
                                    .Include(m => m.Product)
                                    .Include(m => m.AttributeValue).ToList();
            if (listOrderProduct.Count > 0)
            {
                foreach (var itemInOrder in listOrderProduct)
                {
                    var getProduct = _context.Products.FirstOrDefault(m => m.ProductId == itemInOrder.ProductId);
                    if (getProduct.Quantity < itemInOrder.Quantity)
                    {
                        var newItem = new OutOfStockOrder()
                        {
                            ProductId = itemInOrder.ProductId,
                            QuantityOfOrder = itemInOrder.Quantity,
                            QuantityOfStock = getProduct.Quantity
                        };
                        if (!lists.Contains(newItem))
                            lists.Add(newItem);
                    }    
                }
            }

            return lists;
        }
        public bool CheckedOrder(string orderId)
        {
            bool result;
            var order = _context.Orders.Where(m => m.OrderStatus == OrderStatus.NewOrder).FirstOrDefault(m => m.OrderId == orderId);

            if (order != null)
            {
                // Reduce quantity product
                var itemOrderDetail = _context.OrderDetails
                                        .Where(m => m.OrderId == orderId)
                                        .Include(m => m.Product)
                                        .Include(m => m.AttributeValue).ToList();
                foreach (var itemInOrder in itemOrderDetail)
                {
                    var getProduct = _context.Products.FirstOrDefault(m => m.ProductId == itemInOrder.ProductId);
                    if(getProduct != null)
                    {
                        getProduct.Quantity = getProduct.Quantity - itemInOrder.Quantity;

                        _context.Products.Update(getProduct);
                        _context.SaveChanges();
                    }
                }

                // Change state order
                order.OrderStatus = OrderStatus.AwatingShipment;

                _context.Orders.Update(order);
                _context.SaveChanges();
                result = true;
            }
            else
                result = false;
            return result;
        }


        public bool SuccessDeliveryOrder(string orderId)
        {
            bool result;
            var order = _context.Orders.Where(m => m.OrderStatus == OrderStatus.AwatingShipment).FirstOrDefault(m => m.OrderId == orderId);

            if (order != null)
            {
                order.OrderStatus = OrderStatus.Delivered;
                order.DeliveryDate = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

                _context.Orders.Update(order);
                _context.SaveChanges();
                result = true;
            }
            else
                result = false;
            return result;
        }
        public bool CancellationOrder(string orderId)
        {
            bool result;
            var order = _context.Orders.Where(m => m.OrderStatus == OrderStatus.AwatingShipment).FirstOrDefault(m => m.OrderId == orderId);

            if (order != null)
            {
                order.OrderStatus = OrderStatus.Cancelled;
                order.CancellationDate = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

                _context.Orders.Update(order);
                _context.SaveChanges();
                result = true;
            }
            else
                result = false;
            return result;
        }
        public bool DeleteOrderByAdmin(string orderId)
        {
            var order = _context.Orders.FirstOrDefault(m => m.OrderId == orderId);
            if (order != null)
            {
                // Delete order detail
                _context.Database.ExecuteSqlRaw("DELETE FROM OrderDetailS WHERE OrderId = '" + orderId + "'");

                // Delete order
                _context.Orders.Remove(order);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public List<OrderViewModel> GetRecentOrders()
        {
            List<OrderViewModel> orders = new List<OrderViewModel>();
            //DateTime today = DateTime.Today;
            DateTime today = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            orders = _context.Orders
                .Where(m => m.OrderStatus == OrderStatus.NewOrder &&
                        m.OrderDate.Date == today.Date)
                .Include(m => m.Payment)
                .OrderByDescending(m => m.OrderDate)
                .Select(m => new OrderViewModel
                {
                    OrderId = m.OrderId,
                    OrderDate = m.OrderDate,
                    OrderStatus = m.OrderStatus,
                    DeliveryDate = m.DeliveryDate,
                    OrderName = m.OrderName,
                    Address = m.Address,
                    Phone = m.Phone,
                    Note = m.Note,
                    PaymentName = m.Payment.PaymentName,
                    TotalMoney = m.TotalMoney,
                    TotalDiscounted = m.TotalDiscounted
                }).ToList();
            return orders;
        }        

        public List<OrderStatisticViewModel> GetStatisticOrder()
        {
            var list = new List<OrderStatisticViewModel>();
            var newOrder = new OrderStatisticViewModel()
            {
                OrderType = "New Order",
                QuantityOrder = _context.Orders.Where(m => m.OrderStatus == OrderStatus.NewOrder).Count()
            };
            list.Add(newOrder);

            var awatingShipment = new OrderStatisticViewModel()
            {
                OrderType = "Awating Shipment",
                QuantityOrder = _context.Orders.Where(m => m.OrderStatus == OrderStatus.AwatingShipment).Count()
            };
            list.Add(awatingShipment);

            var delivered = new OrderStatisticViewModel()
            {
                OrderType = "Delivered",
                QuantityOrder = _context.Orders.Where(m => m.OrderStatus == OrderStatus.Delivered).Count()
            };
            list.Add(delivered);

            var cancelled = new OrderStatisticViewModel()
            {
                OrderType = "Cancelled",
                QuantityOrder = _context.Orders.Where(m => m.OrderStatus == OrderStatus.Cancelled).Count()
            };
            list.Add(cancelled);
            return list;
        }
    }
}
