using System;

namespace HealthTech.Services
{
    // Matches the 'User' table
    public class User
    {
        public int UserID { get; set; }
        public string Name { get; set; } = "";
<<<<<<< HEAD
        public string Role { get; set; } = ""; // 'Doctor', 'Patient', 'Receptionist'
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public int? Age { get; set; }
        public string Gender { get; set; } = "";
=======
        public string Role { get; set; } = ""; // 'Doctor', 'Patient'
        public string Email { get; set; } = ""; // For email notifications
        public string PhoneNumber { get; set; } = ""; // For WhatsApp/SMS (E.164 format: +60123456789)
>>>>>>> 712a9f04b6880593d519145d9a12e85d868d9f21
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
    
    // Matches the 'WorkingTime' table for doctor availability
    public class WorkingTime
    {
        public int WorkingTimeID { get; set; }
        public int DoctorID { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DateTime? Date { get; set; } // Specific date (for on-leave/off-day)
        public string Day { get; set; } = ""; // Monday-Sunday
        public string Type { get; set; } = "Working"; // 'Working', 'OnLeave', 'OffDay'
        
        // Helper properties
        public string DoctorName { get; set; } = "";
    }
    
    // Time slot for appointment booking
    public class TimeSlot
    {
        public DateTime DateTime { get; set; }
        public bool IsAvailable { get; set; }
        public string? Reason { get; set; } // Why not available
    }
}