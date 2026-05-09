# DEFINITION OF DONE & ANALYSIS PROMPT TEMPLATES
# Use these checklists and prompts AFTER the AI generates each feature.

---

## ACCEPTANCE CRITERIA (Every Feature Must Pass All of These)

### Code Quality
- [ ] No inline SQL — all queries use SqlParameter
- [ ] DAL, BLL, and Form layers are separate (no DB calls in Forms)
- [ ] All SqlConnection objects are inside using() blocks
- [ ] No raw exception messages shown to user (MessageBox only)
- [ ] All form controls follow naming convention (btn, txt, lbl, dgv, cmb)
- [ ] Every public method has an XML doc comment (/// <summary>)

### Functional Correctness
- [ ] Feature works with valid input (happy path tested)
- [ ] Feature rejects invalid input (empty, wrong format, out of range)
- [ ] Database updates correctly after the action
- [ ] Session data (SessionManager) is used correctly
- [ ] Duplicate data is prevented (e.g., can't join same society twice)

### UI/UX
- [ ] Form opens without crashing
- [ ] All required controls are present per ui_specifications.md
- [ ] Error state is shown to user in red label or MessageBox
- [ ] Navigation works (back button, logout returns to Login)

---

## ANALYSIS PROMPT TEMPLATES
# Copy-paste these prompts into your AI agent AFTER it writes a function.

---

### TEMPLATE A — After Writing Any Function (Task 2 + Task 5)
For the function [FUNCTION_NAME] you just wrote:

CYCLOMATIC COMPLEXITY: Count every decision point (if, else if, switch case,
for, while, do-while, catch, && / ||). Add 1. State the final CC number.
TEST CASES: Generate a table with exactly 3 test cases:
| Test Case | Input Values | Expected Output | Path Covered |
Include: one happy path, one boundary/edge case, one invalid input case.
FAULT INJECTION: List exactly 5 faults you could inject into this function:
| Fault # | Fault Type | What You Change | Expected Wrong Behavior |
Fault types to consider: off-by-one, wrong operator (<= vs <), null not checked,
wrong variable used, missing condition, wrong SQL column name.
---

### TEMPLATE B — After Completing a Full Module (Task 3 + Task 4)
For the module/class [CLASS_NAME] you just completed:

STRUCTURAL METRIC — Lines of Code (LOC):
Count: total LOC, blank lines, comment lines, executable LOC.
CK METRICS — Calculate all 6:

WMC: Count all methods. If each method has CC=1, WMC = number of methods.
DIT: How many levels deep is this class in the inheritance tree? (0 if no parent)
NOC: How many classes inherit from this class?
CBO: List every other class this class imports/uses. Count = CBO.
RFC: Count all methods in this class + all external methods it calls.
LCOM: Count method pairs that share NO instance variables.
LCOM = (pairs sharing nothing) - (pairs sharing something). Min 0.


REFACTORING: List any issues found and suggest how to fix them.


---

### TEMPLATE C — After All UI Forms Are Done (Task 6 — KLM)
For the form [FORM_NAME], simulate the task: [DESCRIBE THE USER TASK].
List every user action in sequence using KLM operators:

K = Keystroke (280ms) — each key press
M = Mental preparation (1350ms) — before starting a new action
P = Point/Click (1100ms) — moving mouse and clicking
H = Hand movement (400ms) — switching between keyboard and mouse

Format:
StepOperatorTime (ms)Description1M1350Read the form and locate Email field2H400Move hand to keyboard3K×154200Type email address (15 keystrokes)...
Total time = sum of all rows in ms. Convert to seconds.

---

### TEMPLATE D — Final Code Scan (Task 7 + Task 8)
Scan the entire generated project and provide:

LINES OF CODE COUNT:

Total LOC (including blanks and comments)
Commented lines only (// and /// lines)
Documentation Ratio = Total LOC / Commented Lines

COCOMO ESTIMATE:
Total KLOC (LOC / 1000)
Classify project as: Organic (small, familiar team), Semi-detached, or Embedded
Justification for classification
Apply Basic COCOMO formula:
Effort (PM) = a × (KLOC)^b
Time (TDEV) = c × (PM)^d
(Use Organic mode: a=2.4, b=1.05, c=2.5, d=0.38)
State: Estimated person-months, estimated duration, recommended team size
---
## PHASE COMPLETION GATE
Before moving to the next phase in the roadmap, ALL of the following must be true:
1. All acceptance criteria above are checked off for this phase's features
2. Template A has been run on every new function in this phase
3. No compilation errors exist in the solution
4. The app runs and the new feature can be demoed end-to-end

