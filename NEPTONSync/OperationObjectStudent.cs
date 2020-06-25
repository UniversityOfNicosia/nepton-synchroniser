using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEPTONSync
{
    public enum MoodleAction
    {
        ENROLL,
        UNENROLL
    }
    class OperationObjectStudent
    {
        public int id;
        public MoodleAction action;

        /*
         * User Info
         */
        public string student_id;
        public string password;
        public string first_name;
        public string last_name;
        public string e_mail;
     
    }
}
