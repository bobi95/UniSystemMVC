using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniversitySystemMVC.DA;
using UniversitySystemMVC.Entity;
using UniversitySystemMVC.Filters;
using UniversitySystemMVC.ViewModels.SubjectsVM;
using UniversitySystemMVC.Extensions;
using System.Text;

namespace UniversitySystemMVC.Controllers
{
	[AuthorizeUser(UserType = UserTypeEnum.Administrator, CheckType = true)]
	public class SubjectController : Controller
	{
		UnitOfWork unitOfWork = new UnitOfWork();

		// GET: Subject
		public ActionResult Index()
		{
			return View();
		}

		#region CreateSubject
		[HttpGet]
		public ActionResult CreateSubject()
		{
			SubjectsCreateVM model = new SubjectsCreateVM();
			model.Courses = unitOfWork.CourseRepository.GetAll().ToList();

			return View("CreateEditSubject", model);
		}

		[HttpGet]
		public ActionResult EditSubject(int? id)
		{
			if (!id.HasValue)
			{
				return RedirectToAction("ManageSubjects", "Admin");
			}

			Subject subject = unitOfWork.SubjectRepository.GetById(id.Value);

			if (subject == null)
			{
				return RedirectToAction("ManageSubjects", "Admin");
			}

			SubjectsCreateVM model = new SubjectsCreateVM();
			model.Id = subject.Id;
			model.Name = subject.Name;
			model.Courses = unitOfWork.CourseRepository.GetAll().ToList();

			return View("CreateEditSubject", model);
		}

		[HttpPost]
		public ActionResult CreateEditSubject(SubjectsCreateVM model)
		{
			if (ModelState.IsValid)
			{
				Subject subject;
				if (model.Id == 0)
				{
					subject = new Subject();
				}
				else
				{
					subject = unitOfWork.SubjectRepository.GetById(model.Id);
				}

				subject.Name = model.Name;

				if (model.Id == 0)
				{
					unitOfWork.SubjectRepository.Insert(subject);
					TempData.FlashMessage("Subject has been created!");
				}
				else
				{
					unitOfWork.SubjectRepository.Update(subject);
					TempData.FlashMessage("Subject has been edited!");
				}
				unitOfWork.Save();


				model.Courses = unitOfWork.CourseRepository.GetAll().ToList();

				List<Course> courses = new List<Course>();

				foreach (var s in model.Courses)
				{
					if ((Request.Form[s.Id.ToString()] != null) && (Request.Form[s.Id.ToString()] == "on"))
					{
						courses.Add(s);
					}
				}

				unitOfWork.CoursesSubjectsRepository.UpdateTable(subject, courses);

				return RedirectToAction("ManageSubjects", "Admin");
			}

			return View(model);
		}
		#endregion CreateSubject

		#region DeleteSubject
		[HttpGet]
		public ActionResult DeleteSubject(int? id)
		{
			if (!id.HasValue)
			{
				return RedirectToAction("ManageSubjects", "Admin");
			}

			Subject subject = unitOfWork.SubjectRepository.GetById(id.Value);

			if (subject == null)
			{
				return RedirectToAction("ManageSubjects", "Admin");
			}

			SubjectsDeleteVM model = new SubjectsDeleteVM();
			model.Id = subject.Id;
			model.Name = subject.Name;

			return View(model);
		}
		[HttpPost]
		public ActionResult DeleteSubject(SubjectsDeleteVM model)
		{
			if (ModelState.IsValid)
			{
				Subject subject = unitOfWork.SubjectRepository.GetById(model.Id, true);

				List<CoursesSubjects> cs = unitOfWork.CoursesSubjectsRepository.GetBySubjectId(subject.Id, true);

				cs.ForEach(x => x.Teachers.Clear());

				subject.CoursesSubjects.Clear();
				subject.Grades.Clear();

				unitOfWork.CoursesSubjectsRepository.UpdateTable(subject, new List<Course>());

				unitOfWork.SubjectRepository.Delete(subject.Id);

				unitOfWork.Save();

				TempData.FlashMessage("Subject has been deleted");
				return RedirectToAction("ManageSubjects", "Admin");
			}

			return View(model);
		}
		#endregion DeleteSubject

