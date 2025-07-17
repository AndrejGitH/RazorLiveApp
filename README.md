# RazorLiveApp

I created a web application where users can:  
✅ **Browse sections** and read articles  
✅ **Register an account** on the website  
✅ **Purchase a product** that unlocks a **premium section** (exclusive for premium users)  

🔗 **Live App:** [tradingjunior.com](https://tradingjunior.com/)  (The website is under reconstruction - hosting is stopped)

---

## 🎨 UI & Design  
- **Built with Bootstrap**  
- **Template Source:** [Business Frontpage](https://startbootstrap.com/template/business-frontpage)  

## 🛠 Tech Stack  
- **IDE:** Visual Studio  
- **Framework:** .NET 8.0  
- **Project Type:** ASP.NET Core Web App (Razor Pages)  
- **Database:** MSSQL (via Entity Framework Core)  
- **Authentication:** ASP.NET Core Identity (Scaffolded)  
- **Payments:** Stripe  

---

## 📂 Project Structure  

### 🌍 **Frontend (`wwwroot/`)**  
- `css/` → Stylesheets  
- `js/` → JavaScript files  
- `images/` → Static assets  

### 🔑 **Authentication & Identity (`Areas/Identity/`)**  
- **ASP.NET Core Identity** manages authentication & user roles  
- **Custom Claims** for premium users  

### 🏦 **Backend & Business Logic**  

#### **Controllers (`Controllers/`)**  
- `PaymentsController.cs` → Handles **Stripe checkout**, webhooks, user premium status updates, and **SMTP notifications**  

#### **Database (`Data/`)**  
- `ApplicationDbContext.cs` → **EF Core ORM** for database schema, migrations, and entity handling  

#### **Models (`Models/`)**  
- Define data structures (`ApplicationUser`, `Purchases`, `Articles`)  

#### **Services (`Services/`)**  
- `EmailService.cs` → Handles **email notifications**  
- `SmtpEmailSender.cs` → **SMTP integration** for transactional emails  
- `CustomClaimsPrincipalFactory.cs` → Manages **user claim data**  

### 📄 **Pages & Razor Components (`Pages/`)**  
- **Razor Pages** serve as **View + Controller combined**  
- **Multiple Layouts** for flexible UI design  

### ⚙ **Core Configurations (`Program.cs`)**  
- **Database Connection & Identity Policies**  
- **Stripe Initialization & API Keys**  
- **Authorization Policies** (e.g., **Premium User Role**)  

