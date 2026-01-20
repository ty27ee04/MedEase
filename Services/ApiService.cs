using System.Net.Http.Json;
using HealthTech.Services;

namespace HealthTech.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        public string? LastError { get; private set; }

        public ApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7220/api";
            
            // Set timeout to 5 seconds to prevent long waits
            _httpClient.Timeout = TimeSpan.FromSeconds(5);
        }

        // Health Check - Test if API is running
        public async Task<bool> IsApiAvailableAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/health");
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                LastError = "❌ Cannot connect to API. Please make sure HealthTech.API is running!";
                return false;
            }
            catch (TaskCanceledException)
            {
                LastError = "❌ API connection timeout. Please check if HealthTech.API is running on the correct port.";
                return false;
            }
            catch (Exception ex)
            {
                LastError = $"❌ API Error: {ex.Message}";
                return false;
            }
        }

        // ==================== APPOINTMENTS ====================
        
        public async Task<List<AppointmentModel>> GetAllAppointmentsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<AppointmentModel>>($"{_baseUrl}/appointments") 
                       ?? new List<AppointmentModel>();
            }
            catch (HttpRequestException ex)
            {
                LastError = "Cannot connect to API. Is HealthTech.API running?";
                throw new Exception("API Connection Error: Please start HealthTech.API first.", ex);
            }
            catch (TaskCanceledException)
            {
                LastError = "API request timeout. Please check if HealthTech.API is running.";
                throw new Exception("API Timeout: Cannot reach HealthTech.API.");
            }
            catch (Exception ex)
            {
                LastError = $"API Error: {ex.Message}";
                throw;
            }
        }

        public async Task<List<AppointmentModel>> GetUpcomingAppointmentsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<AppointmentModel>>($"{_baseUrl}/appointments/upcoming") 
                       ?? new List<AppointmentModel>();
            }
            catch
            {
                return new List<AppointmentModel>();
            }
        }

        public async Task<AppointmentModel?> GetAppointmentByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<AppointmentModel>($"{_baseUrl}/appointments/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<int> SaveAppointmentAsync(AppointmentModel appointment)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/appointments", appointment);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<CreateResponse>();
                return result?.Id ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<bool> UpdateAppointmentAsync(AppointmentModel appointment)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/appointments/{appointment.AppointmentID}", appointment);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/appointments/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<TimeSlot>> GetAvailableTimeSlotsAsync(int doctorId, DateTime date, int slotDuration = 30)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<TimeSlot>>(
                    $"{_baseUrl}/appointments/doctor/{doctorId}/available-slots?date={date:yyyy-MM-dd}&slotDuration={slotDuration}") 
                    ?? new List<TimeSlot>();
            }
            catch
            {
                return new List<TimeSlot>();
            }
        }

        public async Task<List<AppointmentModel>> GetAppointmentsByDoctorAsync(int doctorId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<AppointmentModel>>(
                    $"{_baseUrl}/appointments/doctor/{doctorId}") ?? new List<AppointmentModel>();
            }
            catch
            {
                return new List<AppointmentModel>();
            }
        }

        public async Task<List<AppointmentModel>> GetAppointmentsByPatientAsync(int patientId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<AppointmentModel>>(
                    $"{_baseUrl}/appointments/patient/{patientId}") ?? new List<AppointmentModel>();
            }
            catch
            {
                return new List<AppointmentModel>();
            }
        }

        // ==================== USERS ====================
        
        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<User>>($"{_baseUrl}/users") 
                       ?? new List<User>();
            }
            catch
            {
                return new List<User>();
            }
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<User>($"{_baseUrl}/users/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<User>> GetUsersByRoleAsync(string role)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<User>>($"{_baseUrl}/users/role/{role}") 
                       ?? new List<User>();
            }
            catch
            {
                return new List<User>();
            }
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/users/login", 
                    new { username, password });
                
                if (!response.IsSuccessStatusCode)
                    return null;
                    
                return await response.Content.ReadFromJsonAsync<User>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<User?> LoginByNameAsync(string username, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/users/login-by-name", 
                    new { username, password });
                
                if (!response.IsSuccessStatusCode)
                    return null;
                    
                return await response.Content.ReadFromJsonAsync<User>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<int> RegisterUserAsync(string name, string password, string role)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/users/register", 
                    new { name, password, role });
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CreateResponse>();
                    return result?.Id ?? 0;
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<int> SaveUserAsync(string name, string username, string password, string role)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/users", 
                    new { name, username, password, role });
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<CreateResponse>();
                return result?.Id ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<int> RegisterPatientAsync(string name, string email, string password, 
            string phone, int? age, string gender)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/users/register-patient", 
                    new { name, email, password, phone, age, gender });
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CreateResponse>();
                    return result?.Id ?? 0;
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        // ==================== ROOMS ====================
        
        public async Task<List<Room>> GetAllRoomsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Room>>($"{_baseUrl}/rooms") 
                       ?? new List<Room>();
            }
            catch
            {
                return new List<Room>();
            }
        }

        public async Task<Room?> GetRoomByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Room>($"{_baseUrl}/rooms/{id}");
            }
            catch
            {
                return null;
            }
        }

        // ==================== WORKING TIME ====================
        
        public async Task<List<WorkingTime>> GetDoctorWorkingTimesAsync(int doctorId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<WorkingTime>>(
                    $"{_baseUrl}/workingtime/doctor/{doctorId}") ?? new List<WorkingTime>();
            }
            catch
            {
                return new List<WorkingTime>();
            }
        }

        public async Task<bool> AddWorkingTimeAsync(WorkingTime workingTime)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/workingtime", workingTime);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // ==================== PATIENT RECORDS ====================
        
        public async Task<List<RecordModel>> GetRecordsByPatientAsync(int patientId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<RecordModel>>(
                    $"{_baseUrl}/patientrecords/patient/{patientId}") ?? new List<RecordModel>();
            }
            catch
            {
                return new List<RecordModel>();
            }
        }

        public async Task<List<RecordModel>> GetRootRecordsAsync(int patientId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<RecordModel>>(
                    $"{_baseUrl}/patientrecords/patient/{patientId}/root") ?? new List<RecordModel>();
            }
            catch
            {
                return new List<RecordModel>();
            }
        }

        public async Task<List<RecordModel>> GetChildRecordsAsync(int folderId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<RecordModel>>(
                    $"{_baseUrl}/patientrecords/folder/{folderId}/children") ?? new List<RecordModel>();
            }
            catch
            {
                return new List<RecordModel>();
            }
        }

        public async Task<RecordModel?> GetRecordByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<RecordModel>(
                    $"{_baseUrl}/patientrecords/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<int> SaveRecordAsync(RecordModel record)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/patientrecords", record);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<CreateResponse>();
                return result?.Id ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<bool> UpdateRecordAsync(RecordModel record)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(
                    $"{_baseUrl}/patientrecords/{record.RecordID}", record);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteRecordAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/patientrecords/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }

    public class CreateResponse
    {
        public int Id { get; set; }
    }
}
