using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects
{
    public class EmployeeForUpdateDto
    {
        [Required]
        public string Name { get; set; }
        [Range(15,55,ErrorMessage ="سن فرد زیاد یا کم است")]
        public int Age { get; set; }
        public string Position { get; set; }
    }
}
