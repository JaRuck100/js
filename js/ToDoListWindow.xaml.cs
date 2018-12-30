using FontAwesome.WPF;
using js.Entities;
using js.Service;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Task = js.Entities.Task;
namespace js
{
	/// <summary>
	/// Interaktionslogik für ToDoList.xaml
	/// </summary>
	public partial class ToDoListWindow : Window
	{
		int _userId;
		ApplicationService _service;
		int _taskId;
		int _toDoId;
		int _iconWidth = 8;
		int _iconHeight = 8;

		public ToDoListWindow(int userId)
		{
			InitializeComponent();
			_userId = userId;
			_service = new ApplicationService();
			List<ToDoList> toDoLists = _service.GetToDoListsByUserId(userId);

			ToDoListList.Items.Clear();

			foreach (var toDoListItem in toDoLists)
			{
				//Zusammenbau eines Items
				StackPanel toDoNonCheck = new StackPanel() { Orientation = Orientation.Horizontal };
				ImageAwesome image = new ImageAwesome() { Icon = FontAwesomeIcon.Close, Width= _iconWidth, Height= _iconHeight, HorizontalAlignment  = HorizontalAlignment.Center};
				TextBlock textBox = new TextBlock() { Text = toDoListItem.Title+" " };
				toDoNonCheck.Children.Add(textBox);
				toDoNonCheck.Children.Add(image);

				StackPanel toDoCheck = new StackPanel() { Orientation = Orientation.Horizontal };
				image = new ImageAwesome() { Icon = FontAwesomeIcon.Check, Width = _iconWidth, Height = _iconHeight , HorizontalAlignment = HorizontalAlignment.Center };
				textBox = new TextBlock() { Text = toDoListItem.Title + " " };
				toDoCheck.Children.Add(textBox);
				toDoCheck.Children.Add(image);


				TreeViewItem toDoListTitle = new TreeViewItem();
				toDoListTitle.Header = toDoNonCheck;
				toDoListTitle.Name = "toDoList" + toDoListItem.Id.ToString();
				ToDoListList.Items.Add(toDoListTitle);
				List<Task> tasks = _service.GetTaksByToDoListId(toDoListItem.Id);
				int finishedCount = 0;

				foreach (var task in tasks)
				{


					StackPanel taskNonCheck = new StackPanel() { Orientation = Orientation.Horizontal };
					image = new ImageAwesome() { Icon = FontAwesomeIcon.Close, Width = _iconWidth, Height = _iconHeight , HorizontalAlignment = HorizontalAlignment.Center };
					textBox = new TextBlock() { Text = task.Title + " " };
					taskNonCheck.Children.Add(textBox);
					taskNonCheck.Children.Add(image);

					StackPanel taskCheck = new StackPanel() { Orientation = Orientation.Horizontal };
					image = new ImageAwesome() { Icon = FontAwesomeIcon.Check, Width = _iconWidth, Height = _iconHeight, HorizontalAlignment = HorizontalAlignment.Center };
					textBox = new TextBlock() { Text = task.Title + " " };
					taskCheck.Children.Add(textBox);
					taskCheck.Children.Add(image);


					TreeViewItem taskTitle = new TreeViewItem();
					taskTitle.Header = taskNonCheck;

					if (_service.GetBoolFromTask(task.Id))
					{
						taskTitle.Header = taskCheck;
						finishedCount++;
					}

					taskTitle.Name = "task" + task.Id.ToString();
					toDoListTitle.Items.Add(taskTitle);
				}
				if(finishedCount == tasks.Count && finishedCount != 0)
					toDoListTitle.Header = toDoCheck;

			}
		}

		private void Create_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
			CreateToDoList nextpage = new CreateToDoList(_userId, 0);
			nextpage.ShowDialog();

		}

		private void Edit_Click(object sender, RoutedEventArgs e)
		{
			
			if (_toDoId == 0)
				errorMessageContact.Content = "Eine Todo-Liste muss zum Beareiten ausgewählt werden.";
			else
			{
				this.Close();
				CreateToDoList nextpage = new CreateToDoList(_userId, _toDoId);
				nextpage.ShowDialog();
			}
		}

		private void selectedElement(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var item = (TreeViewItem)e.NewValue;
			if (item.Name.Contains("toDoList"))
			{
				var x = item.Name.Replace("toDoList", "");

				_toDoId = int.Parse(x);
			}
			else if (item.Name.Contains("task"))
			{
				var x = item.Name.Replace("task", "");
				_taskId = int.Parse(x);
			}
		}

		private void Delete_Click(object sender, RoutedEventArgs e)
		{
			if (_toDoId != 0)
			{
				this.Close();
				_service.DeleteToDoList(_toDoId);
				ToDoListWindow nextpage = new ToDoListWindow(_userId);
				nextpage.ShowDialog();
			}
			else if (_taskId != 0)
			{
				this.Close();
				_service.DeleteTask(_taskId);
				ToDoListWindow nextpage = new ToDoListWindow(_userId);
				nextpage.ShowDialog();
			}
			else
			{
				errorMessageContact.Content = "Ein Element muss gewählt sein.";
			}
		}

		private void CreateTask_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
			TaskWindow nextpage = new TaskWindow(_toDoId, 0, _userId);
			nextpage.ShowDialog();
		}

		private void EditTask_Click(object sender, RoutedEventArgs e)
		{
			
			if (_taskId == 0)
				errorMessageContact.Content = "Ein Task muss zum Bearbeiten ausgewählt werden.";
			else
			{
				this.Close();
				TaskWindow nextpage = new TaskWindow(_toDoId, _taskId, _userId);
				nextpage.ShowDialog();
			}
		}
	}
}
