# Calculator (WPF, .NET 10)

A desktop calculator for Windows built with WPF on .NET 10. It implements
**immediate-execution** (pocket-calculator) arithmetic — each operation is
resolved as soon as the next operator or `=` is pressed, with no operator
precedence — and adds repeat-`=`, percent, negate, and a live history line.
The UI follows MVVM so the arithmetic engine is a pure, dependency-free class
that is fully unit-testable on its own.

## Key Concepts

- **Immediate execution** — `2 + 3 * 4` computes `(2+3) * 4 = 28`, like a
  physical pocket calculator, not algebraic precedence.
- **MVVM** — `CalculatorEngine` (logic) ← `CalculatorViewModel` (commands +
  bound display) ← `MainWindow` (XAML + keyboard). One command per button;
  the keyboard routes through the *same* commands, so there is a single source
  of truth.
- **Repeat `=`** — after `5 + 3 = 8`, pressing `=` again yields `11`, `14`,
  … Typing a fresh operand disarms the repeat.
- **Recoverable errors** — divide-by-zero and overflow set an `Error` state;
  any clear returns to a fresh calculator.

## Entry Points

- [`Calculator/App.xaml`](https://github.com/djgene01/Calculator/blob/42654e2af49a04a83557b51b7d94450987af8cd0/Calculator/App.xaml) — sets `StartupUri` to `MainWindow.xaml`; this is where the app boots.
- [`Calculator/MainWindow.xaml`](https://github.com/djgene01/Calculator/blob/42654e2af49a04a83557b51b7d94450987af8cd0/Calculator/MainWindow.xaml) — the window; its `DataContext` is a `CalculatorViewModel`.

## High-Level Architecture

User input (button click or key press) reaches a `RelayCommand` on the
`CalculatorViewModel`, which forwards to the `CalculatorEngine`, updates the
engine's internal state, and publishes the new `Display`/`HistoryLabel` back
through `INotifyPropertyChanged` so the XAML re-renders. All arithmetic and
state lives in the engine; the view model is a thin adapter.

See [architecture.md](architecture.md).

## Module Map

| Module | Purpose |
|---|---|
| [`engine`](modules/engine.md) | Pure arithmetic + calculator state machine (`CalculatorEngine`) |
| [`view-model`](modules/view-model.md) | Exposes the engine as commands + bound display (`CalculatorViewModel`, `ViewModelBase`) |
| [`commands`](modules/commands.md) | Lightweight `ICommand` implementation (`RelayCommand`) |
| [`ui`](modules/ui.md) | Window layout, button grid, styling, and keyboard routing |
| [`tests`](modules/tests.md) | xUnit coverage of the engine and view model |

## Getting Started

See [getting-started.md](getting-started.md).
