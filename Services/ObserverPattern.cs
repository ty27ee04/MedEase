using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthTech.Services
{
    // 1. The Observer Interface (From PDF Page 14)
    public interface IObserver
    {
        void Update(string message);
        Task UpdateAsync(string message, User patient, AppointmentModel appointment);
    }

    // 2. The Subject Class (From PDF Page 14)
    // This manages the list of people/systems waiting to be notified.
    public abstract class Subject
    {
        private List<IObserver> _observers = new List<IObserver>();

        public void Attach(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            _observers.Remove(observer);
        }

        // Notify all observers when something happens
        protected void Notify(string message)
        {
            foreach (var observer in _observers)
            {
                observer.Update(message);
            }
        }

        // Async notification with full appointment details
        protected async Task NotifyAsync(string message, User patient, AppointmentModel appointment)
        {
            foreach (var observer in _observers)
            {
                await observer.UpdateAsync(message, patient, appointment);
            }
        }
    }

    // 3. Concrete Observer - Real Email Notifier using NotificationService
    public class EmailNotifier : IObserver
    {
        private readonly NotificationService _notificationService;

        public EmailNotifier(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void Update(string message)
        {
            // Fallback for simple console logging
            Console.WriteLine($"[Email Service] Notification: {message}");
        }

        public async Task UpdateAsync(string message, User patient, AppointmentModel appointment)
        {
            if (string.IsNullOrEmpty(patient.Email))
            {
                Console.WriteLine($"[Email Service] No email for patient: {patient.Name}");
                return;
            }

            string subject = "Appointment Confirmation - HealthTech Medical";
            string body = $@"
                <p>Dear {patient.Name},</p>
                <p>{message}</p>
                <ul>
                    <li><strong>Doctor:</strong> {appointment.DoctorName}</li>
                    <li><strong>Date & Time:</strong> {appointment.DateTime:dd MMMM yyyy, hh:mm tt}</li>
                    <li><strong>Type:</strong> {appointment.Type}</li>
                </ul>
                <p>Thank you for choosing HealthTech Medical.</p>";

            await _notificationService.SendEmailAsync(patient.Email, patient.Name, subject, body);
        }
    }

    // 4. Concrete Observer - WhatsApp Notifier using Twilio
    public class WhatsAppNotifier : IObserver
    {
        private readonly NotificationService _notificationService;

        public WhatsAppNotifier(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void Update(string message)
        {
            // Fallback for simple console logging
            Console.WriteLine($"[WhatsApp Service] Notification: {message}");
        }

        public async Task UpdateAsync(string message, User patient, AppointmentModel appointment)
        {
            if (string.IsNullOrEmpty(patient.Phone))
            {
                Console.WriteLine($"[WhatsApp Service] No phone number for patient: {patient.Name}");
                return;
            }

            string whatsappMessage = $"HealthTech Medical\n" +
                                   $"✅ {message}\n\n" +
                                   $"Patient: {patient.Name}\n" +
                                   $"Doctor: {appointment.DoctorName}\n" +
                                   $"Date: {appointment.DateTime:dd MMM yyyy, hh:mm tt}\n" +
                                   $"Type: {appointment.Type}";

            await _notificationService.SendWhatsAppAsync(patient.Phone, whatsappMessage);
        }
    }

    // 5. Concrete Observer - SMS Notifier (Fallback for WhatsApp)
    public class SmsNotifier : IObserver
    {
        private readonly NotificationService _notificationService;

        public SmsNotifier(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void Update(string message)
        {
            // Fallback for simple console logging
            Console.WriteLine($"[SMS Service] Notification: {message}");
        }

        public async Task UpdateAsync(string message, User patient, AppointmentModel appointment)
        {
            if (string.IsNullOrEmpty(patient.Phone))
            {
                Console.WriteLine($"[SMS Service] No phone number for patient: {patient.Name}");
                return;
            }

            string smsMessage = $"HealthTech: {message} - " +
                              $"Dr. {appointment.DoctorName}, " +
                              $"{appointment.DateTime:dd MMM, hh:mm tt}";

            await _notificationService.SendSmsAsync(patient.Phone, smsMessage);
        }
    }
}