using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCoreEFCoreApp.Domain.Models;
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

            //fiyatı 43 ile 78 arasında olanlar
            var products3 = _db.Products.Where(x => x.Price >= 43 && x.Price <= 78).ToList();

            //ürünleri fiyatına göre artandan azalana sıralama
            var products4 = _db.Products.OrderByDescending(x => x.Price).ToList();

            //isminde ürün geçen productlar
            var products5 = _db.Products.Where(x => x.Name.Contains("ürün")).ToList();
            var products6 = _db.Products.Where(x => EF.Functions.Like(x.Name, "%ürün%")).ToList();

            //ürün ismi ü ile başlayan
            var products7 = _db.Products.Where(x => EF.Functions.Like(x.Name, "%ü"));
            var products8 = _db.Products.Where(x => x.Name.StartsWith("ü")).ToList();

            //ismi s ile biten ürünler
            var products9 = _db.Products.Where(x => EF.Functions.Like(x.Name, "s%"));
            var products10 = _db.Products.Where(x => x.Name.EndsWith("s")).ToList();

            //ürün id sine göre getirme
            var products11 = _db.Products.Find("000DABCC-C924-4963-B342-633CEA1EBF95");
            //firstorDefault ile getirme
            var products12 = _db.Products.FirstOrDefault(x => x.Id == "000DABCC-C924-4963-B342-633CEA1EBF95");
            var products13 = _db.Products.SingleOrDefault(x => x.Id == "000DABCC-C924-4963-B342-633CEA1EBF95");

            //ilk 5 ürünü çekme // ürün fiyatına göre asc olarak sıralanmış ilk 5 adet ürün. Take ile çalışırken öncesinde orderBy sorgusu atalım
            var products14 = _db.Products.OrderBy(x => x.Price).Take(5).ToList();

            //sayfalama işlemleri için
            var products15 = _db.Products.OrderBy(x => x.Price).Skip(2).Take(2).ToList(); //skip methodu ile kayıt atlatma işlemleri yani sqldeki offset işlemleri yaparız.

            //iki farklı alana göre kayıtlarımızı sıralamak için thenBy moethodunu kullanırız.
            //veri tabanında aynı isimde çalışan varsa soy isimlerine göre azalandan artana sıralama yapmak için kullanılabilir.
            var products16 = _db.Products.OrderBy(x => x.Name).ThenBy(x => x.Price).ToList();

            //veritabanında kayıt var mı yok mu sorgulama için any methodu kullanılır.e
            var products17 = _db.Products.Any(x => x.Name.Contains("a"));
            //name alanında a geçen bir kayıt var mı. Any sonuç olarak true yada false değer döndürür.

            //select ile bir tablodaki belirli alanları çekebiliriz.
            var products18 = _db.Products.Select(x => x.Name).ToList();

            //birden fazla alanı çekmek istersek bu durumda ise new keyword ile anonim bir class içerisine alırız
            var products19 = _db.Products.Select(x => new
            {
                Name = x.Name,
                Price = x.Price
            }).ToList();

            //select many ile ise bire çok ilişkili tablolarda koleksiyon içerisinde bir işlem yapabiliriz.
            //kategorinin altındaki ürünlere bağlanıp fiyatı 50 tl üstünde olan ürünlerin filtrelenmesini sağlar.
            var category1 = _db.Categories
                .Include(x=>x.Products)
                .SelectMany(x => x.Products)
                .Where(x => x.Price > 20)
                .ToList();

            //stoğuna göre ürünleri gruplama
            var products21 = _db.Products.GroupBy(x => x.Stock).Select(a => new
            {
                Count = a.Count(),//2
                Name = a.Key //32
            }).ToList();

            //lamda join çok kullanmıyoruz.

            var query = _db.Products.Join(_db.Categories,
                product => product.Category.Id,
                category => category.Id,
                (category, product) => new
                {
                    CategoryName = category.Name,
                    ProductName = product.Name
                }
                ).ToList();

            var query2 = _db.Products.Include(x => x.Category).Select(a => new
            {
                CategoryName = a.Category.Name,
                ProductName = a.Name
            }).ToList();

            var query3 = (from product in _db.Products
                          join category in _db.Categories on product.Category.Id equals category.Id
                          select new
                          {
                              CategoryName = category.Name,
                              ProductName = product.Name
                          }).ToList();

            //sum, count,avarage,max,min aggregate functions

            var totalUnitPrice = _db.Products.Sum(x => x.Price); //decimal döndürür.
            var totalCount = _db.Products.Where(x => x.Price > 50).Count();

            var avgs = _db.Products.Average(x => x.Price);//ortalama 1 ürüne ait birim maliyetini bulur.
            var totalAvgs = _db.Products.Average(x => x.Price * x.Stock); //stoktaki ürünlerin ortalama maliyeti
            var products23 = _db.Products.Where(x => x.Price > _db.Products.Average(x => x.Price)).ToList();
            var product24 = _db.Products.FirstOrDefault(x => x.Price == _db.Products.Max(x => x.Price));

            //kategorileri getir fakat ürünleri fiyatına göre artandan azalana sıralı getir
            var categories10 = _db.Categories.Include(x => x.Products).Select(x => new
            Category {
                Id = x.Id,
                Name = x.Name,
                Products = x.Products.OrderByDescending(y => y.Price).ToList()

            }).ToList();

            //veri çekilince Include ile tüm tavigation property olan koleksiyon alanları içerisibde ramde dönlü tek tek sıralanır.Hatalı yöntemdir. Rami şişir.
            categories10.ForEach(a =>
            {
                a.Products.OrderByDescending(y => y.Price);
            });
            return View();
        }
    }
}
