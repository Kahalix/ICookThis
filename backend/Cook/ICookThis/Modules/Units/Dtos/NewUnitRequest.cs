using ICookThis.Modules.Units.Entities;
using System.ComponentModel.DataAnnotations;
using YourApp.Modules.Units.Entities;

namespace YourApp.Modules.Units.Dtos
{
    public class NewUnitRequest
    {
        [Required, MaxLength(5)]
        public required string Symbol { get; set; }

        [Required]
        public UnitType Type { get; set; }
    }
}
