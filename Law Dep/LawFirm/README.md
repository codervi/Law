# Calder & Rowe Attorneys

A website and admin panel for a fictional law firm, built with **ASP.NET Core MVC (.NET 8)**, **Entity Framework Core** and **SQL Server**.

Visitors can browse practice areas and attorneys and book a consultation. Staff sign in to an admin panel to manage content and confirm or cancel appointments.

## Features

**Public site**
- Practice areas and attorney profiles (many-to-many relationship)
- Consultation booking with live availability: time slots load with `fetch()` and hide already-booked times
- Server-side validation and anti-forgery tokens on every form

**Admin panel** (`/Admin`, role `Admin` only)
- Dashboard with counts and next appointments
- Appointments: filter by status, pagination, confirm and cancel
- Attorneys and practice areas: create, edit, delete

## Tech stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core MVC, .NET 8, C# 12 |
| Data | EF Core 8 (Code First, migrations), SQL Server / LocalDB |
| Auth | ASP.NET Core Identity, cookie auth, role-based authorization, lockout after 5 failed attempts |
| Frontend | HTML, CSS (custom, no framework), vanilla JavaScript |

## Architecture

```
Controllers / Areas/Admin/Controllers   ->  HTTP, validation, view selection
Services (IAppointmentService)          ->  business rules (working hours, no double booking)
Services (IRepository<T>)               ->  data access wrapper over EF Core
Data (AppDbContext, SeedData)           ->  model configuration, seed data
Models / ViewModels                     ->  entities vs. form models
```

Design decisions worth knowing:

- **Double-booking is prevented twice.** The service checks free slots first. A filtered unique index on `(LawyerId, PreferredDate)` where status is not `Cancelled` guarantees it at database level when two people submit at the same moment.
- **View models for forms** (`AppointmentCreateVm`, `LawyerFormVm`) keep entities safe from over-posting. The practice area form uses `[Bind]` for the same reason.
- **Enum stored as text** (`Pending`, `Confirmed`, `Cancelled`) so the table is readable in SSMS.
- **`AsNoTracking()`** on all read-only queries.
- **Admin is an MVC Area** with `[Authorize(Roles = "Admin")]` on every controller.

## Run it locally

Requirements: .NET 8 SDK, SQL Server LocalDB (installed with Visual Studio) or any SQL Server instance.

```bash
cd src/LawFirm.Web

# 1. Create the first migration (only once)
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate

# 2. Run. The app applies migrations and seeds demo data on startup.
dotnet run
```

Open the HTTPS URL printed in the console.

Demo admin account (from `appsettings.json`, **development only**):

```
Email:    admin@calderrowe.test
Password: Admin#12345
```

To use a different SQL Server, change `ConnectionStrings:DefaultConnection`. For anything beyond local use, store the admin credentials with `dotnet user-secrets` instead of `appsettings.json`.

## Possible next steps

- Email notification when an appointment is confirmed
- Unit tests for `AppointmentService` (xUnit + EF Core InMemory/SQLite)
- Image upload for attorney photos
