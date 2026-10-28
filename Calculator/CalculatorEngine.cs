namespace Calculator;

/// <summary>
/// Pure calculator logic (immediate-execution / pocket-calculator semantics).
/// Kept free of any UI dependency so it can be unit-tested in isolation.
/// </summary>
public sealed class CalculatorEngine
{
    /// <summary>Maximum integer digits the user may type.</summary>
    public const int MaxDisplayDigits = 15;

    private string _display = "0";
    private double? _firstOperand;
    private char? _pendingOperator;
    private bool _waitingForOperand;
    private char? _repeatOperator;   // last operator, used by repeated "="
    private double? _repeatOperand;   // last right-hand operand
    private bool _hasError;

    /// <summary>The text to show in the display.</summary>
    public string Display => _display;

    /// <summary>Short context line (e.g. "12 +") shown above the display.</summary>
    public string HistoryLabel
    {
        get
        {
            if (_hasError) return "";
            if (_firstOperand is not null && _pendingOperator is not null)
                return $"{FormatNumber(_firstOperand.Value)} {_pendingOperator.Value}";
            if (_repeatOperator is not null && _repeatOperand is not null && _pendingOperator is null)
                return $"{FormatNumber(_firstOperand ?? 0)} {_repeatOperator.Value} {FormatNumber(_repeatOperand.Value)} =";
            return "";
        }
    }

    /// <summary>True when a non-recoverable error (e.g. divide by zero) occurred.</summary>
    public bool HasError => _hasError;

    /// <summary>AC – clear everything and return to a fresh state.</summary>
    public void Clear()
    {
        _display = "0";
        _firstOperand = null;
        _pendingOperator = null;
        _waitingForOperand = false;
        _repeatOperator = null;
        _repeatOperand = null;
        _hasError = false;
    }

    /// <summary>CE – clear only the current entry.</summary>
    public void ClearEntry()
    {
        if (_hasError) { Clear(); return; }
        _display = "0";
        _waitingForOperand = false;
    }

    /// <summary>Delete the last digit of the current entry.</summary>
    public void Backspace()
    {
        if (_hasError || _waitingForOperand) return;
        _display = _display.Length > 1 ? _display.Substring(0, _display.Length - 1) : "0";
        if (_display.Length == 0 || _display == "-") _display = "0";
    }

    /// <summary>Type a digit (0-9).</summary>
    public void InputDigit(int digit)
    {
        if (_hasError) return;
        if (_waitingForOperand)
        {
            _display = digit == 0 ? "0" : digit.ToString();
            _waitingForOperand = false;
        }
        else if (_display == "0")
        {
            _display = digit.ToString();
        }
        else
        {
            if (CountIntegerDigits(_display) >= MaxDisplayDigits) return; // ignore overflow of typed length
            _display += digit.ToString();
        }
    }

    /// <summary>Type the decimal separator.</summary>
    public void InputDecimal()
    {
        if (_hasError) return;
        if (_waitingForOperand)
        {
            _display = "0.";
            _waitingForOperand = false;
        }
        else if (!_display.Contains('.'))
        {
            _display += ".";
        }
    }

    /// <summary>Start (or chain) an operation: + - * /</summary>
    public void InputOperator(char op)
    {
        if (_hasError) return;
        double current = ParseDisplay();

        if (_firstOperand is null)
        {
            _firstOperand = current;
        }
        else if (!_waitingForOperand)
        {
            // Chain: resolve the pending operation before storing the new one.
            if (!TryCompute(_firstOperand.Value, current, _pendingOperator, out double result))
            {
                SetError();
                return;
            }
            _firstOperand = result;
            _display = FormatNumber(result);
        }

        _pendingOperator = op;
        _waitingForOperand = true;
        _repeatOperator = null;
        _repeatOperand = null;
    }

    /// <summary>Resolve the pending operation, or repeat the previous one.</summary>
    public void Equals()
    {
        if (_hasError) return;
        double current = ParseDisplay();

        if (_pendingOperator is not null && _firstOperand is not null)
        {
            if (!TryCompute(_firstOperand.Value, current, _pendingOperator, out double result))
            {
                SetError();
                return;
            }
            _repeatOperand = current;
            _repeatOperator = _pendingOperator;
            _pendingOperator = null;
            _firstOperand = null;
            _display = FormatNumber(result);
            _waitingForOperand = true;
        }
        else if (_repeatOperator is not null && _repeatOperand is not null)
        {
            // Pressing "=" again repeats the last operation.
            if (!TryCompute(current, _repeatOperand.Value, _repeatOperator, out double result))
            {
                SetError();
                return;
            }
            _display = FormatNumber(result);
            _waitingForOperand = true;
        }
        else
        {
            _waitingForOperand = true;
        }
    }

    /// <summary>Divide the current display by 100.</summary>
    public void Percent()
    {
        if (_hasError) return;
        _display = FormatNumber(ParseDisplay() / 100.0);
        _waitingForOperand = false;
    }

    /// <summary>Negate the current display.</summary>
    public void ToggleSign()
    {
        if (_hasError) return;
        double value = ParseDisplay();
        if (value == 0) return;
        _display = FormatNumber(-value);
        _waitingForOperand = false;
    }

    // ---- helpers ---------------------------------------------------------

    private double ParseDisplay() =>
        double.TryParse(_display, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out double v)
            ? v : 0;

    private static int CountIntegerDigits(string s)
    {
        int start = s.StartsWith("-") ? 1 : 0;
        int dot = s.IndexOf('.');
        int end = dot >= 0 ? dot : s.Length;
        int count = 0;
        for (int i = start; i < end; i++)
            if (char.IsDigit(s[i])) count++;
        return count;
    }

    private static bool TryCompute(double a, double b, char? op, out double result)
    {
        result = 0;
        switch (op)
        {
            case '+': result = a + b; break;
            case '-': result = a - b; break;
            case '*': result = a * b; break;
            case '/':
                if (b == 0) return false;     // division by zero
                result = a / b; break;
            default: return false;
        }
        if (double.IsInfinity(result) || double.IsNaN(result)) return false;
        return true;
    }

    private void SetError()
    {
        _hasError = true;
        _display = "Error";
        _firstOperand = null;
        _pendingOperator = null;
        _waitingForOperand = true;
        _repeatOperator = null;
        _repeatOperand = null;
    }

    private static string FormatNumber(double value)
    {
        if (value == 0) return "0";
        if (value == Math.Floor(value) && Math.Abs(value) < 1e15)
            return ((long)value).ToString(System.Globalization.CultureInfo.InvariantCulture);
        return value.ToString("G15", System.Globalization.CultureInfo.InvariantCulture);
    }
}
