using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;
using UniversitySystemMVC.Service;

namespace UniversitySystemMVC.Models
{
    public static class AuthenticationManager
    {
        public static bool IsAdmin { get { return LoggedUser is Administrator; } }
        public static bool IsStudent { get { return LoggedUser is Student; } }
        public static bool IsTeacher { get { return LoggedUser is Teacher; } }

        public static UserTypeEnum? UserType { get { return AuthenticationServiceInstance.UserType; } }

        private static AuthenticationService AuthenticationServiceInstance
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Session[typeof(AuthenticationService).Name] == null)
                {
                    HttpContext.Current.Session[typeof(AuthenticationService).Name] = new AuthenticationService();
                }

                return (AuthenticationService)HttpContext.Current.Session[typeof(AuthenticationService).Name];
            }
        }

        public static User LoggedUser
        {
            get
            {
                return AuthenticationManager.AuthenticationServiceInstance.LoggedUser;
            }
        }

        public static void AuthenticateUser(string username, string password, UserTypeEnum userType)
        {
            AuthenticationServiceInstance.AuthenticateUser(username, password, userType);
        }

        public static void Logout()
        {
            HttpContext.Current.Session[typeof(AuthenticationService).Name] = null;
            // force auth to null loggeduser
            AuthenticationServiceInstance.AuthenticateUser(null, null, UserTypeEnum.Administrator);
        }
    }
}