# 📚 Student Management System

A full-stack Student Management System built with **ASP.NET Core Web API** (Dapper ORM) and **HTML/CSS/JavaScript** frontend. This system provides role-based access for administrators and students to manage academic records, marks, and results.

## ✨ Features

### 👨‍🎓 Student Portal
- View personal academic results with subject-wise marks
- Calculate overall percentage automatically
- Secure login with session-based authentication
- Clean, responsive interface

### 👨‍🏫 Admin Portal
- **Student Management**: Create, update, and delete student records
- **Marks Management**: Add/update marks for multiple subjects at once
- **Results Overview**: View all student results in a structured format
- **Subject Management**: Manage available subjects
- **Real-time Search**: Quick student search with suggestions

### 🔐 Authentication & Security
- Role-based access control (Admin/Student)
- Session-based authentication with cookies
- Password-protected accounts
- CORS configuration for secure frontend-backend communication

## 🏗️ Architecture

### Backend (ASP.NET Core API)
- **Framework**: .NET 10.0
- **ORM**: Dapper (lightweight, high-performance)
- **Database**: SQL Server
- **Authentication**: Cookie-based sessions
- **Pattern**: Repository Pattern with Dependency Injection

### Frontend
- **HTML5** with Bootstrap 5.3 for styling
- **Vanilla JavaScript** with async/await for API calls
- **Responsive Design** for all devices
- **No frameworks** - lightweight and fast

### Database Schema
```
Users
├── UserId (PK)
├── Username (Unique)
├── Password
├── Role (Admin/Student)
└── IsActive

Students
├── StudentId (PK)
├── UserId (FK → Users)
├── FirstName
├── LastName
├── FullName
├── Email
├── RollNumber
├── Class
└── IsActive

Subjects
├── SubjectId (PK)
├── SubjectName
└── MaxMarks

StudentMarks
├── MarkId (PK)
├── StudentId (FK → Students)
├── SubjectId (FK → Subjects)
├── MarksObtained
└── Unique Constraint (StudentId, SubjectId)
```

## 📂 Project Structure

```
Result-Management-System1/
├── DataBase/                    # SQL scripts
│   └── StudentResult.sql
├── StudentResultManagementSystem_Dapper/  # Backend (.NET)
│   ├── Controllers/            # API endpoints
│   ├── DTOs/                   # Data Transfer Objects
│   ├── Models/                 # Entity models
│   ├── Repositories/           # Data access layer
│   │   ├── Implementations/
│   │   └── Interfaces/
│   ├── appsettings.json        # Configuration
│   └── Program.cs             # Startup configuration
└── StudentResultSystem_UI/    # Frontend
    ├── css/
    │   └── style.css
    ├── js/
    │   ├── admin.js
    │   ├── auth.js
    │   └── student.js
    ├── admin.html
    ├── index.html
    └── student.html
```

## 🚀 Getting Started

### Prerequisites
- **.NET SDK 10.0** or later
- **SQL Server** (2019 or later)
- **Node.js** (for optional development server)
- **Git** (for cloning)

### Installation Steps

#### 1. Clone the Repository
```bash
git clone <your-repository-url>
cd Result-Management-System1
```

#### 2. Database Setup
1. Open SQL Server Management Studio
2. Run the provided `StudentResult.sql` script in `DataBase/` folder
3. Update connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=StudentResultDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

#### 3. Backend Setup
```bash
cd StudentResultManagementSystem_Dapper
dotnet restore
dotnet run
```

The API will run on `https://localhost:7240`

#### 4. Frontend Setup
1. Open the `StudentResultSystem_UI` folder
2. Serve the HTML files using any web server:
   - **VS Code Live Server** extension
   - Python: `python -m http.server 5500`
   - Node.js: `npx serve .`
3. Access the application at `http://localhost:5500`

### Default Credentials (for testing)

**Admin Account:**
- Username: `admin`
- Password: `admin123`

**Student Account:**
- Username: `john.202601001`
- Password: `student123`

## 🔧 API Endpoints

### Authentication (`/api/auth`)
- `POST /login` - User login
- `POST /logout` - User logout
- `GET /profile` - Get current user profile

### Students (`/api/students`)
- `GET /get-all` - Get all students (Admin only)
- `GET /suggest?q={query}` - Search students (Admin only)
- `POST /create` - Create new student (Admin only)
- `POST /update` - Update student (Admin only)
- `GET /delete/{id}` - Soft delete student (Admin only)

### Marks (`/api/marks`)
- `POST /add` - Add/update marks (Admin only)
- `GET /my-result` - Get student's own results (Student only)
- `GET /all-results` - Get all results (Admin only)

### Subjects (`/api/subjects`)
- `GET /` - Get all subjects

## 🎨 Frontend Features

### Login Page
- Role selection (Admin/Student)
- Form validation
- Error messaging

### Student Dashboard
- Clean results table
- Percentage calculation
- One-click results loading

### Admin Dashboard
- Modal for creating students
- Dynamic subject addition/removal
- Real-time student search
- Grouped results view

## 🛠️ Development

### Backend Customization
1. Add new models in `Models/` folder
2. Create repository interfaces and implementations
3. Add controllers for new endpoints
4. Register services in `Program.cs`

### Frontend Customization
1. Modify `style.css` for styling changes
2. Update JavaScript files for new functionality
3. Add new HTML pages as needed

## 🔒 Security Features

- Session-based authentication
- Role-based authorization
- SQL injection prevention (parameterized queries)
- CORS configured for specific origins
- Input validation on both client and server

## 📊 Database Seeding

To seed initial data, run these SQL statements:

```sql
-- Insert default admin user
INSERT INTO Users (Username, Password, Role, IsActive) 
VALUES ('admin', 'admin123', 'Admin', 1);

-- Insert sample subjects
INSERT INTO Subjects (SubjectName, MaxMarks) VALUES
('Mathematics', 100),
('Science', 100),
('English', 100),
('History', 100),
('Computer Science', 100);
```

## 🐛 Troubleshooting

### Common Issues

1. **CORS Errors**
   - Ensure frontend URL is in CORS policy (Program.cs)
   - Check if credentials are included in fetch requests

2. **Session Not Persisting**
   - Verify cookie settings in Program.cs
   - Check SameSite and Secure policy for HTTPS

3. **Database Connection**
   - Verify connection string
   - Check SQL Server is running
   - Ensure `TrustServerCertificate=true` for local development

4. **API Not Responding**
   - Check if backend is running (`https://localhost:7240`)
   - Verify API endpoints match frontend calls

## 📝 License

This project is for educational purposes. Feel free to modify and use as needed.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Open a Pull Request

## 👨‍💻 Author

**Dev Jarwala**
- Student Management System - Academic Project
- Built with ASP.NET Core and Dapper

## 📧 Contact

For questions or support, please open an issue in the GitHub repository.

---