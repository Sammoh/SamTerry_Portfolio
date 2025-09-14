# UI, Flow, & Files (2D Canvas)

A comprehensive 2D application system demonstrating Unity's Canvas UI, data persistence, and file management capabilities.

## 🎯 Overview

This project showcases a complete report management application built using Unity's 2D Canvas system. It demonstrates professional UI/UX design, data serialization (JSON/XML), file operations, and responsive interface design for multiple resolutions.

## ✨ Key Features

### 📱 Report Management System
- **Report Creation**: Create new reports with comprehensive metadata
- **Report Listing**: Display all reports from persistent storage
- **Report Editing**: Modify existing report state and priority
- **Data Validation**: Ensure data integrity and user input validation

### 💾 Data Persistence
- **Dual Format Support**: Switchable between JSON and XML serialization
- **File System Integration**: Persistent storage in `Application.persistentDataPath`
- **Serialization Architecture**: Pluggable serializer pattern for format flexibility
- **Data Integrity**: Robust error handling and data validation

### 🖥️ Responsive UI Design
- **Multi-Resolution Support**: Canvas system optimized for different screen sizes
- **Dynamic Layouts**: Flexible UI components that adapt to content
- **User Experience**: Intuitive navigation flow between screens
- **Accessibility**: Clear visual hierarchy and user feedback

## 🏗️ Technical Implementation

### Architecture
```
2. UI, Flow, & Files (2D Canvas)/
├── Controllers/
│   ├── ReportListController.cs     # Reports list screen
│   ├── ReportDetailController.cs   # Report detail/edit screen
│   ├── NewReportController.cs      # New report creation
│   └── NavigationController.cs     # Screen navigation
├── Data/
│   ├── Report.cs                   # Report data model
│   ├── ReportManager.cs            # File operations manager
│   └── Serialization/              # Serialization implementations
├── UI/
│   ├── Screens/                    # UI screen prefabs
│   ├── Components/                 # Reusable UI components
│   └── Styles/                     # UI styling assets
└── UI_Flow.unity                   # Main demonstration scene
```

### Core Data Model
```csharp
[System.Serializable]
public class Report
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string ReporterEmail { get; set; }
    public DateTime SubmissionDate { get; set; }
    public bool Priority { get; set; }
    public ReportType Type { get; set; }          // Internal, External
    public ReportState State { get; set; }        // New, Review, Complete
}

public enum ReportType { Internal, External }
public enum ReportState { New, Review, Complete }
```

### Serialization System
```csharp
public interface ISerializer
{
    string Serialize(Report report);
    Report Deserialize(string data);
}

public class JsonSerializer : ISerializer
{
    public string Serialize(Report report) => JsonUtility.ToJson(report);
    public Report Deserialize(string data) => JsonUtility.FromJson<Report>(data);
}

public class XmlSerializer : ISerializer
{
    // XML serialization implementation
}
```

## 🚀 Getting Started

### Prerequisites
- Unity 6000.0.49f1 or later
- Understanding of Unity UI (uGUI) system
- Basic knowledge of C# serialization

### Setup Instructions
1. **Open Scene**: Load `UI_Flow.unity`
2. **Configure Settings**: 
   - Set serialization format in `ReportManager` (JSON/XML)
   - Adjust UI scaling for target resolution
3. **Run Application**: Enter play mode to test full functionality
4. **Test Workflow**: Create → List → Edit → Save reports

### Quick Start Example
```csharp
// Create new report
var report = new Report
{
    Id = System.Guid.NewGuid().ToString(),
    Title = "Sample Report",
    ReporterEmail = "user@example.com",
    SubmissionDate = DateTime.Now,
    Priority = false,
    Type = ReportType.Internal,
    State = ReportState.New
};

// Save report
ReportManager.Instance.CreateNewReport(report);

// List all reports
var allReports = ReportManager.Instance.ListReports();
```

## 🎮 User Interface Flow

