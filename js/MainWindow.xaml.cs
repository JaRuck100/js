
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
