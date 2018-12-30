using System.Windows;

namespace js
{
	/// <summary>
	/// Interaktionslogik für Welcome.xaml
	/// </summary>
	public partial class Welcome : Window
    {
		int userId;

		public Welcome(int userId)
		{
			InitializeComponent();
			this.userId = userId;
		}

		private void ToDo_Click(object sender, RoutedEventArgs e)
		{
			ToDoListWindow nextpage = new ToDoListWindow(userId);
			nextpage.ShowDialog();
		}

		private void Contact_Click(object sender, RoutedEventArgs e)
		{
			ContactWindow nextpage = new ContactWindow(userId);
			nextpage.ShowDialog();
		}

		private void Logout_Click(object sender, RoutedEventArgs e)
		{
			MainWindow nextPage = new MainWindow();
			this.Close();
			nextPage.ShowDialog();
		}
	}
}
