using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace UniversitySystemMVC.Service
{
    public static class AppConfig
    {
        static AppConfig()
        {

        }

        public static class FileSystem
        {
            static FileSystem()
            {
                //defaultContactImage = ConfigurationManager.AppSettings["FileSystem:defaultContactImage"] ?? "/images/default.png";
            }
        }

        public static class Mailer
        {
            private static string host;

            private static string password;

            private static int port;
            private static bool ssl;
            private static string username;
            static Mailer()
            {
                username = ConfigurationManager.AppSettings["Mailer:username"] ?? "username";
                password = ConfigurationManager.AppSettings["Mailer:password"] ?? "password";
                host = ConfigurationManager.AppSettings["Mailer:host"] ?? "smtp.gmail.com";
                if (!int.TryParse(ConfigurationManager.AppSettings["Mailer:port"] ?? "25", out port))
                {
                    port = 25;
                }
                if (!bool.TryParse(ConfigurationManager.AppSettings["Mailer:ssl"] ?? "true", out ssl))
                {
                    ssl = true;
                }
            }

            public static string Host
            {
                get { return host; }
            }

            public static string Password
            {
                get { return password; }
            }

            public static int Port
            {
                get { return port; }
            }

            public static bool SSL
            {
                get { return ssl; }
            }

            public static string Username
            {
                get { return username; }
            }
        }

        public static class Notification
        {
            private static int messagesShown;

            static Notification()
            {
                if (!int.TryParse(ConfigurationManager.AppSettings["Notification:messagesShown"] ?? "10", out messagesShown))
                {
                    messagesShown = 10;
                }
            }
            public static int MessagesShown
            {
                get { return messagesShown; }
            }

        }

        public static class Pager
        {
            private static int pagesShown;

            private static int perPage;

            static Pager()
            {
                if (!int.TryParse(ConfigurationManager.AppSettings["Pager:perPage"] ?? "10", out perPage))
                {
                    perPage = 10;
                }

                if (!int.TryParse(ConfigurationManager.AppSettings["Pager:pagesShown"] ?? "7", out pagesShown))
                {
                    pagesShown = 7;
                }
            }
            public static int PagesShown
            {
                get { return pagesShown; }
            }

            public static int PerPage
            {
                get { return perPage; }
            }
        }

        public static class UserRegistration
        {
            private static int minutesEmailSendLock;
            private static int minutesKeyIsValid;

            static UserRegistration()
            {
                //defaultContactImage = ConfigurationManager.AppSettings["FileSystem:defaultContactImage"] ?? "/images/default.png";
                if (!int.TryParse(ConfigurationManager.AppSettings["UserRegistration:minutesKeyIsValid"], out minutesKeyIsValid))
                {
                    minutesKeyIsValid = 60 * 24;
                }

                if (!int.TryParse(ConfigurationManager.AppSettings["UserRegistration:minutesEmailSendLock"], out minutesEmailSendLock))
                {
                    minutesEmailSendLock = 5;
                }
            }
            public static int MinutesEmailSendLock
            {
                get { return minutesEmailSendLock; }
                set { minutesEmailSendLock = value; }
            }

            public static int MinutesKeyIsValid
            {
                get { return minutesKeyIsValid; }
            }
        }
    }
}