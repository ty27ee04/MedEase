using System;

namespace HealthTech.Services
{
    // Matches the 'User' table
    public class User
    {
        public int UserID { get; set; }
        public string Name { get; set; } = "";
        public string Role { get; set; } = ""; // 'Doctor', 'Patient', 'Receptionist'
        public string Email { get; set; } = "";
        public string Password { get; set; } = ""; // Note: In production, use hashed passwords
        public string Phone { get; set; } = "";
        public int? Age { get; set; }
        public string Gender { get; set; } = "";
        public string? Specialization { get; set; } // For Doctors only
        public string? LicenseNumber { get; set; } // For Doctors only
        public DateTime? CreatedAt { get; set; }
    }
    
    // Request model for login
    public class LoginRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
    
    // Response model for login
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public User? User { get; set; }
    }
    
    // Request model for registration
    public class RegisterRequest
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Role { get; set; } = ""; // 'Doctor' or 'Receptionist'
        public int? Age { get; set; }
        public string Gender { get; set; } = "";
        public string? Specialization { get; set; } // Required for Doctor
        public string? LicenseNumber { get; set; } // Required for Doctor
    }

    // Matches the 'Room' table
    public class Room
    {
        public int RoomID { get; set; }
        public string RoomNumber { get; set; } = "";
        public string RoomType { get; set; } = "";
        public string Status { get; set; } = "Available"; // Available, Unavailable, Maintenance
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
        public string? Remark { get; set; } // Reason for appointment
        
        // Helper properties for display (we will JOIN these in SQL)
        public string PatientName { get; set; } = "";
        public string DoctorName { get; set; } = "";
    }
    
    // Matches the 'PatientRecord' table
    public class RecordModel
    {
        public int RecordID { get; set; }
        public int PatientID { get; set; }
        public int? DoctorID { get; set; } // Assigned doctor for this record
        public int? InFolder { get; set; } // The Composite Link - which folder this record belongs to
        public string RecordType { get; set; } = "File"; // 'Folder' or 'File'
        public string Title { get; set; } = "";
        public string Details { get; set; } = "";
        public string FolderName { get; set; } = ""; // Name for folders
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Helper property for display (we will LEFT JOIN this in SQL)
        public string? DoctorName { get; set; }
    }
    
    // Matches the 'WorkingTime' table for doctor availability
    // Matches the 'workingtime' table for doctor availability
    public class WorkingTime
    {
        public int WorkingID { get; set; } // Database column: workingID
        public int DoctorID { get; set; } // Database column: doctorID
        public TimeSpan? StartTime { get; set; } // Database column: startTime
        public TimeSpan? EndTime { get; set; } // Database column: endTime
        public DateTime? Date { get; set; } // Database column: date - Specific date (for on-leave/off-day)
        public string Day { get; set; } = ""; // Database column: day - Monday-Sunday
        public string Type { get; set; } = "working"; // Database column: type - enum('working','on leave','off day')
        
        // Helper properties
        public string DoctorName { get; set; } = "";
        
        // Compatibility property for existing code
        public int WorkingTimeID 
        { 
            get => WorkingID; 
            set => WorkingID = value; 
        }
    }
    
    // Time slot for appointment booking
    public class TimeSlot
    {
        public DateTime DateTime { get; set; }
        public bool IsAvailable { get; set; }
        public string? Reason { get; set; } // Why not available
    }
}