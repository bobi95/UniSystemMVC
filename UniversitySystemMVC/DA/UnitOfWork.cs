﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.DA
{
    public class UnitOfWork
    {
        private AdministratorRepository adminRepo;
        private StudentRepository studentRepo;
        private TeacherRepository teacherRepo;
        private TitleRepository titleRepo;
        private CourseRepository courseRepo;
        private GradeRepository gradeRepo;
        private SubjectRepository subjectRepo;
        private CoursesSubjectsRepository coursesSubjectRepo;
        private ArticleRepository articleRepo;

        private AppContext context;

        public UnitOfWork()
        {
            this.context = new AppContext();
        }

        public AdministratorRepository AdminRepository
        {
            get
            {
                if (adminRepo == null)
                {
                    adminRepo = new AdministratorRepository(context);
                }
                return adminRepo;
            }
        }

        public StudentRepository StudentRepository
        {
            get
            {
                if (studentRepo == null)
                {
                    studentRepo = new StudentRepository(context);
                }
                return studentRepo;
            }
        }

        public TeacherRepository TeacherRepository
        {
            get
            {
                if (teacherRepo == null)
                {
                    teacherRepo = new TeacherRepository(context);
                }
                return teacherRepo;
            }
        }

        public TitleRepository TitleRepository
        {
            get
            {
                if (titleRepo == null)
                {
                    titleRepo = new TitleRepository(context);
                }
                return titleRepo;
            }
        }

        public CourseRepository CourseRepository
        {
            get
            {
                if (courseRepo == null)
                {
                    courseRepo = new CourseRepository(context);
                }
                return courseRepo;
            }
        }

        public GradeRepository GradeRepository
        {
            get
            {
                if (gradeRepo == null)
                {
                    gradeRepo = new GradeRepository(context);
                }
                return gradeRepo;
            }
        }

        public SubjectRepository SubjectRepository
        {
            get
            {
                if (subjectRepo == null)
                {
                    subjectRepo = new SubjectRepository(context);
                }
                return subjectRepo;
            }
        }

        public CoursesSubjectsRepository CoursesSubjectsRepository
        {
            get
            {
                if (coursesSubjectRepo == null)
                {
                    coursesSubjectRepo = new CoursesSubjectsRepository(context);
                }
                return coursesSubjectRepo;
            }
        }

        public ArticleRepository ArticleRepository
        {
            get
            {
                if (articleRepo == null)
                {
                    articleRepo = new ArticleRepository(context);
                }
                return articleRepo;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}