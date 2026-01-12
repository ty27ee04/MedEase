# Patient Record Management - Composite Design Pattern

## Overview
This implementation demonstrates the **Composite Design Pattern** for managing hierarchical patient medical records in the HealthTech system.

## What is the Composite Pattern?

The Composite Pattern is a structural design pattern that lets you compose objects into tree structures to represent part-whole hierarchies. It allows clients to treat individual objects and compositions of objects uniformly.

### Key Components:

1. **Component (Abstract Base)** - `MedicalComponent`
   - Common interface for both leaf and composite objects
   - Defines the `GetHtmlSummary()` operation

2. **Leaf** - `SingleRecord`
   - Represents individual medical records (cannot contain other records)
   - Examples: Blood test results, prescriptions, X-ray reports

3. **Composite** - `RecordFolder`
   - Represents folders that can contain both single records and other folders
   - Manages child components
   - Examples: "Laboratory Results" folder, "Prescriptions" folder

## Implementation Structure

```
MedicalComponent (Abstract)
├── SingleRecord (Leaf)
│   - Represents individual medical records
│   - Cannot have children
│
└── RecordFolder (Composite)
    - Represents folders/categories
    - Can contain SingleRecord objects
    - Can contain other RecordFolder objects (nested hierarchy)
```

## Code Breakdown

### 1. Component Base Class (`CompositePattern.cs`)

```csharp
public abstract class MedicalComponent
{
    protected string name;
    
    public MedicalComponent(string name) 
    { 
        this.name = name; 
    }

    // Common operation for all components
    public abstract string GetHtmlSummary(int indentLevel = 0);
}
```

### 2. Leaf Class - SingleRecord

```csharp
public class SingleRecord : MedicalComponent
{
    public SingleRecord(string name) : base(name) { }

    public override string GetHtmlSummary(int indentLevel)
    {
        return $"<div style='margin-left:{indentLevel * 20}px'>📄 {name}</div>";
    }
}
```

**Key Points:**
- Represents the end nodes of the tree
- No children management methods
- Simple rendering

### 3. Composite Class - RecordFolder

```csharp
public class RecordFolder : MedicalComponent
{
    private List<MedicalComponent> _children = new List<MedicalComponent>();

    public RecordFolder(string name) : base(name) { }

    public void Add(MedicalComponent component)
    {
        _children.Add(component);
    }

    public void Remove(MedicalComponent component)
    {
        _children.Remove(component);
    }

    public override string GetHtmlSummary(int indentLevel)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"<div style='margin-left:{indentLevel * 20}px; font-weight:bold;'>📂 {name}</div>");

        // Recursively print children
        foreach (var child in _children)
        {
            sb.Append(child.GetHtmlSummary(indentLevel + 1));
        }
        
        return sb.ToString();
    }
}
```

**Key Points:**
- Can contain both SingleRecord and other RecordFolder objects
- Manages a collection of children
- Recursively processes all children
- Uniform treatment: doesn't need to know if child is leaf or composite

## Real-World Example

### Sample Hierarchy:

```
📋 Complete Medical History (Root Folder)
│
├── 🧪 Laboratory Results (Folder)
│   ├── 📄 Blood Test - Hemoglobin: 14.5 g/dL
│   ├── 📄 Urine Analysis - Normal
│   └── 📄 Lipid Panel - Cholesterol: 180 mg/dL
│
├── 🩻 Imaging Reports (Folder)
│   ├── 📄 X-Ray Chest - Clear
│   └── 📄 MRI Brain - No abnormalities
│
├── 💊 Prescriptions (Folder)
│   ├── 📂 Current Medications (Nested Folder)
│   │   ├── 📄 Metformin 500mg - Twice daily
│   │   └── 📄 Lisinopril 10mg - Once daily
│   │
│   └── 📂 Past Medications (Nested Folder)
│       └── 📄 Amoxicillin 500mg - Completed
│
└── 📄 General Checkup - Jan 10, 2026
```

## Database Integration

### Database Schema:

```sql
CREATE TABLE PatientRecord (
    recordID INT PRIMARY KEY AUTO_INCREMENT,
    patientID INT NOT NULL,
    parentFolderID INT NULL,              -- NULL = root level
    recordType VARCHAR(10) NOT NULL,       -- 'File' or 'Folder'
    title VARCHAR(255) NOT NULL,
    details TEXT,
    FOREIGN KEY (patientID) REFERENCES User(userID),
    FOREIGN KEY (parentFolderID) REFERENCES PatientRecord(recordID)
);
```

### Tree Building Algorithm (`PatientRecords.razor`)

