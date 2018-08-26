using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using DataAccess;

namespace WebServicesHADS
{
    /// <summary>
    /// Summary description for WebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService : System.Web.Services.WebService
    {
        private DataAccess.DataAccess da = new DataAccess.DataAccess();
        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public int Sumar(int x, int y)
        {
            return x + y;
        }
        
        [WebMethod]
        public DataSet GetTeacherSubjects(string email)
        {
            return da.getSubjectProfesorDataSet(email); 
        }

        [WebMethod]
        public DataSet GetAllSubjects()
        {
            return da.getAllSubjects();
        }

        [WebMethod]
        public float getAvgOverWork(string subject)
        {
            return da.getAvg_OverWork(subject);
        }

        [WebMethod]
        public float getStudentQuantity(string subject)
        {
            return da.getStudentQuantity(subject);
        }

    }
}
