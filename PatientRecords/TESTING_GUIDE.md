# Testing Guide & Troubleshooting

## How to Test Your Composite Pattern Implementation

### Prerequisites
1. Make sure your MySQL/XAMPP server is running
2. Database `MedEaseDB` exists
3. Table `PatientRecord` is created with the correct schema
4. At least one patient exists in the `User` table

### Test Scenario 1: Load Sample Data (Hardcoded)
**Purpose**: Verify the Composite Pattern works without database dependency

**Steps**:
1. Navigate to http://localhost:5000/patient-records
2. Click "📋 Load Sample Data" button
3. **Expected Result**: 
   - See a hierarchical tree structure
   - Folders (📂) should be bold
   - Files (📄) should be indented under their folders
   - Nested folders should show deeper indentation

**What This Tests**:
- `RecordFolder.Add()` method works
- `SingleRecord` creation works
- `GetHtmlSummary()` recursion works correctly
- Indentation levels work

---

### Test Scenario 2: Load from Empty Database
**Purpose**: Handle edge case of no records

**Steps**:
1. Make sure PatientRecord table is empty for patient ID 2:
   ```sql
   DELETE FROM PatientRecord WHERE patientID = 2;
   ```
2. Click "📂 Load Full History"
3. **Expected Result**: 
   - Message: "No records found for this patient."

**What This Tests**:
- Empty list handling
- No crashes with empty data

---

### Test Scenario 3: Load Real Database Records
**Purpose**: Test database integration and tree building

**Steps**:
1. Run the `SampleData.sql` script in your MySQL
2. Click "📂 Load Full History"
3. **Expected Result**:
   - See all records from database
   - Hierarchy matches the parent-child relationships
   - Status message shows count: "✅ Loaded X records from database"

**What This Tests**:
- Database connectivity
- `GetPatientRecords()` method
- `BuildRecordTree()` algorithm
- Correct parent-child linking

---

### Test Scenario 4: Add a Root-Level File
**Purpose**: Test CREATE operation (leaf node)

**Steps**:
1. Fill in the form:
   - Title: "Blood Pressure Check"
   - Details: "120/80 mmHg - Normal"
   - Type: "📄 File (Leaf)"
   - Parent Folder ID: (leave empty)
2. Click "➕ Add Record"
3. **Expected Result**:
   - Status message: "✅ Added new File: 'Blood Pressure Check'"
   - Page automatically reloads
   - New record appears at root level

**What This Tests**:
- Form validation
- `AddPatientRecord()` method
- NULL parent handling
- UI refresh after add

---

### Test Scenario 5: Add a Folder
**Purpose**: Test CREATE operation (composite node)

**Steps**:
1. Fill in the form:
   - Title: "Cardiology Records"
   - Details: "Heart-related tests and reports"
   - Type: "📂 Folder (Composite)"
   - Parent Folder ID: (leave empty)
2. Click "➕ Add Record"
3. Note the recordID from the database or UI
4. **Expected Result**:
   - New folder appears as bold text
   - Folder can be used as parent for other records

**What This Tests**:
- Folder creation
- `RecordFolder` instantiation
- Type distinction in database

---

### Test Scenario 6: Add a Nested File
**Purpose**: Test hierarchy creation

**Steps**:
1. First, ensure you have a folder (e.g., "Laboratory Results" with ID = 1)
2. Fill in the form:
   - Title: "Glucose Test"
   - Details: "Fasting glucose: 95 mg/dL - Normal"
   - Type: "📄 File (Leaf)"
   - Parent Folder ID: 1
3. Click "➕ Add Record"
4. **Expected Result**:
   - New record appears indented under "Laboratory Results" folder
   - Correct indentation level (20px more than parent)

**What This Tests**:
- Parent-child linking
- Foreign key relationship
- Tree hierarchy with correct indentation

---

### Test Scenario 7: Add Deeply Nested Structure
**Purpose**: Test multiple levels of nesting

