Backend API - Leave Management System

Prerequisites

Make sure you have the following installed on your system:

- .NET 6.0 or later

- SQL Server

- Postman (optional, for testing APIs)

Setup Instructions

Clone the repository

- git clone <your-repo-url>
- cd backend-api-folder

Configure the database

- Update appsettings.json with your database connection string.

Example:

"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=LeaveManagementDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}

- Run Migrations (if using Entity Framework Core)

- dotnet ef database update

Run the application

- dotnet run

- Access API Endpoints

The API will run at http://localhost:5000 (or the configured port).

Use Postman or Swagger UI (http://localhost:5000/swagger) to test.

Admin Credentials

- Email: admin@google.com

- Password: Admin@123
