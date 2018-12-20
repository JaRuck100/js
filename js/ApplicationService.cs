using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

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

        public bool IsValueAlreadyUsed(string value, string table, string collumName, SQLiteConnection conn)
        {
            string sql = String.Format("SELECT Count({0}) AS Count FROM {1} WHERE {0} LIKE '{2}'", collumName, table, value);
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            SQLiteDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                int count = Convert.ToInt32(reader["Count"]);
                if (count != 0)
                {
                    reader.Close();
                    return true;
                } 
                reader.Close();
                return false;
            }
            return false;
        }

        public bool IsUsernameAlreadyUsed( string username, SQLiteConnection conn)
        {
            return IsValueAlreadyUsed(username, "User", "username", conn);
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
                string sql = String.Format("insert into User ( username , password ) values ('{0}', '{1}')", username, sha1(password));
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

            string sql = String.Format("Select * from User where username = '{0}' AND password = '{1}'", username, verpassword);
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();

            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int idreader = reader.GetInt32(0);
                var usernamereader = reader.GetString(1);
                var passwordreader = reader.GetString(2);
                

                if (!string.IsNullOrEmpty(usernamereader) && !string.IsNullOrEmpty(passwordreader))
                {
                    userlist.Add(new User() { Username = usernamereader, Password = passwordreader, Id = idreader});
                }

            }
            conn.Close();
            return userlist;
        }

        public bool IsToDoListTitleAlreadyUsed(string title)
        {
            var conn = DatabaseConnection.openConnection();
            return IsValueAlreadyUsed(title, "ToDoList", "Title", conn);
        }

        public Dictionary<int, string> getToDoListsByUserId(int userId)
        {
            Dictionary<int, string> toDoLists = new Dictionary<int, string>();
            var conn = DatabaseConnection.openConnection();
            string sql = "SELECT id, title FROM ToDoList WHERE userId = " + userId;
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                toDoLists.Add(reader.GetInt32(0), reader.GetString(1));
            }
            conn.Close();
            return toDoLists;
        }

        public List<Task> getTaksByToDoListId(int toDoListId)
        {
            List<Task> tasks = new List<Task>();
            var conn = DatabaseConnection.openConnection();
            string sql = "SELECT * FROM Task WHERE toDoListId = " + toDoListId;
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                tasks.Add(new Task() {id = reader.GetInt32(0), Title = reader.GetString(1), StartDate = DateTime.Parse(reader.GetString(2)), EndDate = DateTime.Parse(reader.GetString(3)), Priority = reader.GetInt32(5), TaskFininshed = reader.GetBoolean(6)});
            }
            conn.Close();
            return tasks;
        }

        public List<Contacts> getContactsByUserId(int userId)
        {
            List<Contacts> contacts = new List<Contacts>();
            var conn = DatabaseConnection.openConnection();
            string sql = "SELECT * FROM Contact WHERE userId = " + userId;
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                contacts.Add(new Contacts() { Id = reader.GetInt32(0), FirstName = reader.GetString(1), Surname = reader.GetString(2), Phone = reader.GetString(3), Email = reader.GetString(4), Street = reader.GetString(5), City = reader.GetString(6), Postalcode = reader.GetString(7), PicturePath = reader.GetString(8) });
            }
            conn.Close();
            return contacts;
        }

        public List<string> getContactNameByTaskId(int taskId)
        {
            List<string> contactNames = new List<string>();
            var conn = DatabaseConnection.openConnection();
            string sql = String.Format("SELECT firstName, surname FROM Contact WHERE id IN (SELECT contactId FROM TaskContact WHERE taskId = {0})", taskId);
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                contactNames.Add(reader.GetString(0) + " " + reader.GetString(1));
            }
            conn.Close();
            return contactNames;
        }

        public int getToDoListIdByTitle(string toDoListTitle, SQLiteConnection conn)
        {
            string sqlToDoListId = String.Format("SELECT Id FROM ToDoList WHERE title = '{0}'", toDoListTitle);
            SQLiteCommand command = new SQLiteCommand(sqlToDoListId, conn);
            command.ExecuteNonQuery();
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                return reader.GetInt32(0);
            }
            return 0;
        }

        public void updateContactTable(Contacts alteredContact)
        {
            var conn = DatabaseConnection.openConnection();
            string sql = String.Format("UPDATE Contact SET firstName = '{0}', surname = '{1}', phone = '{2}', email = '{3}', street = '{4}', city = '{5}', postalCode = '{6}', picturePath = '{7}' WHERE id = {8}", alteredContact.FirstName, alteredContact.Surname, alteredContact.Phone, alteredContact.Email, alteredContact.Street, alteredContact.City, alteredContact.Postalcode, alteredContact.PicturePath, alteredContact.Id);
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            conn.Close();
        }

        public void updateToDoListTable(int toDoListId, string newTitle)
        {
            var conn = DatabaseConnection.openConnection();
            string sql = String.Format("UPDATE ToDoList SET title = '{0}' WHERE id = {1}", newTitle, toDoListId);
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            conn.Close();
        }

        public void updateTaskTable(Task alteredTask)
        {
            var conn = DatabaseConnection.openConnection();
            string sql = String.Format("UPDATE Task SET title = '{0}', startDate = '{1}', enddate = '{2}', priority = {3}, taskFinished = {4}, description = {5} WHERE id = {6}", alteredTask.Title, alteredTask.StartDate.ToString("d"), alteredTask.EndDate.ToString("d"), alteredTask.Priority, alteredTask.TaskFininshed, alteredTask.Description ,alteredTask.id);
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            conn.Close();
        }


        public void CreateToDoList(string title, int userId)
        {
            var conn = DatabaseConnection.openConnection();
            string sql = String.Format("INSERT INTO ToDoList (title, userId) VALUES('{0}', {1})", title, userId);
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            conn.Close();
        }

        public void CreateTask(Task task, string toDoListTitle)
        {
            var conn = DatabaseConnection.openConnection();
            int toDoListId = getToDoListIdByTitle(toDoListTitle, conn);
            string sql =  String.Format("INSERT INTO Task (title, startDate, enddate, toDoListId, priority, taskFinished, description) VALUES ('{0}', '{1}', '{2}', {3}, {4}, {5}, '{6}')", task.Title, task.StartDate.ToString("d"), task.EndDate.ToString("d"), toDoListId, task.Priority, (task.TaskFininshed ? 1 : 0), task.Description);
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            conn.Close();
        }

        public void CreateContact(Contacts contact)
        {
            var conn = DatabaseConnection.openConnection();
            string sql = String.Format("INSERT INTO Contact (firstName, surname, phone, email, street, city, postalCode, picturePath, userId) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', {8})", contact.FirstName, contact.Surname, contact.Phone, contact.Email, contact.Street, contact.City, contact.Postalcode, contact.PicturePath, contact.UserId);
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            conn.Close();
        }

        public void DeleteContact(int contactId)
        {
            var conn = DatabaseConnection.openConnection();
            string sql = String.Format("DELETE FROM Contact WHERE id = {0}", contactId);
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();

            string sql2  = String.Format("DELETE FROM TaskContact WHERE contactId = {0}", contactId);
            SQLiteCommand command2 = new SQLiteCommand(sql2, conn);
            command2.ExecuteNonQuery();
            conn.Close();
        }

        public void DeleteToDoList(string toDoListTitle)
        {
            /// Holt die ID der To Do list anhand des Titels, und löscht dann alle zur Todo Liste gehörigen Tasks 
            /// und die Zuweisungen der Kontakte zu den Tasks   
            var conn = DatabaseConnection.openConnection();

            int toDoListId = getToDoListIdByTitle(toDoListTitle, conn);

            string sqlTaskIds = String.Format("SELECT id FROM Task WHERE toDoListId = {0}", toDoListId);
            SQLiteCommand command2 = new SQLiteCommand(sqlTaskIds, conn);
            command2.ExecuteNonQuery();
            List<int> taskIds = new List<int>();
            SQLiteDataReader reader2 = command2.ExecuteReader();
            while (reader2.Read())
            {
                taskIds.Add(reader2.GetInt32(0));
            }

            string ids = String.Join(", ", taskIds.ToArray());
            string deleteContactTaskAssignment = String.Format("DELETE FROM TaskContact WHERE taskId IN ({0})", ids);
            SQLiteCommand command3 = new SQLiteCommand(deleteContactTaskAssignment, conn);
            command3.ExecuteNonQuery();

            string sqlDeleteTaks = String.Format("DELETE FROM Task WHERE toDoListId = {0}", toDoListId);
            SQLiteCommand command4 = new SQLiteCommand(sqlDeleteTaks, conn);
            command4.ExecuteNonQuery();

            string deleteToDoList = String.Format("DELETE FROM ToDoList WHERE id = {0}", toDoListId);
            SQLiteCommand command5 = new SQLiteCommand(deleteToDoList, conn);
            command5.ExecuteNonQuery();

            conn.Close();
        }
    }
}
