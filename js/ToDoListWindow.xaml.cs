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
    /// Interaktionslogik für ToDoList.xaml
    /// </summary>
    public partial class ToDoListWindow : Window
    {
        int _userId;
        ApplicationService _service;
        string _objectTitle;
        string _objectName;
		int _taskId;
		int _toDoId;

		public ToDoListWindow(int userId)
        {
            InitializeComponent();
            _userId = userId;
            _service = new ApplicationService();
            List<ToDoList> toDoLists = _service.GetToDoListsByUserId(userId);

			ToDoListList.Items.Clear();

			foreach (var toDoListItem in toDoLists)
            {
                TreeViewItem toDoListTitle = new TreeViewItem();
                toDoListTitle.Header = toDoListItem.Title;
                toDoListTitle.Name = "toDoList" + toDoListItem.Id.ToString(); 
                ToDoListList.Items.Add(toDoListTitle);
                List<Task> tasks = _service.GetTaksByToDoListId(toDoListItem.Id);
                foreach (var task in tasks)
                {
                    TreeViewItem taskTitle = new TreeViewItem();
                    taskTitle.Header = task.Title;
                    taskTitle.Name = "task" + task.Id.ToString();
                    toDoListTitle.Items.Add(taskTitle);                  
                }
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
			this.Close();
			CreateToDoList nextpage = new CreateToDoList(_userId, _toDoId);
			nextpage.ShowDialog();
		}

		private void selectedElement(object sender, RoutedPropertyChangedEventArgs<object> e )
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
            if(_toDoId != 0)
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
        }

		private void CreateTask_Click(object sender, RoutedEventArgs e)
        {
				this.Close();
				TaskWindow nextpage = new TaskWindow(_toDoId, _taskId, _userId);
				nextpage.ShowDialog();
        }
	}
}
