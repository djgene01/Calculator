# Module: `view-model`

The `CalculatorViewModel` is the MVVM adapter between the UI and the engine.
It owns a single `CalculatorEngine` instance, exposes one `ICommand` per
button, and two bound strings (`Display`, `HistoryLabel`) that the XAML reads.
It contains **no logic of its own** — every command just calls an engine method
and then refreshes the bound properties.

## Responsibilities

- Construct and own the `CalculatorEngine`.
- Map each physical button (and keyboard key, via the UI) to a command.
- Publish engine state to the view after every action (`Refresh()`).
- Provide `INotifyPropertyChanged` so the display updates reactively.

## Key Files

- [`Calculator/CalculatorViewModel.cs`](https://github.com/djgene01/Calculator/blob/42654e2af49a04a83557b51b7d94450987af8cd0/Calculator/CalculatorViewModel.cs) — commands + bound state (82 lines).
- [`Calculator/ViewModelBase.cs`](https://github.com/djgene01/Calculator/blob/42654e2af49a04a83557b51b7d94450987af8cd0/Calculator/ViewModelBase.cs) — `INotifyPropertyChanged` base with a `SetProperty` helper (23 lines).

## Public API

Commands (all `ICommand`, `CanExecute` always `true`):

`ClearCommand`, `ClearEntryCommand`, `BackspaceCommand`, `DecimalCommand`,
`PercentCommand`, `NegateCommand`, `EqualsCommand`, `AddCommand`,
`SubtractCommand`, `MultiplyCommand`, `DivideCommand`, and `Digit0Command`
… `Digit9Command`.

Bound properties:

| Property | Binds to |
|---|---|
| `Display` | the display `TextBlock` |
| `HistoryLabel` | the history `TextBlock` |

## Internal Structure

- The constructor builds each command as a `RelayCommand` whose lambda calls
  the corresponding engine method and then `Refresh()`.
- The ten digit commands are created in a loop. **Critical detail:** the loop
  declares `int digit = i;` inside the body so each lambda captures its own
  value. Capturing the loop variable directly would make all ten commands call
  `InputDigit(10)` — the root cause of the original "every button shows 10" bug.
- `Refresh()` copies `engine.Display` and `engine.HistoryLabel` into the bound
  properties; `SetProperty` suppresses redundant `PropertyChanged` when the
  value hasn't changed.

## Dependencies

- **Used by:** `ui` (as the window `DataContext`), `tests`.
- **Uses:** `engine`, `commands` (`RelayCommand`), `ViewModelBase`,
  `System.Windows.Input`.

## Notable Patterns / Gotchas

- **Stateless adapter** — the view model holds no calculator state; everything
  round-trips through the engine. This keeps it trivially testable.
- **`ViewModelBase.SetProperty` returns `bool`** — callers can short-circuit on
  no-change; the view model ignores the return and relies on the property setter.
