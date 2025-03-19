namespace Employee_Leave_System_Backend.Entities.DTO
{
    public record ReportFilterDTO(string? StartDate, string? EndDate, int LeaveType);

    public class ReportData
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string LeaveType { get; set; }
        public string Status { get; set; }
        public string AppliedOn { get; set; }
    }
}
