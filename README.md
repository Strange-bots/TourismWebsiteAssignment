Troubleshooting
1) Fix: Missing Roslyn compiler (bin\roslyn\csc.exe)

Error example

Could not find a part of the path ...\bin\roslyn\csc.exe

System.IO.DirectoryNotFoundException ... csc.exe

Why it happens
This project uses the Roslyn compiler for runtime compilation (Razor/views). If the NuGet package isn’t restored correctly, the bin\roslyn folder (and csc.exe) won’t exist, so the app crashes on run.

Fix
Run this in Visual Studio:

Open:
Tools → NuGet Package Manager → Package Manager Console

Run:

Install-Package Microsoft.CodeDom.Providers.DotNetCompilerPlatform


Then:
Build → Rebuild Solution

This will restore the package and generate the required bin\roslyn files.

2) Fix: Database errors (reset LocalDB database)

If you still get database/migration issues (FK errors, broken schema, unexpected data), reset the local database.

What this does
The command forces the database into SINGLE_USER mode (kicks out any active connections), then drops the database so Entity Framework can recreate it cleanly on the next run/migration.

Run in Windows Terminal (PowerShell or CMD)

Close the app first (and stop debugging in Visual Studio), then run:

sqlcmd -S "(localdb)\MSSQLLocalDB" -E -Q "ALTER DATABASE [TourismWebsiteAssignmentContext] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [TourismWebsiteAssignmentContext];"


After dropping the DB

Run the project again, or run migrations/update database (depending on your setup).

The database should be recreated automatically.

Notes

-S "(localdb)\MSSQLLocalDB" connects to LocalDB

-E uses Windows authentication

ROLLBACK IMMEDIATE forces disconnect of anything using the DB

Quick checklist if errors persist

Make sure NuGet packages are restored (Rebuild Solution)

Stop IIS Express / stop debugging before dropping DB

Confirm your connection string uses LocalDB and the correct DB name

----------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------\
---------------------------------------------------------------------------------------------------]


Test Login Credentials

Use the following accounts to access different parts of the system:

🔐 Admin Dashboard
Username: AdminSachin
Password: sachin123


Access: Admin Dashboard, User Management, Roles, Agencies, Packages, Bookings, Payments, Feedback

🏢 Agency Dashboard
Username: maniAgency
Password: helloworld


Access: Agency Dashboard, Create & Manage Packages, View Bookings, Manage Agency Profile

👤 User / Tourist Access
Username: Strange-bots
Password: helloworld


Access: Browse Packages, Make Bookings, View Booking Status, Submit Feedback

Important Notes

Role-based access control is implemented (Admin, Agency, Tourist).

Users cannot access pages outside their assigned role.

All passwords are stored securely using hashing.
