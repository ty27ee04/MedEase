using System;
using System.Collections.Generic;

namespace HealthTech.Services
{
    // 1. The Observer Interface (From PDF Page 14)
    public interface IObserver
    {
        void Update(string message);
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
    }

    // 3. Concrete Observer (From PDF Page 14)
    // This simulates sending an email.
    public class EmailNotifier : IObserver
    {
        public void Update(string message)
        {
            // In a real app, this would send an SMTP email.
            // For now, we print to the console logs.
            Console.WriteLine($"[Email Service] Notification Sent: {message}");
        }
    }
}