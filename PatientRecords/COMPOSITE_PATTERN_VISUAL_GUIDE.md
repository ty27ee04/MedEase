# Composite Pattern - Visual Guide

## UML Class Diagram

```
┌─────────────────────────────────┐
│     MedicalComponent            │ ◄─── Abstract Component
│     (Abstract)                  │
├─────────────────────────────────┤
│ # name: string                  │
├─────────────────────────────────┤
│ + MedicalComponent(name)        │
│ + GetHtmlSummary(level): string │ ◄─── Common Operation
└─────────────────────────────────┘
              △
              │
              │ inherits
      ┌───────┴────────┐
      │                │
      │                │
┌─────▼──────┐   ┌────▼──────────────────┐
│SingleRecord│   │   RecordFolder        │
│   (Leaf)   │   │   (Composite)         │
├────────────┤   ├───────────────────────┤
│            │   │ - children: List<MC>  │ ◄─── Contains Children
├────────────┤   ├───────────────────────┤
│ + GetHtml  │   │ + Add(component)      │
│   Summary()│   │ + Remove(component)   │
│            │   │ + GetHtmlSummary()    │ ◄─── Recursive Call
└────────────┘   └───────────────────────┘
                          │
                          │ contains
                          ▼
                 ┌────────────────┐
                 │ MedicalComponent│
                 └────────────────┘
```

## Object Structure Example

```
root: RecordFolder ("Complete Medical History")
│
├── labResults: RecordFolder ("Laboratory Results")
│   │
│   ├── bloodTest: SingleRecord ("Blood Test - Hemoglobin: 14.5")
│   │
│   ├── urineTest: SingleRecord ("Urine Analysis - Normal")
│   │
│   └── lipidPanel: SingleRecord ("Lipid Panel - Cholesterol: 180")
│
├── imaging: RecordFolder ("Imaging Reports")
│   │
│   ├── xray: SingleRecord ("X-Ray Chest - Clear")
│   │
│   └── mri: SingleRecord ("MRI Brain - Normal")
│
├── prescriptions: RecordFolder ("Prescriptions")
│   │
│   ├── currentMeds: RecordFolder ("Current Medications")
│   │   │
│   │   ├── metformin: SingleRecord ("Metformin 500mg")
│   │   │
│   │   └── lisinopril: SingleRecord ("Lisinopril 10mg")
│   │
│   └── pastMeds: RecordFolder ("Past Medications")
│       │
│       └── amoxicillin: SingleRecord ("Amoxicillin 500mg")
│
└── checkup: SingleRecord ("General Checkup - Jan 10, 2026")
```

## Database to Object Mapping

### Database (Flat Structure)
```
┌──────────┬───────────┬───────────────┬────────────┬─────────────────────────┐
│ RecordID │ PatientID │ ParentFolderID│ RecordType │ Title                   │
├──────────┼───────────┼───────────────┼────────────┼─────────────────────────┤
│    1     │     2     │     NULL      │  Folder    │ Laboratory Results      │
│    2     │     2     │       1       │  File      │ Blood Test              │
│    3     │     2     │       1       │  File      │ Urine Analysis          │
│    4     │     2     │     NULL      │  Folder    │ Prescriptions           │
│    5     │     2     │       4       │  Folder    │ Current Medications     │
│    6     │     2     │       5       │  File      │ Metformin 500mg         │
│    7     │     2     │     NULL      │  File      │ General Checkup         │
└──────────┴───────────┴───────────────┴────────────┴─────────────────────────┘
```

### Transformed to Tree Structure
```
                        ROOT
                          │
         ┌────────────────┼────────────────┐
         │                │                │
    [ID: 1]          [ID: 4]          [ID: 7]
    Folder           Folder            File
    Lab Results      Prescriptions     Checkup
         │                │
    ┌────┴───┐           │
    │        │      [ID: 5]
 [ID: 2]  [ID: 3]   Folder
  File     File     Current Meds
  Blood    Urine         │
  Test     Analysis      │
                    [ID: 6]
                     File
                     Metformin
```

## Tree Building Algorithm Flow

```
Step 1: Load All Records from Database
┌─────────────────────────────────────┐
│ Get all records for patientID = 2   │
│ Result: List<RecordModel>           │
└────────────┬────────────────────────┘
             │
             ▼
Step 2: Create Folder Objects First
┌─────────────────────────────────────┐
│ Loop through records                │
│ If RecordType == "Folder":          │
│   - Create RecordFolder object      │
│   - Store in folderMap[recordID]    │
└────────────┬────────────────────────┘
             │
             ▼
Step 3: Build Hierarchy
┌─────────────────────────────────────┐
│ Loop through records again          │
│ For each record:                    │
│   If RecordType == "Folder":        │
│     component = folderMap[id]       │
│   Else:                             │
│     component = new SingleRecord()  │
│                                     │
│   If has ParentFolderID:            │
│     parent.Add(component)           │
│   Else:                             │
│     root.Add(component)             │
└────────────┬────────────────────────┘
             │
             ▼
Step 4: Render HTML
┌─────────────────────────────────────┐
│ root.GetHtmlSummary(0)              │
│   → Recursively calls GetHtml on    │
│     all children                    │
└─────────────────────────────────────┘
```

