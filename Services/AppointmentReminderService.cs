using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace HealthTech.Services
{
    /// <summary>
    /// Background service that runs every hour to check for upcoming appointments
    /// and send reminders 24 hours and 2 hours before the appointment
    /// </summary>
    public class AppointmentReminderService : BackgroundService
    {
        private readonly ILogger<AppointmentReminderService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public AppointmentReminderService(
            ILogger<AppointmentReminderService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[Reminder Service] Started at {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndSendReminders();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[Reminder Service] Error occurred while checking reminders");
                }

                // Wait 1 hour before checking again
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private async Task CheckAndSendReminders()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var db = DatabaseManager.GetInstance();
                var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();

                var allAppointments = db.GetAllAppointments();
                var now = DateTime.Now;

                foreach (var appointment in allAppointments)
                {
                    // Skip if already sent reminder or appointment is in the past
                    if (appointment.Status == "ReminderSent" || appointment.DateTime < now)
                        continue;

                    var timeUntilAppointment = appointment.DateTime - now;

                    // Send reminder 24 hours before (23-25 hours window)
                    if (timeUntilAppointment.TotalHours >= 23 && timeUntilAppointment.TotalHours <= 25)
                    {
                        await SendReminder(appointment, 24, notificationService, db);
                        _logger.LogInformation(
                            "[Reminder Service] 24h reminder sent for Appointment ID {id}",
                            appointment.AppointmentID);
                    }
                    // Send reminder 2 hours before (1-3 hours window)
                    else if (timeUntilAppointment.TotalHours >= 1 && timeUntilAppointment.TotalHours <= 3)
                    {
                        await SendReminder(appointment, 2, notificationService, db);
                        _logger.LogInformation(
                            "[Reminder Service] 2h reminder sent for Appointment ID {id}",
                            appointment.AppointmentID);
                    }
                }
            }
        }

        private async Task SendReminder(
            AppointmentModel appointment,
            int hoursBeforeAppointment,
            NotificationService notificationService,
            DatabaseManager db)
        {
            try
            {
                // Get patient details (including email and phone)
                var patient = db.GetUserById(appointment.PatientID);
                if (patient == null)
                {
                    _logger.LogWarning(
                        "[Reminder Service] Patient not found for Appointment ID {id}",
                        appointment.AppointmentID);
                    return;
                }

                // Validate contact information
                if (string.IsNullOrEmpty(patient.Email) || string.IsNullOrEmpty(patient.PhoneNumber))
                {
                    _logger.LogWarning(
                        "[Reminder Service] Missing contact info for Patient ID {id}",
                        patient.UserID);
                    return;
                }

                // Send reminder
                await notificationService.SendAppointmentReminderAsync(
                    patientName: patient.Name,
                    patientEmail: patient.Email,
                    patientPhone: patient.PhoneNumber,
                    doctorName: appointment.DoctorName,
                    appointmentDate: appointment.DateTime,
                    appointmentType: appointment.Type,
                    hoursBeforeAppointment: hoursBeforeAppointment
                );

                // Update appointment status to mark reminder as sent
                appointment.Status = "ReminderSent";
                db.UpdateAppointment(appointment);

                _logger.LogInformation(
                    "[Reminder Service] Reminder sent successfully for Appointment ID {id}",
                    appointment.AppointmentID);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[Reminder Service] Failed to send reminder for Appointment ID {id}",
                    appointment.AppointmentID);
            }
        }
    }
}