### Screen Navigation
```
Main Menu
    ↓
Reports List Screen ←→ New Report Screen
    ↓
Report Detail Screen
    ↓
Edit Report Screen
```

### Screen Descriptions

#### **Reports List Screen**
- **Purpose**: Display all saved reports in a scrollable list
- **Features**: 
  - Search/filter functionality
  - Priority indicators
  - State color coding
  - Tap to open report details

#### **New Report Screen**
- **Purpose**: Create new reports with all required data
- **Features**:
  - Form validation
  - Date picker integration (planned)
  - Email validation
  - Cancel/Save operations

#### **Report Detail Screen**
- **Purpose**: View report information and modify state/priority
- **Features**:
  - Read-only data display
  - State modification controls
  - Priority toggle
  - Edit/Delete options

### UI Components

#### `ReportListItem`
```csharp
public class ReportListItem : MonoBehaviour
{
    [SerializeField] private Text titleText;
    [SerializeField] private Text statusText;
    [SerializeField] private Image priorityIndicator;
    [SerializeField] private Button selectButton;
    
    public void SetReport(Report report) { /* ... */ }
    public void OnSelectReport() { /* Navigation logic */ }
}
```

#### `StateToggleGroup`
```csharp
public class StateToggleGroup : MonoBehaviour
{
    [SerializeField] private Toggle newToggle;
    [SerializeField] private Toggle reviewToggle;
    [SerializeField] private Toggle completeToggle;
    
    public ReportState SelectedState { get; private set; }
    public event System.Action<ReportState> OnStateChanged;
}
```

## ⚙️ Configuration

### Report Manager Settings
```csharp
[SerializeField] private bool useJson = true;           // JSON vs XML
[SerializeField] private string customFolderName = "Reports"; // Storage folder
[SerializeField] private bool enableAutoSave = true;   // Auto-save on changes
[SerializeField] private float autoSaveInterval = 30f; // Auto-save frequency
```

### UI Scaling Configuration
```csharp
// Canvas Scaler settings for responsive design
CanvasScaler scaler = GetComponent<CanvasScaler>();
scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
scaler.referenceResolution = new Vector2(1920, 1080);
scaler.matchWidthOrHeight = 0.5f; // Balance between width and height matching
```

### Validation Rules
```csharp
public static class ReportValidator
{
    public static bool IsValidEmail(string email)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(email, 
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
    
    public static bool IsValidTitle(string title)
    {
        return !string.IsNullOrWhiteSpace(title) && title.Length >= 3;
    }
}
```

## 🧪 Testing

### Functional Testing
```csharp
[Test]
public void CreateReport_ValidData_SavesSuccessfully()
{
    // Arrange
    var report = CreateValidReport();
    
    // Act
    ReportManager.Instance.CreateNewReport(report);
    
    // Assert
    var savedReports = ReportManager.Instance.ListReports();
    Assert.Contains(report, savedReports);
}

[Test]
public void SerializationFormat_SwitchingFormats_PreservesData()
{
    // Test JSON to XML conversion and vice versa
}
```

### UI Testing Checklist
- [ ] **Screen Navigation**: All screens accessible and return properly
- [ ] **Form Validation**: Invalid inputs show appropriate error messages
- [ ] **Data Persistence**: Reports save and load correctly
- [ ] **Resolution Scaling**: UI elements scale properly on different resolutions
- [ ] **State Management**: Report states update correctly
- [ ] **Priority System**: Priority indicators work as expected

### Manual Testing Workflow
1. **Create Report**: Test new report creation with various data
2. **List Reports**: Verify all reports appear in list with correct information
3. **Edit Report**: Modify existing reports and verify changes persist
4. **Delete Report**: Test report deletion functionality
5. **Format Switching**: Switch between JSON/XML and verify data integrity

## 📁 File System Organization

### Storage Structure
```
Application.persistentDataPath/
└── Reports/
    ├── report1.json (or .xml)
    ├── report2.json
    └── report3.json
```

