namespace Employee_Leave_System_Backend.Entities.DTO
{
    public record LoginDto(string Email, string Password);
    public record RegisterDto(string Name, string Email, string Password);
}
