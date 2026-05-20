# FUNews Management System

PRN222 Assignment 02 - Building a News Management website with ASP.NET Core Razor Pages and SignalR

## Project Overview

This is a comprehensive News Management System developed using ASP.NET Core Razor Pages with SignalR. The system implements a 3-layer architecture with Repository pattern and provides role-based access control for different user types.

## Features

### Authentication & Authorization
- **Default Admin Account**: 
  - Email: `admin@FUNewsManagementSystem.org`
  - Password: `@@abc123@@`
- **Role-based Access Control**:
  - **Admin**: Full system access, account management, statistics
  - **Staff**: News/category management, profile management
  - **Lecturer**: View active news only

### Core Functionality
1. **News Management** (Staff/Admin)
   - Create, Read, Update, Delete news articles
   - Real-time updates using SignalR
   - Tag management for news articles
   - Popup dialogs for CRUD operations
   - Search functionality

2. **Category Management** (Staff/Admin)
   - Hierarchical category structure
   - Active/Inactive status management
   - Delete protection for categories with associated news

3. **Account Management** (Admin only)
   - User account CRUD operations
   - Role assignment (Staff/Lecturer)
   - Email uniqueness validation

4. **Profile Management** (All authenticated users)
   - Update personal information
   - Password change functionality
   - Account statistics

5. **Statistics & Reports** (Admin only)
   - Date range filtering
   - News statistics with charts
   - Category and creator analytics

6. **News Viewing** (Public)
   - Browse news articles
   - Category filtering
   - Search functionality
   - Responsive card layout

## Technical Implementation

### Architecture
- **3-Layer Architecture**:
  - **Presentation Layer**: Razor Pages
  - **Business Layer**: Services and Repositories
  - **Data Layer**: Entity Framework Core

### Design Patterns
- **Repository Pattern**: Generic and specific repositories
- **Service Layer**: Business logic separation
- **Singleton Pattern**: SignalR Hub management

### Technologies Used
- **.NET 10.0**
- **ASP.NET Core Razor Pages**
- **Entity Framework Core 8.0**
- **SignalR** for real-time communication
- **SQL Server** database
- **Bootstrap 5** for UI
- **jQuery** for client-side interactions

### Database Schema
The system uses the `FUNewsManagement` database with the following tables:
- `SystemAccount` - User accounts and roles
- `Category` - News categories (hierarchical)
- `NewsArticle` - News articles
- `Tag` - News tags
- `NewsTag` - Many-to-many relationship between news and tags

## Project Structure

```
NguyenBinhAnRazorPages/
├── Pages/
│   ├── Login.cshtml              # Authentication page
│   ├── Index.cshtml              # Dashboard
│   ├── Profile.cshtml            # User profile management
│   ├── Statistics.cshtml         # Admin statistics
│   ├── AccessDenied.cshtml       # Access denied page
│   ├── Accounts/                 # Account management (Admin only)
│   │   └── Index.cshtml
│   ├── Categories/               # Category management (Staff/Admin)
│   │   └── Index.cshtml
│   └── News/                     # News management
│       ├── Index.cshtml          # News CRUD (Staff/Admin)
│       ├── View.cshtml           # Public news viewing
│       └── History.cshtml        # Staff news history
├── Hubs/
│   └── NewsHub.cs                # SignalR hub for real-time updates
└── BasePageModel.cs              # Base class with authentication

NguyenBinhAn_A02_Business/
├── Repositories/                 # Repository pattern implementation
│   ├── IGenericRepository.cs
│   ├── GenericRepository.cs
│   ├── INewsArticleRepository.cs
│   ├── NewsArticleRepository.cs
│   ├── ICategoryRepository.cs
│   ├── CategoryRepository.cs
│   ├── ISystemAccountRepository.cs
│   ├── SystemAccountRepository.cs
│   ├── ITagRepository.cs
│   └── TagRepository.cs
└── Services/                     # Business logic layer
    ├── INewsArticleService.cs
    ├── NewsArticleService.cs
    ├── ICategoryService.cs
    ├── CategoryService.cs
    ├── IAccountService.cs
    ├── AccountService.cs
    ├── ITagService.cs
    └── TagService.cs

NguyenBinhAn_A02_Data/
├── Models/                       # Entity Framework models
│   ├── SystemAccount.cs
│   ├── Category.cs
│   ├── NewsArticle.cs
│   ├── Tag.cs
│   └── NewsTag.cs
└── FUNewsManagementDbContext.cs  # Database context
```

## Setup Instructions

### Prerequisites
- Visual Studio 2019 or later
- .NET 10.0 SDK
- SQL Server 2012 or later
- Git (for cloning)

### Database Setup
1. Run the `FUNewsManagement.sql` script to create the database
2. Update the connection string in `appsettings.json` if needed:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=FUNewsManagement;Trusted_Connection=true;TrustServerCertificate=true;"
   }
   ```

### Running the Application
1. Open the solution in Visual Studio
2. Restore NuGet packages
3. Set `NguyenBinhAnRazorPages` as the startup project
4. Press F5 or click "Start Debugging"

### Default Login
- **Email**: `admin@FUNewsManagementSystem.org`
- **Password**: `@@abc123@@`

## User Roles & Permissions

### Admin (Role 0)
- Full system access
- Manage user accounts
- View statistics and reports
- Manage news and categories
- Cannot be deleted through UI

### Staff (Role 1)
- Manage news articles
- Manage categories
- View own news history
- Update profile

### Lecturer (Role 2)
- View active news articles only
- Update profile
- No management capabilities

## Key Features Implementation

### Real-time Communication
- SignalR Hub (`NewsHub.cs`) provides real-time updates
- Automatic UI refresh when news/categories are created/updated/deleted
- Connected to all management pages

### Validation & Error Handling
- Client-side validation using jQuery Validation
- Server-side validation in services
- User-friendly error messages
- Confirmation dialogs for destructive actions

### Search Functionality
- News search by title, headline, and content
- Category and tag filtering
- Account search by name and email

### Security
- Session-based authentication
- Role-based access control
- Input validation and sanitization
- SQL injection prevention through Entity Framework

## Assignment Requirements Met

✅ **ASP.NET Core Razor Pages with SignalR**
✅ **CRUD actions using Entity Framework Core**
✅ **LINQ for data querying and sorting**
✅ **3-Layer architecture**
✅ **Repository pattern and Singleton pattern**
✅ **CRUD and searching actions**
✅ **Data type validation**
✅ **Popup dialogs for create/update operations**
✅ **Delete confirmation dialogs**
✅ **Role-based access control**
✅ **Real-time communication**
✅ **Default admin account configuration**
✅ **Database connection from appsettings.json**

## Future Enhancements
- Email notifications for news updates
- File upload for news images
- Advanced search with filters
- User activity logging
- API endpoints for mobile integration
- Multi-language support

## Support
For any issues or questions regarding this project, please refer to the assignment documentation or contact the development team.
