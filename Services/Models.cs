using System;

namespace HealthTech.Services
{
    // Matches the 'User' table
    public class User
    {
        public int UserID { get; set; }
        public string Name { get; set; } = "";
        public string Role { get; set; } = ""; // 'Doctor', 'Patient'
    }

    // Matches the 'Room' table
    public class Room
    {
        public int RoomID { get; set; }
        public string RoomNumber { get; set; } = "";
        public string RoomType { get; set; } = "";
    }

    // Matches the 'Appointment' table (Updated with Foreign Keys)
    public class AppointmentModel
    {
        public int AppointmentID { get; set; }
        public int PatientID { get; set; }
        public int DoctorID { get; set; }
        public int? RoomID { get; set; } // Nullable (for Online)
        public string Type { get; set; } = "";
        public string Status { get; set; } = "Pending";
        public DateTime DateTime { get; set; }
        public string? Link { get; set; } // For Zoom links
        
        // Helper properties for display (we will JOIN these in SQL)
        public string PatientName { get; set; } = "";
        public string DoctorName { get; set; } = "";
    }
    
    // Matches the 'PatientRecord' table
    public class RecordModel
    {
        public int RecordID { get; set; }
        public int PatientID { get; set; }
        public int? ParentFolderID { get; set; } // The Composite Link
        public string RecordType { get; set; } = "File"; // 'Folder' or 'File'
        public string Title { get; set; } = "";
        public string Details { get; set; } = "";
    }
}