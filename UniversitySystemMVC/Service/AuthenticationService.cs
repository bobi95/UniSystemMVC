using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversitySystemMVC.DA;
using UniversitySystemMVC.Entity;
using UniversitySystemMVC.Hasher;

namespace UniversitySystemMVC.Service
{
    public class AuthenticationService
    {
        UnitOfWork unitOfWork = new UnitOfWork();

        public UserTypeEnum? UserType { get; set; }

        public User LoggedUser { get; private set; }

        public bool AuthenticateUser(string username, string password, UserTypeEnum userType)
        {
            LoggedUser = null;
            UserType = null;

            AppContext ctx = new AppContext();

            switch (userType)
            {
                case UserTypeEnum.Administrator:
                    AdministratorRepository adminRepo = new AdministratorRepository(new AppContext());
                    LoggedUser = unitOfWork.AdminRepository.GetByUsername(username);
                    break;
                case UserTypeEnum.Student:
                    StudentRepository studentRepo = new StudentRepository(new AppContext());
                    LoggedUser = unitOfWork.StudentRepository.GetByUsername(username);
                    break;
                case UserTypeEnum.Teacher:
                    TeacherRepository teacherRepo = new TeacherRepository(new AppContext());
                    LoggedUser = unitOfWork.TeacherRepository.GetByUsername(username);
                    break;
            }

            if (LoggedUser != null)
            {
                if (PasswordHasher.Equals(password, LoggedUser.Salt, LoggedUser.Hash))
                {
                    UserType = userType;
                    return true;
                }
                LoggedUser = null;
            }

            return false;
        }
    }
}