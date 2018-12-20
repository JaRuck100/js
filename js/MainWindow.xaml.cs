
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
            DatabaseConnection.Seed();
            _service = new ApplicationService();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var username = usernameText.Text;
            var password = passwordText.Text;
            var passwortUsernameRight = false;
            var list = _service.CheckUser(username, password);

            if (list.Count == 1)
                passwortUsernameRight = true;
            if (passwortUsernameRight)
            {
                Welcome nextpage = new Welcome(list[0].Id);
                nextpage.Show();
                this.Close();
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var username = usernameText.Text;
            var password = passwordText.Text;
            _service.CreateUser(username, password);
        }
    }
}
