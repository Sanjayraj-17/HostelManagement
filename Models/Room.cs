
using System.ComponentModel.DataAnnotations;

namespace Hostel_Room_Management.Models
{
    public class Room : IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string RoomNumber { get; set; } = string.Empty;

        [Required]
        [RegularExpression(
            "^(Single|Shared)$",
            ErrorMessage = "Room type must be Single or Shared.")]
        public string RoomType { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }

        [Range(0, int.MaxValue)]
        public int OccupiedBeds { get; set; }

        [Range(typeof(decimal), "0", "999999999999.99")]
        public decimal MonthlyFee { get; set; }

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (OccupiedBeds > Capacity)
            {
                yield return new ValidationResult(
                    "Occupied beds cannot exceed capacity.",
                    new[]
                    {
                        nameof(OccupiedBeds),
                        nameof(Capacity)
                    });
            }
        }
    }
}