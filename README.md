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
- Shared SQLite DB across desktop and web  
- Optional REST API for frontend/backend decoupling  

---

## 🏗️ Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)  
- SQLite CLI or viewer  
- Visual Studio 2022+ or JetBrains Rider  
