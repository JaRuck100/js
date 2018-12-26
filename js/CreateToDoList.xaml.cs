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
        int userId;
        public CreateToDoList(int userId)
        {
            this.userId = userId;
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            _service = new ApplicationService();
            string title = Title.Text;
            if (!_service.IsToDoListTitleAlreadyUsed(title))
            {
                _service.CreateToDoList(new ToDoList() { Title = title, UserId = userId });
                this.Close();
                Back_Click(sender, e);
                
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
			ToDoListWindow nextpage = new ToDoListWindow(userId);
            nextpage.ShowDialog();

        }
    }
}
