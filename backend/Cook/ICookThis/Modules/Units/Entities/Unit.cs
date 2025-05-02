using ICookThis.Modules.Units.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace YourApp.Modules.Units.Entities
{
    public class Unit
    {
        public int Id { get; set; }

        [Required, MaxLength(5)]
        public required string Symbol { get; set; }    // "g","kg","ml","l","szt."

        [Required]
        public UnitType Type { get; set; }
    }
}
