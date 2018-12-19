using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace js
{
    public class ApplicationService
    {
        public ApplicationService()
        {

        }

        string sha1(string input)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(input);
            string result = "";
            using (HashAlgorithm hash = SHA1.Create())
            {
                result = Convert.ToBase64String(hash.ComputeHash(byteArray));
            }
            return result;
        }

        public bool IsUsernameAlreadyUsed( string username, SQLiteConnection conn)
        {
            string sql = "SELECT * FROM User WHERE username = '" + username + "'";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            SQLiteDataReader reader = command.ExecuteReader();
            // if reader has rows the username already exists
            if(reader.Read())
            {
                return true;
            }
            return false;  
        }

        public void CreateUser(string username, string password)
        {
            var conn = DatabaseConnection.openConnection();
            bool usernameAlreadyUsed = IsUsernameAlreadyUsed(username, conn);
            if (usernameAlreadyUsed)
            {
                //anzeige auf Seite, dass username schon vergeben ist
            }
            else
            {
                string sql = "insert into User ( username , password ) values ('" + username + "', '" + sha1(password) + "')";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery(); 
            }
            conn.Close();
        }

        public List<User> CheckUser(string username, string password)
        {
            List<User> userlist = new List<User>();
            var conn = DatabaseConnection.openConnection();
            var verpassword = sha1(password);

            string sql = "Select * from User where username ='" + username + "' AND password ='" + verpassword + "'";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();

            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {

                var usernamereader = reader.GetString(1);
                var passwordreader = reader.GetString(2);

                if (!string.IsNullOrEmpty(usernamereader) && !string.IsNullOrEmpty(passwordreader))
                {
                    userlist.Add(new User() { Username = usernamereader, Password = passwordreader });
                }

            }

            conn.Close();
            return userlist;
        }

        public void CreateTask(Task task, int todoListId)
        {
            var conn = DatabaseConnection.openConnection();
            string sql = "insert into Task (title, startDate, enddate, toDoListId, priority, taskFinished) values ('" + task.Title + "', '" + task.StartDate.ToString("d") + "', '" + task.EndDate.ToString("d") + "'," + todoListId + "," + task.Priority + ", " + (task.TaskFininshed ? 1 : 0) + ")";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            conn.Close();
        }

    }
}
