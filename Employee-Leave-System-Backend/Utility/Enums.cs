namespace Employee_Leave_System_Backend.Utility
{
    public class Enums
    {
        public enum RoleType
        { 
            User = 1,
            Admin = 2
        }

        public enum LeaveStatus
        {
            Pending = 0,
            Approved = 1,
            Rejected = -1
        }

        public enum LeaveType
        {
            Sick = 1,
            Casual = 2,
            Vacation = 3,
            Wedding = 4
        }
    }
}
