# 🚗 AutoCare Platform

**AutoCare** is a comprehensive car maintenance platform that connects customers with nearby service centers. The platform enables users to view available services, manage their vehicles, and book maintenance appointments seamlessly.

---

## 🏗️ Architecture

The platform follows **Clean Architecture** principles with clear separation of concerns:

```
📦 AutoCare/
├─ 🖥️ server/              # Backend (.NET 9 Web API)
│  ├─ AutoCare.Api/         # API Layer (Controllers, Swagger)
│  ├─ AutoCare.Application/ # Business Logic (CQRS, Handlers)
│  ├─ AutoCare.Domain/      # Domain Entities & Business Rules
│  ├─ AutoCare.Infrastructure/ # Data Access (EF Core, Services)
│  └─ AutoCare.Contracts/   # Shared DTOs
│
└─ 🌐 client/               # Frontend (React + Vite + TypeScript)
   ├─ src/
   │  ├─ api/              # API integration layer
   │  ├─ components/       # Reusable UI components
   │  ├─ features/         # Feature modules
   │  ├─ hooks/            # Custom React hooks
   │  └─ routes/           # Application routing
   └─ public/
```

---

## ✨ Features

### 👥 For Customers

- ✅ User registration and authentication (JWT-based)
- ✅ Vehicle management (add, edit, delete)
- ✅ Browse service centers (with GPS location support)
- ✅ View available services and pricing
- ✅ Book maintenance appointments
- ✅ Track booking status
- ✅ Cancel bookings

### 🏢 For Service Centers

- ✅ Manage offered services with custom pricing
- ✅ View and confirm bookings
- ✅ Update booking status
- ✅ Track service history

### 🔐 Security

- ✅ JWT authentication with refresh tokens
- ✅ Token rotation for enhanced security
- ✅ Role-based authorization (Customer, Employee, Admin)
- ✅ Password hashing with BCrypt

---

## 🛠️ Technology Stack

### Backend

- **.NET 9** - Web API framework
- **Entity Framework Core 9** - ORM for database access
- **SQL Server** - Relational database
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Input validation
- **Mapster** - Object mapping
- **BCrypt.Net** - Password hashing
- **JWT Bearer** - Authentication
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - API documentation

### Frontend

- **React 18** - UI library
- **TypeScript** - Type-safe JavaScript
- **Vite** - Fast build tool
- **Tailwind CSS** - Utility-first CSS
- **React Router** - Client-side routing
- **Axios** - HTTP client

---

## 🚀 Getting Started

### Prerequisites

Before you begin, ensure you have:

1. **.NET 9 SDK** installed - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
2. **SQL Server** (Express, Developer, or LocalDB) - [Download](https://www.microsoft.com/sql-server/sql-server-downloads)
3. **Node.js 18+** and **npm** - [Download](https://nodejs.org/)
4. **Git** - [Download](https://git-scm.com/)

#### Verify Installation

```bash
# Check .NET version
dotnet --version  # Should show 9.0.x

# Check Node.js version
node --version    # Should show v18.x or higher
npm --version     # Should show 9.x or higher
```

---

## 🔧 Backend Setup

### Step 1: Clone Repository

```bash
git clone <repository-url>
cd AutoCare
```

### Step 2: Configure Database Connection

#### Find Your SQL Server Name

**Option 1 - Using SSMS:**

1. Open SQL Server Management Studio
2. Copy the "Server name" from the connection dialog
3. Examples: `localhost`, `.\SQLEXPRESS`, `(localdb)\MSSQLLocalDB`

**Option 2 - Command Line:**

```cmd
sqlcmd -L
```

#### Update Connection String

Edit `server/src/AutoCare.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=AutoCare;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

**Common Connection String Examples:**

```json
// SQL Server Express
"Server=.\\SQLEXPRESS;Database=AutoCare;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"

// SQL Server LocalDB
"Server=(localdb)\\MSSQLLocalDB;Database=AutoCare;Integrated Security=true;TrustServerCertificate=True"

// SQL Server with SQL Authentication
"Server=YOUR_SERVER;Database=AutoCare;User Id=sa;Password=YourPassword;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

### Step 3: Install EF Core Tools

```bash
dotnet tool install --global dotnet-ef
# Or update if already installed:
dotnet tool update --global dotnet-ef
```

### Step 4: Create Database

```bash
cd server/src/AutoCare.Api

# Create initial migration
dotnet ef migrations add InitialCreate --project ../AutoCare.Infrastructure --startup-project .

# Apply migration to database
dotnet ef database update --project ../AutoCare.Infrastructure --startup-project .
```

### Step 5: Run Backend

```bash
dotnet run
```

**Expected Output:**

```
Now listening on: https://localhost:7146
Now listening on: http://localhost:5088
```

### Step 6: Verify API

Open browser and navigate to:

- **Swagger UI:** <https://localhost:7146>
- **API Base:** <https://localhost:7146/api>

---

## 🌐 Frontend Setup

### Step 1: Install Dependencies

```bash
cd client
npm install
```

### Step 2: Configure API Base URL

Edit `client/src/api/config.ts` (or wherever API configuration is):

```typescript
export const API_BASE_URL = "https://localhost:7146/api";
// Or for HTTP: 'http://localhost:5088/api'
```

### Step 3: Run Frontend

```bash
npm run dev
```

**Expected Output:**

```
  VITE v5.x.x  ready in xxx ms

  ➜  Local:   http://localhost:5173/
  ➜  Network: use --host to expose
```

### Step 4: Access Application

Open browser: <http://localhost:5173>

---

## 🔑 Test Accounts

The database is seeded with test accounts:

### Admin

- Email: `admin@autocare.com`
- Password: `Password123@`

### Customers

- Email: `ahmed.mohamed@gmail.com` / Password: `Password123@`
- Email: `sara.hassan@gmail.com` / Password: `Password123@`
- Email: `mohamed.omar@gmail.com` / Password: `Password123@`

### Employees

- Email: `manager1@autocare.com` / Password: `Password123@`
- Email: `tech1@autocare.com` / Password: `Password123@`

---

## 📚 API Documentation

### Authentication Flow

#### 1. Register (Customer)

```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123@",
  "fullName": "John Doe",
  "phoneNumber": "01012345678",
  "address": "123 Main St",
  "city": "Cairo"
}
```

#### 2. Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123@"
}
```

**Response:**

```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGci...",
    "refreshToken": "abc123...",
    "accessTokenExpiresAt": "2024-12-22T10:15:00Z",
    "refreshTokenExpiresAt": "2024-12-29T10:00:00Z",
    "user": {
      "userId": 1,
      "email": "user@example.com",
      "fullName": "John Doe",
      "userType": "Customer"
    }
  }
}
```

#### 3. Refresh Token

```http
POST /api/auth/refresh-token
Content-Type: application/json

