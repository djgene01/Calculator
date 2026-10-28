# Module: `engine`

The `CalculatorEngine` is a **pure, immediate-execution calculator state
machine**. It has no WPF dependency, so its entire behaviour is unit-testable
in isolation. All calculator state lives here; the rest of the app is a thin
adapter around it.

## Responsibilities

- Hold the current display string and the in-progress operation
  (`_firstOperand`, `_pendingOperator`, `_waitingForOperand`).
- Resolve operations with **immediate execution** (no precedence): a pending
  op is computed the moment the next operator or `=` arrives.
- Support **repeat `=`** via `_repeatOperator`/`_repeatOperand`/`_repeatLeftOperand`.
- Guard against **divide-by-zero**, **infinity/NaN**, and **display length
  overflow** by entering a sticky, recoverable `Error` state.
- Format numbers (integers without a decimal point, otherwise `G15`).

## Key Files

- [`Calculator/CalculatorEngine.cs`](https://github.com/djgene01/Calculator/blob/42654e2af49a04a83557b51b7d94450987af8cd0/Calculator/CalculatorEngine.cs) — the only file in this module (256 lines).

## Public API

State (read-only):

| Member | Type | Meaning |
|---|---|---|
| `Display` | `string` | Current display text (or `"Error"`). |
| `HistoryLabel` | `string` | Context line, e.g. `12 +` or `5 + 3 =`; empty when idle. |
| `HasError` | `bool` | `true` while in the sticky error state. |
| `MaxDisplayDigits` | `const int` | `15` — max integer digits the user may type. |

Input methods (each is a no-op while `HasError` is `true`):

| Method | Effect |
|---|---|
| `InputDigit(int digit)` | Append/replace a digit; respects `_waitingForOperand` and the length cap. |
| `InputDecimal()` | Insert a single `.` (or start `0.` for a fresh operand). |
| `InputOperator(char op)` | Start or chain an operation (`+ - * /`); computes a pending op first if chaining. |
| `Equals()` | Resolve the pending op, **or** repeat the last op if there is no pending op. |
| `Percent()` | Replace display with `display / 100`. |
| `ToggleSign()` | Negate the display (no-op at `0`). |
| `Backspace()` | Remove the last digit of the current entry. |
| `ClearEntry()` | Reset the current entry to `0` (clears error if present). |
| `Clear()` | Reset to a fresh calculator (the only recovery from `Error`). |

## Internal Structure

State fields:

- `_display` — the raw display string (source of truth for what's shown).
- `_firstOperand`, `_pendingOperator`, `_waitingForOperand` — the in-progress
  binary operation.
- `_repeatOperator`, `_repeatOperand`, `_repeatLeftOperand` — the completed
  operation remembered for repeat `=`; the left operand is stored separately
  because `Equals()` clears `_firstOperand` but the history label still needs
  it.
- `_hasError` — sticky error latch.

Key helpers:

- `TryCompute(a, b, op, out result)` — the only place arithmetic happens;
  returns `false` for divide-by-zero, infinity, or NaN, which the caller turns
  into `SetError()`.
- `FormatNumber(value)` — integers render without `.`; otherwise `G15`.
- `CountIntegerDigits(s)` — counts digits before the decimal point to enforce
  the length cap (so decimals can still grow the fractional part).
- `ResetRepeatState()` — clears all three repeat fields; called by **every**
  fresh operand-entry path so a stale repeat never leaks forward.

## Dependencies

- **Used by:** `view-model` (the only consumer).
- **Uses:** BCL only (`System`, `System.Globalization`). No WPF, no I/O.

## Notable Patterns / Gotchas

- **Loop-variable capture lives in the view model, not here** — the engine's
  `InputDigit` is correct; the earlier "every button shows 10" bug was a lambda
  capturing the loop index in the view model.
- **Repeat disarming is deliberately broad** — `InputDecimal`, `Backspace`,
  `Percent`, and `ToggleSign` also call `ResetRepeatState()`, so *any* edit to
  the current entry ends the repeat chain (matching physical-calculator feel).
- **`FormatNumber` casts to `long`** for integral values below `1e15`; larger
  or fractional values fall through to `G15` to avoid precision artifacts.
