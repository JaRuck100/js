using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace js.Entities
{
    public class Task
    {
        public int id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Priority { get; set; }
        public bool TaskFininshed { get; set; }
        public string Description { get; set; }
		public int ToDoListId { get; set; }
	}

}
