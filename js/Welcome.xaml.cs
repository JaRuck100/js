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
    /// Interaktionslogik für Welcome.xaml
    /// </summary>
    public partial class Welcome : Window
    {
        public Welcome()
        {
            InitializeComponent();
        }

        private void ToDo_Click(object sender, RoutedEventArgs e)
        {
            ToDoList nextpage = new ToDoList();
            nextpage.ShowDialog();
        }

        private void Contact_Click(object sender, RoutedEventArgs e)
        {
            Contact nextpage = new Contact();
            nextpage.ShowDialog();

        }
    }
}
