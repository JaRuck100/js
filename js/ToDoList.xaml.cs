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
    /// Interaktionslogik für ToDoList.xaml
    /// </summary>
    public partial class ToDoList : Window
    {
        int userId;
        ApplicationService _service;
        string toDoListTitle;

        public ToDoList(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            _service = new ApplicationService();
            Dictionary<int, string> toDoLists = _service.getToDoListsByUserId(userId);
            foreach (var list in toDoLists)
            {
                TreeViewItem toDoListTitle = new TreeViewItem();
                toDoListTitle.Header = list.Value;
                ToDoListList.Items.Add(toDoListTitle);
            }

        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            CreateToDoList nextpage = new CreateToDoList(userId);
            nextpage.ShowDialog();
            
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (toDoListTitle != String.Empty)
            {
                this.Close();
                string hh = toDoListTitle;
                _service.DeleteToDoList(toDoListTitle);
                ToDoList nextpage = new ToDoList(userId);
                nextpage.ShowDialog();
            }
    
        }

        private void ToDoListList_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectetItem = (TreeViewItem) e.OriginalSource;
            toDoListTitle = (string) selectetItem.Header;
        }

        private void CreateTask_Click(object sender, RoutedEventArgs e)
        {
            if (toDoListTitle != String.Empty)
            {
                this.Close();
                bool isCecked = false;
                if (TaskFinished.IsChecked == true)
                {
                    isCecked = true;
                }
                _service.CreateTask(new Task() { Title = Title.Text, StartDate = DateTime.Parse(StartDate.Text), EndDate = DateTime.Parse(EndDate.Text), Priority = Int32.Parse(Priority.Text), Description = Description.Text, TaskFininshed = isCecked }, toDoListTitle);
                // TO DO: Add selected Cotacts to TaskContact Table
                ToDoList nextpage = new ToDoList(userId);
                nextpage.ShowDialog();
            }

        }
        
    }
}
