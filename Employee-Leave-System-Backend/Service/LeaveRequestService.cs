using Employee_Leave_System_Backend.Data;
using Employee_Leave_System_Backend.Entities.DbModels;
using Employee_Leave_System_Backend.Entities.DTO;
using Microsoft.EntityFrameworkCore;
using static Employee_Leave_System_Backend.Utility.Enums;

namespace Employee_Leave_System_Backend.Service
{

    public interface ILeaveRequestService
    {
        /// <summary>
        /// User submits leave request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<bool> SubmitLeaveRequestAsync(LeaveRequest request);  // 
        /// <summary>
        /// Get user's leave history
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<LeaveRequest>> GetLeaveRequestsByUserAsync(int userId);
        /// <summary>
        /// Admin: View all leave requests
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<LeaveRequest>> GetAllLeaveRequestsAsync();
        /// <summary>
        /// Approve leave request
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> ApproveLeaveRequestAsync(int id);
        /// <summary>
        /// Admin: Reject leave request
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> RejectLeaveRequestAsync(int id);
        /// <summary>
        /// User: Cancel pending request
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> CancelLeaveRequestAsync(int id, int userId);

        /// <summary>
        /// Admin : Generate Leave Report
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<ReportData>> GenerateLeaveReportAsync(ReportFilterDTO filter);
        /// <summary>
        /// Admin : Track each leave usage per employee
        /// </summary>
        /// <returns></returns>
        Task<List<EmployeeLeaveBalanceDto>> GetEmployeeLeaveBalancesAsync();
    }

    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly SQLDBContext _context;

        // Injecting Database Context
        public LeaveRequestService(SQLDBContext context)
        {
            _context = context;
        }


        // Submit a new leave request
        public async Task<bool> SubmitLeaveRequestAsync(LeaveRequest request)
        {
            _context.LeaveRequests.Add(request);
            return await _context.SaveChangesAsync() > 0;
        }

        // Get leave requests for a specific user
        public async Task<IEnumerable<LeaveRequest>> GetLeaveRequestsByUserAsync(int userId)
        {
            return await _context.LeaveRequests
                .Where(lr => lr.UserId == userId)
                .ToListAsync();
        }

        // Get all leave requests (Admin)
        public async Task<IEnumerable<LeaveRequest>> GetAllLeaveRequestsAsync()
        {
            return await _context.LeaveRequests.Include(lr => lr.User).ToListAsync();
        }

        // Approve leave request (Admin)
        public async Task<bool> ApproveLeaveRequestAsync(int id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null) return false;

            leaveRequest.Status = LeaveStatus.Approved;
            return await _context.SaveChangesAsync() > 0;
        }

        // Reject leave request (Admin)
        public async Task<bool> RejectLeaveRequestAsync(int id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null) return false;

            leaveRequest.Status = LeaveStatus.Rejected;
            return await _context.SaveChangesAsync() > 0;
        }

        // Cancel leave request (User)
        public async Task<bool> CancelLeaveRequestAsync(int id, int userId)
        {
            var leaveRequest = await _context.LeaveRequests
                .Where(lr => lr.Id == id && lr.UserId == userId && lr.Status == LeaveStatus.Pending)
                .FirstOrDefaultAsync();

            if (leaveRequest == null) return false;

            _context.LeaveRequests.Remove(leaveRequest);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<ReportData>> GenerateLeaveReportAsync(ReportFilterDTO filter)
        {
            var query = _context.LeaveRequests.AsQueryable();

            // Apply filters only if values are provided
            if (!string.IsNullOrWhiteSpace(filter.StartDate))
            {
                var startDate = DateTime.Parse(filter.StartDate);
                query = query.Where(x => x.StartDate >= startDate);
            }

            if (!string.IsNullOrWhiteSpace(filter.EndDate))
            {
                var endDate = DateTime.Parse(filter.EndDate);
                query = query.Where(x => x.EndDate <= endDate);
            }

            if (filter.LeaveType != null && filter.LeaveType != 0)
            {
                query = query.Where(x => x.LeaveType == (LeaveType)filter.LeaveType);
            }

            // Fetch the filtered data
            var data = await query
                .Select(x => new ReportData
                {
                    UserName = x.User.Name,
                    UserEmail = x.User.Email,
                    StartDate = x.StartDate.ToString("yyyy-MM-dd"),
                    EndDate = x.EndDate.ToString("yyyy-MM-dd"),
                    LeaveType = x.LeaveType.ToString(),
                    Status = x.Status.ToString(),
                    AppliedOn = x.AppliedOn.ToString("yyyy-MM-dd")
                })
                .ToListAsync();


            return data;
        }


        public async Task<List<EmployeeLeaveBalanceDto>> GetEmployeeLeaveBalancesAsync()
        {
            var leaveBalances = await _context.LeaveRequests
                .Where(lr => lr.Status == LeaveStatus.Approved) // Only count approved leaves
                .GroupBy(lr => new { lr.UserId, lr.User.Name }) // Group by User
                .Select(g => new EmployeeLeaveBalanceDto
                {
                    EmployeeName = g.Key.Name,
                    SickLeaveBalance = g.Count(lr => lr.LeaveType == LeaveType.Sick),
                    CasualLeaveBalance = g.Count(lr => lr.LeaveType == LeaveType.Casual),
                    VacationLeaveBalance = g.Count(lr => lr.LeaveType == LeaveType.Vacation),
                    WeddingLeaveBalance = g.Count(lr => lr.LeaveType == LeaveType.Wedding)
                })
                .ToListAsync();


            return leaveBalances;
        }

    }
}
