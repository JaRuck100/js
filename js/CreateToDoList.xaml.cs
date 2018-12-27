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

namespace js
{
    /// <summary>
    /// Interaction logic for CreateToDoList.xaml
    /// </summary>
    public partial class CreateToDoList : Window
    {
        ApplicationService _service;
        int _userId;
		int _toDoListId;

        public CreateToDoList(int userId, int toDoListId)
        {
            this._userId = userId;
            InitializeComponent();
			_service = new ApplicationService();

			if (toDoListId != 0)
			{
				ToDoList toDoList = _service.GetToDoById(toDoListId);
				
				Title.Text = toDoList.Title;
				_toDoListId = toDoList.Id;
				this._userId = toDoList.UserId;

				CreateUpdateTitleToDo.Content = "Liste bearbeiten";
			}
			else
				CreateUpdateTitleToDo.Content = "Neue Liste hinzufügen";

		}

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string title = Title.Text;
            if (!_service.IsToDoListTitleAlreadyUsed(title))
            {
                _service.CreateOrUpdateToDoList(new ToDoList() { Title = title, UserId = _userId, Id = _toDoListId });
                this.Close();
				Back_Click(sender, e);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
			ToDoListWindow nextpage = new ToDoListWindow(_userId);
            nextpage.ShowDialog();
        }
    }
}
