using Microsoft.AspNetCore.Mvc;
using SimpleExampleUsingRedis.Models;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace SimpleExampleUsingRedis.Controllers
{
    public class HomeController : Controller
    {
        private readonly CarContext db;

        public HomeController(CarContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await db.Cars.ToListAsync());
        }

        [Route("Index/{sortOrder?}")]
        public async Task<IActionResult> Index(SortState sortOrder = SortState.BrandAsc)
        {
            IQueryable<Car>? cars = db.Cars;

            ViewData["BrandSort"] = sortOrder == SortState.BrandAsc ? SortState.BrandAsc : SortState.BrandDesc;
            ViewData["ModelSort"] = sortOrder == SortState.ModelAsc ? SortState.ModelAsc : SortState.ModelDesc;
            ViewData["EngineCapacitySort"] = sortOrder == SortState.EngineCapacityAsc ? SortState.EngineCapacityAsc : SortState.EngineCapacityDesc;
            ViewData["EnginePowerSort"] = sortOrder == SortState.EnginePowerAsc ? SortState.EnginePowerAsc : SortState.EnginePowerDesc;
            ViewData["CPPSort"] = sortOrder == SortState.CPPAsc ? SortState.CPPAsc : SortState.CPPDesc;
            ViewData["MaxSpeedSort"] = sortOrder == SortState.MaxSpeedAsc ? SortState.MaxSpeedAsc : SortState.MaxSpeedDesc;

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
            return View(await cars.AsNoTracking().ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Car car)
        {
            db.Cars.Add(car);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                Car car = new() { Id = id.Value };
                db.Entry(car).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                Car? user = await db.Cars.FirstOrDefaultAsync(p => p.Id == id);
                if (user != null) return View(user);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Car user)
        {
            db.Cars.Update(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "");
        }
    }
}
