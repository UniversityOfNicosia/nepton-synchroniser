using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEPTONSync
{
    class MoodleUser
    { 
        public int id { get; set; }
        public String username { get; set; }
        public String password { get; set; }
        public String firstname { get; set; }
        public String lastname { get; set; }
        public String e_mail_for_creation { get; set; }
        public String email { get; set; }
        public String e_mail_at_college { get; set; }
        public String auth { get; set; }
        public String idnumber { get; set; }
        public String lang { get; set; }
        public String calendartype { get; set; }
        public String theme { get; set; }
        public String timezone { get; set; }
        public int mailformat { get; set; }
        public String description { get; set; }
        public String city { get; set; }
        public String country { get; set; }
        public String firstnamephonetic { get; set; }
        public String lastnamephonetic { get; set; }
        public String middlename { get; set; }
        public String alternatname { get; set; }
        public String programmeid { get; set; }
        public String hometelephonenumber { get; set; }
        public String mobiletelephonenumber { get; set; }

        public String firstname_gr;
        public string lastname_gr;

        public List<string> enrolled_courses { get; set; }

    }
}