### File Naming Convention
- **Format**: `{reportId}.{extension}`
- **Extensions**: `.json` or `.xml` based on serialization setting
- **IDs**: GUID strings for uniqueness

### Data Migration
```csharp
public class DataMigrationManager
{
    public static void MigrateToNewFormat(SerializationFormat newFormat)
    {
        // Convert existing files to new format
        var existingReports = LoadAllReports();
        
        foreach (var report in existingReports)
        {
            SaveReportInFormat(report, newFormat);
        }
    }
}
```

## 🎯 Current Development Status

### ✅ Completed Features
- [x] Basic report data model
- [x] JSON/XML serialization system
- [x] File persistence manager
- [x] UI framework and navigation
- [x] Report list display
- [x] Multi-resolution canvas setup

### 🚧 In Progress
- [ ] **Date Picker Integration**: Calendar popup for date selection
- [ ] **Form Validation**: Complete input validation system
- [ ] **Report Filtering**: Filter by date, priority, and state
- [ ] **Search Functionality**: Text-based report searching
- [ ] **State Flow Management**: Complete screen navigation system

### 📋 Planned Features
- [ ] **Advanced Filtering**: Date range, multiple criteria
- [ ] **Export Functionality**: PDF/CSV export options
- [ ] **Batch Operations**: Multiple report selection and operations
- [ ] **User Preferences**: Customizable display settings
- [ ] **Offline Sync**: Local storage with cloud sync capability

## 🐛 Known Issues & Workarounds

### Date Input Handling
**Issue**: No native date picker in Unity UI
**Current Solution**: Text input with validation rules
**Planned Solution**: Integration with Unity Asset Store date picker

### Performance with Large Datasets
**Issue**: Large report lists may cause UI performance issues
**Current Solution**: Limit display to recent reports
**Planned Solution**: Virtual scrolling and pagination

### Platform-Specific Storage
**Issue**: File paths differ between platforms
**Current Solution**: Use `Application.persistentDataPath`
**Testing Required**: Verify on mobile platforms

## 🔧 Customization

### Adding New Report Fields
```csharp
// Extend Report class
public class ExtendedReport : Report
{
    public string Department { get; set; }
    public ReportSeverity Severity { get; set; }
    public List<string> Tags { get; set; }
}

// Update serialization
public class ExtendedJsonSerializer : ISerializer
{
    public string Serialize(ExtendedReport report) => JsonUtility.ToJson(report);
    // Implementation for extended fields
}
```

### Custom UI Themes
```csharp
[CreateAssetMenu(fileName = "UITheme", menuName = "Portfolio/UI Theme")]
public class UITheme : ScriptableObject
{
    public Color primaryColor;
    public Color secondaryColor;
    public Color backgroundColor;
    public Font headerFont;
    public Font bodyFont;
}
```

### Validation Extensions
```csharp
public interface IReportValidator
{
    ValidationResult Validate(Report report);
}

public class CustomReportValidator : IReportValidator
{
    public ValidationResult Validate(Report report)
    {
        // Custom validation logic
        return new ValidationResult { IsValid = true };
    }
}
```

## 📋 Dependencies

### Unity Packages
- **Unity UI** (Built-in) - Canvas and UI components
- **Unity Serialization** (Built-in) - JSON serialization

### Planned Integrations
- **Date Picker Asset**: Unity Asset Store calendar component
- **JSON.NET**: More robust JSON serialization (if needed)

### Technical Requirements
- **Unity Version**: 6000.0.49f1 or later
- **Platform**: Windows, Mac, Linux, Mobile
- **Storage**: File system access for persistent data

## 💡 Learning Outcomes

This project demonstrates:
- **Unity UI System**: Complete 2D interface development
- **Data Architecture**: Serialization patterns and file management
- **User Experience**: Professional UI/UX design practices
- **Software Patterns**: Singleton, Strategy, and Observer patterns
- **Testing**: Unit testing for data operations

---

**Note**: This system provides a solid foundation for enterprise-level report management applications and demonstrates production-ready Unity development practices.