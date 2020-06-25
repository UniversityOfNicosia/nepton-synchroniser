using System;
using System.Collections.Generic;
using System.Text;

namespace NEPTONSync
{
    class MoodleObj
    {
        public MoodleObj(string baseurl, string webserviceurl, string token, int moodle_id, string name)
        {
            this.baseurl = baseurl;
            this.webserviceurl = webserviceurl;
            this.token = token;
            this.moodle_id = moodle_id;
            this.name = name;
        }
        public string baseurl;
        public string webserviceurl;
        public string token;
        public int moodle_id;
        public string name;
    }
}
