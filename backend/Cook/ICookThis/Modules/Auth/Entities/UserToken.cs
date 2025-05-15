using System;
using System.ComponentModel.DataAnnotations;

namespace ICookThis.Modules.Auth.Entities
{

    public class UserToken
    {
        [Key]
        public Guid Id { get; set; }

        public int UserId { get; set; }
        public TokenType Type { get; set; }
        public DateTime Expiry { get; set; }
    }
}