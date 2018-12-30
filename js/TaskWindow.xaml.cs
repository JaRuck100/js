using js.Entities;
using js.Service;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
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
		Contact _contatToRemove;

		public TaskWindow(int toDoListId, int taskId, int userId)
		{
			InitializeComponent();
			_toDoListId = toDoListId;
			_taskId = taskId;
			_service = new ApplicationService();
			_userId = userId;
			_selectedContacts = new List<Contact>();

			if (taskId == 0)
			{
				Textlabel.Content = "Neuen Task hinzufügen";
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
				_selectedContacts.AddRange(_service.GetContactsByTaskId(_taskId));
				ReloadTaskContacts();
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

			if (StartDate.Text != string.Empty && EndDate.Text != string.Empty && TaskTitle.Text != string.Empty)
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

				foreach (Contact contact in _selectedContacts)
				{
					if (contact.Id != 0)
					{
						_service.CreateTaskContact(_taskId, contact.Id);
					}
				}

				Abort_Click(sender, e);
			}
			else {
				errorMessageContact.Content = "Titel, Startdatum und Enddatum müssen gegeben sein.";
			}
		}

		private void Select_Contacts_Click(object sender, RoutedEventArgs e)
		{
			ContactSelect nextpage = new ContactSelect(this, _userId);
			nextpage.Closed += ReloadTaskContactsHandler;
			nextpage.ShowDialog();

		}

		public void ReloadTaskContactsHandler(object sender, EventArgs e)
		{
			ReloadTaskContacts();
			ContactSelect nextpage = new ContactSelect(this, _userId);
			nextpage.Closed -= ReloadTaskContactsHandler;
		}

		private void Delete_Select_Contacts_Click(object sender, RoutedEventArgs e)
		{
			if (_contatToRemove != null && _contatToRemove.Id != 0)
			{
				_selectedContacts.Remove(_contatToRemove);
				ReloadTaskContacts();
			}
		}

		private void SelectedContacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_contatToRemove = (Contact)this.SelectedContacts.SelectedItem;

		}

		public void ReloadTaskContacts()
		{
			var list = _selectedContacts;
			
			SelectedContacts.ItemsSource = list;
			SelectedContacts.DisplayMemberPath = "Fullname";
			SelectedContacts.Items.Refresh();
		}
	}
}
