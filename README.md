# 🚗 AutoCare Platform

**AutoCare** is a car maintenance platform that connects customers with nearby service branches.  
It allows users to:

- View branches and their available services.
- Get contact details and locations on a map.
- Communicate with service centers easily.

The platform is built using **.NET 8 (Web API)** for backend services and **React (Vite + TypeScript)** for the client interface.  
This modular design ensures scalability, clean architecture, and easy integration with future mobile or admin dashboards.

---

## 🏗️ Project Structure

The solution is divided into two main parts: **server** and **client**.

📦 AutoCare/

├─ 🖥️ server/ # Backend (.NET 8 Web API)

├─ 📂 src/

│ ├─ 🗄️ AutoCare.Api/ # API endpoints (Controllers, Program.cs, Swagger)

│ ├─ ⚙️ AutoCare.Application/ # Business logic & service layer

│ ├─ 📚 AutoCare.Domain/ # Core entities (Branch, Service, etc.)

│ ├─ 💾 AutoCare.Infrastructure/ # EF Core, repositories, database context

│ ├─ 🧪 AutoCare.Tests/ # Unit & integration tests

│ └─ 📄 AutoCare.sln # Main .NET solution file

│

└─ 🌐 client/ # Frontend (React + Vite + TypeScript)

├─ 🏠 src/

│ ├─ 🌐 api/ # API calls (Axios)

│ ├─ 🧩 components/ # Shared UI components

│ ├─ 🚀 features/ # Feature-based folders (e.g., branches, services)

│ ├─ 🔗 context/ # Global contexts (Auth, Theme, etc.)

│ ├─ 🪝 hooks/ # Custom hooks

│ ├─ 🗺️ routes/ # Routing configuration

│ ├─ 🎨 styles/ # Global styles / Tailwind

│ ├─ 🛠️ utils/ # Helper functions

│ ├─ 🗂️ App.tsx # App root layout

│ └─ 🏁 main.tsx # Entry point

│
├─ 🌐 public/ # Static assets (HTML, images)

├─ 📦 package.json

├─ 📦 tsconfig.json

└─ ⚙️ vite.config.ts

---

🧠 **Purpose:** provide a clean, maintainable, and extendable architecture for AutoCare’s backend and frontend.  
🎯 **Next step:** after completing the implementation, we’ll extend this README with setup, features, and run instructions.
