# Baobab Ridge Wildlife Rehabilitation Records System

A Windows desktop application for recording animals receiving care at the fictional Baobab Ridge Wildlife Rehabilitation Centre. It stores animal records in a plain-text file, calculates each animal's recovery classification, and produces a summary for the centre manager.

## Project overview

The project addresses the scenario in which wildlife rehabilitation records are kept on clipboards and in separate spreadsheets. The application is intended for centre staff who record animal details and recovery scores. Its scope is limited to the five record-management and reporting features in the PRG2782 brief.

## Project information

| Item | Details |
|---|---|
| Module | PRG2782 |
| Institution | Belgium Campus iTversity |
| Language | C# |
| UI | Windows Forms |
| Configured target | .NET Framework 4.7.2 (the project does not yet meet the brief's .NET 8.0-or-newer requirement) |
| Storage | UTF-8 plain-text files; no database |

## Features

All five features are present in the current source. The core logic and file operations have focused checks, but the full interactive form acceptance and fresh-clone run have not been verified.

| Feature | Current status |
|---|---|
| Add an animal with input validation and calculated status/housing | Implemented in source |
| View records in a read-only grid | Implemented in source |
| Update by grid selection or exact ID search | Implemented in source |
| Delete a selected record after confirmation | Implemented in source |
| Generate and display a summary report and write `summary.txt` | Implemented in source |

## Recovery classification

Classification is calculated from the whole-number recovery score:

| Recovery score | Status | Housing unit |
|---:|---|---|
| 0–19 | Critical | Intensive Care Unit |
| 20–39 | Serious | High-Dependency Ward |
| 40–59 | Stable | Recovery Ward |
| 60–79 | Recovering | Outdoor Enclosure |
| 80–100 | Release-Ready | Pre-Release Camp |

## Technology stack

- C# and Windows Forms
- .NET Framework 4.7.2, as configured in the current project
- Git and GitHub, as required by the brief
- UTF-8 text files for application data

## Prerequisites

- Windows, because the application uses Windows Forms.
- Visual Studio with the .NET desktop development workload and .NET Framework 4.7.2 targeting support, or compatible MSBuild/.NET SDK tooling.
- The configured .NET Framework 4.7.2 runtime to run the built application.

The application declares no external NuGet dependencies. The brief requires .NET 8.0 or newer; the current project configuration has not yet been migrated to that target.

## Installation and setup

In PowerShell, clone the repository and enter its directory:

```powershell
git clone https://github.com/bennymuguruka/PRG2782-Baobab-Ridge.git
cd PRG2782-Baobab-Ridge
```

Open `PRG2782-Baobab-Ridge.slnx` in a compatible Visual Studio installation. No third-party packages are declared; restore dependencies if Visual Studio prompts for it. To build from a terminal with compatible tooling:

```powershell
dotnet build .\PRG2782-Baobab-Ridge.slnx
```

Run the built Windows application from the repository root:

```powershell
.\bin\Debug\PRG2782-Baobab-Ridge.exe
```

The solution and project filenames above match the files currently in this repository. Fresh-clone build and run have not been verified.

## Data storage and file formats

### `animals.txt`

The application reads and writes `animals.txt` beside the running executable. It is UTF-8, has no header, and stores one animal per line with seven pipe-separated fields in this order:

```text
Animal ID | Name | Species | Age | Recovery Score | Status | Housing Unit
```

Illustrative record:

```text
WR-0001|Thabo|African Penguin|4|85|Release-Ready|Pre-Release Camp
```

The repository includes starter records, and the project configures the file to copy to the output directory when newer. Missing files are created empty; malformed records are skipped when loading.

### `summary.txt`

The application creates or overwrites `summary.txt` beside the executable when Generate Summary is used. The report contains the generation time, total animal count, average age, average recovery score, and counts for all five statuses. Averages use two decimal places; for an empty dataset they are `N/A`. The generated report file is not committed.

## Usage guide

- **View:** animal records load into the read-only grid when the application starts.
- **Add:** enter the animal ID, name, species, age, and recovery score, then select **Add**. The ID is normalized to uppercase; status and housing are calculated automatically.
- **Update:** select a grid row or enter an exact animal ID and select **Search**. Edit the available fields and select **Save Changes**. **Clear** exits edit mode.
- **Delete:** select a grid row, select **Delete**, and confirm the animal ID and name.
- **Summary:** select **Generate Summary** to display the report on the form and write `summary.txt`.

## Project structure

```text
.
├── Data/
│   └── AnimalFileRepository.cs
├── Models/
│   └── Animal.cs
├── Properties/
├── Services/
│   ├── AnimalService.cs
│   └── ReportService.cs
├── AGENTS.md
├── AUDIT.md
├── animals.txt
├── App.config
├── Form1.cs
├── Form1.Designer.cs
├── PRG2782 Project 2026.pdf
├── PRG2782-Baobab-Ridge.csproj
├── PRG2782-Baobab-Ridge.slnx
├── Program.cs
├── README.md
└── TASKS.md
```

Build output and Visual Studio-generated folders such as `bin/`, `obj/`, and `.vs/` are excluded.

## Team members

The provided member information is:

| Name | Student number |
|---|---|
| Mugisho Benny Muguruka | 603643 |
| To be confirmed | To be confirmed |
| To be confirmed | To be confirmed |

The brief requires a group of 3–4 students from the same class/module type. Confirm the remaining member details with the team and lecturer before submission.

## Development workflow

The brief requires each feature (Add, View, Update, Delete, and Summary) to be developed on its own branch and merged into `main` through a GitHub pull request. Commits should describe meaningful changes, and each member must make genuine contributions under their own Git identity. This README does not imply that any unverified branch, pull request, or contribution has occurred.

## Limitations and outstanding work

- The project targets .NET Framework 4.7.2 rather than the required .NET 8.0 or newer.
- The main form is currently named `Form1`; the search input and Search button overlap in the current form layout.
- Focused temporary checks were run for core behavior, but the repository has no persistent automated test project. Full UI interaction and a fresh-clone build/run remain unverified.
- The current Git history and contribution records do not yet satisfy the brief's branch, pull-request, commit-count, and group-participation requirements. See `AUDIT.md` for the detailed audit.
- Presentation and BCConnect submission requirements have not been verified.

## Academic integrity

The project brief requires the group to submit and understand its own work and prohibits presenting AI-generated code or text as the group's own. This README is an AI-assisted draft; review and adapt it with the team and follow the lecturer's guidance before submission.
