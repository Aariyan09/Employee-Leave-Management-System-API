using Employee_Leave_System_Backend.Entities.DbModels;
using Employee_Leave_System_Backend.Entities.DTO;
using Employee_Leave_System_Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Employee_Leave_System_Backend.Controllers
{
    [Authorize] // Using policy for Authorization
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestController : ControllerBase
    {
        #region Initialization
        private readonly ILeaveRequestService _leaveRequestService;

        public LeaveRequestController(ILeaveRequestService leaveRequestService)
        {
            _leaveRequestService = leaveRequestService;
        }
        #endregion

        #region Admin

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("admin")]
        /// <summary>
        /// Get All Leave Requests 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetAllLeaveRequests()
        {
            var leaveRequests = await _leaveRequestService.GetAllLeaveRequestsAsync();
            return Ok(leaveRequests);
        }


        [Authorize(Policy = "AdminPolicy")]
        [HttpPut("approve/{id}")]
        /// <summary>
        /// Approve Leave Request (Admin) 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> ApproveLeaveRequest(int id)
        {
            var result = await _leaveRequestService.ApproveLeaveRequestAsync(id);
            return result ? Ok() : NotFound();
        }


        [Authorize(Policy = "AdminPolicy")]
        [HttpPut("reject/{id}")]
        /// <summary>
        /// Reject Leave Request (Admin) 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> RejectLeaveRequest(int id)
        {
            var result = await _leaveRequestService.RejectLeaveRequestAsync(id);
            return result ? Ok() : NotFound();
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("reports/generate-excel")]
        public async Task<IActionResult> GenerateExcelReport([FromBody] ReportFilterDTO filters)
        {
            var data = await _leaveRequestService.GenerateLeaveReportAsync(filters);
            return Ok(data);
        }


        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("admin/leave-balances")]
        public async Task<IActionResult> GetEmployeeLeaveBalancesAsync()
        {
            var data = await _leaveRequestService.GetEmployeeLeaveBalancesAsync();
            return Ok(data);
        }



        #endregion

        #region Employee

        [Authorize(Policy = "UserPolicy")]
        //[Authorize]
        [HttpPost]
        // Submit Leave Request (User)
        public async Task<IActionResult> SubmitLeaveRequest([FromBody] LeaveRequestDTO request)
        {
            if (request == null)
                return BadRequest("Invalid request");

            // Get UserId from JWT token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            #region Map DTO to Model Entity
            LeaveRequest requestForAdd = new();
            requestForAdd.UserId = userId;
            requestForAdd.LeaveType = (Utility.Enums.LeaveType)request.LeaveType;
            requestForAdd.StartDate = DateTime.Parse(request.StartDate);
            requestForAdd.EndDate = DateTime.Parse(request.EndDate);
            requestForAdd.Reason = request.Reason;
            #endregion

            var result = await _leaveRequestService.SubmitLeaveRequestAsync(requestForAdd);
            if (result)
                return Ok();
            else
                return BadRequest();
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpGet("user/{userId}")]
        // Get Leave History (User)
        public async Task<IActionResult> GetUserLeaveRequests(int userId)
        {
            var leaveRequests = await _leaveRequestService.GetLeaveRequestsByUserAsync(userId);
            return Ok(leaveRequests);
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpDelete("{id}")]
        // Cancel Leave Request (User)
        public async Task<IActionResult> CancelLeaveRequest(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _leaveRequestService.CancelLeaveRequestAsync(id, userId);
            if (result)
                return Ok();
            else
                return BadRequest();
        }

        #endregion

    }
}
