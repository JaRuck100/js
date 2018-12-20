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
        int userId;
        ApplicationService _service;

        public ContactAdd(int userId)
        {
            this.userId = userId;
            
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            _service = new ApplicationService();
            Contacts newContact = new Contacts() { FirstName = FirstName.Text, Surname = Surname.Text, Phone = Phone.Text, Street = Street.Text, City = City.Text, Email = Email.Text, Postalcode = Postalcode.Text, PicturePath = PicturePath.Text, UserId = this.userId};
            _service.CreateContact(newContact);
            Back_Click(sender, e);

        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Contact nextpage = new Contact(this.userId);
            nextpage.ShowDialog();
        }
    }
}
