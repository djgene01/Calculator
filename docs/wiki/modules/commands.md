# Module: `commands`

A single-purpose `ICommand` implementation used by every button and keyboard
shortcut in the app. There is intentionally no `CanExecute` gating — a
calculator button is always actionable — so `CanExecute` always returns `true`.

## Responsibilities

- Wrap a plain `Action` as an `ICommand`.
- Re-raise `CanExecuteChanged` whenever WPF's `CommandManager.RequerySuggested`
  fires, so commands refresh when focus/input changes.

## Key Files

- [`Calculator/RelayCommand.cs`](https://github.com/djgene01/Calculator/blob/42654e2af49a04a83557b51b7d94450987af8cd0/Calculator/RelayCommand.cs) — 26 lines.

## Public API

```csharp
public sealed class RelayCommand : ICommand
{
    public RelayCommand(Action execute);   // throws ArgumentNullException if null
    public bool CanExecute(object? parameter);   // always true
    public void Execute(object? parameter);      // invokes the wrapped Action
    public event EventHandler? CanExecuteChanged;
}
```

## Internal Structure

- Stores the `Action` in `_execute` (null-checked at construction).
- `CanExecuteChanged` add/remove delegate to
  `CommandManager.RequerySuggested`, the standard WPF mechanism for re-querying
  command availability.

## Dependencies

- **Used by:** `view-model` (every command is a `RelayCommand`).
- **Uses:** `System.Windows.Input` (`ICommand`, `CommandManager`).

## Notable Patterns / Gotchas

- **`CommandManager` requires a running `Application`** — in the test project a
  WPF `Application` is kept alive for the test lifetime for this reason. This is
  why `Calculator.Tests` targets `net10.0-windows` with `UseWPF` rather than a
  plain `net10.0`.
- **No parameter support** — `Execute` ignores its argument; the calculator
  never needs to pass data through a command.
