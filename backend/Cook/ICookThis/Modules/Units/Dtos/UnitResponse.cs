using ICookThis.Modules.Units.Entities;
using YourApp.Modules.Units.Entities;

namespace YourApp.Modules.Units.Dtos
{
    public class UnitResponse
    {
        public int Id { get; set; }
        public required string Symbol { get; set; }
        public UnitType Type { get; set; }
    }
}