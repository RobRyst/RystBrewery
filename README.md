RystBrewery – Automated Brewing & Cleaning System
A full-featured brewing control system built using C# WPF for desktop operations and ASP.NET Core + Blazor for web-based monitoring. Inspired by industrial process automation in the brewing industry, the solution includes real-time sensor simulation, automated Clean-in-Place (CIP) handling, alarm management, and process visualization using LiveCharts2.

📌 Project Overview
This project is a simulation-driven brewing and cleaning platform featuring:
Desktop control interface for brewing and washing cycles
Live temperature tracking and process state monitoring
Automatic safety shutdowns and alarms
SQLite-based logging and history tracking
Web dashboard for real-time monitoring and historical analysis
Modular architecture designed for future sensor integration

🚀 Tech Stack
Desktop Application
.NET 8 / C# – WPF application with MVVM pattern
LiveCharts2 – Real-time charts and temperature tracking
SQLite – Embedded logging and recipe storage
Serilog – Logging with optional file/DB output

Web Application
ASP.NET Core + Blazor Server – UI for monitoring and analysis
Chart.js / LiveCharts.js – Web-based visual analytics
REST API – (Optional) Shared data layer between desktop and web

Data Storage
JSON/XML – Recipe definitions
SQLite – Local history, alarm, and user logs

✨ Features
🖥️ Desktop Controller
🔘 Program Selector
Choose between IPA brew, Pilsner brew, and CIP wash cycles
Program-specific steps loaded from JSON/XML config

▶️ Process Execution
Start/stop controls with live process indicator (Green = Ready, Yellow = Running, Red = Error)
Automatic sequence simulation: Brew → Empty → Wash

📊 Live Monitoring
Real-time temperature curves via LiveCharts2
Step-by-step UI feedback with timers and status

🚨 Safety & Error Handling

Over-temperature detection (e.g., >50°C triggers error)
Auto-stop, error display, and detailed logging
Manual stop forces immediate tank emptying and cleaning

✅ Completion & Workflow Enforcement

Brewing requires clean tank; new brew programs locked until wash completed
Completion messages and process reset logic included
