
namespace SimpleExampleUsingRedis.Models
{
    public interface ICar
    {
        void Move();
        void Refuel();
        void Braking();
        void EngineStart();
        void EngineStop();
    }

    public class Car : ICar
    {
        public int Id { get; set; } // Первичный ключ
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public float EngineCapacity { get; set; }
        public int EnginePower { get; set; }
        public string? CPP { get; set; }
        public float MaxSpeed { get; set; }

        #region Methods
        public void Move()
        {
            Console.WriteLine($"{Brand} {Model} is moving with a speed of {MaxSpeed} km/h.");
        }

        public void Refuel()
        {
            Console.WriteLine($"{Brand} {Model} is refueling.");
        }

        public void Braking()
        {
            Console.WriteLine($"{Brand} {Model} is braking.");
        }

        public void EngineStart()
        {
            Console.WriteLine($"The engine of {Brand} {Model} is starting.");
        }

        public void EngineStop()
        {
            Console.WriteLine($"The engine of {Brand} {Model} is stopping.");
        }
        #endregion
    }

    public enum SortState
    {
        BrandAsc,
        BrandDesc,
        ModelAsc,
        ModelDesc,
        EngineCapacityAsc,
        EngineCapacityDesc,
        EnginePowerAsc,
        EnginePowerDesc,
        CPPAsc,
        CPPDesc,
        MaxSpeedAsc,
        MaxSpeedDesc
    }

    public class CarsGenerator
    {
        private static readonly string[] _brand = { "BMW", "KIA", "Mercedes", "Ford", "Audi", "Toyota", "Hyundai", "Chevrolet", "Nissan", "Volkswagen", "Honda", "Peugeot", "Ferrari", "Porsche", "Lamborghini" };
        private static readonly string[] _model = { "M2", "M3", "M4", "M5", "A6", "A8", "Camry", "Corolla", "Elantra", "Cruze", "Altima", "Golf", "Civic", "208", "488", "911", "Huracan" };
        private static readonly float[] _engineCapacity = { 1.2f, 1.5f, 1.7f, 1.9f, 2.2f, 2.5f, 2.7f, 3f, 3.2f, 3.5f, 3.8f, 4.0f, 4.2f, 4.5f, 4.7f, 5.0f, 5.2f, 5.5f, 5.7f, 6.0f };
        private static readonly int[] _enginePower = { 150, 170, 200, 220, 240, 260, 290, 310, 330, 350, 400, 420, 450, 480, 510, 540, 570, 600, 630, 660 };
        private static readonly string[] _cpp = { "Automatic", "Manual", "Semi-automatic", "Continuously Variable", "Dual Clutch", "Direct Shift Gearbox", "Tiptronic", "Steptronic", "S Tronic", "PDK" };
        private static readonly float[] _maxSpeed = { 100, 120, 150, 170, 190, 200, 220, 250, 270, 290, 320, 350, 380, 410, 440, 470, 500, 530, 560, 590 };

        private static readonly Random rand = new();

        public static async Task GenerateRandomCarAsync(CarContext db, int countCars)
        {
            for (int i = 0; i < countCars; i++)
            {
                var car = new Car
                {
                    Brand = _brand[rand.Next(_brand.Length)],
                    Model = _model[rand.Next(_model.Length)],
                    EngineCapacity = _engineCapacity[rand.Next(_engineCapacity.Length)],
                    EnginePower = _enginePower[rand.Next(_enginePower.Length)],
                    CPP = _cpp[rand.Next(_cpp.Length)],
                    MaxSpeed = _maxSpeed[rand.Next(_maxSpeed.Length)]
                };

                db.Cars.Add(car);
                await db.SaveChangesAsync();
            }
        }
    }


}