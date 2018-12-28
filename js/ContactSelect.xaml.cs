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
using Contact = js.Entities.Contact;

namespace js
{
    /// <summary>
    /// Interaktionslogik für ContactSelect.xaml
    /// </summary>
    public partial class ContactSelect : Window
    {
        int _userId;
        ApplicationService _service;
        TaskWindow _taskWindow;

        public ContactSelect(TaskWindow taskWindow, int userId)
        {
            InitializeComponent();
            _service = new ApplicationService();
            _userId = userId;
            _taskWindow = taskWindow;
            List<Contact> Contact = _service.GetContactsByUserId(userId);
            foreach (Contact contact in Contact)
            {
                this.contactSelectView.Items.Add(contact);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();          
        }

        private void Save_Selection (object sender, RoutedEventArgs e)
        {
            this.Close();
            var selectedElemts = this.contactSelectView.SelectedItems;
            foreach (Contact contact in selectedElemts)
            {
                _taskWindow.SelectedContacts.Items.Add(contact.FirstName + " " + contact.Surname);
                _taskWindow.ContactIds.Content += contact.Id.ToString() + " "; 
                
            }
            
        }
    }
}
