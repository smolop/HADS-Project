using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using Sql.Library;
using System.Data.SqlClient;
using System.Data;

namespace DataAccess
{
    public class DataAccess
    {
        private SQL_Query dbManager = new SQL_Query();
        private AdoNet adoMng = new AdoNet();
        public DataAccess() { }

        Boolean insertQuery()
        {
            Boolean b = false;
            return b;
        }

        public bool insertUser(string email, string name, string lastName, int numConf, bool isVerify, string type, string password)
        {
            StringBuilder queryString = new StringBuilder();
            queryString.AppendFormat("INSERT INTO Usuarios VALUES('{0}', '{1}', '{2}', {3}, '{4}', '{5}', '{6}')", email, name, lastName, numConf, isVerify, type, password);
            return dbManager.insertQuery(queryString.ToString());
        }

        public DataSet getTareasGenericasDataSet(string subject)
        {
            StringBuilder queryString = new StringBuilder();
            queryString.AppendFormat("SELECT * FROM TareasGenericas WHERE CodAsig='{0}'", subject);
            return adoMng.selectDataSet("TareasGenericas", queryString.ToString());
        }

        public DataSet getAllSubjects()
        {
            StringBuilder queryString = new StringBuilder();
            queryString.AppendFormat("select codigo from Asignaturas");
            return adoMng.selectDataSet("Asignaturas", queryString.ToString());
        }

        public float getAvg_OverWork(string subject)
        {
            StringBuilder sb = new StringBuilder();
            float avgOverWork = 0;
            float aux = -1F;
            sb.AppendFormat("select avg(HReales) from tareasGenericas as tg join estudiantesTareas as et on tg.Codigo=et.CodTarea where tg.CodAsig='{0}'", subject);
            dbManager.Open();
            SqlDataReader dr = dbManager.readQuery(sb.ToString());
            if (dr.Read())
                if (dr[0] != null)
                    if(dr[0] is int || dr[0] is float)
                    avgOverWork = float.Parse(dr[0].ToString());
            dbManager.Close();
            return avgOverWork;
        }

        public float getStudentQuantity(string subject)
        {
            StringBuilder sb = new StringBuilder();
            float studentsQuantity = 0;
            sb.AppendFormat("select count(DISTINCT et.email) from tareasGenericas as tg join estudiantesTareas as et on tg.Codigo=et.CodTarea where tg.CodAsig='{0}'", subject);
            dbManager.Open();
            SqlDataReader dr = dbManager.readQuery(sb.ToString());
            if (dr.Read())
                if (dr[0] != null)
                    if (dr[0] is int || dr[0] is float)
                        studentsQuantity = float.Parse(dr[0].ToString());
            dbManager.Close();
            return studentsQuantity;
        }

        public List<string> getEmailsWith(string subject)
        {
            List<string> emails = new List<string>();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("select eg.email from GruposClase as gc inner join EstudiantesGrupo as eg on gc.codigo=eg.grupo where gc.codigoasig='{0}'", subject);

            dbManager.Open();
            SqlDataReader reader = dbManager.readQuery(sb.ToString());
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                    emails.Add(string.Format("{0}", reader[i]));
            }
            dbManager.Close();
            return emails;
        }

        public DataSet getTareasGenericasDataSet()
        {
            StringBuilder queryString = new StringBuilder();
            queryString.AppendFormat("SELECT * FROM TareasGenericas");
            return adoMng.selectDataSet("TareasGenericas", queryString.ToString());
        }

        public DataSet getSubjectProfesorDataSet(string email)
        {
            StringBuilder queryString = new StringBuilder();
            queryString.AppendFormat("SELECT DISTINCT gc.codigoasig FROM GruposClase AS gc INNER JOIN ProfesoresGrupo AS pg ON pg.codigogrupo = gc.codigo WHERE pg.email = '{0}'", email);
            return adoMng.selectDataSet("GruposClase", queryString.ToString());
        }

        public DataSet getOnlySubjectDataSet(string email)
        {
            StringBuilder queryString = new StringBuilder();
            queryString.AppendFormat("SELECT CodAsig FROM TareasGenericas AS tg INNER JOIN EstudiantesTareas AS et ON tg.Codigo=et.CodTarea WHERE Email='{0}'", email);
            return adoMng.selectDataSet("TareasGenericas", queryString.ToString());
        }

