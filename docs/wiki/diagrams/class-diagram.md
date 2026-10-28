# Class Diagram

The object graph is small and linear: a `MainWindow` hosts a
`CalculatorViewModel`, which owns a `CalculatorEngine` and a set of
`RelayCommand`s. `ViewModelBase` supplies change-notification.

```mermaid
classDiagram
    class ViewModelBase {
        <<abstract>>
        #PropertyChanged event
        #OnPropertyChanged(name) void
        #SetProperty(field, value, name) bool
    }
    class RelayCommand {
        -Action _execute
        +RelayCommand(execute)
        +CanExecute(p) bool
        +Execute(p) void
        +CanExecuteChanged event
    }
    class CalculatorEngine {
        -string _display
        -double? _firstOperand
        -char? _pendingOperator
        -bool _waitingForOperand
        -char? _repeatOperator
        -double? _repeatOperand
        -double? _repeatLeftOperand
        -bool _hasError
        +Display string
        +HistoryLabel string
        +HasError bool
        +InputDigit(digit) void
        +InputDecimal() void
        +InputOperator(op) void
        +Equals() void
        +Percent() void
        +ToggleSign() void
        +Backspace() void
        +ClearEntry() void
        +Clear() void
    }
    class CalculatorViewModel {
        -CalculatorEngine _engine
        -ICommand[] _digitCommands
        +Display string
        +HistoryLabel string
        +ClearCommand ICommand
        +EqualsCommand ICommand
        +AddCommand ICommand
        +Digit0Command .. Digit9Command ICommand
    }
    class MainWindow {
        +MainWindow()
        -OnPreviewKeyDown(sender, e) void
        -MapKey(key) string
        -ExecuteCommand(name) void
    }

    ViewModelBase <|-- CalculatorViewModel
    CalculatorViewModel o-- CalculatorEngine : owns
    CalculatorViewModel o-- RelayCommand : "many"
    MainWindow o-- CalculatorViewModel : DataContext
```

## Notes

- **`ICommand` is the seam.** Both `MainWindow` (keyboard) and the XAML
  (buttons) hold references to the same `RelayCommand` instances owned by the
  view model — there is no second command path.
- **`CommandManager.RequerySuggested`** is how `RelayCommand.CanExecuteChanged`
  is raised; it requires a live WPF `Application`, which is why the test project
  keeps one alive.
- The engine's private fields are the complete calculator state; no state
  lives in the view model or the window.
