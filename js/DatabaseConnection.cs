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

                /*string sql = "insert into User (id ,username , password ) values (1, 'sunny', '"+sha1("blub")+"')";
                SQLiteCommand command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();*/

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
            //createTables(dbConnection);
            return dbConnection;
        }

        ////string sql = "create table highscores (name varchar(20), score int)";
        //string sql = "insert into highscores (name, score) values ('Me', 3000)";
        //SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
        //command.ExecuteNonQuery();
        //string sql1 = "select * from highscores order by score desc";
        //SQLiteCommand command1 = new SQLiteCommand(sql1, m_dbConnection);
        //SQLiteDataReader reader = command1.ExecuteReader();
        //while (reader.Read())
        //    Console.WriteLine("Name: " + reader["name"] + "\tScore: " + reader["score"]);

        public static void createTables(SQLiteConnection dbConnection)
        {
            createUserTable(dbConnection);
            createToDoListTable(dbConnection);
            createTaskTable(dbConnection);
            createContacts(dbConnection);
            createTaskContactTable(dbConnection);
        }

        private static void createUserTable(SQLiteConnection dbConnection)
        {
            string sql = "CREATE TABLE IF NOT EXISTS User (id INTEGER PRIMARY KEY AUTOINCREMENT, username VARCHAR(255), password VARCHAR(225))";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            command.ExecuteNonQuery();
        }

        public static void createToDoListTable(SQLiteConnection dbConnection)
        {
            string sql = "CREATE TABLE IF NOT EXISTS ToDoList (id INTEGER PRIMARY KEY AUTOINCREMENT, title VARCHAR(255))";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            command.ExecuteNonQuery();
        }

        public static void createTaskTable(SQLiteConnection dbConnection)
        {
            string sql = "CREATE TABLE IF NOT EXISTS Task (id INTEGER PRIMARY KEY AUTOINCREMENT, title VARCHAR(255), startDate DATE, enddate DATE, toDoListId INT, priority INT, taskFinished INT DEFAULT 0)";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            command.ExecuteNonQuery();
        }

        public static void createContacts(SQLiteConnection dbConnection)
        {
            string sql = "CREATE TABLE IF NOT EXISTS ContactTest (id INTEGER PRIMARY KEY AUTOINCREMENT, firstName VARCHAR(50), surname VARCHAR(50), phone VARCHAR(50), email VARCHAR(100), street VARCHAR(50), houseNumer VARCHAR(5), city VARCHAr(50), postalCode VARCHAR(5), picturePath VARCHAR(250))";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            command.ExecuteNonQuery();
        }

        public static void createTaskContactTable(SQLiteConnection dbConnection)
        {
            string sql = "CREATE TABLE IF NOT EXISTS TaskContact (taskId INTEGER, contactId INT, PRIMARY KEY (taskId, contactId))";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            command.ExecuteNonQuery();
        }

    }
}
