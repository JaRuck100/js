using js.Entities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace js
{
	/// <summary>
	/// Interaction logic for ContactAdd.xaml
	/// </summary>
	public partial class ContactAdd : Window
	{
		int userId, contactId;
		ApplicationService _service;
		string path, picturename;
		string programmPath = @"c:\temp\";

		public ContactAdd(Entities.Contact selectedContact, int userId)
		{
			this.userId = userId;
			_service = new ApplicationService();
			InitializeComponent();

			if (selectedContact != null)
			{
				FirstName.Text = selectedContact.Firstname;
				Surname.Text = selectedContact.Surname;
				Phone.Text = selectedContact.Phone;
				Email.Text = selectedContact.Email;
				Street.Text = selectedContact.Street;
				City.Text = selectedContact.City;
				Postalcode.Text = selectedContact.Postalcode;
				contactId = selectedContact.Id;
				titleForAddEdit.Content = "Kontakt bearbeiten";
				ContactPicture.Source = new BitmapImage(new Uri(selectedContact.PicturePath));
			}
			else
			{
				titleForAddEdit.Content = "Kontakt hinzufügen";
			}
		}

		private void Add_Click(object sender, RoutedEventArgs e)
		{
			if (FirstName.Text.Length + Surname.Text.Length == 0)
				errorMessageContact.Content = "Es muss mindestens der Vor- oder Nachname gegeben werden.";
			else
			{
				string databasePath = string.Empty;
				string data = programmPath + picturename;
				int number = 1;
				int picterNumber = 0;
				if (path != string.Empty  && picturename != null)
				{
					if (!Directory.Exists(programmPath))
						Directory.CreateDirectory(programmPath);

					if (!File.Exists(data))
						File.Copy(path, data);
					else
					{
						var list = Directory.GetFiles(programmPath, picturename);
						if (list.Length == 1)
							number = 1;
						else
						{
							foreach (var item in list)
							{
								string pictureCopyName = item;
								pictureCopyName = pictureCopyName.Substring(pictureCopyName.LastIndexOf('_'));
								var pictureCopyNameNumber = pictureCopyName.Remove(0,1);
								if (int.TryParse(pictureCopyNameNumber, out picterNumber))
									number = int.Parse(pictureCopyNameNumber);
							}
						}
						data = data.Substring(0,data.LastIndexOf('.')) + "_" + number + data.Substring(data.LastIndexOf('.'));
						databasePath = data;
						File.Copy(path, databasePath);
					}
				}


				Entities.Contact newContact = new Entities.Contact()
				{
					Firstname = FirstName.Text,
					Surname = Surname.Text,
					Phone = Phone.Text,
					Street = Street.Text,
					City = City.Text,
					Email = Email.Text,
					Postalcode = Postalcode.Text,
					PicturePath = databasePath,
					UserId = this.userId,
					Id = contactId
				
				};
				_service.CreateOrUpdateContact(newContact);
				Back_Click(sender, e);
			}

		}

		private void SelectedPicture_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "Bildauswahl";
			openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
			if (openFileDialog.ShowDialog() == true)
			{
				path = openFileDialog.FileName;
				picturename = Path.GetFileName(path);
				ContactPicture.Source = new BitmapImage(new Uri(path));
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