{
  "refreshToken": "abc123..."
}
```

#### 4. Making Authenticated Requests

```http
GET /api/vehicles
Authorization: Bearer eyJhbGci...
```

### Main Endpoints

#### Service Centers

- `GET /api/service-centers` - List all centers (with filters)
- `GET /api/service-centers/{id}` - Get center details
- `GET /api/service-centers/nearby?latitude=30.0444&longitude=31.2357&radiusKm=50` - Find nearby centers

#### Services

- `GET /api/services` - List all services
- `GET /api/services/{id}` - Get service details
- `GET /api/services/by-service-center/{id}` - Services at specific center

#### Vehicles (Requires Auth)

- `GET /api/vehicles` - Get customer's vehicles
- `POST /api/vehicles` - Add new vehicle
- `PUT /api/vehicles/{id}` - Update vehicle
- `DELETE /api/vehicles/{id}` - Delete vehicle

#### Bookings (Requires Auth)

- `GET /api/bookings` - Get customer's bookings
- `GET /api/bookings/{id}` - Get booking details
- `POST /api/bookings` - Create new booking
- `POST /api/bookings/{id}/cancel` - Cancel booking

---

## 🧪 Testing with Swagger

1. Navigate to <https://localhost:7146>
2. Click **POST /api/auth/login**
3. Click "Try it out"
4. Enter credentials (e.g., `ahmed.mohamed@gmail.com` / `Password123@`)
5. Click "Execute"
6. Copy the `accessToken` from response
7. Click **"Authorize"** button at top of page
8. Enter: `Bearer <paste-token-here>`
9. Click "Authorize"
10. Now you can test protected endpoints!

---

## 🐛 Troubleshooting

### Database Connection Issues

**Error:** "A network-related or instance-specific error..."

**Solution:**

1. Verify SQL Server is running (Windows Services)
2. Test connection string in SSMS first
3. Try different connection string formats from examples above
4. Check SQL Server Configuration Manager → SQL Server Network Configuration → Protocols → TCP/IP is Enabled

### Migration Issues

**Error:** "dotnet ef: command not found"

**Solution:**

```bash
dotnet tool install --global dotnet-ef --version 9.0.0
```

**Error:** "Build failed"

**Solution:**

```bash
cd server
dotnet restore
dotnet build
```

### CORS Issues

**Error:** "blocked by CORS policy" in browser console

**Solution:**
Ensure your frontend URL is in the CORS configuration:

Edit `server/src/AutoCare.Api/Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",  // Add your frontend URL
                "http://localhost:3000"
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
```

### Port Already in Use

Edit `server/src/AutoCare.Api/Properties/launchSettings.json`:

```json
"applicationUrl": "https://localhost:7147;http://localhost:5089"
```

Then update API_BASE_URL in frontend.

---

## 📂 Project Structure Details

### Backend Layers

#### 🔷 AutoCare.Domain

Core business entities and rules:

- Entities (User, Customer, Vehicle, Booking, Service, ServiceCenter)
- Value Objects (Coordinate, Email, PhoneNumber)
- Enums (BookingStatus, UserType, ServiceType)
- Domain Events (BookingCreated, UserRegistered, etc.)

#### 🔷 AutoCare.Application

Business logic implementation:

- CQRS Commands & Queries
- Handlers (CommandHandler, QueryHandler)
- Validators (FluentValidation)
- DTOs (Data Transfer Objects)
- Interfaces for infrastructure services

#### 🔷 AutoCare.Infrastructure

External concerns:

- EF Core DbContext & Configurations
- Identity (JWT, Password Hashing)
- Services (Email, SMS, DateTime)
- Data Seeding

#### 🔷 AutoCare.Api

API entry point:

- Controllers (thin, delegate to MediatR)
- Middleware (Exception Handling)
- Swagger Configuration
- Authentication Setup

---

## 🔄 Database Management

### Reset Database (Clean Start)

```bash
cd server/src/AutoCare.Api

# Drop database
dotnet ef database drop --project ../AutoCare.Infrastructure --startup-project . --force

# Recreate and seed
dotnet ef database update --project ../AutoCare.Infrastructure
```
