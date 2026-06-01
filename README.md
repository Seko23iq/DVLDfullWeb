<div align="left">

# 🚗 DVLD — Driving & Vehicle License Department System

> A full-stack web application to manage driving licenses end-to-end.
---

## 📌 Overview

**DVLD** is a complete driving license management system built with a 3-tier architecture. It handles everything from person registration to issuing, renewing, and detaining driving licenses — mimicking a real government department workflow.

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|-----------|
| **Frontend** | HTML · CSS · JavaScript - AI |
| **API Layer** | RESTful API + Swagger |
| **DTO** | Data Transfer Objects (separate layer) |
| **Business Layer** | C# |
| **Data Layer** | ADO.NET |
| **DataBase** | SQL Server |

---

## 📦 Modules

### 👤 People & Users
- Full CRUD for People (Add · Edit · Delete · Info)
- User authentication system (Login · Logout · Remember Me)
- Role-based access with `isActive` flag stored in LocalStorage
- Account settings & change password

### 🪪 Local Driving License Applications
- Apply for a new local driving license
- Vision · Written · Street tests management
- Track application status and process flow

### 🌍 International Licenses
- Apply for international license (requires active local license)
- Validation: no detained or already active international license

### 🔄 Renewal & Replacement
- Renew expired local licenses
- Replace damaged or lost licenses

### 🔒 Detain & Release
- Detain active licenses with reason tracking
- Release detained licenses
- Full management dashboard

### ⚙️ System Management
- Manage Application Types & fees
- Manage Test Types
- Drivers overview dashboard

---

## ✨ Key Features

- 🔐 **Authentication** — Login, Logout, Remember Me, LocalStorage session
- 📄 **Pagination** — Navigate large datasets with page controls
- 🔍 **Filter By** — Dynamic filtering across all tables
- 🖼️ **Image Upload** — Person profile photo support
- 📚 **Swagger UI** — Full API documentation
- 🏗️ **Clean Architecture** — DataLayer → BusinessLayer → DTO → API

---

## 🗂️ Project Structure

````
DVLDfullWebProject/
│
├── 📁 FrontEnd/
│   ├── 📁 Images/
│   ├── 📁 Sections/
│   ├── 📁 components/
│   ├── 📁 css/
│   ├── 📁 js/
│   └── 📄 main.html
│
├── 📁 BackEnd/
│   ├── 📁 DataLayer/
│   ├── 📁 BusinessLayer/
│   ├── 📁 DTOLayer/
│   └── 📁 APILayer/
│
└── 🗄️ DataBase/
````

---

## 🚀 Getting Started

### Prerequisites
- SQL Server
- .NET (C#) environment
- Any modern browser

### Setup
1. **Clone the repository**
   ```bash
   git clone https://github.com/Seko23iq/DVLDfullWeb.git
   ```

2. **Database** — Restore the database backup file located in `/DataBase`.

3. **Backend** — Open the solution in Visual Studio, update the connection string, and run the API.

4. **Frontend** — Open `index.html` (login page) in your browser or serve via Live Server.

---


## 🙏 Acknowledgment

This project was built as a full learning experience — from the first entity to the final test.

**الحمد لله** — Alhamdulillah ✅

---

<div align="center">
  Made with ❤️ — 2026
</div>
