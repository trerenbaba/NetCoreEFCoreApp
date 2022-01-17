using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCoreEFCoreApp.Persistences.EFCore.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreEFCoreApp.Controllers
{
    public class QueryController : Controller
    {
        private AppDbContext _db;

        //repositoryleri kullanarak araya interface koyacağız böylelikle dependency inversion prensibini uygulayacağız.
        public QueryController(AppDbContext db) //Dependency Injection (DI constructor üzerinden bir nesneye başka nesnenin instance gönderilmesi DI diyoruz.)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var products = _db.Products.ToList();//lambda expression linq
            var products2 = (from p in _db.Products select p).ToList();//

            //Include ile navigation property üzerinden bağlamış olduk.
            //Eager Loading her zaman lazy loading göre daha performanslı bir yöntemdir.
            var productIncludeWithCategory = _db.Products.Include(x => x.Category).ToList();
            return View();
        }
    }
}