## Method Call Flow (Recursion)

```
User clicks "Load Full History"
        │
        ▼
┌─────────────────────────┐
│ LoadFullHistory()       │
└──────────┬──────────────┘
           │
           ▼
┌─────────────────────────┐
│ GetPatientRecords(2)    │ ─── Database Query
└──────────┬──────────────┘
           │
           ▼
┌─────────────────────────┐
│ BuildRecordTree(records)│
└──────────┬──────────────┘
           │
           ▼
┌─────────────────────────┐
│ root.GetHtmlSummary(0)  │ ◄──┐
└──────────┬──────────────┘    │
           │                   │ Recursive
           ▼                   │ Calls
┌─────────────────────────┐    │
│ child1.GetHtmlSummary(1)│ ───┤
└──────────┬──────────────┘    │
           │                   │
           ▼                   │
┌─────────────────────────┐    │
│ child2.GetHtmlSummary(1)│ ───┤
└──────────┬──────────────┘    │
           │                   │
           ▼                   │
┌─────────────────────────┐    │
│   ... (continues)       │ ───┘
└─────────────────────────┘
```

## Add Record Flow

```
User fills form and clicks "Add Record"
        │
        ▼
┌──────────────────────────────┐
│ AddNewRecord()               │
│  - Creates RecordModel       │
│  - PatientID = 2             │
│  - ParentFolderID = ?        │
│  - RecordType = "File/Folder"│
│  - Title, Details            │
└──────────┬───────────────────┘
           │
           ▼
┌──────────────────────────────┐
│ DatabaseManager              │
│  .AddPatientRecord(record)   │
└──────────┬───────────────────┘
           │
           ▼
┌──────────────────────────────┐
│ INSERT INTO PatientRecord... │ ─── SQL Insert
└──────────┬───────────────────┘
           │
           ▼
┌──────────────────────────────┐
│ LoadFullHistory()            │ ─── Refresh UI
│  - Rebuilds entire tree      │
│  - Shows new record          │
└──────────────────────────────┘
```

## Pattern Benefits Illustrated

### Before Composite Pattern (Bad Approach):
```csharp
// Messy if-else checking everywhere!
void DisplayRecord(RecordModel record) {
    if (record.Type == "Folder") {
        // Display folder
        var children = GetChildren(record.ID);
        foreach (var child in children) {
            if (child.Type == "Folder") {
                // Display nested folder
                var grandChildren = GetChildren(child.ID);
                foreach (var gc in grandChildren) {
                    // ... endless nesting!
                }
            } else {
                // Display file
            }
        }
    } else {
        // Display file
    }
}
```

### With Composite Pattern (Clean):
```csharp
// One line! Pattern handles all complexity
string html = root.GetHtmlSummary(0);
```

## Memory Structure

```
Root RecordFolder Object
├── Reference to List<MedicalComponent>
│   │
│   ├── [0] → RecordFolder ("Lab Results")
│   │         ├── Reference to List<MedicalComponent>
│   │         │   ├── [0] → SingleRecord ("Blood Test")
│   │         │   └── [1] → SingleRecord ("Urine Test")
│   │         
│   ├── [1] → RecordFolder ("Prescriptions")
│   │         ├── Reference to List<MedicalComponent>
│   │         │   └── [0] → RecordFolder ("Current Meds")
│   │         │             └── Reference to List<MedicalComponent>
│   │         │                 └── [0] → SingleRecord ("Metformin")
│   │
│   └── [2] → SingleRecord ("General Checkup")
```

## Real-World Analogy

Think of a file system:

```
📁 C:\Documents (Folder = Composite)
│
├── 📁 Work (Folder = Composite)
│   ├── 📄 Report.docx (File = Leaf)
│   └── 📄 Presentation.pptx (File = Leaf)
│
├── 📁 Personal (Folder = Composite)
│   ├── 📁 Photos (Folder = Composite)
│   │   ├── 📄 vacation1.jpg (File = Leaf)
│   │   └── 📄 vacation2.jpg (File = Leaf)
│   │
│   └── 📄 diary.txt (File = Leaf)
│
└── 📄 readme.txt (File = Leaf)
```

Both folders and files can be:
- Displayed
- Copied
- Deleted
- Moved

But only folders can **contain** other items!

## Key Takeaways

1. **Uniform Interface**: Both leaf and composite implement the same interface
2. **Recursive Structure**: Composites can contain other composites (folders in folders)
3. **Transparency**: Client doesn't need to know if it's dealing with a leaf or composite
4. **Flexibility**: Easy to add new nodes, reorganize tree, or add new operations

This is the essence of the Composite Design Pattern! 🎯
