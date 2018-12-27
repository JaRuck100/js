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
        int userId;
        ApplicationService _service;
        string objectTitle;
        string objectName;
        bool isToDoList = false;
        bool isTask = false;

		int selectedToDoId;

		public ToDoListWindow(int userId)
        {
            InitializeComponent();
            this.userId = userId;
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
                    taskTitle.Name = "task" + task.id.ToString();
                    toDoListTitle.Items.Add(taskTitle);                  
                }
            }      
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
			this.Close();
            CreateToDoList nextpage = new CreateToDoList(userId, 0);
            nextpage.ShowDialog();
            
        }

		private void Edit_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
			CreateToDoList nextpage = new CreateToDoList(userId, selectedToDoId);
			nextpage.ShowDialog();
		}

		private void selectedElement(object sender, RoutedPropertyChangedEventArgs<object> e )
		{
			var item = (TreeViewItem)e.NewValue;
			if (item.Name.Contains("toDoList"))
			{
				var x = item.Name.Replace("toDoList", "");

				selectedToDoId = int.Parse(x);
			}
		}

		private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if(isToDoList)
            {
                this.Close();
                int toDoListId = Convert.ToInt32(objectName.Replace("doDoList", "")); 
                _service.DeleteToDoList(toDoListId);
				ToDoListWindow nextpage = new ToDoListWindow(userId);
                nextpage.ShowDialog();
            }
            else if (isTask)
            {
                this.Close();
                int taskId = Convert.ToInt32(objectName.Replace("task", ""));
                _service.DeleteTask(taskId);
				ToDoListWindow nextpage = new ToDoListWindow(userId);
                nextpage.ShowDialog();
            }
        }

        private void Object_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectetItem = (TreeViewItem) e.OriginalSource;
            objectTitle = (string) selectetItem.Header;
            objectName = selectetItem.Name;
            isToDoList = objectName.Contains("doDoList");
            isTask = objectName.Contains("task");
            if (isTask)
            {
                int taskId = Convert.ToInt32(objectName.Replace("task", ""));
                Task task =_service.GetTaskById(taskId);
                Title.Text = task.Title;
                StartDate.Text = task.StartDate.ToString("d");
                EndDate.Text = task.EndDate.ToString("d");
                Priority.Text = task.Priority.ToString();
                Description.Text = task.Description;
                TaskFinished.IsChecked = task.TaskFininshed;
            }
        }

		private void CreateTask_Click(object sender, RoutedEventArgs e)
        {
            if (isToDoList)
            {
                this.Close();
                bool isCecked = false;
                if (TaskFinished.IsChecked == true)
                {
                    isCecked = true;
                }
                int toDoListId = Convert.ToInt32(objectName.Replace("doDoList", ""));
                _service.CreateTask(new Task() { Title = Title.Text, StartDate = DateTime.Parse(StartDate.Text), EndDate = DateTime.Parse(EndDate.Text), Priority = Int32.Parse(Priority.Text), Description = Description.Text, TaskFininshed = isCecked, ToDoListId = toDoListId });
				// TO DO: Add selected Cotacts to TaskContact Table
				ToDoListWindow nextpage = new ToDoListWindow(userId);
                nextpage.ShowDialog();
            }


        }

	
	}
}
