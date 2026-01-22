using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace HealthTech.Services
{
    public sealed class DatabaseManager
    {
        private static DatabaseManager? _instance = null;
        private static readonly object padlock = new object();

        // CHANGE THIS to match your XAMPP/MySQL settings
        // Default XAMPP user is 'root' with no password
        private string connectionString = "Server=localhost;Database=MedEaseDB;Uid=root;Pwd=;";

        private DatabaseManager()
        {
            Console.WriteLine("[Singleton] Database Manager Initialized for MySQL.");
        }

        public static DatabaseManager GetInstance()
        {
            lock (padlock)
            {
                if (_instance == null)
                {
                    _instance = new DatabaseManager();
                }
                return _instance;
            }
        }

        // --- REAL SQL CRUD OPERATIONS ---

public void AddAppointment(AppointmentModel appt)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO Appointment 
                               (patientID, doctorID, roomID, dateTime, type, status, link) 
                               VALUES (@pid, @did, @rid, @date, @type, @status, @link)";
                
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@pid", appt.PatientID);
                    cmd.Parameters.AddWithValue("@did", appt.DoctorID);
                    cmd.Parameters.AddWithValue("@rid", appt.RoomID.HasValue ? appt.RoomID : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@date", appt.DateTime);
                    cmd.Parameters.AddWithValue("@type", appt.Type);
                    cmd.Parameters.AddWithValue("@status", appt.Status);
                    cmd.Parameters.AddWithValue("@link", appt.Link);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 2. READ (Updated: Uses JOIN to get Names, maps 'AppointmentID' correctly)
        public List<AppointmentModel> GetAllAppointments()
        {
            List<AppointmentModel> list = new List<AppointmentModel>();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                // We JOIN with the User table twice (once for Patient, once for Doctor) to get names
                string sql = @"
                    SELECT a.appointmentID, a.patientID, a.doctorID, a.roomID, a.dateTime, a.type, a.status, a.link,
                           p.Name AS PatientName, d.Name AS DoctorName
                    FROM Appointment a
                    JOIN User p ON a.patientID = p.userID
                    JOIN User d ON a.doctorID = d.userID
                    ORDER BY a.appointmentID DESC";
                
                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new AppointmentModel
                        {
                            AppointmentID = Convert.ToInt32(reader["appointmentID"]),
                            PatientID = Convert.ToInt32(reader["patientID"]),
                            DoctorID = Convert.ToInt32(reader["doctorID"]),
                            RoomID = reader["roomID"] == DBNull.Value ? null : Convert.ToInt32(reader["roomID"]),
                            DateTime = Convert.ToDateTime(reader["dateTime"]),
                            Type = reader["type"].ToString() ?? "",
                            Status = reader["status"].ToString() ?? "",
                            Link = reader["link"].ToString() ?? "",
                            
                            // Populated from the JOIN aliases
                            PatientName = reader["PatientName"].ToString() ?? "",
                            DoctorName = reader["DoctorName"].ToString() ?? ""
                        });
                    }
                }
            }
            return list;
        }

        // 3. UPDATE (Updated: Uses 'AppointmentID' and supports datetime changes)
        public void UpdateAppointment(AppointmentModel appt)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE Appointment SET status=@status, dateTime=@datetime WHERE appointmentID=@id";
                
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@status", appt.Status);
                    cmd.Parameters.AddWithValue("@datetime", appt.DateTime);
                    cmd.Parameters.AddWithValue("@id", appt.AppointmentID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 3b. UPDATE APPOINTMENT DETAILS (For Receptionist - Change Doctor, Status, DateTime, Room)
        public void UpdateAppointmentDetails(int appointmentId, int? newDoctorId, string? newStatus, DateTime? newDateTime, int? newRoomId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                
                // Build dynamic SQL based on what needs to be updated
                List<string> updates = new List<string>();
                if (newDoctorId.HasValue) updates.Add("doctorID=@doctorId");
                if (!string.IsNullOrEmpty(newStatus)) updates.Add("status=@status");
                if (newDateTime.HasValue) updates.Add("dateTime=@dateTime");
                if (newRoomId.HasValue) updates.Add("roomID=@roomId");
                
                if (updates.Count == 0) return; // Nothing to update
                
                string sql = $"UPDATE Appointment SET {string.Join(", ", updates)} WHERE appointmentID=@id";
                
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", appointmentId);
                    if (newDoctorId.HasValue) cmd.Parameters.AddWithValue("@doctorId", newDoctorId.Value);
                    if (!string.IsNullOrEmpty(newStatus)) cmd.Parameters.AddWithValue("@status", newStatus);
                    if (newDateTime.HasValue) cmd.Parameters.AddWithValue("@dateTime", newDateTime.Value);
                    if (newRoomId.HasValue) cmd.Parameters.AddWithValue("@roomId", newRoomId.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 4. DELETE (Updated: Uses 'AppointmentID')
        public void DeleteAppointment(int id)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = "DELETE FROM Appointment WHERE appointmentID=@id";
                
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Helper for your existing code
        public void ExecuteQuery(string query)
        {
            // Warning: This method is unsafe for raw input but keeps your old code working
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 1. HELPER: Get Users for Dropdowns (Create Appointment needs this!)
        public List<User> GetUsersByRole(string role)
        {
            List<User> users = new List<User>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                
                // Try to get age and gender if columns exist
                string sql = "SELECT userID, name, role, email, phone FROM User WHERE role = @role";
                bool hasAgeGender = false;
                
                try
                {
                    string checkSql = "SELECT age, gender FROM User LIMIT 1";
                    using (var checkCmd = new MySqlCommand(checkSql, conn))
                    {
                        checkCmd.ExecuteScalar();
                        hasAgeGender = true;
                    }
                }
                catch (MySqlException) { }
                
                if (hasAgeGender)
                {
                    sql = "SELECT userID, name, role, email, phone, age, gender FROM User WHERE role = @role";
                }
                
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@role", role);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new User 
                            { 
                                UserID = Convert.ToInt32(reader["userID"]), 
                                Name = reader["name"].ToString() ?? "",
                                Role = reader["role"].ToString() ?? "",
                                Email = reader["email"].ToString() ?? "",
                                Phone = reader["phone"].ToString() ?? ""
                            };
                            
                            if (hasAgeGender)
                            {
                                user.Age = reader["age"] == DBNull.Value ? null : Convert.ToInt32(reader["age"]);
                                user.Gender = reader["gender"].ToString() ?? "";
                            }
                            
                            users.Add(user);
                        }
                    }
                }
            }
            return users;
        }

        // 1b. HELPER: Get All Patients (For Receptionist)
        public List<User> GetAllPatients()
        {
            return GetUsersByRole("Patient");
        }

        // 1c. HELPER: Get All Doctors (For Receptionist)
        public List<User> GetAllDoctors()
        {
            return GetUsersByRole("Doctor");
        }

        // HELPER: Get User by ID with contact info (for notifications)
        public User? GetUserById(int userId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT UserID, Name, Role, Email, Phone FROM User WHERE UserID = @id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                Name = reader["Name"].ToString() ?? "",
                                Role = reader["Role"].ToString() ?? "",
                                Email = reader["Email"].ToString() ?? "",
                                Phone = reader["Phone"].ToString() ?? ""
                            };
                        }
                    }
                }
            }
            return null;
        }

        // 2. HELPER: Get Available Rooms
        public List<Room> GetRooms()
        {
            List<Room> rooms = new List<Room>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Room";
                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rooms.Add(new Room 
                        { 
                            RoomID = Convert.ToInt32(reader["roomID"]), 
                            RoomNumber = reader["roomNumber"].ToString() ?? "",
                            RoomType = reader["roomType"].ToString() ?? ""
                        });
                    }
                }
            }
            return rooms;
        }

        // 4. COMPOSITE PATTERN: Fetch Recursive Records
        public List<RecordModel> GetPatientRecords(int patientId)
        {
            var list = new List<RecordModel>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                
                // Check if timestamp columns exist
                bool hasTimestamps = false;
                try
                {
                    string checkSql = "SELECT createdDate, lastUpdated FROM PatientRecord LIMIT 1";
                    using (var checkCmd = new MySqlCommand(checkSql, conn))
                    {
                        using (var checkReader = checkCmd.ExecuteReader())
                        {
                            hasTimestamps = true;
                        }
                    }
                }
                catch (MySqlException)
                {
                    hasTimestamps = false;
                }
                
                string sql = hasTimestamps 
                    ? "SELECT * FROM PatientRecord WHERE patientID = @pid ORDER BY createdDate DESC"
                    : "SELECT * FROM PatientRecord WHERE patientID = @pid ORDER BY recordID DESC";
                    
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@pid", patientId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var record = new RecordModel
                            {
                                RecordID = Convert.ToInt32(reader["recordID"]),
                                PatientID = Convert.ToInt32(reader["patientID"]),
                                RecordType = reader["recordType"].ToString() ?? "",
                                Title = reader["title"].ToString() ?? "",
                                Details = reader["details"].ToString() ?? ""
                            };
                            
                            // Try to read InFolder (new name) or parentFolderID (old name)
                            try
                            {
                                int inFolderIndex = reader.GetOrdinal("InFolder");
                                record.InFolder = reader[inFolderIndex] != DBNull.Value ? (int?)Convert.ToInt32(reader[inFolderIndex]) : null;
                            }
                            catch
                            {
                                try
                                {
                                    int parentIndex = reader.GetOrdinal("parentFolderID");
                                    record.InFolder = reader[parentIndex] != DBNull.Value ? (int?)Convert.ToInt32(reader[parentIndex]) : null;
                                }
                                catch
                                {
                                    record.InFolder = null;
                                }
                            }
                            
                            // Try to read FolderName if column exists
                            try
                            {
                                int folderNameIndex = reader.GetOrdinal("FolderName");
                                record.FolderName = reader[folderNameIndex] != DBNull.Value ? reader[folderNameIndex].ToString() ?? "" : "";
                            }
                            catch
                            {
                                record.FolderName = "";
                            }
                            
                            // Set timestamps if columns exist
                            if (hasTimestamps)
                            {
                                record.CreatedAt = reader["createdDate"] != DBNull.Value ? Convert.ToDateTime(reader["createdDate"]) : DateTime.Now;
                                record.UpdatedAt = reader["lastUpdated"] != DBNull.Value ? Convert.ToDateTime(reader["lastUpdated"]) : DateTime.Now;
                            }
                            else
                            {
                                record.CreatedAt = DateTime.Now;
                                record.UpdatedAt = DateTime.Now;
                            }
                            
                            list.Add(record);
                        }
                    }
                }
            }
            return list;
        }

        // 5. COMPOSITE PATTERN: Add New Patient Record
        public void AddPatientRecord(RecordModel record)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                
                // Check if timestamp columns exist
                bool hasTimestamps = false;
                try
                {
                    string checkSql = "SELECT createdDate FROM PatientRecord LIMIT 1";
                    using (var checkCmd = new MySqlCommand(checkSql, conn))
                    {
                        checkCmd.ExecuteScalar();
                        hasTimestamps = true;
                    }
                }
                catch (MySqlException)
                {
                    hasTimestamps = false;
                }
                
                // Check if FolderName column exists
                bool hasFolderName = false;
                try
                {
                    string checkSql = "SELECT FolderName FROM PatientRecord LIMIT 1";
                    using (var checkCmd = new MySqlCommand(checkSql, conn))
                    {
                        checkCmd.ExecuteScalar();
                        hasFolderName = true;
                    }
                }
                catch (MySqlException)
                {
                    hasFolderName = false;
                }
                
                // Check if InFolder column exists (new name)
                bool hasInFolder = false;
                try
                {
                    string checkSql = "SELECT InFolder FROM PatientRecord LIMIT 1";
                    using (var checkCmd = new MySqlCommand(checkSql, conn))
                    {
                        checkCmd.ExecuteScalar();
                        hasInFolder = true;
                    }
                }
                catch (MySqlException)
                {
                    hasInFolder = false;
                }
                
                string columnName = hasInFolder ? "InFolder" : "parentFolderID";
                
                string sql;
                if (hasTimestamps && hasFolderName)
                {
                    sql = $@"INSERT INTO PatientRecord 
                           (patientID, {columnName}, recordType, title, details, FolderName, createdDate, lastUpdated) 
                           VALUES (@pid, @parentId, @type, @title, @details, @folderName, NOW(), NOW())";
                }
                else if (hasTimestamps)
                {
                    sql = $@"INSERT INTO PatientRecord 
                           (patientID, {columnName}, recordType, title, details, createdDate, lastUpdated) 
                           VALUES (@pid, @parentId, @type, @title, @details, NOW(), NOW())";
                }
                else if (hasFolderName)
                {
                    sql = $@"INSERT INTO PatientRecord 
                           (patientID, {columnName}, recordType, title, details, FolderName) 
                           VALUES (@pid, @parentId, @type, @title, @details, @folderName)";
                }
                else
                {
                    sql = $@"INSERT INTO PatientRecord 
                           (patientID, {columnName}, recordType, title, details) 
                           VALUES (@pid, @parentId, @type, @title, @details)";
                }
                
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@pid", record.PatientID);
                    cmd.Parameters.AddWithValue("@parentId", record.InFolder.HasValue ? (object)record.InFolder.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@type", record.RecordType);
                    cmd.Parameters.AddWithValue("@title", record.Title);
                    cmd.Parameters.AddWithValue("@details", record.Details ?? "");
                    if (hasFolderName)
                    {
                        cmd.Parameters.AddWithValue("@folderName", record.FolderName ?? "");
                    }
                    cmd.ExecuteNonQuery();
                    
                    // Get the auto-generated RecordID
                    record.RecordID = (int)cmd.LastInsertedId;
                }
            }
        }

        public void UpdatePatientRecord(RecordModel record)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                
                // Check if timestamp columns exist
                bool hasTimestamps = false;
                try
                {
                    string checkSql = "SELECT lastUpdated FROM PatientRecord LIMIT 1";
                    using (var checkCmd = new MySqlCommand(checkSql, conn))
                    {
                        checkCmd.ExecuteScalar();
                        hasTimestamps = true;
                    }
                }
                catch (MySqlException)
                {
                    hasTimestamps = false;
                }
                
                // Check if FolderName column exists
                bool hasFolderName = false;
                try
                {
                    string checkSql = "SELECT FolderName FROM PatientRecord LIMIT 1";
                    using (var checkCmd = new MySqlCommand(checkSql, conn))
                    {
                        checkCmd.ExecuteScalar();
                        hasFolderName = true;
                    }
                }
                catch (MySqlException)
                {
                    hasFolderName = false;
                }
                
                // Check if InFolder column exists (new name)
                bool hasInFolder = false;
                try
                {
                    string checkSql = "SELECT InFolder FROM PatientRecord LIMIT 1";
                    using (var checkCmd = new MySqlCommand(checkSql, conn))
                    {
                        checkCmd.ExecuteScalar();
                        hasInFolder = true;
                    }
                }
                catch (MySqlException)
                {
                    hasInFolder = false;
                }
                
                string columnName = hasInFolder ? "InFolder" : "parentFolderID";
                
                string sql;
                if (hasTimestamps && hasFolderName)
                {
                    sql = $@"UPDATE PatientRecord 
                           SET title = @title, details = @details, FolderName = @folderName, {columnName} = @inFolder, lastUpdated = NOW() 
                           WHERE recordID = @rid";
                }
                else if (hasTimestamps)
                {
                    sql = $@"UPDATE PatientRecord 
                           SET title = @title, details = @details, {columnName} = @inFolder, lastUpdated = NOW() 
                           WHERE recordID = @rid";
                }
                else if (hasFolderName)
                {
                    sql = $@"UPDATE PatientRecord 
                           SET title = @title, details = @details, FolderName = @folderName, {columnName} = @inFolder 
                           WHERE recordID = @rid";
                }
                else
                {
                    sql = $@"UPDATE PatientRecord 
                           SET title = @title, details = @details, {columnName} = @inFolder 
                           WHERE recordID = @rid";
                }
                
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@rid", record.RecordID);
                    cmd.Parameters.AddWithValue("@title", record.Title);
                    cmd.Parameters.AddWithValue("@details", record.Details ?? "");
                    cmd.Parameters.AddWithValue("@inFolder", record.InFolder.HasValue ? (object)record.InFolder.Value : DBNull.Value);
                    if (hasFolderName)
                    {
                        cmd.Parameters.AddWithValue("@folderName", record.FolderName ?? "");
                    }
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 6. COMPOSITE PATTERN: Delete Patient Record (and its children if it's a folder)
        public void DeletePatientRecord(int recordId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                // First, recursively delete all children
                // Check which column name exists
                bool hasInFolder = false;
                try
                {
                    string checkSql = "SELECT InFolder FROM PatientRecord LIMIT 1";
                    using (var checkCmd = new MySqlCommand(checkSql, conn))
                    {
                        checkCmd.ExecuteScalar();
                        hasInFolder = true;
                    }
                }
                catch (MySqlException)
                {
                    hasInFolder = false;
                }
                
                string columnName = hasInFolder ? "InFolder" : "parentFolderID";
                string deleteSql = $"DELETE FROM PatientRecord WHERE recordID = @rid OR {columnName} = @rid";
                using (var cmd = new MySqlCommand(deleteSql, conn))
                {
                    cmd.Parameters.AddWithValue("@rid", recordId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 1. REGISTER: Add a new user to the database
        // 1. REGISTER (Add User)
        public void RegisterUser(string name, string password, string role)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                // We generate a fake email/phone for now to satisfy the DB constraints
                string sql = @"INSERT INTO User (name, email, password, role, phone) 
                               VALUES (@name, @email, @pass, @role, '0123456789')";
                
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@email", name.Replace(" ", "") + "@test.com"); 
                    cmd.Parameters.AddWithValue("@pass", password);
                    cmd.Parameters.AddWithValue("@role", role);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 1b. REGISTER PATIENT (Enhanced - For Receptionist with Full Details)
        public int RegisterPatient(string name, string email, string password, string phone, int? age, string gender)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                
                // Check if age and gender columns exist, if not add them
                try
                {
                    string checkSql = "SELECT age, gender FROM User LIMIT 1";
                    using (var checkCmd = new MySqlCommand(checkSql, conn))
                    {
                        checkCmd.ExecuteScalar();
                    }
                    
                    // Columns exist, use them
                    string sql = @"INSERT INTO User (name, email, password, role, phone, age, gender) 
                                   VALUES (@name, @email, @pass, 'Patient', @phone, @age, @gender)";
                    
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@pass", password);
                        cmd.Parameters.AddWithValue("@phone", phone);
                        cmd.Parameters.AddWithValue("@age", age.HasValue ? age.Value : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@gender", string.IsNullOrEmpty(gender) ? (object)DBNull.Value : gender);
                        cmd.ExecuteNonQuery();
                        
                        return (int)cmd.LastInsertedId;
                    }
                }
                catch (MySqlException)
                {
                    // Columns don't exist, add them first
                    string alterSql = @"ALTER TABLE User 
                                       ADD COLUMN age INT NULL,
                                       ADD COLUMN gender VARCHAR(10) NULL";
                    using (var alterCmd = new MySqlCommand(alterSql, conn))
                    {
                        alterCmd.ExecuteNonQuery();
                    }
                    
                    // Now insert with age and gender
                    string sql = @"INSERT INTO User (name, email, password, role, phone, age, gender) 
                                   VALUES (@name, @email, @pass, 'Patient', @phone, @age, @gender)";
                    
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@pass", password);
                        cmd.Parameters.AddWithValue("@phone", phone);
                        cmd.Parameters.AddWithValue("@age", age.HasValue ? age.Value : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@gender", string.IsNullOrEmpty(gender) ? (object)DBNull.Value : gender);
                        cmd.ExecuteNonQuery();
                        
                        return (int)cmd.LastInsertedId;
                    }
                }
            }
        }

        // 2. LOGIN (Check User)
        public User? LoginUser(string name, string password)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM User WHERE name=@name AND password=@pass";
                
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@pass", password);
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserID = Convert.ToInt32(reader["userID"]),
                                Name = reader["name"].ToString() ?? "",
                                Role = reader["role"].ToString() ?? ""
                            };
                        }
                    }
                }
            }
            return null; // Login failed
        }

        // --- WORKING TIME & AVAILABILITY METHODS ---

        // Get doctor's working times
        public List<WorkingTime> GetDoctorWorkingTimes(int doctorId)
        {
            var list = new List<WorkingTime>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"SELECT wt.*, u.Name as DoctorName 
                             FROM WorkingTime wt
                             JOIN User u ON wt.doctorID = u.userID
                             WHERE wt.doctorID = @doctorId
                             ORDER BY wt.date, wt.startTime";
                
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@doctorId", doctorId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new WorkingTime
                            {
                                WorkingTimeID = Convert.ToInt32(reader["workingTimeID"]),
                                DoctorID = Convert.ToInt32(reader["doctorID"]),
                                StartTime = (TimeSpan)reader["startTime"],
                                EndTime = (TimeSpan)reader["endTime"],
                                Date = reader["date"] == DBNull.Value ? null : Convert.ToDateTime(reader["date"]),
                                Day = reader["day"].ToString() ?? "",
                                Type = reader["type"].ToString() ?? "",
                                DoctorName = reader["DoctorName"].ToString() ?? ""
                            });
                        }
                    }
                }
            }
            return list;
        }

        // Get available time slots for a doctor on a specific date
        public List<TimeSlot> GetAvailableTimeSlots(int doctorId, DateTime date, int slotDurationMinutes = 30)
        {
            var slots = new List<TimeSlot>();
            var dayName = date.DayOfWeek.ToString();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // 1. Check if doctor has specific date override (on-leave or off-day)
                // This checks for exact date match regardless of startTime/endTime values
                string checkSpecificDate = @"SELECT type FROM WorkingTime 
                                           WHERE doctorID = @doctorId 
                                           AND date = @date";
                
                using (var cmd = new MySqlCommand(checkSpecificDate, conn))
                {
                    cmd.Parameters.AddWithValue("@doctorId", doctorId);
                    cmd.Parameters.AddWithValue("@date", date.Date);
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string? type = reader["type"].ToString();
                            if (type == "on leave" || type == "off day")
                            {
                                // Doctor is not available on this date
                                return slots; // Empty list
                            }
                        }
                    }
                }

                // 2. Get doctor's regular working hours for this day
                string getWorkingHours = @"SELECT * FROM WorkingTime 
                                         WHERE doctorID = @doctorId 
                                         AND day = @day 
                                         AND type = 'Working'
                                         AND (date IS NULL OR date = @date)
                                         ORDER BY date DESC LIMIT 1";
                
                TimeSpan? startTime = null;
                TimeSpan? endTime = null;

                using (var cmd = new MySqlCommand(getWorkingHours, conn))
                {
                    cmd.Parameters.AddWithValue("@doctorId", doctorId);
                    cmd.Parameters.AddWithValue("@day", dayName);
                    cmd.Parameters.AddWithValue("@date", date.Date);
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            startTime = (TimeSpan)reader["startTime"];
                            endTime = (TimeSpan)reader["endTime"];
                        }
                    }
                }

                if (!startTime.HasValue || !endTime.HasValue)
                {
                    // No working hours defined for this day
                    return slots;
                }

                // 3. Get existing appointments for this doctor on this date (including next day for overnight shifts)
                var bookedTimes = new List<DateTime>();
                string getAppointments = @"SELECT dateTime FROM Appointment 
                                         WHERE doctorID = @doctorId 
                                         AND (DATE(dateTime) = @date OR DATE(dateTime) = DATE_ADD(@date, INTERVAL 1 DAY))
                                         AND status NOT IN ('Cancelled', 'Completed')";
                
                using (var cmd = new MySqlCommand(getAppointments, conn))
                {
                    cmd.Parameters.AddWithValue("@doctorId", doctorId);
                    cmd.Parameters.AddWithValue("@date", date.Date);
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            bookedTimes.Add(Convert.ToDateTime(reader["dateTime"]));
                        }
                    }
                }

                // 4. Generate time slots (handles overnight shifts)
                var currentTime = date.Date + startTime.Value;
                var endDateTime = date.Date + endTime.Value;
                
                // If endTime is less than or equal to startTime, it means the shift crosses midnight
                bool isOvernightShift = endTime.Value <= startTime.Value;
                
                if (isOvernightShift)
                {
                    // For overnight shifts, we need to check if we're showing:
                    // - The first part (e.g., Sunday 17:00 to 23:59) OR
                    // - The second part (e.g., Monday 00:00 to 04:00)
                    
                    // Show only the portion on the selected date (e.g., 17:00 to 23:30)
                    endDateTime = date.Date.AddDays(1); // End at midnight for the start day
                }
                else
                {
                    // Normal shift (not overnight), check if we should show overnight portion from previous day
                    var previousDay = date.AddDays(-1);
                    var previousDayName = previousDay.DayOfWeek.ToString();
                    
                    string checkPreviousDayShift = @"SELECT startTime, endTime FROM WorkingTime 
                                                    WHERE doctorID = @doctorId 
                                                    AND day = @previousDay 
                                                    AND type = 'Working'
                                                    AND (date IS NULL OR date = @previousDate)
                                                    ORDER BY date DESC LIMIT 1";
                    
                    TimeSpan? prevStartTime = null;
                    TimeSpan? prevEndTime = null;
                    
                    using (var cmd = new MySqlCommand(checkPreviousDayShift, conn))
                    {
                        cmd.Parameters.AddWithValue("@doctorId", doctorId);
                        cmd.Parameters.AddWithValue("@previousDay", previousDayName);
                        cmd.Parameters.AddWithValue("@previousDate", previousDay.Date);
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                prevStartTime = (TimeSpan)reader["startTime"];
                                prevEndTime = (TimeSpan)reader["endTime"];
                            }
                        }
                    }
                    
                    if (prevStartTime.HasValue && prevEndTime.HasValue && prevEndTime.Value <= prevStartTime.Value)
                    {
                        // Previous day has overnight shift, add the second part (e.g., 00:00 to 04:00)
                        // First add the overnight portion from previous day
                        var overnightCurrentTime = date.Date; // Start at midnight
                        var overnightEndTime = date.Date + prevEndTime.Value;
                        
                        while (overnightCurrentTime < overnightEndTime)
                        {
                            var slot = new TimeSlot
                            {
                                DateTime = overnightCurrentTime,
                                IsAvailable = true
                            };

                            // Check if this slot is already booked
                            if (bookedTimes.Any(bt => bt == overnightCurrentTime))
                            {
                                slot.IsAvailable = false;
                                slot.Reason = "Already booked";
                            }

                            // Don't show past time slots
                            if (overnightCurrentTime <= DateTime.Now)
                            {
                                slot.IsAvailable = false;
                                slot.Reason = "Past time";
                            }

                            slots.Add(slot);
                            overnightCurrentTime = overnightCurrentTime.AddMinutes(slotDurationMinutes);
                        }
                    }
                }

                while (currentTime < endDateTime)
                {
                    var slot = new TimeSlot
                    {
                        DateTime = currentTime,
                        IsAvailable = true
                    };

                    // Check if this slot is already booked
                    if (bookedTimes.Any(bt => bt == currentTime))
                    {
                        slot.IsAvailable = false;
                        slot.Reason = "Already booked";
                    }

                    // Don't show past time slots
                    if (currentTime <= DateTime.Now)
                    {
                        slot.IsAvailable = false;
                        slot.Reason = "Past time";
                    }

                    slots.Add(slot);
                    currentTime = currentTime.AddMinutes(slotDurationMinutes);
                }
            }

            return slots;
        }

        // Get upcoming appointments (future appointments)
        public List<AppointmentModel> GetUpcomingAppointments()
        {
            var list = new List<AppointmentModel>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"
                    SELECT a.appointmentID, a.patientID, a.doctorID, a.roomID, a.dateTime, a.type, a.status, a.link,
                           p.Name AS PatientName, p.Email AS PatientEmail, p.Phone AS PatientPhone,
                           d.Name AS DoctorName
                    FROM Appointment a
                    JOIN User p ON a.patientID = p.userID
                    JOIN User d ON a.doctorID = d.userID
                    WHERE a.dateTime >= @now
                    AND a.status NOT IN ('Cancelled', 'Completed')
                    ORDER BY a.dateTime ASC";
                
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@now", DateTime.Now);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var appt = new AppointmentModel
                            {
                                AppointmentID = Convert.ToInt32(reader["appointmentID"]),
                                PatientID = Convert.ToInt32(reader["patientID"]),
                                DoctorID = Convert.ToInt32(reader["doctorID"]),
                                RoomID = reader["roomID"] == DBNull.Value ? null : Convert.ToInt32(reader["roomID"]),
                                DateTime = Convert.ToDateTime(reader["dateTime"]),
                                Type = reader["type"].ToString() ?? "",
                                Status = reader["status"].ToString() ?? "",
                                Link = reader["link"]?.ToString(),
                                PatientName = reader["PatientName"].ToString() ?? "",
                                DoctorName = reader["DoctorName"].ToString() ?? ""
                            };
                            list.Add(appt);
                        }
                    }
                }
            }
            return list;
        }

        // Add working time for a doctor
        public void AddWorkingTime(WorkingTime workingTime)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO WorkingTime 
                             (doctorID, startTime, endTime, date, day, type) 
                             VALUES (@doctorId, @start, @end, @date, @day, @type)";
                
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@doctorId", workingTime.DoctorID);
                    cmd.Parameters.AddWithValue("@start", workingTime.StartTime);
                    cmd.Parameters.AddWithValue("@end", workingTime.EndTime);
                    cmd.Parameters.AddWithValue("@date", workingTime.Date.HasValue ? workingTime.Date : DBNull.Value);
                    cmd.Parameters.AddWithValue("@day", workingTime.Day);
                    cmd.Parameters.AddWithValue("@type", workingTime.Type);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}