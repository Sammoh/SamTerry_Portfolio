# Data Flow Diagram - Before and After Changes

## Before Changes

```
┌─────────────────────────────────────────────────┐
│           Report Data Model                     │
│                                                  │
│  Type: string                                   │
│  Priority: PriorityLevel (enum)                 │
│  State: ReportState (enum)                      │
└─────────────────────────────────────────────────┘
                    ▼
┌─────────────────────────────────────────────────┐
│      NewReportController                        │
│                                                  │
│  Type = typeToggle.isOn ?                       │
│         "External" : "Internal"  ❌ Hardcoded  │
└─────────────────────────────────────────────────┘
                    ▼
┌─────────────────────────────────────────────────┐
│      OpenReportController                       │
│                                                  │
│  type.text = _currentReport.Type  ✓ String     │
└─────────────────────────────────────────────────┘
                    
┌─────────────────────────────────────────────────┐
│      ListReportsController                      │
│                                                  │
│  Priority Dropdown:                             │
│    Options: ["High", "Low"]  ❌ Wrong Order    │
│                                                  │
│  Listener:                                      │
│    stateDropdown.onValueChanged                 │
│      .AddListener(FilterByPriority) ❌ Bug     │
│                                                  │
│  Filter Logic:                                  │
│    priority == 0 ? High : Low  ❌ Inverted     │
└─────────────────────────────────────────────────┘
```

## After Changes

```
┌─────────────────────────────────────────────────┐
│           Report Data Model                     │
│                                                  │
│  Type: ReportType (enum)  ✅ Type-Safe         │
│    - Internal                                   │
│    - External                                   │
│                                                  │
│  Priority: PriorityLevel (enum)                 │
│    - none                                       │
│    - Low                                        │
│    - High                                       │
│                                                  │
│  State: ReportState (enum)                      │
│    - none                                       │
│    - New                                        │
│    - Review                                     │
│    - Complete                                   │
└─────────────────────────────────────────────────┘
                    ▼
┌─────────────────────────────────────────────────┐
│      NewReportController                        │
│                                                  │
│  Type = typeToggle.isOn ?                       │
│         ReportType.External :                   │
│         ReportType.Internal  ✅ Type-Safe       │
└─────────────────────────────────────────────────┘
                    ▼
┌─────────────────────────────────────────────────┐
│      OpenReportController                       │
│                                                  │
│  type.text = _currentReport.Type.ToString()     │
│              ✅ Enum to String                  │
└─────────────────────────────────────────────────┘
                    
┌─────────────────────────────────────────────────┐
│      ListReportsController                      │
│                                                  │
│  Priority Dropdown:                             │
│    Options: Enum.GetNames(PriorityLevel)        │
│            .Where(name != "none")               │
│    Result: ["Low", "High"]  ✅ Correct Order   │
│                                                  │
│  Listener:                                      │
│    priorityDropdown.onValueChanged              │
│      .AddListener(FilterByPriority)  ✅ Fixed  │
│                                                  │
│  Filter Logic:                                  │
│    priority == 0 ? Low : High  ✅ Correct      │
└─────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────┐
│      CommandController (NEW)                    │
│                                                  │
│  Commands:                                      │
│    /help    - List all commands                 │
│    /clear   - Clear output                      │
│    /list    - List all reports                  │
│    /new     - Open new report screen            │
│    /refresh - Refresh reports list              │
│                                                  │
│  Components:                                    │
│    TMP_InputField: commandInput                 │
│    TMP_Text: outputText                         │
│                                                  │
│  Usage:                                         │
│    User types "/help" → Shows all commands      │
└─────────────────────────────────────────────────┘
```

## Enum Mapping

### PriorityLevel Enum
```
Index in Enum    Dropdown Index    Display Name
─────────────────────────────────────────────────
0: none          (filtered out)    -
1: Low           0                 "Low"
2: High          1                 "High"
```

### ReportType Enum
```
Enum Value       Display Name      Used In
─────────────────────────────────────────────────
0: Internal      "Internal"        NewReportController
1: External      "External"        Toggle: isOn = true
```

### ReportState Enum
```
Index in Enum    Dropdown Index    Display Name
─────────────────────────────────────────────────
0: none          0                 "none"
1: New           1                 "New"
2: Review        2                 "Review"
3: Complete      3                 "Complete"
```

## Data Consistency Matrix

| Component              | Before                | After                      | Status |
|------------------------|----------------------|----------------------------|--------|
| Report.Type            | string               | ReportType enum            | ✅ Fixed |
| NewReportController    | "Internal"/"External"| ReportType.Internal/.External | ✅ Fixed |
| OpenReportController   | Direct string        | enum.ToString()            | ✅ Fixed |
| Priority Dropdown      | ["High", "Low"]      | ["Low", "High"]            | ✅ Fixed |
| Priority Listener      | stateDropdown (bug)  | priorityDropdown           | ✅ Fixed |
| Priority Filter Logic  | Inverted             | Correct                    | ✅ Fixed |
| State Dropdown         | Enum.GetNames()      | Enum.GetNames()            | ✅ Already correct |
| Help Command           | Not implemented      | CommandController          | ✅ Added |

## Command System Architecture

```
User Input
    ↓
TMP_InputField (commandInput)
    ↓
CommandController.ProcessCommand(string)
    ↓
Switch on command
    ├─ /help    → ShowHelp()
    ├─ /clear   → ClearOutput()
    ├─ /list    → ListReports()
    ├─ /new     → CreateNewReport()
    ├─ /refresh → RefreshReports()
    └─ default  → Unknown command message
    ↓
TMP_Text (outputText)
    ↓
User sees result
```
