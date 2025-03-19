namespace Employee_Leave_System_Backend.Entities.DTO
{
    public class EmployeeLeaveBalanceDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int SickLeaveBalance { get; set; }
        public int CasualLeaveBalance { get; set; }
        public int VacationLeaveBalance { get; set; }
        public int WeddingLeaveBalance { get; set; }
    }
}
