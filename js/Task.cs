using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace js
{
    public class Task
    {
        public int id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Priority { get; set; }
        public bool TaskFininshed { get; set; }
    }

}
