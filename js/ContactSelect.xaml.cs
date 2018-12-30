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
			var userToRemove = new List<Contact>();
			var userList = new List<Contact>();
			var userNotUsedList = new List<Contact>();

			if (_taskWindow._selectedContacts.Count != 0)
			{
				foreach (Contact item in this.contactSelectView.SelectedItems)
				{
					userList.Add(item);
					userNotUsedList.Add(item);
					foreach (var contact in _taskWindow._selectedContacts)
					{
						if (contact.Id == item.Id)
						{
							userToRemove.Add(contact);
							continue;
						}
					}
				}

				foreach (var item in userList)
				{
					foreach (var contact in userToRemove)
					{
						if (contact.Id == item.Id)
						{
							userNotUsedList.Remove(item);
						}
					}
				}
				foreach (Contact item in userNotUsedList)
				{
					_taskWindow._selectedContacts.Add(item);
					_taskWindow.SelectedContacts.Items.Add(string.Format("{0} {1}", item.FirstName, item.Surname));
				}

			}
			else
			{
				foreach (Contact item in this.contactSelectView.SelectedItems)
				{
					_taskWindow._selectedContacts.Add(item);
					_taskWindow.SelectedContacts.Items.Add(string.Format("{0} {1}", item.FirstName, item.Surname));
				}
			}
		}

	}
}
