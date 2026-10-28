using System.Windows;
using System.Windows.Input;

namespace Calculator;

/// <summary>
/// Interaction logic for MainWindow.xaml.
/// Keyboard input is routed through the same commands the buttons use,
/// so there is a single source of truth in the view model.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        PreviewKeyDown += OnPreviewKeyDown;
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Handled) return;
        string? commandName = MapKey(e.Key);
        if (commandName is null) return;
        e.Handled = true;
        ExecuteCommand(commandName);
    }

    private static string? MapKey(Key key) => key switch
    {
        Key.D0 or Key.NumPad0 => "Digit0Command",
        Key.D1 or Key.NumPad1 => "Digit1Command",
        Key.D2 or Key.NumPad2 => "Digit2Command",
        Key.D3 or Key.NumPad3 => "Digit3Command",
        Key.D4 or Key.NumPad4 => "Digit4Command",
        Key.D5 or Key.NumPad5 => "Digit5Command",
        Key.D6 or Key.NumPad6 => "Digit6Command",
        Key.D7 or Key.NumPad7 => "Digit7Command",
        Key.D8 or Key.NumPad8 => "Digit8Command",
        Key.D9 or Key.NumPad9 => "Digit9Command",
        Key.Add => "AddCommand",
        Key.Subtract => "SubtractCommand",
        Key.Multiply => "MultiplyCommand",
        Key.Divide => "DivideCommand",
        Key.OemPeriod or Key.Decimal => "DecimalCommand",
        Key.Enter => "EqualsCommand",
        Key.Back => "BackspaceCommand",
        Key.Escape => "ClearCommand",
        _ => null
    };

    private void ExecuteCommand(string commandName)
    {
        var vm = DataContext as CalculatorViewModel;
        if (vm is null) return;
        ICommand? command = commandName switch
        {
            "Digit0Command" => vm.Digit0Command,
            "Digit1Command" => vm.Digit1Command,
            "Digit2Command" => vm.Digit2Command,
            "Digit3Command" => vm.Digit3Command,
            "Digit4Command" => vm.Digit4Command,
            "Digit5Command" => vm.Digit5Command,
            "Digit6Command" => vm.Digit6Command,
            "Digit7Command" => vm.Digit7Command,
            "Digit8Command" => vm.Digit8Command,
            "Digit9Command" => vm.Digit9Command,
            "AddCommand" => vm.AddCommand,
            "SubtractCommand" => vm.SubtractCommand,
            "MultiplyCommand" => vm.MultiplyCommand,
            "DivideCommand" => vm.DivideCommand,
            "DecimalCommand" => vm.DecimalCommand,
            "EqualsCommand" => vm.EqualsCommand,
            "BackspaceCommand" => vm.BackspaceCommand,
            "ClearCommand" => vm.ClearCommand,
            _ => null
        };
        command?.Execute(null);
    }
}
