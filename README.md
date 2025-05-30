RystBrewery – Brewing & CIP Automation System
A simulation-based brewing automation solution built using C# WPF for desktop operations and ASP.NET Core with Blazor for web-based monitoring. The system controls brewing and Clean-in-Place (CIP) wash cycles, logs all process events, enforces safety rules, and provides real-time and historical analytics.

📌 Project Overview
This project is a practical implementation of a brewery control and monitoring platform featuring:

Desktop application for brewing and cleaning control

Process step simulation with live temperature tracking

Automatic alarm handling and safety enforcement

SQLite-based logging and recipe configuration

Web dashboard with real-time and historical process views

Exportable logs and graphs for process analysis

🚀 Tech Stack
Desktop Application
.NET 8 / C# – WPF desktop application

MVVM Pattern – For clean separation of logic and UI

LiveCharts2 – Real-time charting for temperature and process steps

SQLite – Embedded database for logs, recipes, and user sessions

Serilog – Structured logging to file and/or database

JSON/XML – Recipe and configuration files

Web Application
ASP.NET Core 8.0 – Web backend (Blazor Server or MVC)

Blazor Server – Real-time UI updates (optional MVC alternative)

Chart.js / LiveCharts.js – Web-based data visualizations

TailwindCSS – Utility-first CSS framework for responsive UI

SignalR – Live updates from desktop (optional integration)

Export & Reporting
QuestPDF / iText7 – PDF generation for reports

CSV Export – Data export for logs and analysis

✨ Features
🧪 Brewing & Cleaning Process Control

Select IPA, Pilsner, or CIP program

Start, stop, and monitor active processes

Step-by-step simulation with process enforcement

Temperature visualization in real time

🚨 Safety & Alarm Handling

Auto-stop and alarm on over-temperature (>50°C)

Red/yellow/green indicator lights for process status

Manual stop triggers automatic tank emptying and cleaning

📊 Real-Time and Historical Data

Temperature curves and process states displayed live

Dashboard shows active tanks, past runs, last wash

Filter logs by tank, date, recipe, or error

🧾 User Access Logging

Login required before starting processes

Logs both successful and failed authentication attempts

Stored with timestamp and user context

📄 Reports & Exports

Export logs and history to PDF or CSV

Graphical report generation from past runs

Track duration, alarms, and outcomes per session

🧱 Clean Architecture

Organized into services, models, and UI layers

Shared SQLite database for desktop and web

Optional API layer for decoupling UI and backend

🏗️ Getting Started
Prerequisites
.NET 8 SDK

SQLite CLI or DB viewer

Visual Studio 2022+ or JetBrains Rider
