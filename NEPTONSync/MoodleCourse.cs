using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEPTONSync
{
    class MoodleCourse
    {

        public int id { get; set; }

        public String fullname { get; set; }

        public String shortname { get; set; }

        public int categoryid { get; set; } //int

        public String idnumber { get; set; }

        public String summary { get; set; }

        public int summaryformat { get; set; } //int

        public String format { get; set; }

        public int showgrades { get; set; } //int

        public int newsitems { get; set; } //int

        public int startdate { get; set; } //int

        public int enddate { get; set; } //int

        public int numsections { get; set; } //int

        public int maxbytes { get; set; } //int

        public int showreports { get; set; } //int

        public int visible { get; set; }//int

        public int hiddensections { get; set; } //int

        public int groupmode { get; set; } //int

        public int groupmodeforce { get; set; } //int

        public int defaultgroupingid { get; set; } //int

        public int enablecompletion { get; set; } //int

        public int completionnotify { get; set; } //int

        public String lang { get; set; }

        public String forcetheme { get; set; }

        public String courseformatoptions { get; set; }

        public int timecreated { get; set; }

        public int timemodified { get; set; }
    }
}
