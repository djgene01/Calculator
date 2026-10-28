using System.Windows;
using System.Windows.Input;
using Xunit;

namespace Calculator.Tests;

/// <summary>
/// Tests for the calculator (engine + view model). A WPF <see cref="Application"/>
/// is kept alive for the test lifetime because <c>RelayCommand</c> hooks
/// <see cref="CommandManager.RequerySuggested"/>, which requires one.
/// </summary>
[Collection("Calculator")]
public class CalculatorEngineTests
{
    private static CalculatorViewModel NewVm() => new();

    // --- digit input (regression: all buttons used to show "10") ----------

    [Theory]
    [InlineData(0)] [InlineData(1)] [InlineData(2)] [InlineData(3)]
    [InlineData(4)] [InlineData(5)] [InlineData(6)] [InlineData(7)]
    [InlineData(8)] [InlineData(9)]
    public void EachDigitButtonEntersItsOwnDigit(int digit)
    {
        var vm = NewVm();
        var command = vm.GetType().GetProperty($"Digit{digit}Command")!.GetValue(vm) as ICommand;
        command!.Execute(null);
        Assert.Equal(digit.ToString(), vm.Display);
    }

    [Fact]
    public void MultiDigitNumberIsBuiltLeftToRight()
    {
        var vm = NewVm();
        vm.Digit5Command.Execute(null);
        vm.Digit3Command.Execute(null);
        Assert.Equal("53", vm.Display);
    }

    // --- arithmetic -------------------------------------------------------

    [Fact]
    public void AdditionWorks()
    {
        var vm = NewVm();
        vm.Digit2Command.Execute(null);
        vm.Digit7Command.Execute(null);
        vm.AddCommand.Execute(null);
        vm.Digit3Command.Execute(null);
        vm.Digit5Command.Execute(null);
        vm.EqualsCommand.Execute(null);
        Assert.Equal("62", vm.Display);
    }

    [Fact]
    public void SubtractionWorks()
    {
        var vm = NewVm();
        vm.Digit9Command.Execute(null);
        vm.Digit8Command.Execute(null);
        vm.SubtractCommand.Execute(null);
        vm.Digit4Command.Execute(null);
        vm.Digit5Command.Execute(null);
        vm.EqualsCommand.Execute(null);
        Assert.Equal("53", vm.Display);
    }

    [Fact]
    public void MultiplicationWorks()
    {
        var vm = NewVm();
        vm.Digit6Command.Execute(null);
        vm.MultiplyCommand.Execute(null);
        vm.Digit7Command.Execute(null);
        vm.EqualsCommand.Execute(null);
        Assert.Equal("42", vm.Display);
    }

    [Fact]
    public void DivisionWorks()
    {
        var vm = NewVm();
        vm.Digit8Command.Execute(null);
        vm.DivideCommand.Execute(null);
        vm.Digit2Command.Execute(null);
        vm.EqualsCommand.Execute(null);
        Assert.Equal("4", vm.Display);
    }

    [Fact]
    public void DivisionByZeroShowsError()
    {
        var vm = NewVm();
        vm.Digit5Command.Execute(null);
        vm.DivideCommand.Execute(null);
        vm.Digit0Command.Execute(null);
        vm.EqualsCommand.Execute(null);
        Assert.Equal("Error", vm.Display);
    }

    [Fact]
    public void ChainedOperationsResolveLeftToRight()
    {
        // 10 + 4 * 2 => (10 + 4) * 2 = 28 (immediate execution, no precedence)
        var vm = NewVm();
        vm.Digit1Command.Execute(null);
        vm.Digit0Command.Execute(null);
        vm.AddCommand.Execute(null);
        vm.Digit4Command.Execute(null);
        vm.MultiplyCommand.Execute(null);
        vm.Digit2Command.Execute(null);
        vm.EqualsCommand.Execute(null);
        Assert.Equal("28", vm.Display);
    }

    [Fact]
    public void RepeatEqualsRepeatsLastOperation()
    {
        // 5 + 3 = 8, then = again -> 8 + 3 = 11
        var vm = NewVm();
        vm.Digit5Command.Execute(null);
        vm.AddCommand.Execute(null);
        vm.Digit3Command.Execute(null);
        vm.EqualsCommand.Execute(null);
        Assert.Equal("8", vm.Display);
        vm.EqualsCommand.Execute(null);
        Assert.Equal("11", vm.Display);
    }

    [Fact]
    public void DecimalInputWorks()
    {
        var vm = NewVm();
        vm.Digit1Command.Execute(null);
        vm.DecimalCommand.Execute(null);
        vm.Digit5Command.Execute(null);
        Assert.Equal("1.5", vm.Display);
    }

    [Fact]
    public void NegateTogglesSign()
    {
        var vm = NewVm();
        vm.Digit5Command.Execute(null);
        vm.NegateCommand.Execute(null);
        Assert.Equal("-5", vm.Display);
    }

    [Fact]
    public void PercentDividesBy100()
    {
        var vm = NewVm();
        vm.Digit5Command.Execute(null);
        vm.Digit0Command.Execute(null);
        vm.PercentCommand.Execute(null);
        Assert.Equal("0.5", vm.Display);
    }

    [Fact]
    public void BackspaceRemovesLastDigit()
    {
        var vm = NewVm();
        vm.Digit1Command.Execute(null);
        vm.Digit2Command.Execute(null);
        vm.Digit3Command.Execute(null);
        vm.BackspaceCommand.Execute(null);
        Assert.Equal("12", vm.Display);
    }

    [Fact]
    public void ClearResetsState()
    {
        var vm = NewVm();
        vm.Digit5Command.Execute(null);
        vm.Digit5Command.Execute(null);
        vm.AddCommand.Execute(null);
        vm.Digit3Command.Execute(null);
        vm.ClearCommand.Execute(null);
        Assert.Equal("0", vm.Display);
    }
}
