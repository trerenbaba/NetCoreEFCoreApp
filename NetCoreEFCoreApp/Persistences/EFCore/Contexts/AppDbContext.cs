using Microsoft.EntityFrameworkCore;
using NetCoreEFCoreApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreEFCoreApp.Persistences.EFCore.Contexts
{
    public class AppDbContext:DbContext
    {
        //IOC ile uygulamımızın belirli bir ayara göre instance alınması için yazıyoruz.Yani biz startup dosyasında buranın MySql,sqlserver,postgres vs ile çalışıtığın söyleyeceğiz .Bu arkadaş da ona göre instance alacak. op=> dediğimiz şey options.
        //yöntem 2 --> bu yöntemi kullanacağız.
        public AppDbContext(DbContextOptions<AppDbContext> opt):base(opt)
        {
            
        }

        //OnConfiguring ile buradaki constructordan instance alırız.
        public AppDbContext()
        {

        }
        public DbSet<Product>Products { get; set; }
        public DbSet<Category> Categories { get; set; }


        //yöntem 1 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies(); // bu kısımda tüm ef core genelinde lazy loading  aktif hale getirdik (lazy:tembel yükleme)
            //uygulama ilk ayağa kalkarken hangi db provider ile çalışacağını buradan söyleriz.
            optionsBuilder.UseSqlServer("Server=.;Database=TestEFCoreDb;Trusted_Connection=true;");

            base.OnConfiguring(optionsBuilder);
        }
    }
}
