using System.Windows.Input;

namespace Calculator;

/// <summary>
/// View model for the calculator window. Owns a <see cref="CalculatorEngine"/>
/// and exposes the display plus one command per button, so the XAML stays
/// declarative and testable.
/// </summary>
public sealed class CalculatorViewModel : ViewModelBase
{
    private readonly CalculatorEngine _engine = new();

    public CalculatorViewModel()
    {
        ClearCommand = new RelayCommand(() => { _engine.Clear(); Refresh(); });
        ClearEntryCommand = new RelayCommand(() => { _engine.ClearEntry(); Refresh(); });
        BackspaceCommand = new RelayCommand(() => { _engine.Backspace(); Refresh(); });
        DecimalCommand = new RelayCommand(() => { _engine.InputDecimal(); Refresh(); });
        PercentCommand = new RelayCommand(() => { _engine.Percent(); Refresh(); });
        NegateCommand = new RelayCommand(() => { _engine.ToggleSign(); Refresh(); });
        EqualsCommand = new RelayCommand(() => { _engine.Equals(); Refresh(); });

        AddCommand = new RelayCommand(() => { _engine.InputOperator('+'); Refresh(); });
        SubtractCommand = new RelayCommand(() => { _engine.InputOperator('-'); Refresh(); });
        MultiplyCommand = new RelayCommand(() => { _engine.InputOperator('*'); Refresh(); });
        DivideCommand = new RelayCommand(() => { _engine.InputOperator('/'); Refresh(); });

        for (int i = 0; i <= 9; i++)
        {
            int digit = i; // capture by value — a lambda would otherwise see the loop's final value (10)
            _digitCommands[i] = new RelayCommand(() => { _engine.InputDigit(digit); Refresh(); });
        }
    }

    // Commands
    public ICommand ClearCommand { get; }
    public ICommand ClearEntryCommand { get; }
    public ICommand BackspaceCommand { get; }
    public ICommand DecimalCommand { get; }
    public ICommand PercentCommand { get; }
    public ICommand NegateCommand { get; }
    public ICommand EqualsCommand { get; }
    public ICommand AddCommand { get; }
    public ICommand SubtractCommand { get; }
    public ICommand MultiplyCommand { get; }
    public ICommand DivideCommand { get; }

    public ICommand Digit0Command => _digitCommands[0];
    public ICommand Digit1Command => _digitCommands[1];
    public ICommand Digit2Command => _digitCommands[2];
    public ICommand Digit3Command => _digitCommands[3];
    public ICommand Digit4Command => _digitCommands[4];
    public ICommand Digit5Command => _digitCommands[5];
    public ICommand Digit6Command => _digitCommands[6];
    public ICommand Digit7Command => _digitCommands[7];
    public ICommand Digit8Command => _digitCommands[8];
    public ICommand Digit9Command => _digitCommands[9];

    private readonly ICommand[] _digitCommands = new ICommand[10];

    // Bound display state
    private string _display = "0";
    public string Display
    {
        get => _display;
        private set => SetProperty(ref _display, value);
    }

    private string _historyLabel = "";
    public string HistoryLabel
    {
        get => _historyLabel;
        private set => SetProperty(ref _historyLabel, value);
    }

    private void Refresh()
    {
        Display = _engine.Display;
        HistoryLabel = _engine.HistoryLabel;
    }
}
