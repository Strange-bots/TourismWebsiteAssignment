# TourismWebsiteAssignment

Bash the Code in the Windows Terminal

sqlcmd -S "(localdb)\MSSQLLocalDB" -E -Q "ALTER DATABASE [TourismWebsiteAssignmentContext] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [TourismWebsiteAssignmentContext];"