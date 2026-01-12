# Composite Pattern Implementation - Quick Reference

## Project: Patient Record Management System

### Design Pattern: **Composite Pattern** (Structural)

---

## 📋 Quick Overview

**Purpose**: Manage hierarchical patient medical records where folders can contain both individual records and other folders.

**Pattern Type**: Structural Design Pattern

**Use Case**: Medical records organized in a tree structure (like a file system)

---

## 🎯 Key Components

| Component | Class | Role |
|-----------|-------|------|
| **Component** | `MedicalComponent` | Abstract base class |
| **Leaf** | `SingleRecord` | Individual record (cannot have children) |
| **Composite** | `RecordFolder` | Folder (can contain children) |

---

## 📁 File Structure

```
MedEase/
├── Services/
│   ├── CompositePattern.cs          ← Pattern implementation (3 classes)
│   ├── DatabaseManager.cs           ← Database CRUD operations
│   └── Models.cs                    ← RecordModel class
│
├── Components/Pages/
│   └── PatientRecords.razor         ← UI and tree building logic
│
└── PatientRecords/                  ← Documentation
    ├── SampleData.sql               ← Test data
    ├── COMPOSITE_PATTERN_VISUAL_GUIDE.md
    └── TESTING_GUIDE.md
```

---

## 💻 Core Code

### 1. Abstract Base (Component)
```csharp
public abstract class MedicalComponent
{
    protected string name;
    public MedicalComponent(string name) { this.name = name; }
    public abstract string GetHtmlSummary(int indentLevel = 0);
}
```

### 2. Leaf Node
```csharp
public class SingleRecord : MedicalComponent
{
    public override string GetHtmlSummary(int indentLevel)
    {
        return $"<div style='margin-left:{indentLevel * 20}px'>📄 {name}</div>";
    }
}
```

### 3. Composite Node
```csharp
public class RecordFolder : MedicalComponent
{
    private List<MedicalComponent> _children = new List<MedicalComponent>();
    
    public void Add(MedicalComponent component) 
    { 
        _children.Add(component); 
    }
    
    public override string GetHtmlSummary(int indentLevel)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"<div style='margin-left:{indentLevel * 20}px; font-weight:bold;'>📂 {name}</div>");
        
        foreach (var child in _children)
            sb.Append(child.GetHtmlSummary(indentLevel + 1)); // Recursion!
            
        return sb.ToString();
    }
}
```

---

## 🗄️ Database Schema

```sql
CREATE TABLE PatientRecord (
    recordID INT PRIMARY KEY AUTO_INCREMENT,
    patientID INT NOT NULL,
    parentFolderID INT NULL,              -- Key field for hierarchy!
    recordType VARCHAR(10) NOT NULL,       -- 'File' or 'Folder'
    title VARCHAR(255) NOT NULL,
    details TEXT,
    FOREIGN KEY (patientID) REFERENCES User(userID),
    FOREIGN KEY (parentFolderID) REFERENCES PatientRecord(recordID)
);
```

**Key Points**:
- `parentFolderID = NULL` → Root level item
- `parentFolderID = X` → Child of record with ID = X
- `recordType` → Distinguishes folders from files

---

## 🔨 Tree Building Algorithm

```csharp
private RecordFolder BuildRecordTree(List<RecordModel> allRecords)
{
    RecordFolder root = new RecordFolder("📋 Complete Medical History");
    Dictionary<int, RecordFolder> folderMap = new Dictionary<int, RecordFolder>();
    
    // PASS 1: Create all folders
    foreach (var record in allRecords)
    {
        if (record.RecordType == "Folder")
        {
            var folder = new RecordFolder(record.Title);
            folderMap[record.RecordID] = folder;
        }
    }
    
    // PASS 2: Build hierarchy
    foreach (var record in allRecords)
    {
        MedicalComponent component;
        
        if (record.RecordType == "Folder")
            component = folderMap[record.RecordID];
        else
            component = new SingleRecord($"{record.Title} - {record.Details}");
        
        // Add to parent or root
        if (record.ParentFolderID.HasValue && folderMap.ContainsKey(record.ParentFolderID.Value))
            folderMap[record.ParentFolderID.Value].Add(component);
        else
            root.Add(component);
    }
    
    return root;
}
```

**Why Two Passes?**
- Pass 1: Create all folders first (so they can be referenced as parents)
- Pass 2: Link everything together (children to parents)

---

## 🎬 Usage Examples

