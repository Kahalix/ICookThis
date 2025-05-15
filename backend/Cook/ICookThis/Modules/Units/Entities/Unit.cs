using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ICookThis.Modules.Units.Entities
{
    public class Unit
    {
        public int Id { get; set; }

        [Required, MaxLength(5)]
        public required string Symbol { get; set; }

        [Required]
        public UnitType Type { get; set; }
    }
}