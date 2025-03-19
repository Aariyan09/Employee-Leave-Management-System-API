namespace Employee_Leave_System_Backend.Entities.DTO
{
    public record LeaveRequestDTO(int LeaveType,string StartDate,string EndDate, string Reason);
}
