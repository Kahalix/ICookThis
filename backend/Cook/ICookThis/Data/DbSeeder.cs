using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ICookThis.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(CookThisDbContext db)
        {
            // get all types implementing ISeeder
            var seederTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(ISeeder).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .Select(t => new {
                    Type = t,
                    Order = t.GetCustomAttribute<SeederOrderAttribute>()?.Order ?? 0
                })
                .OrderBy(x => x.Order)
                .Select(x => x.Type);

            foreach (var type in seederTypes)
            {
                var seeder = (ISeeder)Activator.CreateInstance(type)!;
                await seeder.SeedAsync(db);
            }
        }
    }
}