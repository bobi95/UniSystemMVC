﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.ViewModels.AdminsVM
{
    public class AdminsManageCoursesVM
    {
        public List<Course> Courses { get; set; }

        public List<CoursesSubjects> CoursesSubjects { get; set; }
    }
}