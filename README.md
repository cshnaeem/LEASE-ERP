Installation
1. Clone the Repository

bash

git clone https://github.com/cshnaeem/LEASE-ERP.git

2. Set Up the Database

    Create a new SQL Server database.
    Run the scripts in the DatabaseScripts folder to set up the initial schema.

3. Configure the Application

    Open Web.config and update the connection string to point to your SQL Server instance.

xml

<connectionStrings>
  <add name="LeasingERPContext" connectionString="Server=YOUR_SERVER;Database=LeasingERP;User Id=YOUR_USER;Password=YOUR_PASSWORD;" providerName="System.Data.SqlClient" />
</connectionStrings>

4. Restore NuGet Packages

    Open the solution in Visual Studio.
    Right-click on the solution and select "Restore NuGet Packages".


5. Build and Run

    Build the solution in Visual Studio.
    Run the application (F5 or Ctrl+F5).
