using Model;
using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace Taller1.Controllers
{
    public class ClienteController : Controller
    {
        // GET: Cliente
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Listado()
        {
            return View(DynamicInvocation("GetCustomers", null));
        }

        public ActionResult Detalles(int idCustomer)
        {
            return View(DynamicInvocation("GetCustomer", new object[] { idCustomer }));
        }

        public ActionResult Nuevo()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Nuevo(Customer customer)
        {
            DynamicInvocation("NewCustomer", new object[] { customer });
            return View("Listado");
        }

        private object DynamicInvocation(string metodo, object []parameters, Type paramType = null)
        {
            try
            {
                var path = Server.MapPath("../");
                var assembly = Assembly.LoadFile(path + "\\bin\\Model.dll");
                Type type = assembly.GetType("Model.CustomerService", false, true);
                
                if (type != null)
                {
                    ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                    object classObj = constructor.Invoke(new object[] { });
                    MethodInfo method = type.GetMethod(metodo);
                    if(parameters[0] is Model.Customer)
                    {
                        var otherType = assembly.GetType("Model.Customer");
                        ConstructorInfo otherConstructor = otherType.GetConstructor(Type.EmptyTypes);
                        object otherClass = otherConstructor.Invoke(new object[] { });
                        CastTypes(otherClass, parameters[0]);
                        parameters = new object[] { otherClass };
                    }
                    object result = method.Invoke(classObj, parameters);
                    return result;
                }
            }
            catch(Exception ex)
            {
                //ignoramos errores...
            }

            return null;
        }

        static void CastTypes(object otherClass, object parameter)
        {
            var propO = parameter.GetType().GetProperties();
            var propC = otherClass.GetType().GetProperties();

            foreach (var p in propO)
            {
                var pC = propC.FirstOrDefault(t => t.Name == p.Name);
                pC.SetValue(otherClass, p.GetValue(parameter));
            }

        }



    }
}