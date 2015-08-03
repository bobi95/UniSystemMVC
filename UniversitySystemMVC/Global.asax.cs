using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using UniversitySystemMVC.Controllers;
using UniversitySystemMVC.DA;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            LoadSiteStructure();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private void LoadSiteStructure()
        {
            UnitOfWork unitOfWork = new UnitOfWork();

            var assembly = Assembly.GetExecutingAssembly();

            //AuthControllersRepository authControllersRepo = new AuthControllersRepository();
            //AuthActionsRepository authActionsRepo = new AuthActionsRepository();

            Dictionary<string, List<string>> StructureInAssembly = new Dictionary<string, List<string>>();
            List<string> authActionsInAssembly = new List<string>();

            foreach (Type T in assembly.GetTypes())
            {
                if (typeof(BaseController).IsAssignableFrom(T))
                {
                    foreach (MethodInfo mi in T.GetMethods())
                    {
                        if (mi.ReturnType == typeof(ActionResult) || mi.ReturnType == typeof(JsonResult) || mi.ReturnType == typeof(Task<ActionResult>))
                        {
                            authActionsInAssembly.Add(mi.Name);
                        }
                    }

                    StructureInAssembly.Add(T.Name, authActionsInAssembly);
                    authActionsInAssembly = new List<string>();
                }
            }
            
            Dictionary<string, List<string>> StructureInDb = new Dictionary<string, List<string>>();
            List<string> authActionsInDb = new List<string>();

            var authControllers = unitOfWork.AuthControllersRepository.GetAll();

            foreach (var authController in authControllers)
            {
                foreach (var authAction in authController.AuthActions)
                {
                    if (authController.Id == authAction.AuthControllerId)
                    {
                        authActionsInDb.Add(authAction.Name);
                    }
                }

                StructureInDb.Add(authController.Name, authActionsInDb);
                authActionsInDb = new List<string>();
            }

            foreach (KeyValuePair<string, List<string>> kvPair in StructureInAssembly)
            {
                var structureInDb = StructureInDb.FirstOrDefault(kv => kv.Key == kvPair.Key);

                if (structureInDb.Key == null)
                {
                    AuthController authController = new AuthController();
                    authController.Name = kvPair.Key;
                    unitOfWork.AuthControllersRepository.Save(authController);

                    foreach (var item in kvPair.Value)
                    {
                        AuthAction authAction = new AuthAction();
                        authAction.Name = item;
                        authAction.AuthController = authController;
                        unitOfWork.AuthActionsRepository.Save(authAction);
                    }
                }
                else
                {
                    foreach (string action in kvPair.Value)
                    {
                        string actionInDb = structureInDb.Value.FirstOrDefault(v => v == action);

                        if (actionInDb == null)
                        {
                            AuthController authController = unitOfWork.AuthControllersRepository.GetAll(a => a.Name == kvPair.Key).FirstOrDefault();

                            AuthAction authAction = new AuthAction();
                            authAction.Name = action;
                            authAction.AuthController = authController;
                            unitOfWork.AuthActionsRepository.Save(authAction);
                        }
                    }
                }
            }

            foreach (KeyValuePair<string, List<string>> kvPair in StructureInDb)
            {
                KeyValuePair<string, List<string>> structureInAssembly = StructureInAssembly.FirstOrDefault(kv => kv.Key == kvPair.Key);

                if (structureInAssembly.Key == null)
                {
                    AuthController authController = unitOfWork.AuthControllersRepository.GetAll(c => c.Name == kvPair.Key).FirstOrDefault();

                    foreach (var item in kvPair.Value)
                    {
                        AuthAction authAction = unitOfWork.AuthActionsRepository.GetAll(a => a.AuthController.Id == authController.Id).FirstOrDefault();

                        unitOfWork.AuthActionsRepository.Delete(authAction.Id);
                    }

                    unitOfWork.AuthControllersRepository.Delete(authController.Id);
                }
                else
                {
                    foreach (string action in kvPair.Value)
                    {
                        string actionToAdd = structureInAssembly.Value.FirstOrDefault(v => v == action);

                        if (actionToAdd == null)
                        {
                            AuthAction authAction = unitOfWork.AuthActionsRepository.GetAll(a => a.Name == action).FirstOrDefault();

                            unitOfWork.AuthActionsRepository.Delete(authAction.Id);
                            unitOfWork.Save();
                        }
                    }
                }
            }

            unitOfWork.Save();


        }
    }
}
