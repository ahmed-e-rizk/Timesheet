## 🕒 Timesheet Project

### 📋 Overview

Timesheet is a multi-layered web application built using **.NET 8**, implementing clean architecture principles with separation of concerns across the following layers:

* `Timesheet.Api` – RESTful Web API
* `Timesheet.Client` – ASP.NET Core MVC Client
* `Timesheet.BLL` – Business Logic Layer
* `Timesheet.Core` – Core Interfaces and Models
* `Timesheet.DTO` – Data Transfer Objects
* `Timesheet.Helper` – Common Helpers
* `Timesheet.Repositroy` – Data Access (Generic Repository)
* `Timesheet.tests` – Unit Tests

---

### ⚙️ Requirements

* [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
* SQL Server

---

### 🚀 Getting Started

1. **Clone the Repository**

   ```bash
   git clone <https://github.com/ahmed-e-rizk/Timesheet>
   ```

2. **Setup the Database**

   * Execute the provided SQL script in your SQL Server instance.
   * Change ConnectionString in Appsetting with new path of  database 

3. **Build the Solution**

   * Open the solution in Visual Studio.
   * Build the entire solution to restore packages and compile all projects.

4. **Run the Application**

   * Set the startup projects to:

     * `Timesheet.Api`
     * `Timesheet.Client`
   * Run both projects simultaneously in **debug mode**.

---

### 🌐 Client (MVC)

The client is built using **ASP.NET Core MVC** and calls the API to manage:

* User Registration
* Login / JWT Authentication
* Home Page (requires authentication)
* Logout Button (shown only when logged in)

The UI is designed using **JavaScript** and the **Bootstrap** library.

---

### 🔐 Authentication & Validation

* ✅ JWT Authentication
* ✅ Cookie-based Session Support
* ✅ FluentValidation for all forms (ensures required fields are not null or empty)
* ✅ Middleware for token handling and global exception handling
* ✅ log4net
---

### 🧹 Architecture & Tools Used

* ✅ Clean Architecture
* ✅ Generic Repository Pattern
* ✅ AutoMapper
* ✅ Generic API Response Wrapping
* ✅ Middleware
* ✅ FluentValidation
* ✅ Bootstrap + JavaScript UI
* ✅ Unit Tests in `Timesheet.tests`

---

### 📁 Folder Structure (Projects)

```
Timesheet.sln
 ├── Timesheet.Api
 ├── Timesheet.BLL
 ├── Timesheet.Client
 ├── Timesheet.Core
 ├── Timesheet.DTO
 ├── Timesheet.Helper
 ├── Timesheet.Repositroy
 └── Timesheet.tests
```

---

### 🧪 Testing

Run tests from `Timesheet.tests` project using Test Explorer.

---

### ✅ Notes

* Make sure the SQL Server is running and accessible.
* Change Conection String with new path 
* Tokens are stored in cookies and validated using middleware.
* API and Client should always run together for full functionality.
