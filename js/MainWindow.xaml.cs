
using js.Service;
using System.Windows;

namespace js
{
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
    {
		ApplicationService _service;
		public MainWindow()
		{
			InitializeComponent();
			DatabaseConnection.Seed();
			_service = new ApplicationService();
		}

		private void Login_Click(object sender, RoutedEventArgs e)
		{
			var username = usernameText.Text;
			var password = passwordText.Password;
			var passwortUsernameRight = false;

#if DEBUG
			username = "sunny";
			password = "123";
#endif
			if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
			{
				var user = _service.CheckUser(username, password);

				if (user != null)
					passwortUsernameRight = true;

				if (passwortUsernameRight)
				{
					Welcome nextpage = new Welcome(user.Id);
					nextpage.Show();
					this.Close();
				}
				else
				{
					errorMessageText.Content = "Username oder Passwort falsch.";
				}
			}
			else { errorMessageText.Content = "Username und Passwort angeben!"; }
		}

		private void Register_Click(object sender, RoutedEventArgs e)
		{
			var username = usernameText.Text;
			var password = passwordText.Password;

			if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
			{

				bool isCreated = _service.CreateUser(username, password);

				if (!isCreated)
					errorMessageText.Content = "User existiert schon!";
				else
					errorMessageText.Content = "User wurde erstellt!";
			}
			else { errorMessageText.Content = "Username und Passwort angeben!"; }
		}
	}
}
