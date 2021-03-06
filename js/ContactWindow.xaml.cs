﻿using js.Entities;
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
            List<Contact> contacts = _service.GetContactsByUserId(userId);

			contactView.ItemsSource = contacts;

		}

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (selectedContact != null)
            {
				ContactAdd nextpage = new ContactAdd(selectedContact, userId);
				nextpage.Show();
				this.Close();
			}
			else
			{
				errorMessageContact.Content = "Ein Kontakt muss gewählt sein.";
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
			else
			{
				errorMessageContact.Content = "Ein Kontakt muss gewählt sein.";
			}

        }

        private void selectedElement(object sender, SelectionChangedEventArgs e)
        {
            selectedContact = (Contact) e.AddedItems[0];
        }
    }
}
