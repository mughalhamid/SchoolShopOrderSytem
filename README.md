# 🍽️ School Canteen Shop System API

A lightweight **.NET 8 Minimal API** project designed to manage a school canteen’s order workflow.  
This project demonstrates clean architecture principles with **EF Core** and **C#**, focusing on simplicity and real-world constraints such as wallet balance, stock availability, cut-off times, and allergy checks.

---

## 🚀 Features

- **Request Order**  
  - Validates wallet balance  
  - Checks daily stock availability  
  - Enforces canteen cut-off time  
  - Allergy-aware ordering  

- **Get Order Details**  
  - Retrieve order information with schema and API endpoints exposed at runtime  

---

## 🛠️ Tech Stack

- [.NET 8](https://dotnet.microsoft.com/)  
- [C#](https://learn.microsoft.com/en-us/dotnet/csharp/)  
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)  
- Minimal REST APIs  

---

## 📦 Getting Started

### Prerequisites
- Install **.NET 8 SDK** on your machine

### Setup
```bash
# Clone the repository
git clone https://github.com/mughalhamid/SchoolShopOrderSytem.git

# Navigate into the project folder
cd SchoolShopOrderSytem

# Run the project
dotnet run
