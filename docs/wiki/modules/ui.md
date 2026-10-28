# Module: `ui`

The WPF presentation: the `MainWindow` visual tree (display + a 4×6 button
grid), the button styling, and the keyboard layer. All behaviour is delegated
to the view model's commands — the code-behind contains no calculator logic.

## Responsibilities

- Define the window layout: a history line, a large display, and a 4-column ×
  6-row button grid.
- Provide a shared `ControlTemplate` plus three button styles (digit, function,
  accent) with hover/press feedback.
- Translate physical keys into the same commands the buttons use, so keyboard
  and mouse input are handled identically.

## Key Files

- [`Calculator/MainWindow.xaml`](https://github.com/djgene01/Calculator/blob/42654e2af49a04a83557b51b7d94450987af8cd0/Calculator/MainWindow.xaml) — layout, palette, button template/styles, command bindings (182 lines).
- [`Calculator/MainWindow.xaml.cs`](https://github.com/djgene01/Calculator/blob/42654e2af49a04a83557b51b7d94450987af8cd0/Calculator/MainWindow.xaml.cs) — keyboard mapping (79 lines).
- [`Calculator/App.xaml`](https://github.com/djgene01/Calculator/blob/42654e2af49a04a83557b51b7d94450987af8cd0/Calculator/App.xaml) — `StartupUri` to `MainWindow.xaml`.

## Public API

- `MainWindow : Window` — the only constructible UI type; its `DataContext` is
  set in XAML to a new `CalculatorViewModel`.

## Internal Structure

**XAML** (`MainWindow.xaml`):
- `Window.DataContext` instantiates `<local:CalculatorViewModel/>`.
- `Window.Resources` define a palette of `Color`/`SolidColorBrush` resources and
  three styles that all reuse one `CalculatorButtonTemplate` (a rounded `Border`
  with `IsMouseOver`/`IsPressed` triggers).
- The display is two right-aligned `TextBlock`s (`HistoryLabel`, `DisplayLabel`)
  bound `OneWay` to the view model.
- Buttons bind their `Command` to the matching view-model command property. The
  `=` button uses the accent style; the `.` button spans three columns.

**Code-behind** (`MainWindow.xaml.cs`):
- `OnPreviewKeyDown` → `MapKey(e.Key)` → command-name string → `ExecuteCommand`.
- `MapKey` is a `switch` over `System.Windows.Input.Key`. **Platform gotcha:**
  the numpad operators arrive as `Key.Add`/`Subtract`/`Multiply`/`Divide` (there
  is no `Key.NumPadAdd`), and the numpad/`decimal` key is `Key.Decimal`. The
  main-row `Key.D8` is the digit 8, not the decimal point.
- `ExecuteCommand` resolves the `CalculatorViewModel` from `DataContext` and
  dispatches by command-name string, then sets `e.Handled` so the key isn't
  processed again.

## Dependencies

- **Used by:** `App` (startup).
- **Uses:** `view-model` (commands + display bindings), WPF (`System.Windows`,
  `System.Windows.Input`).

## Notable Patterns / Gotchas

- **Single source of truth** — both the mouse and the keyboard funnel into the
  same view-model commands; there is no parallel keyboard logic in the engine.
- **`Focusable=False`** on all buttons keeps the display as the focus target so
  typed input isn't stolen by a focused button.
- **`ResizeMode=CanMinimize`** — the window is a fixed 380×560 size by design.
