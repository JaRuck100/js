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

		internal ToDoList GetToDoById(int toDoListId)
		{
			string sql = "Select * from ToDoList where id = @ToDoListId";

			using (var conn = GetSQLiteConnection())
			{
				return conn.Query<ToDoList>(sql, new { ToDoListId = toDoListId }).FirstOrDefault();
			}
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
			var sql = "Select * from ToDoList where title = @Title";

			using (var conn = GetSQLiteConnection())
			{
				var list = conn.Query<ToDoList>(sql, new { Title = title }).ToList();

				if (list.Count != 0)
					return true;
			}
			return false;
		}

		public List<ToDoList> GetToDoListsByUserId(int userId)
		{
			string sql = "SELECT * FROM ToDoList WHERE userId = @UserId";

			using (var con = GetSQLiteConnection())
			{
				return con.Query<ToDoList>(sql, new { UserId = userId }).ToList();
			}
			
		}

		public List<Task> GetTaksByToDoListId(int toDoListId)
		{
			string sql = "SELECT * FROM Task WHERE toDoListId = @ToDoListId";

			using (var conn = GetSQLiteConnection())
			{
				return conn.Query<Task>(sql, new { ToDoListId = toDoListId }).ToList();
			}
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
			var sql = "SELECT * FROM Task WHERE id = @TaskId";
			using (var con = GetSQLiteConnection())
			{
				return con.Query<Task>(sql, new { TaskId = taskId }).FirstOrDefault();
			}
		}

		public bool GetBoolFromTask(int taskId)
		{
			var sql = "SELECT taskFinished FROM Task WHERE id = @TaskId";
			using (var con = GetSQLiteConnection())
			{
				return con.Query<Boolean>(sql, new { TaskId = taskId }).FirstOrDefault();
			}
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

		public void CreateOrUpdateTask(Task task)
		{
			string sql = "INSERT INTO Task (title, startDate, enddate, toDoListId, priority, taskFinished, description) VALUES (@Title, @StartDate, @EndDate, @ToDoListId, @Priority, @TaskFininshed, @Description)";
			string sqlUpdate = "UPDATE Task SET title = @Title, startDate = @StartDate, enddate = @EndDate, priority = @Priority, taskFinished = @TaskFininshed, description = @Description WHERE id = @Id";

			using (var conn = GetSQLiteConnection())
			{
				if (task.Id ==0)
					conn.Execute(sql, task);
				else
					conn.Execute(sqlUpdate, task);
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
			var selectAllTasks = "Select id From Task where toDoListId = @ToDoListId";
			var sqlDeleteTaskContact = "DELETE FROM TaskContact WHERE taskId IN (@TaskIds)";
			var deletedTasks = "DELETE FROM Task WHERE toDoListId = @ToDoListId";
			var deleteToDo = "DELETE FROM ToDoList WHERE id = @ToDoListId";
			using (var conn = GetSQLiteConnection())
			{
				List<int> tasksIds = conn.Query<int>(selectAllTasks, new { ToDoListId = toDoListId }).ToList();
				conn.Execute(sqlDeleteTaskContact, new { TaskIds = tasksIds });
				conn.Execute(deletedTasks, new { ToDoListId = toDoListId });
				conn.Execute(deleteToDo, new { ToDoListId = toDoListId });
			}
		}
	}
}
