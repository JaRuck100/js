using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
// System.Data.SQLite installieren in Visual Studio



namespace js
{
    public class DatabaseConnection
    {

       static bool isCreate = false;
       public static string sha1(string input)
       {
           byte[] byteArray = Encoding.UTF8.GetBytes(input);
           string result = "";
           using (HashAlgorithm hash = SHA1.Create())
           {
               result = Convert.ToBase64String(hash.ComputeHash(byteArray));
           }
           return result;
       }

        public static void Seed() 
        {
            if (!isCreate)
            {
                isCreate = true;

                createDatbase();
                var connection = openConnection();
                createTables(connection);
                connection.Close();
            }
        }

        public static void createDatbase()
        {
            if(File.Exists("ToDoListDatabase.sqlite") == false) 
            {
                SQLiteConnection.CreateFile("ToDoListDatabase.sqlite");
            }         
        }

        public static SQLiteConnection openConnection()
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Data Source=ToDoListDatabase.sqlite;Version=3;");
            dbConnection.Open();
            return dbConnection;
        }

        public static void createTables(SQLiteConnection dbConnection)
        {
            createUserTable(dbConnection);
            createToDoListTable(dbConnection);
            createTaskTable(dbConnection);
            createContact(dbConnection);
            createTaskContactTable(dbConnection);
        }

        private static void createUserTable(SQLiteConnection dbConnection)
        {
            string sql = "CREATE TABLE IF NOT EXISTS User (id INTEGER PRIMARY KEY AUTOINCREMENT, username VARCHAR(255) NOT NULL, password VARCHAR(225) NOT NULL)";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            command.ExecuteNonQuery();
        }

        public static void createToDoListTable(SQLiteConnection dbConnection)
        {
            string sql = "CREATE TABLE IF NOT EXISTS ToDoList (id INTEGER PRIMARY KEY AUTOINCREMENT, title VARCHAR(255) NOT NULL, userId INTEGER, FOREIGN KEY(userId) REFERENCES User(id))";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            command.ExecuteNonQuery();
        }

        public static void createTaskTable(SQLiteConnection dbConnection)
        {
            string sql = "CREATE TABLE IF NOT EXISTS Task (id INTEGER PRIMARY KEY AUTOINCREMENT, title VARCHAR(255) NOT NULL, startDate DATE NOT NULL, enddate DATE NOT NULL, toDoListId INT, priority INT NOT NULL, taskFinished boolean, description VARCHAR(1024), FOREIGN KEY(toDoListId) REFERENCES ToDoList(id))";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            command.ExecuteNonQuery();
        }

        public static void createContact(SQLiteConnection dbConnection)
        {
			string sql = "CREATE TABLE IF NOT EXISTS Contact (id INTEGER PRIMARY KEY AUTOINCREMENT, firstName VARCHAR(50) NOT NULL, surname VARCHAR(50) NOT NULL, phone VARCHAR(50), email VARCHAR(100) NOT NULL, street VARCHAR(50) NOT NULL, city VARCHAR(50) NOT NULL, postalCode VARCHAR(5) NOT NULL, picturePath VARCHAR(250), userId INTEGER NOT NULL, FOREIGN KEY(userId) REFERENCES User(id))";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            command.ExecuteNonQuery();
        }

        public static void createTaskContactTable(SQLiteConnection dbConnection)
        {
            string sql = "CREATE TABLE IF NOT EXISTS TaskContact (id INTEGER PRIMARY KEY AUTOINCREMENT,  taskId INTEGER, contactId INT , FOREIGN KEY(taskId) REFERENCES Task(id), FOREIGN KEY(contactId) REFERENCES Contact(id))";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            command.ExecuteNonQuery();
        }

    }
}
