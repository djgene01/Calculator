# Getting Started

## Prerequisites

- **.NET 10 SDK** (`dotnet --version` → `10.x`). The projects target
  `net10.0-windows`, so Windows (or a Windows build agent) is required for the
  WPF app and its tests.
- **Git** (already used for the repo).
- Optional: Visual Studio 2022/2026 with the .NET desktop workload, to open
  `Calculator.slnx`.

## Installation

There is nothing to install — clone and build:

```bash
git clone https://github.com/djgene01/Calculator.git
cd Calculator
dotnet build Calculator.slnx --configuration Release
```

## First Run

```bash
dotnet run --project Calculator
```

The window opens centered. Type `2`, `+`, `3`, `=` (or use the buttons) and the
display shows `5`.

## Common Workflows

### Run the test suite

```bash
dotnet test Calculator.Tests/Calculator.Tests.csproj --configuration Release
```

Expect `26` passing tests.

### Add a button

1. Add a `RelayCommand` + engine method (if new behaviour is needed) in
   `CalculatorViewModel`.
2. Add a bound `<Button>` in `MainWindow.xaml`.
3. If it should be reachable from the keyboard, add a `Key` arm to
   `MainWindow.xaml.cs:MapKey` **and** a name arm to `ExecuteCommand`.
4. Add a test in `Calculator.Tests/UnitTest1.cs` that drives the command and
   asserts on `Display`/`HistoryLabel`.

### Rebuild the wiki

Re-run the `code-wiki` skill from the repo root; it records the source SHA in
`.codewiki-state.json` so you can regenerate only after the code changes.

## Configuration

There is no runtime configuration file. Behaviour is fixed in code:

- `CalculatorEngine.MaxDisplayDigits` (`15`) — the typed-length cap.
- `MainWindow` size (`380×560`) and the button palette in `MainWindow.xaml`
  `Window.Resources`.

## Where to Go Next

- Architecture + data flow: [architecture.md](architecture.md)
- Class relationships: [diagrams/class-diagram.md](diagrams/class-diagram.md)
- Call sequences: [diagrams/sequences.md](diagrams/sequences.md)
- Module reference: [README.md#module-map](README.md#module-map)