        public DataSet getStudenTasksDataSet(string email, string codAsig)
        {
            StringBuilder queryString = new StringBuilder();
            queryString.AppendFormat("SELECT tg.Codigo, tg.Descripcion, tg.CodAsig, et.HEstimadas, et.HReales, tg.Explotacion, tg.TipoTarea FROM TareasGenericas AS tg INNER JOIN EstudiantesTareas AS et ON tg.Codigo = et.CodTarea WHERE et.Email = '{0}' AND tg.CodAsig = '{1}' AND tg.Explotacion = 'true' AND et.HReales > 0", email, codAsig);
            return adoMng.selectDataSet(queryString.ToString());
        }

        public DataSet getSubjectDataSet(string email)
        {
            StringBuilder queryString = new StringBuilder();
            queryString.AppendFormat("SELECT CodAsig FROM TareasGenericas AS tg INNER JOIN EstudiantesTareas AS et ON tg.Codigo=et.CodTarea WHERE Email='{0}'", email);
            return adoMng.selectDataSet("TareasGenericas", queryString.ToString());
        }

        public string getUserRole(string email)
        {
            String role = "";
            StringBuilder queryString = new StringBuilder();
            queryString.AppendFormat("SELECT tipo FROM Usuarios WHERE email='{0}'", email);
            dbManager.Open();
            SqlDataReader r = dbManager.readQuery(queryString.ToString());
            if (r.Read())
                role = r[0].ToString();
            dbManager.Close();
            return role;
        }

        public bool correctUser(string email, string role)
        {
            return userExists(email, role);
        }

        public DataSet getConfirmCodeDataSet(string email)
        {
            StringBuilder queryString = new StringBuilder();
            queryString.AppendFormat("SELECT CodAsig FROM TareasGenericas AS tg INNER JOIN EstudiantesTareas AS et ON tg.Codigo=et.CodTarea WHERE Email='{0}'", email);
            return adoMng.selectDataSet("TareasGenericas", queryString.ToString());
        }

        public string getPassword(string email)
        {
            String password = "";
            StringBuilder queryString = new StringBuilder();
            queryString.AppendFormat("SELECT pass FROM Usuarios WHERE email='{0}'", email);
            dbManager.Open();
            SqlDataReader r = dbManager.readQuery(queryString.ToString());
            if (r.Read())
                password = r[0].ToString();
            dbManager.Close();
            return password;
        }

        public bool userExists(string email)
        {
            return userExists(email, "$role!");
        }

        protected bool userExists(string email, string role)
        {
            Boolean b;
            StringBuilder queryString = new StringBuilder();
            dbManager.Open();
            if (role.Equals("$role!"))
                queryString.AppendFormat("SELECT email FROM Usuarios WHERE email='{0}'", email);
            if (role.Equals("Alumno") || role.Equals("Profesor") || role.Equals("Administrador"))
                queryString.AppendFormat("SELECT email FROM Usuarios WHERE email='{0}' AND tipo='{1}'", email, role);
            b = (dbManager.readQuery(queryString.ToString()).HasRows ? true : false);
            dbManager.Close();
            return b;
        }

        public bool confirmUser(string email)
        {
            StringBuilder queryString = new StringBuilder("UPDATE Usuarios SET confirmado='True', numconfir=0");
            return dbManager.updateQuery(queryString.ToString());
        }

        public String getConfirmCode(string email)
        {
            return this.getConfirmNum(email);
        }

        public bool updateConfirmNum(string numConf, string email)
        {
            StringBuilder queryString = new StringBuilder();
            queryString.AppendFormat("UPDATE Usuarios SET numconfir={0} WHERE email='{1}'", numConf, email);
            return dbManager.updateQuery(queryString.ToString());
        }

        public bool updatePassword(string email, string password)
        {
            StringBuilder queryString = new StringBuilder();
            queryString.AppendFormat("UPDATE Usuarios SET pass='{0}' WHERE email='{1}'", password, email);
            return dbManager.updateQuery(queryString.ToString());
        }

        public String getConfirmNum(string email)
        {
            String numConf = "-1";
            StringBuilder queryString = new StringBuilder();
            queryString.AppendFormat("SELECT numconfir FROM Usuarios WHERE email='{0}'", email);
            dbManager.Open();
            SqlDataReader dataReader = dbManager.readQuery(queryString.ToString());
            if (dataReader.Read())
                numConf = dataReader[0].ToString();
            dbManager.Close();
            return numConf;
        }

        public List<List<string>> getUsers()
        {
            List<List<string>> users = new List<List<string>>();
            dbManager.Open();
            SqlDataReader reader = dbManager.readQuery("SELECT * FROM Usuarios");
            while (reader.Read())
            {
                List<String> elements = new List<string>();
                for (int i = 0; i < reader.FieldCount; i++)
                    elements.Add(string.Format("{0}", reader[i]));
                users.Add(elements);
            }
            dbManager.Close();
            return users;
        }

    }
}