```csharp
private RecordFolder BuildRecordTree(List<RecordModel> allRecords)
{
    RecordFolder root = new RecordFolder("📋 Complete Medical History");
    
    // Step 1: Create all folders first (for reference)
    Dictionary<int, RecordFolder> folderMap = new Dictionary<int, RecordFolder>();
    foreach (var record in allRecords)
    {
        if (record.RecordType == "Folder")
        {
            var folder = new RecordFolder($"{record.Title} (ID: {record.RecordID})");
            folderMap[record.RecordID] = folder;
        }
    }
    
    // Step 2: Build hierarchy by linking parents and children
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

## Benefits of This Pattern

### 1. **Uniform Treatment**
```csharp
// Client code doesn't need to distinguish between leaf and composite
MedicalComponent record = new SingleRecord("Blood Test");
MedicalComponent folder = new RecordFolder("Lab Results");

// Both use the same interface
string html1 = record.GetHtmlSummary();
string html2 = folder.GetHtmlSummary();
```

### 2. **Easy to Add New Types**
Want to add a new type like `SharedRecord` or `EncryptedRecord`? Just extend `MedicalComponent`.

### 3. **Recursive Operations**
Operations on the root automatically apply to the entire tree:
```csharp
RecordFolder root = BuildRecordTree(allRecords);
string entireTree = root.GetHtmlSummary(0); // Recursively renders everything
```

### 4. **Flexible Structure**
- Add/remove records dynamically
- Nest folders arbitrarily deep
- Reorganize hierarchy easily

## CRUD Operations

### Create (Add Record)
```csharp
var newRecord = new RecordModel
{
    PatientID = 2,
    ParentFolderID = 5,        // ID of parent folder (null for root)
    RecordType = "File",       // or "Folder"
    Title = "Blood Test Results",
    Details = "Hemoglobin: 14.5 g/dL"
};

DatabaseManager.GetInstance().AddPatientRecord(newRecord);
```

### Read (Load Hierarchy)
```csharp
var dbRecords = DatabaseManager.GetInstance().GetPatientRecords(patientId);
RecordFolder tree = BuildRecordTree(dbRecords);
string html = tree.GetHtmlSummary(0);
```

### Update
Update records by modifying database entries and rebuilding the tree.

### Delete
```csharp
DatabaseManager.GetInstance().DeletePatientRecord(recordId);
// Automatically deletes all children if it's a folder
```

## Testing Your Implementation

### Step 1: Add Sample Data to Database

```sql
-- Add root-level folder
INSERT INTO PatientRecord (patientID, parentFolderID, recordType, title, details)
VALUES (2, NULL, 'Folder', 'Laboratory Results', '');

-- Get the recordID of the folder you just created (let's say it's 1)

-- Add records inside that folder
INSERT INTO PatientRecord (patientID, parentFolderID, recordType, title, details)
VALUES 
(2, 1, 'File', 'Blood Test', 'Hemoglobin: 14.5 g/dL'),
(2, 1, 'File', 'Urine Analysis', 'Normal');

-- Add another root-level record
INSERT INTO PatientRecord (patientID, parentFolderID, recordType, title, details)
VALUES (2, NULL, 'File', 'General Checkup', 'All vitals normal');
```

### Step 2: Test in UI
1. Navigate to `/patient-records`
2. Click "Load Full History" to see database records
3. Click "Load Sample Data" to see hardcoded example
4. Use the form to add new records

## Key Design Pattern Principles Demonstrated

✅ **Single Responsibility**: Each class has one clear purpose
✅ **Open/Closed**: Easy to extend with new component types
✅ **Polymorphism**: Treat leaves and composites uniformly
✅ **Recursion**: Natural tree traversal
✅ **Encapsulation**: Internal structure hidden from clients

## Common Interview Questions

**Q: Why not just use a flat list?**
A: Flat lists don't represent hierarchy well. Composite pattern maintains parent-child relationships and allows recursive operations.

**Q: What's the difference between Composite and Decorator patterns?**
A: Composite manages multiple children (tree structure), Decorator adds behavior to a single object (wrapping).

**Q: Can a leaf have children?**
A: No, by definition a leaf cannot have children. Only composites can contain other components.

## Conclusion

This implementation showcases:
- ✅ Proper Composite Pattern structure
- ✅ Database integration with hierarchical data
- ✅ Recursive tree building from flat database records
- ✅ CRUD operations maintaining hierarchy
- ✅ Clean separation of concerns
- ✅ Real-world medical records management scenario

The pattern elegantly handles the complexity of nested medical records while keeping the code simple and maintainable.
