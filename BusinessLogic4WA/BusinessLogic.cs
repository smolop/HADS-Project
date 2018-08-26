using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;

namespace BusinessLogic4WA
{
    public class BusinessLogic
    {
        private DataAccess.DataAccess ado = new DataAccess.DataAccess();
        private byte[] Buffer = new byte[0];
        public BusinessLogic() {}
       
        public bool registeUser(string email, string name, string lastName, int numConf, bool isVerify, string type, string password)
        {
            String encryptedPassword = EncryptTripleDES(password, "!@3$A");
           return ado.insertUser(email, name, lastName, numConf, isVerify, type, encryptedPassword); 
        }

        public DataSet getTareasGenericasDataSet()
        {
            return ado.getTareasGenericasDataSet();
        }

        public DataSet getSubjectsProfesorDataSet(string email)
        {
            return ado.getSubjectProfesorDataSet(email);
        }

        public DataSet getTareasGenericasDataSet(string subject)
        {
            return ado.getTareasGenericasDataSet(subject);
        }

        public DataSet getSubjectCodesDataSet(string email)
        {
            return ado.getSubjectDataSet(email);
        }
        public DataSet getOnlySubjectsDataSet(string email)
        {
            return ado.getOnlySubjectDataSet(email);
        }

        public string getUserRole(string email)
        {
            return ado.getUserRole(email);
        }

        public bool correctUser(string email, string role)
        {
            if (!userExists(email))
                return false;
            return ado.correctUser(email, role);
        }

        public bool passwordIsCorrect(string email, string password)
        {
            if (!userExists(email))
                return false;
            String pass = DecryptTripleDES(ado.getPassword(email), "!@3$A");
            return  password == pass;
        }

        public DataSet getStudentTasksDataSet(string email, string codAsig)
        {
            return ado.getStudenTasksDataSet(email, codAsig);
        }

        public bool userExists(string email)
        {
            return ado.userExists(email);
        }
        
        public string getConfirmNum(string email)
        {
           return ado.getConfirmNum(email);
        }

        public bool confirmUser(string email)
        {
            return ado.confirmUser(email);
        }

        public bool matchConfirmCode(string email, string code)
        {
            String dbCode = ado.getConfirmCode(email);
            return (dbCode == int.Parse(code).ToString());
        }
        
        public bool isPasswordMatched(string password, string repassword)
        {
            return password == repassword;
        }

        public bool changePassword(string email, string password, string repassword)
        {
            bool b = false;
            if (isPasswordMatched(password, repassword))
                if (ado.updatePassword(email, EncryptTripleDES(password, "!@3$A")))
                    b = true;
            return b;
        }

        public List<List<string>> getUSers()
        {
            return ado.getUsers();
        }

        public Boolean updateConfirmNum(string numConf, string email)
        {
            return ado.updateConfirmNum(numConf, email);
        }

        //Encryption method for credit card
        public string EncryptTripleDES(string Plaintext, string Key)
        {
            System.Security.Cryptography.TripleDESCryptoServiceProvider DES = new System.Security.Cryptography.TripleDESCryptoServiceProvider();
            System.Security.Cryptography.MD5CryptoServiceProvider hashMD5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            DES.Key = hashMD5.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(Key));
            DES.Mode = System.Security.Cryptography.CipherMode.ECB;
            System.Security.Cryptography.ICryptoTransform DESEncrypt = DES.CreateEncryptor();
            Buffer = System.Text.ASCIIEncoding.ASCII.GetBytes(Plaintext);
            string TripleDES = Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
            return TripleDES;
        }

        //Decryption Method 
        public string DecryptTripleDES(string base64Text, string Key)
        {
            System.Security.Cryptography.TripleDESCryptoServiceProvider DES = new System.Security.Cryptography.TripleDESCryptoServiceProvider();
            System.Security.Cryptography.MD5CryptoServiceProvider hashMD5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            DES.Key = hashMD5.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(Key));
            DES.Mode = System.Security.Cryptography.CipherMode.ECB;
            System.Security.Cryptography.ICryptoTransform DESDecrypt = DES.CreateDecryptor();
            Buffer = Convert.FromBase64String(base64Text);
            string DecTripleDES = System.Text.ASCIIEncoding.ASCII.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
            return DecTripleDES;
        }


        public void passwordUpadter()
        {
            List<List<string>> userList = new List<List<string>>();
            String user2update, pass2update;
            userList = getUSers();

            foreach (List<string> user in userList) {
                user2update = user.ElementAt(0).ToString();
                pass2update = user.ElementAt(user.Count).ToString();
                changePassword(user2update, pass2update, pass2update);
            }

        }

    }
}
