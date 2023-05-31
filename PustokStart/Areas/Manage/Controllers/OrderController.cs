using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokStart.DAL;
using PustokStart.Enums;
using PustokStart.Models;
using PustokStart.ViewModels;

namespace PustokStart.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles ="SuperAdmin,Admin")]
    public class OrderController : Controller
    {
        private readonly PustokContext _context;

        public OrderController(PustokContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1,byte orderStatus=0)
        {
            var query =_context.Orders.Include(x=>x.OrderItems).AsQueryable();
            if (orderStatus != 0)
            {
                query = query.Where(x=>(byte)x.Status==orderStatus);
            }
         ViewBag.OrderStatus = orderStatus;
   
            var data = PaginatedList<Order>.Create(query, page, 2);
            return View(data);
        }
        public IActionResult Detail(int id )
        {
            Order order =_context.Orders.Include(x=>x.OrderItems).ThenInclude(x=>x.Book).FirstOrDefault(x=>x.Id== id);
            if (order == null)
            {
                return View("Error");
            }
            return View(order);
        }
        public IActionResult Accept(int id)
        {
            Order order = _context.Orders.Include(x => x.OrderItems).ThenInclude(x => x.Book).FirstOrDefault(x => x.Id == id);
            if (order == null)
            {
                return View("Error");
            }
            order.Status = OrderStatus.Accepted;
            _context.SaveChanges();
            return RedirectToAction("index");
        }
        public IActionResult Reject(int id)
        {
            Order order = _context.Orders.Include(x => x.OrderItems).ThenInclude(x => x.Book).FirstOrDefault(x => x.Id == id);
            if (order == null)
            {
                return View("Error");
            }
            order.Status = OrderStatus.Rejected;
            _context.SaveChanges();
            return RedirectToAction("index");
        }
        //public IActionResult Test()
        //{
        //    var enums =Enum.GetValues(typeof(OrderStatus));   
        //    Dictionary<byte,string> data = new Dictionary<byte,string>();

        //    foreach (var item in enums)
        //    {
        //        data.Add((byte)item,item.ToString());
        //    }
        //    return Json(data);
        //}
    }
}
