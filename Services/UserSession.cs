using System;

namespace HealthTech.Services
{
    public class UserSession
    {
        public User? CurrentUser { get; private set; }

        public void Login(User user)
        {
            CurrentUser = user;
        }

        public void Logout()
        {
            CurrentUser = null;
        }

        // Helper to check if we are logged in
        public bool IsLoggedIn => CurrentUser != null;
    }
}