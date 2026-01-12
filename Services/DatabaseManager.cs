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
                            Type = reader["type"].ToString(),
                            Status = reader["status"].ToString(),
                            Link = reader["link"].ToString(),
                            
                            // Populated from the JOIN aliases
                            PatientName = reader["PatientName"].ToString(),
                            DoctorName = reader["DoctorName"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // 3. UPDATE (Updated: Uses 'AppointmentID' and 'Status')
        public void UpdateAppointment(AppointmentModel appt)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE Appointment SET status=@status WHERE appointmentID=@id";
                
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@status", appt.Status);
                    cmd.Parameters.AddWithValue("@id", appt.AppointmentID); // Was 'Id'
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
                string sql = "SELECT UserID, Name, Role FROM User WHERE Role = @role";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@role", role);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User 
                            { 
                                UserID = Convert.ToInt32(reader["UserID"]), 
                                Name = reader["Name"].ToString() 
                            });
                        }
                    }
                }
            }
            return users;
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
                            RoomNumber = reader["roomNumber"].ToString(),
                            RoomType = reader["roomType"].ToString()
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
                string sql = "SELECT * FROM PatientRecord WHERE patientID = @pid ORDER BY recordID";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@pid", patientId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new RecordModel
                            {
                                RecordID = Convert.ToInt32(reader["recordID"]),
                                PatientID = Convert.ToInt32(reader["patientID"]),
                                ParentFolderID = reader["parentFolderID"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["parentFolderID"]),
                                RecordType = reader["recordType"].ToString(),
                                Title = reader["title"].ToString(),
                                Details = reader["details"].ToString()
                            });
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
                string sql = @"INSERT INTO PatientRecord 
                               (patientID, parentFolderID, recordType, title, details) 
                               VALUES (@pid, @parentId, @type, @title, @details)";
                
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@pid", record.PatientID);
                    cmd.Parameters.AddWithValue("@parentId", record.ParentFolderID.HasValue ? (object)record.ParentFolderID.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@type", record.RecordType);
                    cmd.Parameters.AddWithValue("@title", record.Title);
                    cmd.Parameters.AddWithValue("@details", record.Details ?? "");
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
                string deleteSql = "DELETE FROM PatientRecord WHERE recordID = @rid OR parentFolderID = @rid";
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
                                Name = reader["name"].ToString(),
                                Role = reader["role"].ToString()
                            };
                        }
                    }
                }
            }
            return null; // Login failed
        }
    }
}