		public ActionResult Details(int? id, SubjectsDetailsVM model, string submitBtn)
		{
			if (!id.HasValue)
			{
				return RedirectToAction("ManageSubjects", "Admin");
			}

			Subject subject = unitOfWork.SubjectRepository.GetById(id.Value);

			if (subject == null)
			{
				return RedirectToAction("ManageSubjects", "Admin");
			}

			model.Courses = unitOfWork.CourseRepository.GetAll().ToList();
			model.Subject = subject;
			model.CoursesSubjects = unitOfWork.CoursesSubjectsRepository.GetBySubjectId(subject.Id, true);

			model.SubjectAverages = new Dictionary<int, double>();

			model.CoursesSubjects.ForEach(cs =>
			{
				if (cs.Course.Students == null || cs.Course.Students.Count(s => s.IsActive) == 0)
				{
					return;
				}

				var studentaverages = cs.Course.Students.Where(s => s.IsActive && s.Grades.Count > 0).Select(s =>
				{
					var grades = s.Grades.Where(g => g.SubjectId == cs.Subject.Id).ToArray();
					if (grades.Length > 0)
					{
						return grades.Average(g => g.GradeValue);
					}
					return 0;

				}).ToArray();

				if (studentaverages.Length > 0)
				{
					model.SubjectAverages.Add(cs.Course.Id, studentaverages.Average());
				}
			});

			#region SortingFiltering
			model.Props = new Dictionary<string, object>();

			model.Props["firstname"] = model.FirstName;
			model.Props["lastname"] = model.LastName;
			model.Props["facultyNumber"] = model.FacultyNumber;
			model.Props["courseId"] = model.CourseId;

			if (model.CourseId!= 0)
			{
				model.CoursesSubjects = model.CoursesSubjects.Where(cs => cs.Course.Id == model.CourseId).ToList();
			}

			if (!String.IsNullOrEmpty(model.FirstName))
			{
				foreach (var cs in model.CoursesSubjects)
				{
					List<Student> students = new List<Student>();
					foreach (var s in cs.Course.Students)
					{
						if (s.FirstName.ToLower().Contains(model.FirstName.ToLower()))
						{
							students.Add(s);
						}
					}
					cs.Course.Students = students;
					students = null;
				}
			}
			if (!String.IsNullOrEmpty(model.FacultyNumber))
			{

				foreach (var cs in model.CoursesSubjects)
				{
					List<Student> students = new List<Student>();
					foreach (var s in cs.Course.Students)
					{
						if (s.FacultyNumber == model.FacultyNumber)
						{
							students.Add(s);
						}
					}
					cs.Course.Students = students;
					students = null;
				}
			}
			if (!String.IsNullOrEmpty(model.LastName))
			{
				
				foreach (var cs in model.CoursesSubjects)
				{
					List<Student> students = new List<Student>();
					foreach (var s in cs.Course.Students)
					{
						if (s.LastName.ToLower().Contains(model.LastName.ToLower()))
						{
							students.Add(s);
						}
					}
					cs.Course.Students = students;
					students = null;
				}
			}

			switch (model.SortOrder)
			{
				case "fnum_desc":
					model.CoursesSubjects = model.CoursesSubjects.Select(cs => {
						if (cs.Course.Students != null)
						{
							cs.Course.Students = cs.Course.Students.OrderByDescending(c => c.FacultyNumber).ToList(); 
						}
						return cs; 
					}).ToList();
					break;
				case "name_asc":
					model.CoursesSubjects = model.CoursesSubjects.Select(cs => {
						if (cs.Course.Students != null)
						{
							cs.Course.Students = cs.Course.Students.OrderBy(c => c.FirstName).ToList(); 
						}
						return cs; 
					}).ToList();
					break;
				case "name_desc":
					model.CoursesSubjects = model.CoursesSubjects.Select(cs => {
						if (cs.Course.Students != null)
						{
							cs.Course.Students = cs.Course.Students.OrderByDescending(c => c.FirstName).ToList(); 
						}
						return cs; 
					}).ToList();
					break;
				case "fnum_asc":
				default:
					model.CoursesSubjects = model.CoursesSubjects.Select(cs => {
						if (cs.Course.Students != null)
						{
							cs.Course.Students = cs.Course.Students.OrderBy(c => c.FacultyNumber).ToList();   
						}
						return cs;
					}).ToList();
					break;
			}

			#endregion SortingFiltering

			if (submitBtn == "Export") // Export grades for single Subject
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendLine(String.Format("Faculty Number,Name,Grade"));
				foreach (var cs in model.CoursesSubjects)
				{
					foreach (var student in cs.Course.Students)
					{
						double total = 0;
						foreach (var grade in student.Grades.Where(g => g.Subject.Id == model.Subject.Id)) // ???
						{
							total += grade.GradeValue;
						}
						double avg = total / student.Grades.Count(g => g.SubjectId == model.Subject.Id);
						sb.AppendLine(String.Format("{0},{1},{2}", student.FacultyNumber, student.FirstName + " " + student.LastName, avg));
					}
				}

				string filename = "subject-grades-" + model.Subject.Name + "-" + DateTime.Now.Date + ".csv"; 

				return File(new System.Text.UTF8Encoding().GetBytes(sb.ToString()), "text/csv", filename);
			}

			return View(model);
		}

		[AuthorizeUser(UserType=UserTypeEnum.Administrator, CheckType=true)]
		public ActionResult ExportGrades(SubjectsDetailsVM model)
		{
			StringBuilder sb = new StringBuilder();
			//sb.AppendLine(String.Format("{0},{1},{2},{3}", ));
			string csv = "Charlie, Chaplin, Chuckles";
			return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", "Report123.csv");
		}
	}
}