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
    /// Interaction logic for ContactAdd.xaml
    /// </summary>
    public partial class ContactAdd : Window
    {
		int userId, contactId;
		ApplicationService _service;

		public ContactAdd(Entities.Contact selectedContact, int userId)
		{
			this.userId = userId;
			_service = new ApplicationService();
			InitializeComponent();

			if (selectedContact != null)
			{
				FirstName.Text = selectedContact.FirstName;
				Surname.Text = selectedContact.Surname;
				Phone.Text = selectedContact.Phone;
				Email.Text = selectedContact.Email;
				Street.Text = selectedContact.Street;
				City.Text = selectedContact.City;
				Postalcode.Text = selectedContact.Postalcode;
				PicturePath.Text = selectedContact.PicturePath;
				contactId = selectedContact.Id;
				titleForAddEdit.Content = "Bearbeiten";
			}
			else
			{
				titleForAddEdit.Content = "Hinzufügen";
			}
		}

		private void Add_Click(object sender, RoutedEventArgs e)
		{
			if (FirstName.Text.Length + Surname.Text.Length == 0)
				errorMessageContact.Content = "Es muss mindestens der Vor- oder Nachname gegeben werden.";
			else
			{
				Entities.Contact newContact = new Entities.Contact() { FirstName = FirstName.Text, Surname = Surname.Text, Phone = Phone.Text, Street = Street.Text, City = City.Text, Email = Email.Text, Postalcode = Postalcode.Text, PicturePath = PicturePath.Text, UserId = this.userId, Id = contactId };
				_service.CreateOrUpdateContact(newContact);
				Back_Click(sender, e);
			}

		}

		private void Back_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
			ContactWindow nextpage = new ContactWindow(this.userId);
			nextpage.ShowDialog();
		}
	}
}
