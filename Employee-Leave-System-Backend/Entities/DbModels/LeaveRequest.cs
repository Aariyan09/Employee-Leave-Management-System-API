using static Employee_Leave_System_Backend.Utility.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Employee_Leave_System_Backend.Entities.DbModels
{
    public class LeaveRequest
    {
        /// <summary>
        /// Unique Id for each row
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// User Id <-> (Foreign Key relationship with Users tbl id)
        /// </summary>
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public Users User { get; set; }  // Navigation property

        /// <summary>
        /// Type of Leave
        /// </summary>
        [Required]
        public LeaveType LeaveType { get; set; }

        /// <summary>
        /// Start Date of leave
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Leave End Date
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Reason for Leave
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Reason { get; set; }

        /// <summary>
        /// Current Status of Leave (Default Pending)
        /// </summary>
        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;

        /// <summary>
        /// Leave Applied On (Default current date)
        /// </summary>
        public DateTime AppliedOn { get; set; } = DateTime.UtcNow;

    }
}
