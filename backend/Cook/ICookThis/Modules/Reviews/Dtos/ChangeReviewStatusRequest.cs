using ICookThis.Modules.Reviews.Entities;
using System.ComponentModel.DataAnnotations;

namespace ICookThis.Modules.Reviews.Dtos
{
    public class ChangeReviewStatusRequest
    {

        [Required]
        public ReviewStatus Status { get; set; }
    }
}
