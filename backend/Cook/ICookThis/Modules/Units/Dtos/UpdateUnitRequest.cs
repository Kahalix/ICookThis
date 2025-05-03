using ICookThis.Modules.Units.Entities;
using System.ComponentModel.DataAnnotations;

namespace ICookThis.Modules.Units.Dtos
{
    public class UpdateUnitRequest
    {
        [MaxLength(5)]
        public required string Symbol { get; set; }

        public UnitType? Type { get; set; }
    }
}