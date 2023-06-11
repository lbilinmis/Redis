using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Redis.InMemory.WebUI.Models;

namespace Redis.InMemory.WebUI.Controllers
{
    public class ProductController : Controller
    {

        private readonly IMemoryCache _memoryCache;

        public ProductController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            //if (string.IsNullOrEmpty(_memoryCache.Get<string>("zaman")))
            //{
            //    _memoryCache.Set<string>("zaman", DateTime.Now.ToString());
            //}

            // ya da 

            //if (_memoryCache.TryGetValue("zaman", out string zamanCache))
            //{
            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
            options.AbsoluteExpiration = DateTime.Now.AddSeconds(10);
            // aşağıdaki kod blogığ 10 sn ye de tekrar okuma işlemi yapılıp okunduğunda
            //cache den okumaya devam edecel her istek 10 sn kadar cache de tutulmasını sağlar

            //options.SlidingExpiration = TimeSpan.FromSeconds(10);
            options.Priority = CacheItemPriority.High;
            // cache önem sırası ona göre cache den silme işlemi yaoılmasını sağlar

            //delege PostEvictionDelegate çağrılıyor
            options.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                _memoryCache.Set("callback", $"{key}->{value}=> Sebebi :{reason}");
            });

            _memoryCache.Set<string>("zaman", DateTime.Now.ToString(), options);
            //}


            Product product = new Product();
            product.Price = 50;
            product.Name = "Mouse";
            product.Id = 1;

            _memoryCache.Set<Product>("product:1", product);

            return View();
        }

        public IActionResult Show()
        {
            //_memoryCache.GetOrCreate<string>("zaman", entry =>
            //{
            //    return DateTime.Now.ToString();
            //});

            //ViewBag.Cache_Zaman = _memoryCache.Get<string>("zaman");



            _memoryCache.TryGetValue("zaman", out string zamanCache);
            _memoryCache.TryGetValue("callback", out string callback);

            ViewBag.Cache_Zaman = zamanCache;
            ViewBag.Cache_Callback = callback;
            ViewBag.Cache_Product = _memoryCache.Get<Product>("product:1");
            return View();
        }
    }
}
