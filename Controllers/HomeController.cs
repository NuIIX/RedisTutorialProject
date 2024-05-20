using Microsoft.AspNetCore.Mvc;
using SimpleExampleUsingRedis.Models;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;

namespace SimpleExampleUsingRedis.Controllers
{
    public class HomeController : Controller
    {
        private readonly CarContext _db;
        private readonly IRedisCacheService _redis;
        private const string CacheKey = "carsList";

        public HomeController(CarContext context, IRedisCacheService redis)
        {
            _db = context;
            _redis = redis;
        }

        public async Task<IActionResult> Index()
        {
            var carsList = await _redis.GetAsync(CacheKey);

            if (carsList is null)
            {
                var cars = await _db.Cars.AsNoTracking().ToListAsync();
                await _redis.SetAsync(CacheKey, JsonSerializer.Serialize(cars));
                Console.WriteLine($"{cars[0]} извлечен из базы данных");
                return View("Index", cars);
            }
            else
            {
                var cars = JsonSerializer.Deserialize<List<Car>>(carsList);
                Console.WriteLine($"{cars[0]} извлечен из кэша");
                return View("Index", cars);
            }
        }

        [Route("Index/{sortOrder?}")]
        public async Task<IActionResult> Index(SortState sortOrder = SortState.BrandAsc)
        {
            var carsList = await _redis.GetAsync(CacheKey);

            if (carsList is null)
            {
                IQueryable<Car> cars = _db.Cars;

                cars = sortOrder switch
                {
                    SortState.BrandDesc => cars.OrderByDescending(s => s.Brand),
                    SortState.ModelAsc => cars.OrderBy(s => s.Model),
                    SortState.ModelDesc => cars.OrderByDescending(s => s.Model),
                    SortState.EngineCapacityAsc => cars.OrderBy(s => s.EngineCapacity),
                    SortState.EngineCapacityDesc => cars.OrderByDescending(s => s.EngineCapacity),
                    SortState.EnginePowerAsc => cars.OrderBy(s => s.EnginePower),
                    SortState.EnginePowerDesc => cars.OrderByDescending(s => s.EnginePower),
                    SortState.CPPAsc => cars.OrderBy(s => s.CPP),
                    SortState.CPPDesc => cars.OrderByDescending(s => s.CPP),
                    SortState.MaxSpeedAsc => cars.OrderBy(s => s.MaxSpeed),
                    SortState.MaxSpeedDesc => cars.OrderByDescending(s => s.MaxSpeed),
                    _ => cars.OrderBy(s => s.Brand)
                };

                var sortedCars = await cars.AsNoTracking().ToListAsync();
                await _redis.SetAsync(CacheKey, JsonSerializer.Serialize(sortedCars));
                Console.WriteLine($"{sortedCars[0]} извлечен из базы данных");
                return View(sortedCars);
            }
            else
            {
                var cars = JsonSerializer.Deserialize<List<Car>>(carsList);
                Console.WriteLine($"{cars[0]} извлечен из кэша");
                return View(cars);
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Car car)
        {
            _db.Cars.Add(car);
            await _db.SaveChangesAsync();
            await UpdateCache();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                Car car = new() { Id = id.Value };
                _db.Entry(car).State = EntityState.Deleted;
                await _db.SaveChangesAsync();
                await UpdateCache();
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                var carsList = await _redis.GetAsync(CacheKey);
                if (carsList is not null)
                {
                    var cars = JsonSerializer.Deserialize<List<Car>>(carsList);
                    Car? car = cars.FirstOrDefault(p => p.Id == id);
                    if (car != null) return View(car);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Car car)
        {
            _db.Cars.Update(car);
            await _db.SaveChangesAsync();
            await UpdateCache();
            return RedirectToAction("Index","");
        }

        private async Task UpdateCache()
        {
            var cars = await _db.Cars.AsNoTracking().ToListAsync();
            await _redis.SetAsync(CacheKey, JsonSerializer.Serialize(cars));
        }
    }
}
