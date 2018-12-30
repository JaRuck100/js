using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace js.Entities
{
    public class Contact
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Postalcode { get; set; }
        public string PicturePath { get; set; }
        public int UserId { get; set; }
		public string Fullname { get { return string.Format("{0} {1}", Firstname, Surname); } }
	}
}
