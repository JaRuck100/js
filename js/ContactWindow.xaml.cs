using js.Entities;
using js.Service;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace js
{
	/// <summary>
	/// Interaktionslogik für Contact.xaml
	/// </summary>
	public partial class ContactWindow : Window
    {
        ApplicationService _service;
		int userId;
        Contact selectedContact;

        public ContactWindow(int userId)
        {
            InitializeComponent();
            _service = new ApplicationService();
            this.userId = userId;
            List<Contact> Contact = _service.GetContactsByUserId(userId);
            foreach (Contact contact in Contact)
            {
                this.contactView.Items.Add(contact);
            }
            
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (selectedContact != null)
            {
				ContactAdd nextpage = new ContactAdd(selectedContact, userId);
				nextpage.Show();
				this.Close();
			}
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            ContactAdd nextpage = new ContactAdd(null, userId);
            nextpage.Show();
            this.Close();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedContact != null)
            {
                _service.DeleteContact(selectedContact.Id);
				ContactWindow nextpage = new ContactWindow(userId);
                nextpage.Show();
                this.Close();
            }

        }

        private void selectedElement(object sender, SelectionChangedEventArgs e)
        {
            selectedContact = (Contact) e.AddedItems[0];
        }
    }
}
