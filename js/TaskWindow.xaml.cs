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

		public TaskWindow(int toDoListId, int taskId, int userId)
		{
			InitializeComponent();
			_toDoListId = toDoListId;
			_taskId = taskId;
			_service = new ApplicationService();
			_userId = userId;

			if (taskId == 0)
			{
				Textlabel.Content = "Neuer Task";
			}
			else
			{ 
				Textlabel.Content = "Task bearbeiten";
				Task task = _service.GetTaskById(_taskId);

				TaskTitle.Text = task.Title;
				StartDate.Text = task.StartDate.ToString("d");
				EndDate.Text = task.EndDate.ToString("d");
				Priority.Text = task.Priority.ToString();
				TaskDescription.Text = task.Description;
				TaskFinished.IsChecked = task.TaskFininshed;
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

			Abort_Click(sender, e);
		}
	}
}
