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
    /// Interaction logic for ContactEdit.xaml
    /// </summary>
    public partial class ContactEdit : Window
    {
        ApplicationService _service;
        int userId;
        int contactId;

        public ContactEdit(Contacts selectedContact, int userId)
        {
            InitializeComponent();
            this.userId = userId; 
            _service = new ApplicationService();
            FirstName.Text = selectedContact.FirstName;
            Surname.Text = selectedContact.Surname;
            Phone.Text = selectedContact.Phone;
            Email.Text = selectedContact.Email;
            Street.Text = selectedContact.Street;
            City.Text = selectedContact.City;
            Postalcode.Text = selectedContact.Postalcode;
            PicturePath.Text = selectedContact.PicturePath;
            contactId = selectedContact.Id;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _service = new ApplicationService();
            Contacts newContact = new Contacts() { FirstName = FirstName.Text, Surname = Surname.Text, Phone = Phone.Text, Street = Street.Text, City = City.Text, Email = Email.Text, Postalcode = Postalcode.Text, PicturePath = PicturePath.Text, UserId = this.userId, Id = contactId};
            _service.updateContactTable(newContact);
            this.Close();
            Back_Click(sender, e);

        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {          
            Contact nextpage = new Contact(this.userId);
            nextpage.ShowDialog();
            this.Close();
        }
    }
}
