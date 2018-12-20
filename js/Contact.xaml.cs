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
    /// Interaktionslogik für Contact.xaml
    /// </summary>
    public partial class Contact : Window
    {
        ApplicationService _service;
        int userId;
        Contacts selectedContact;

        public Contact(int userId)
        {
            InitializeComponent();
            _service = new ApplicationService();
            this.userId = userId;
            List<Contacts> contacts = _service.getContactsByUserId(userId);
            foreach (Contacts contact in contacts)
            {
                this.contactView.Items.Add(contact);
            }
            
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (selectedContact != null)
            {
                ContactEdit nextpage = new ContactEdit(selectedContact, userId);
                nextpage.Show();
                this.Close();
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            ContactAdd nextpage = new ContactAdd(userId);
            nextpage.Show();
            this.Close();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedContact != null)
            {
                _service.DeleteContact(selectedContact.Id);
                Contact nextpage = new Contact(userId);
                nextpage.Show();
                this.Close();
            }

        }

        private void selectedElement(object sender, SelectionChangedEventArgs e)
        {
            selectedContact = (Contacts) e.AddedItems[0];
        }
    }
}
