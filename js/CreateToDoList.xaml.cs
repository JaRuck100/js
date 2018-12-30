using js.Entities;
using js.Service;
using System.Windows;

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

			if (title == string.Empty)
			{
				errorMessageContact.Content = "Der Titel muss gegeben sein.";
			}
			else if (!_service.IsToDoListTitleAlreadyUsed(title))
			{
				_service.CreateOrUpdateToDoList(new ToDoList() { Title = title, UserId = _userId, Id = _toDoListId });
				this.Close();
				Back_Click(sender, e);
			}
			else if (_service.IsToDoListTitleAlreadyUsed(title))
				errorMessageContact.Content = "Listenname ist schon vergeben.";
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
			ToDoListWindow nextpage = new ToDoListWindow(_userId);
            nextpage.ShowDialog();
        }
    }
}
