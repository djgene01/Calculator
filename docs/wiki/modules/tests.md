# Module: `tests`

The xUnit test suite. It exercises the calculator **through the public view-model
command surface** (not the engine directly), which means it also covers the
MVVM wiring — the exact layer where the original loop-capture bug lived.

## Responsibilities

- Verify digit entry (regression for the "every button shows 10" bug).
- Verify all four operations, chaining, and repeat `=`.
- Verify error handling (divide by zero), percent, negate, decimal, backspace,
  and clear.
- Guard the review-found fixes: history label after `=`, and repeat-`=`
  disarming on fresh operand entry.

## Key Files

- [`Calculator.Tests/UnitTest1.cs`](https://github.com/djgene01/Calculator/blob/42654e2af49a04a83557b51b7d94450987af8cd0/Calculator.Tests/UnitTest1.cs) — 26 tests (229 lines).
- [`Calculator.Tests/Calculator.Tests.csproj`](https://github.com/djgene01/Calculator/blob/42654e2af49a04a83557b51b7d94450987af8cd0/Calculator.Tests/Calculator.Tests.csproj) — `net10.0-windows`, `UseWPF`, references `Calculator`.

## Public API

- `CalculatorEngineTests` — the single test class, in xUnit `Collection("Calculator")`.
- Helper `NewVm()` returns a fresh `CalculatorViewModel` per test.

## Internal Structure

Tests drive the VM by calling `command.Execute(null)` in sequence, then assert
on `vm.Display` / `vm.HistoryLabel`. Notable tests:

| Test | Proves |
|---|---|
| `EachDigitButtonEntersItsOwnDigit` (`[Theory]`, 0–9) | Regression: each digit command enters its own digit (fails on the old loop-capture bug). |
| `ChainedOperationsResolveLeftToRight` | Immediate execution: `10+4*2 = 28`. |
| `RepeatEqualsRepeatsLastOperation` | `5+3=8` then `=` → `11`. |
| `DivisionByZeroShowsError` | Sticky `Error` state. |
| `HistoryLabelShowsCompletedOperationAfterEquals` | Review fix: label reads `5 + 3 =`, not `0 + 3 =`. |
| `DigitEntryAfterEqualsDisarmsRepeatEquals` | Review fix: typing `2` after `=` prevents a stale `2+3`. |
| `RepeatEqualsStillWorksWhenNoNewOperandEntered` | Guard: plain `=` repeats still produce `8 → 11 → 14`. |

## Dependencies

- **Uses:** `Calculator` (via `ProjectReference`), xUnit 2.9.3,
  `Microsoft.NET.Test.Sdk`, `coverlet.collector`.
- **Used by:** nothing (leaf).

## Notable Patterns / Gotchas

- **Must target `net10.0-windows` + `UseWPF`** — `RelayCommand` hooks
  `CommandManager.RequerySuggested`, which needs a WPF `Application`; a plain
  `net10.0` target won't reference WPF.
- **No `[Apartment(ApartmentState.STA)]`** — that attribute is not an xUnit
  attribute and is not needed here; the tests run fine on the default thread.
- **Collection `"Calculator"`** serializes the test class so the shared
  `Application` lifetime is well-defined.
