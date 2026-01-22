using System;

namespace HealthTech.Services
{
    public class UserSession
    {
        public User? CurrentUser { get; private set; }
        
        // Event to notify when login state changes
        public event Action? OnChange;

        public void Login(User user)
        {
            CurrentUser = user;
            NotifyStateChanged();
        }

        public void Logout()
        {
            CurrentUser = null;
            NotifyStateChanged();
        }

        // Helper to check if we are logged in
        public bool IsLoggedIn => CurrentUser != null;
        
        // Helper to get current user's name
        public string UserName => CurrentUser?.Name ?? "Guest";
        
        // Helper to check if user has a specific role
        public bool IsInRole(string role)
        {
            return CurrentUser?.Role?.Equals(role, StringComparison.OrdinalIgnoreCase) ?? false;
        }
        
        // Notify subscribers that state has changed
        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}