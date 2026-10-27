2. **Store secrets** (from the project folder). Replace the placeholders:
   ```powershell
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "server=localhost;port=3306;database=CollegeComplaintDB;user=root;password=YOUR_MYSQL_PASSWORD"
   dotnet user-secrets set "AdminSeed:Email" "admin@college.edu"
   dotnet user-secrets set "AdminSeed:Password" "Admin@12345"
   ```
   In Visual Studio you can instead right-click the project, choose **Manage User Secrets**, and paste:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "server=localhost;port=3306;database=CollegeComplaintDB;user=root;password=YOUR_MYSQL_PASSWORD"
     },
     "AdminSeed": { "Email": "admin@college.edu", "Password": "Admin@12345" }
   }
   ```
3. **Create the migration and the database:**
   ```powershell
   dotnet restore
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
   Visual Studio alternative (Package Manager Console): `Add-Migration InitialCreate`, then `Update-Database`.
4. **Run:** `dotnet run` or F5.

The app also applies pending migrations and creates the roles and admin account automatically at startup. You do not need to create any tables by hand. The database is created automatically if it doesn't exist, or you can create it yourself first:

```sql
CREATE DATABASE CollegeComplaintDB CHARACTER SET utf8mb4;
```

Open the URL printed in the terminal (Visual Studio profile: `https://localhost:7100`, or `http://localhost:5100`).

```bash
git init
git add .
git commit -m "College Complaint Management System"
git branch -M main
git remote add origin https://github.com/YOUR_USERNAME/CollegeComplaintManagement.git
git push -u origin main
```


