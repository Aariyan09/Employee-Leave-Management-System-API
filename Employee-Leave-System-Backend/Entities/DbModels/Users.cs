using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Employee_Leave_System_Backend.Utility.Enums;

namespace Employee_Leave_System_Backend.Entities.DbModels
{
    public class Users
    {
        /// <summary>
        /// Unique Id for each row
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// User Name
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name should be between 2 and 100 characters.")]
        public string Name { get; set; }

        /// <summary>
        /// User Email
        /// </summary>
        [Required] // Email cannot be null
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        public string Email { get; set; }

        /// <summary>
        /// User Password (stored in HASH)
        /// </summary>
        [Required] // Password Hash must always be present
        public string PasswordHash { get; set; }

        /// <summary>
        /// User Role (Employee or Admin)
        /// </summary>
        [Required] // Role cannot be null
        public RoleType Role { get; set; }
    }
}