**Steps**:
1. Create structure: Root → Folder A → Folder B → File
2. Use the form to create:
   - Folder A (parent: NULL)
   - Folder B (parent: Folder A's ID)
   - File (parent: Folder B's ID)
3. Load full history
4. **Expected Result**:
   - Three levels of indentation
   - Each level is 20px more indented
   - Correct hierarchy displayed

**What This Tests**:
- Deep recursion
- Multiple nesting levels
- `folderMap` lookup working correctly

---

### Test Scenario 8: Invalid Input Handling
**Purpose**: Test validation

**Steps**:
1. Leave Title field empty
2. Click "➕ Add Record"
3. **Expected Result**: 
   - Error message: "❌ Please enter a title."
   - No database insert

**What This Tests**:
- Form validation
- Error messaging
- Preventing empty records

---

### Test Scenario 9: Invalid Parent ID
**Purpose**: Test orphaned records

**Steps**:
1. Enter a non-existent Parent Folder ID (e.g., 9999)
2. Add a file
3. Load full history
4. **Expected Result**:
   - Record appears at root level (because parent lookup fails)

**What This Tests**:
- Graceful handling of missing parents
- `folderMap.ContainsKey()` check
- Fallback to root

---

### Test Scenario 10: Multiple Patients
**Purpose**: Ensure data isolation

**Steps**:
1. Add records for patient ID 2
2. Add records for patient ID 3 (if exists)
3. Load records for patient 2
4. **Expected Result**:
   - Only patient 2's records are shown
   - No mixing of patient data

**What This Tests**:
- Correct WHERE clause in SQL
- Patient ID filtering

---

## Common Issues and Solutions

### Issue 1: "No records found" even though data exists

**Possible Causes**:
- Wrong patient ID
- Database connection string incorrect
- Records exist but for different patient

**Solution**:
```sql
-- Check what's in the table
SELECT * FROM PatientRecord;

-- Check patient IDs
SELECT DISTINCT patientID FROM PatientRecord;

-- Verify patient exists
SELECT * FROM User WHERE userID = 2;
```

---

### Issue 2: Records appear flat (no hierarchy)

**Possible Causes**:
- `ParentFolderID` is NULL for all records
- `BuildRecordTree()` logic error
- Folders not created in first pass

**Solution**:
```sql
-- Check parent-child relationships
SELECT recordID, title, parentFolderID, recordType 
FROM PatientRecord 
WHERE patientID = 2;

-- Ensure some records have non-NULL parentFolderID
```

**Debug Code**:
```csharp
// Add logging in BuildRecordTree
Console.WriteLine($"Record {record.RecordID}: Type={record.RecordType}, Parent={record.ParentFolderID}");
```

---

### Issue 3: Folders appear but empty

**Possible Causes**:
- Children not being added to correct folder
- `folderMap` not finding parent ID
- Child records not matching parent ID

**Solution**:
Check the key-value pairs:
```csharp
// In BuildRecordTree, after first pass:
foreach (var kvp in folderMap)
{
    Console.WriteLine($"FolderMap: ID={kvp.Key}, Name={kvp.Value.name}");
}

// In second pass, check parent lookup:
if (record.ParentFolderID.HasValue)
{
    if (folderMap.ContainsKey(record.ParentFolderID.Value))
        Console.WriteLine($"✅ Found parent {record.ParentFolderID.Value}");
    else
        Console.WriteLine($"❌ Parent {record.ParentFolderID.Value} not found!");
}
```

---

### Issue 4: MySQL connection error

**Error**: `Unable to connect to any of the specified MySQL hosts`

**Solution**:
1. Check XAMPP MySQL is running
2. Verify connection string in `DatabaseManager.cs`:
   ```csharp
   private string connectionString = "Server=localhost;Database=MedEaseDB;Uid=root;Pwd=;";
   ```
3. Test connection:
   ```sql
   mysql -u root -p
   USE MedEaseDB;
   SHOW TABLES;
   ```

---

### Issue 5: Foreign key constraint error when adding record

**Error**: `Cannot add or update a child row: a foreign key constraint fails`

**Possible Causes**:
- Parent folder ID doesn't exist
- Patient ID doesn't exist

**Solution**:
```sql
-- Verify parent folder exists
SELECT * FROM PatientRecord WHERE recordID = <parentID>;

-- Verify patient exists
SELECT * FROM User WHERE userID = <patientID>;
```

---

### Issue 6: Records don't refresh after adding

**Possible Cause**:
- `LoadFullHistory()` not called after add
- Database insert failed silently

**Solution**:
Add error handling:
```csharp
try
{
    DatabaseManager.GetInstance().AddPatientRecord(newRecord);
    LoadFullHistory(); // Make sure this is called!
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    statusMessage = $"❌ Error: {ex.Message}";
}
```

---

### Issue 7: HTML rendering shows raw tags

**Symptom**: See literal `<div>` text instead of formatted HTML

**Cause**: Missing `@((MarkupString)recordsHtml)`

**Solution**:
Make sure you use `MarkupString`:
```razor
@((MarkupString)recordsHtml)
NOT
@recordsHtml  ❌
```

---

### Issue 8: Indentation not working

**Symptom**: All records at same level visually

**Cause**: CSS inline styles not applied

**Solution**:
Check `GetHtmlSummary()`:
```csharp
// Make sure style attribute is properly formatted
$"<div style='margin-left:{indentLevel * 20}px'>..."
```

---

## Performance Considerations

### For Large Patient Records (1000+ records)

**Issue**: Slow loading, high memory usage

**Optimization 1**: Lazy Loading
```csharp
// Don't load all records at once
// Load only root level first, then expand on click
```

**Optimization 2**: Caching
```csharp
// Cache the built tree
private Dictionary<int, RecordFolder> _treeCache = new();
```

**Optimization 3**: Pagination
```sql
SELECT * FROM PatientRecord 
WHERE patientID = @pid 
ORDER BY recordID 
LIMIT 100 OFFSET 0;
```

---

## Debugging Checklist

When something doesn't work:

- [ ] Database connection successful?
- [ ] PatientRecord table exists and has correct schema?
- [ ] Sample data inserted correctly?
- [ ] PatientID matches the ID used in queries?
- [ ] RecordType values are exactly "File" or "Folder"?
- [ ] ParentFolderID is NULL for root items?
- [ ] ParentFolderID refers to existing recordID for nested items?
- [ ] Page is using `@rendermode InteractiveServer`?
- [ ] Browser console shows no JavaScript errors?
- [ ] `dotnet watch` is running without build errors?

---

## SQL Queries for Verification

```sql
-- 1. Check total records for a patient
SELECT COUNT(*) FROM PatientRecord WHERE patientID = 2;

-- 2. See hierarchy structure
SELECT 
    recordID,
    CASE WHEN parentFolderID IS NULL THEN 'ROOT' 
         ELSE CONCAT('Child of ', parentFolderID) END as Level,
    recordType,
    title
FROM PatientRecord 
WHERE patientID = 2
ORDER BY parentFolderID, recordID;

-- 3. Find orphaned records (parent doesn't exist)
SELECT r1.*
FROM PatientRecord r1
LEFT JOIN PatientRecord r2 ON r1.parentFolderID = r2.recordID
WHERE r1.parentFolderID IS NOT NULL 
  AND r2.recordID IS NULL;

-- 4. Count by type
SELECT recordType, COUNT(*) as count
FROM PatientRecord
WHERE patientID = 2
GROUP BY recordType;

-- 5. Show tree depth
SELECT 
    title,
    (SELECT COUNT(*) 
     FROM PatientRecord r2 
     WHERE r2.recordID = r1.parentFolderID) as has_parent
FROM PatientRecord r1
WHERE patientID = 2;
```

---

## What to Show in Your Presentation/Demo

1. **Code Structure**: Show the three classes in `CompositePattern.cs`
2. **Database Schema**: Show the `PatientRecord` table with `parentFolderID`
3. **Tree Building Algorithm**: Explain the two-pass approach in `BuildRecordTree()`
4. **Live Demo**: 
   - Add a folder
   - Add files inside it
   - Show the hierarchy
5. **Benefits**: Explain how easy it is to add new operations or node types
6. **Real-World Application**: Medical records, file systems, organization charts

---

## Success Criteria

Your implementation is successful if:

✅ Can create folders and files
✅ Files can be nested inside folders
✅ Folders can contain other folders (multiple levels)
✅ Hierarchy displays with correct indentation
✅ Data persists in database
✅ Can reload and see the same structure
✅ Root-level items appear at top level
✅ No errors when adding records
✅ Pattern follows UML diagram correctly

Good luck with your project! 🎉
