# 🍺 RystBrewery – Brewing & CIP Automation System

> Simulation-based brewery automation system with desktop monitoring.  
> Built with C# WPF, ASP.NET Core, and Blazor.

---

## 📌 Project Overview

RystBrewery is a control and monitoring solution for breweries, featuring:

- 🖥️ Desktop app for brewing and CIP control  
- 🔁 Step-by-step brewing process simulation  
- 🚨 Alarm handling and safety rule enforcement  
- 🗃️ SQLite-based recipe and log storage  
---

## 🚀 Tech Stack

### 🖥️ Desktop Application

- `.NET 8 / C#` – WPF  
- `MVVM` – Clean separation of UI and logic  
- `LiveCharts2` – Real-time charting  
- `SQLite` – Embedded logging and recipes DB  
- `Serilog` – Structured logging  
- `JSON/XML` – Config and recipe files  

---

## ✨ Features

### 🧪 Brewing & Cleaning

- Choose between **IPA**, **Eplecider**, **Sommerøl** or **CIP** program  
- Start/stop simulation with visual monitoring  
- Enforces process steps and safety limits  
- Live temperature graphing  

### 🚨 Safety System

- Auto-alarm and stop at >50°C  
- Status indicators (Red / Yellow / Green)  
- Manual stop triggers auto cleaning  

### 📊 Data & History

- Live temperature & state curves  
- Dashboard view:
  - Active tanks  
  - Last wash session  
  - Previous brews  
- Log filters by tank, time, recipe, error  

### 🔐 User Access

- Login required before any process starts  
- Logs login attempts with timestamps and user IDs  

### 📁 Reporting

- Text file with log information
- Auto-generate charts and session summaries  
- Track alarms, durations, and outcomes  

---

## 🧱 Architecture

- Clean separation: Services, Models, UI
- SQlite for Database
- Optional REST API for frontend/backend decoupling  

---

##🧠 What I Learned
Working on the RystBrewery Brewing & CIP Automation System has significantly enhanced my understanding of building simulation based control systems and full-stack desktop applications. This project combined real-time logic, UI responsiveness, and backend integration using modern .NET technologies. Here are the key takeaways:

### C# WPF & MVVM Architecture
- I gained valuable experience building a responsive desktop application using WPF and the MVVM design pattern. This helped me understand how to separate concerns between UI, logic, and data, while maintaining scalability and testability.

### Real-Time Simulation and State Handling
- Implementing step-by-step brewing and CIP simulations taught me how to model dynamic processes, manage time-based events, and update UI elements in real time — critical skills for industrial or process-control software.

### Safety Systems & Alarm Handling
- Designing and enforcing safety rules (e.g., auto-stop on temperature thresholds) gave me practical insight into implementing fault-tolerant, user-safe systems, including visual status indicators and emergency logic.

### Data Logging & Storage with SQLite
- Using SQLite for persistent storage of logs and recipes improved my understanding of embedded databases, data filtering, and local data querying within desktop applications.

### Structured Logging with Serilog
- I learned to implement structured and file-based logging for traceability, debugging, and system transparency, especially important in production or audit-heavy environments.

### User Authentication & Access Control
- Integrating login functionality into a local app taught me how to restrict system access, track user sessions, and securely manage interactions — even outside of a web environment.

### Data Visualization with LiveCharts2
- Leveraging LiveCharts2 enabled me to render live-updating charts and temperature curves, enhancing both user experience and operational insight.

### Configuration Management
- Working with JSON and XML files for recipes and settings deepened my understanding of configuration-driven application design, enabling easier extensibility and customization.

### Clean Architecture Principles
- I applied service abstraction, dependency injection, and layered separation (Models, Services, UI), resulting in a maintainable and modular codebase that can evolve or scale over time.
