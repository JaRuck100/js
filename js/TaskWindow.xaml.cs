using js.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Task = js.Entities.Task;

namespace js
{
	/// <summary>
	/// Interaktionslogik für TaskWindow.xaml
	/// </summary>
	public partial class TaskWindow : Window
	{

		int _toDoListId;
		int _taskId;
		int _userId;
		ApplicationService _service;
		public List<Contact> _selectedContacts;
		List<Contact> _contatsToRemove;

		public TaskWindow(int toDoListId, int taskId, int userId)
		{
			InitializeComponent();
			_toDoListId = toDoListId;
			_taskId = taskId;
			_service = new ApplicationService();
			_userId = userId;
			_selectedContacts = new List<Contact>();
			_contatsToRemove = new List<Contact>();


			if (taskId == 0)
			{
				Textlabel.Content = "Neuer Task";
			}
			else
			{ 
				Textlabel.Content = "Task bearbeiten";
				Task task = _service.GetTaskById(_taskId);
				task.TaskFininshed = _service.GetBoolFromTask(_taskId);

				TaskTitle.Text = task.Title;
				StartDate.Text = task.StartDate.ToString("d");
				EndDate.Text = task.EndDate.ToString("d");
				Priority.Text = task.Priority.ToString();
				TaskDescription.Text = task.Description;
				TaskFinished.IsChecked = task.TaskFininshed;

				var list = _service.GetContactsByTaskId(_taskId);

				foreach (var item in list)
				{
					_selectedContacts.Add(item);
					SelectedContacts.Items.Add(string.Format("{0} {1}", item.FirstName, item.Surname));
				}
				
			}
		}

		private void Abort_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
			ToDoListWindow nextpage = new ToDoListWindow(_userId);
			nextpage.ShowDialog();

		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			int newInt;
		     _service.CreateOrUpdateTask(new Task()
			{
				Title = TaskTitle.Text,
				StartDate = DateTime.Parse(StartDate.Text),
				EndDate = DateTime.Parse(EndDate.Text),
				Priority = int.TryParse(Priority.Text, out newInt) ? newInt : 0,
				Description = TaskDescription.Text,
				TaskFininshed = TaskFinished.IsChecked.Value,
				ToDoListId = _toDoListId,
				Id = _taskId
			});
			
            foreach(Contact contact in _selectedContacts)
            {
				if (contact.Id != 0)
				{
					_service.CreateTaskContact(_taskId, contact.Id);
				}
            }
			
            Abort_Click(sender, e);
		}

        private void Select_Contacts_Click(object sender, RoutedEventArgs e)
        {      
            ContactSelect nextpage = new ContactSelect(this, _userId);
            nextpage.ShowDialog();
        }

		private void Delete_Select_Contacts_Click(object sender, RoutedEventArgs e)
		{

		}


		private void SelectedContacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}
	}
}