### Creating a Simple Tree (Hardcoded)
```csharp
RecordFolder root = new RecordFolder("Medical Records");

RecordFolder labResults = new RecordFolder("Lab Results");
labResults.Add(new SingleRecord("Blood Test - Hemoglobin: 14.5"));
labResults.Add(new SingleRecord("Urine Test - Normal"));

root.Add(labResults);
root.Add(new SingleRecord("General Checkup - Jan 10"));

string html = root.GetHtmlSummary(0);
```

### Loading from Database
```csharp
var dbRecords = DatabaseManager.GetInstance().GetPatientRecords(patientId);
RecordFolder tree = BuildRecordTree(dbRecords);
string html = tree.GetHtmlSummary(0);
```

### Adding a New Record
```csharp
var newRecord = new RecordModel
{
    PatientID = 2,
    ParentFolderID = 5,        // ID of parent folder (or NULL)
    RecordType = "File",       // "File" or "Folder"
    Title = "Blood Test Results",
    Details = "Hemoglobin: 14.5 g/dL"
};

DatabaseManager.GetInstance().AddPatientRecord(newRecord);
```

---

## ✅ Pattern Benefits

| Benefit | Explanation |
|---------|-------------|
| **Uniform Treatment** | Same interface for files and folders |
| **Recursive Structure** | Naturally handles nested hierarchies |
| **Easy to Extend** | Add new component types easily |
| **Flexible** | Reorganize tree without code changes |
| **Maintainable** | Clean, organized code structure |

---

## 🧪 Quick Test

1. **Start XAMPP** → MySQL must be running
2. **Run sample data**:
   ```sql
   source PatientRecords/SampleData.sql
   ```
3. **Run application**:
   ```bash
   dotnet watch
   ```
4. **Navigate**: http://localhost:5000/patient-records
5. **Click**: "Load Full History"
6. **Verify**: See hierarchical tree structure

---

## 🐛 Common Issues

| Problem | Solution |
|---------|----------|
| No records displayed | Check patientID in code matches database |
| Flat hierarchy | Verify `parentFolderID` values in database |
| Connection error | Check XAMPP MySQL is running |
| Empty folders | Check `BuildRecordTree()` parent-child linking |

---

## 📊 Example Output

```
📋 Complete Medical History
  📂 Laboratory Results
    📄 Blood Test - Hemoglobin: 14.5 g/dL
    📄 Urine Analysis - Normal
  📂 Prescriptions
    📂 Current Medications
      📄 Metformin 500mg - Twice daily
      📄 Lisinopril 10mg - Once daily
    📂 Past Medications
      📄 Amoxicillin 500mg - Completed
  📄 General Checkup - Jan 10, 2026
```

---

## 🎓 Presentation Tips

1. **Start with a real-world example**: File system, organization chart
2. **Show the UML diagram**: Component → Leaf & Composite
3. **Explain the problem**: Need to treat individual items and groups uniformly
4. **Live demo**: Add a folder, add files, show hierarchy
5. **Highlight benefits**: Recursion, flexibility, extensibility
6. **Show database**: How flat table becomes tree structure

---

## 📚 Key Concepts to Explain

1. **Component Pattern**: Abstract base class
2. **Leaf vs Composite**: Files vs Folders
3. **Recursion**: How `GetHtmlSummary()` calls itself
4. **Two-Pass Algorithm**: Why we build folders first
5. **Database Design**: Self-referencing foreign key
6. **Polymorphism**: Treating different types uniformly

---

## 🔑 Important Code Locations

- Pattern implementation: [Services/CompositePattern.cs](Services/CompositePattern.cs)
- Tree building: [Components/Pages/PatientRecords.razor](Components/Pages/PatientRecords.razor#L85-L117)
- Database operations: [Services/DatabaseManager.cs](Services/DatabaseManager.cs#L200-L250)
- Data model: [Services/Models.cs](Services/Models.cs#L38-L46)

---

## 📖 Additional Resources

- Full explanation: `COMPOSITE_PATTERN_EXPLANATION.md`
- Visual guide: `PatientRecords/COMPOSITE_PATTERN_VISUAL_GUIDE.md`
- Testing guide: `PatientRecords/TESTING_GUIDE.md`
- Sample data: `PatientRecords/SampleData.sql`

---

## ✨ Summary

**What**: Hierarchical patient medical records
**How**: Composite design pattern with recursive structure
**Why**: Uniform treatment of files and folders, flexible organization
**Result**: Clean, maintainable, extensible code

---

### Quick Commands

```bash
# Start application
dotnet watch

# Run SQL script
mysql -u root MedEaseDB < PatientRecords/SampleData.sql

# Check database
mysql -u root
USE MedEaseDB;
SELECT * FROM PatientRecord WHERE patientID = 2;
```

---

**Good luck with your software design project! 🚀**
