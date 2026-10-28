# Calculator (WPF, .NET 10)

A desktop calculator built with WPF on .NET 10, using a clean MVVM architecture.

## Features

- Pocket-calculator (immediate-execution) arithmetic: `+ - * /`
- Repeat `=` to repeat the last operation
- Percent, negate, decimal, backspace, clear / clear-entry
- Full keyboard support (top-row and numpad)
- Live history line above the display
- Divide-by-zero and overflow guarded with a recoverable `Error` state

## Architecture

```
Calculator/                 WPF app (net10.0-windows)
├── CalculatorEngine.cs     Pure logic — no UI dependency, unit-testable
├── CalculatorViewModel.cs  MVVM view model exposing one ICommand per button
├── ViewModelBase.cs        INotifyPropertyChanged base
├── RelayCommand.cs         ICommand helper (RequerySuggested)
└── MainWindow.xaml(.cs)    UI + keyboard mapping routed through the same commands

Calculator.Tests/           xUnit tests (net10.0-windows)
└── UnitTest1.cs            Engine + view-model coverage (26 tests)
```

The engine is deliberately free of any WPF dependency so its arithmetic and
state machine are testable in isolation. The view model owns the engine and
exposes it as commands; `MainWindow` binds buttons to those commands and maps
keyboard input to the same commands.

## Build

```bash
dotnet build Calculator.slnx --configuration Release
```

## Run

```bash
dotnet run --project Calculator
```

Or open `Calculator.slnx` in Visual Studio.

## Test

```bash
dotnet test Calculator.Tests/Calculator.Tests.csproj --configuration Release
```

## License

[MIT](LICENSE) © djgene01
