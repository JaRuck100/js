using Dapper;
using js.Entities;
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

		private SQLiteConnection GetSQLiteConnection()
		{
			return new SQLiteConnection("Data Source=ToDoListDatabase.sqlite;Version=3;");
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
			string sql = String.Format("SELECT Count({0}) AS Count FROM {1} WHERE {0} = '{2}'", collumName, table, value);
			SQLiteCommand command = new SQLiteCommand(sql, conn);
			SQLiteDataReader reader = command.ExecuteReader();
			while (reader.Read())
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

		internal ToDoList GetToDoById(int toDoListId)
		{
			string sql = "Select * from ToDoList where id = @ToDoListId";

			using (var conn = GetSQLiteConnection())
			{
				return conn.Query<ToDoList>(sql, new { ToDoListId = toDoListId }).FirstOrDefault();
			}

		}

		public bool IsUsernameAlreadyUsed(string username, SQLiteConnection conn)
		{
			return IsValueAlreadyUsed(username, "User", "username", conn);
		}

		public bool CreateUser(string username, string password)
		{

			string sql = "SELECT * from User where username = @Username";
			int userListCount = 0;

			try
			{
				using (var conn = GetSQLiteConnection())
				{
					userListCount = conn.Query<User>(sql, new { Username = username }).ToList().Count;
				}

				if (userListCount != 0)
					return false;

				string insertSql = "insert into User ( username , password ) values (@Username, @Password)";
				using (var conn = GetSQLiteConnection())
				{
					conn.Execute(insertSql, new { Username = username, Password = sha1(password) });
				}
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		public User CheckUser(string username, string password)
		{
			string sql = "Select * from User where username = @Username AND password = @Password";
			var verpassword = sha1(password);

			using (var conn = GetSQLiteConnection())
			{
				return conn.Query<User>(sql, new { Username = username, Password = verpassword }).FirstOrDefault();
			}
		}

		public bool IsToDoListTitleAlreadyUsed(string title)
		{
			var conn = DatabaseConnection.openConnection();
			return IsValueAlreadyUsed(title, "ToDoList", "Title", conn);
		}

		public List<ToDoList> GetToDoListsByUserId(int userId)
		{
			string sql = "SELECT * FROM ToDoList WHERE userId = @UserId";

			using (var con = GetSQLiteConnection())
			{
				return con.Query<ToDoList>(sql, new { UserId = userId }).ToList();
			}


			/*
			Dictionary<int, string> toDoLists = new Dictionary<int, string>();
			var conn = DatabaseConnection.openConnection();
			
			SQLiteCommand command = new SQLiteCommand(sql, conn);
			command.ExecuteNonQuery();
			SQLiteDataReader reader = command.ExecuteReader();
			while (reader.Read())
			{
				toDoLists.Add(reader.GetInt32(0), reader.GetString(1));
			}
			conn.Close();
			return toDoLists;*/
		}

		public List<Task> GetTaksByToDoListId(int toDoListId)
		{
			string sql = "SELECT * FROM Task WHERE toDoListId = @ToDoListId";

			using (var conn = GetSQLiteConnection())
			{
				return conn.Query<Task>(sql, new { ToDoListId = toDoListId }).ToList();
			}

			/*List<Task> tasks = new List<Task>();
			var conn = DatabaseConnection.openConnection();
		
			SQLiteCommand command = new SQLiteCommand(sql, conn);
			command.ExecuteNonQuery();
			SQLiteDataReader reader = command.ExecuteReader();
			while (reader.Read())
			{
				tasks.Add(new Task() { id = reader.GetInt32(0), Title = reader.GetString(1), StartDate = DateTime.Parse(reader.GetString(2)), EndDate = DateTime.Parse(reader.GetString(3)), Priority = reader.GetInt32(5), TaskFininshed = reader.GetBoolean(6) });
			}
			conn.Close();
			return tasks;*/
		}

		public List<Entities.Contact> GetContactsByUserId(int userId)
		{
			string sql = "SELECT * FROM Contact WHERE userId = @UserId";
			List<Entities.Contact> contacts = new List<Entities.Contact>();

			using (var conn = GetSQLiteConnection())
			{
				contacts = conn.Query<Contact>(sql, new { UserId = userId }).ToList();
			}
			return contacts;
		}

		public Task GetTaskById(int taskId)
		{
			Task task = new Task();
			var conn = DatabaseConnection.openConnection();
			string sql = String.Format("SELECT * FROM Task WHERE id = {0}", taskId);
			SQLiteCommand command = new SQLiteCommand(sql, conn);
			command.ExecuteNonQuery();
			SQLiteDataReader reader = command.ExecuteReader();
			while (reader.Read())
			{
				task = new Task() { id = taskId, Title = reader.GetString(1), StartDate = DateTime.Parse(reader.GetString(2)), EndDate = DateTime.Parse(reader.GetString(3)), Priority = reader.GetInt32(5), Description = (reader["description"] == null ? " " : reader["description"].ToString()), TaskFininshed = reader.GetInt32(6) == 1 };
			}
			return task;
		}

		public List<string> GetContactNameByTaskId(int taskId)
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
			string sql = String.Format("UPDATE Task SET title = '{0}', startDate = '{1}', enddate = '{2}', priority = {3}, taskFinished = {4}, description = {5} WHERE id = {6}", alteredTask.Title, alteredTask.StartDate.ToString("d"), alteredTask.EndDate.ToString("d"), alteredTask.Priority, alteredTask.TaskFininshed, alteredTask.Description, alteredTask.id);
			SQLiteCommand command = new SQLiteCommand(sql, conn);
			command.ExecuteNonQuery();
			conn.Close();
		}


		public void CreateOrUpdateToDoList(ToDoList toDoList)
		{
			string sql = "INSERT INTO ToDoList (title, userId) VALUES(@Title, @UserId)";
			string sqlUpdate = "Update ToDoList SET title = @Title where id = @Id";
			using (var conn = GetSQLiteConnection())
			{
				if (toDoList.Id == 0)
					conn.Execute(sql, toDoList);
				else
					conn.Execute(sqlUpdate, toDoList);
			}
		}

		public void CreateTask(Task task)
		{
			string sql = "INSERT INTO Task (title, startDate, enddate, toDoListId, priority, taskFinished, description) VALUES (@Title, @StartDate, @EndDate, @ToDoListId, @Priority, @TaskFininshed, @Description)";

			using (var conn = GetSQLiteConnection())
			{
				conn.Execute(sql, task);
			}
		}

		public void CreateOrUpdateContact(Entities.Contact contact)
		{
			string sqlAdd = "INSERT INTO Contact (firstName, surname, phone, email, street, city, postalCode, picturePath, userId) VALUES (@Firstname, @Surname, @Phone, @Email, @Street, @City, @Postalcode, @PicturePath, @UserId)";
			string sqlUpdate = "UPDATE Contact SET firstName = @Firstname, surname = @Surname, phone = @Phone', email = @Email, street = @Street, city = @City, postalCode = @Postalcode, picturePath = @PicturePath WHERE id = @Id";

			using (var conn = GetSQLiteConnection())
			{
				if (contact.Id == 0)
					conn.Execute(sqlAdd, contact);
				else
					conn.Execute(sqlUpdate, contact);
			}
		}

		public void DeleteContact(int contactId)
		{
			string sqlTaskFromContact = "DELETE FROM TaskContact WHERE contactId = @ContactId";
			string sqlContact = "DELETE FROM Contact WHERE id = @ContactId";

			using (var conn = GetSQLiteConnection())
			{
				conn.Execute(sqlTaskFromContact, new { ContactId = contactId });
				conn.Execute(sqlContact, new { ContactId = contactId });
			}
		}

		public void DeleteTask(int taskId)
		{
			var sql2 = "DELETE FROM TaskContact WHERE taskId =@TaskId";
			var sql = "DELETE FROM Task WHERE id = @TaskId";
			using (var con = GetSQLiteConnection())
			{
				con.Execute(sql2, new { TaskId = taskId });
				con.Execute(sql, new { TaskId = taskId });
			}

		}

		public void DeleteToDoList(int toDoListId)
		{
			/// Holt die ID der To Do list anhand des Titels, und löscht dann alle zur Todo Liste gehörigen Tasks 
			/// und die Zuweisungen der Kontakte zu den Tasks   
			var conn = DatabaseConnection.openConnection();

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
