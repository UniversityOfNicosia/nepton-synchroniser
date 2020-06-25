using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace NEPTONSync
{
    class MoodleOperations
    {
        public enum HttpVerb
        {
            GET,
            POST,
            PUT,
            DELETE
        }
        struct MoodleFunctions
        {
            public const string COHORTADDMEMEBERS = "core_cohort_add_cohort_members";
            public const string COHORTDELETEMEMEBERS = "core_cohort_delete_cohort_members";
            public const string GETUSERCOHORTS = "local_wsgetusercohorts";
            public const string COURSECREATE = "core_course_create_courses";
            public const string COURSEGET = "core_course_get_courses";
            public const string GETCOURSESBYFIELD = "core_course_get_courses_by_field";
            public const string GETENROLLEDUSERS = "core_enrol_get_enrolled_users";
            public const string GETUSERCOURSES = "local_wsgetusercourses";
            public const string CREATEUSER = "core_user_create_users";
            public const string GETUSERS = "core_user_get_users";
            public const string GETUSERBYFIELD = "core_user_get_users_by_field";
            public const string UPDATEUSER = "core_user_update_users";
            public const string ENROLUSER = "enrol_manual_enrol_users";
            public const string UNENROLUSER = "enrol_manual_unenrol_users";
            public const string ADDGROUPMEMBER = "core_group_add_group_members";
            public const string GETCOURSEGROUPS = "core_group_get_course_groups";
            public const string COREROLEASSIGNROLES = "core_role_assign_roles";
            public const string COREROLEUNASSIGNROLES = "core_role_unassign_roles";
            public const string IMPORTCOURSE = "core_course_import_course";

        }


        private MoodleObj active_moodle;
        public MoodleOperations(MoodleObj m)
        {
            this.active_moodle = m;
        }

        public int createUser(MoodleUser user)
        {
            var hometelephone = "";
            var mobiletelephone = "";
            if (user.hometelephonenumber != null)
            {
                hometelephone = user.hometelephonenumber.Trim(new Char[] { ' ', '+', '.' });
            }
            if (user.mobiletelephonenumber != null)
            {
                mobiletelephone = user.mobiletelephonenumber.Trim(new Char[] { ' ', '+', '.' });
            }
            string postData = @"users[0][username]=" + user.username.ToLower() + "&" +
                              "users[0][password]=" + user.password + "&" +
                              "users[0][email]=" + user.email + "&" +
                              "users[0][firstname]=" + user.firstname + "&" +
                              "users[0][lastname]=" + user.lastname + "&" +
                              "users[0][idnumber]=" + user.idnumber;


            var response = MakeRequest(HttpVerb.POST, postData, MoodleFunctions.CREATEUSER, null);

            JToken data = JToken.Parse(response);

            if (data is JArray)
            {
                return (int)data[0]["id"];
            }
            else if (data is JObject)
            {
                if (data["exception"] == null)
                {
                    return 1;
                }
                else
                {
                    Console.WriteLine(data.ToString());
                    return 0;
                }
            }
            return 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <returns>return user object, or null if user does not exist</returns>
        public MoodleUser getMoodleUser(string searchterm, int mode = 1)
        {
            var response = "";

            if (mode == 1)
                response = MakeRequest(HttpVerb.GET, "&criteria[0][key]=username&criteria[0][value]=" + searchterm, MoodleFunctions.GETUSERS, null);
            else
                response = MakeRequest(HttpVerb.GET, "&criteria[0][key]=email&criteria[0][value]=" + searchterm, MoodleFunctions.GETUSERS, null);

            JObject result = JObject.Parse(response);

            if (!result["users"].HasValues)
                return null;


            MoodleUser user = new MoodleUser();

            foreach (var u in result["users"])
            {
                user.id = (int)u["id"];
                user.username = u["username"].ToString();
                user.firstname = u["fullname"].ToString().Split(" ").First().ToString();
                user.lastname = u["fullname"].ToString().Split(" ").Last().ToString();
                user.email = u["email"].ToString();
                user.idnumber = (u["idnumber"] == null) ? "" : u["idnumber"].ToString();
            }

            return user;
        }
        /*
         * mode:    1 - when mode is 1 then we update user data
         *          2 - when mode is 2 we update user username and password
         */
        public bool UpdateUser(MoodleUser user, int mode = 1)
        {
            string postData = @"users[0][id]=" + user.id + "&";


            postData += "users[0][username]=" + user.username.ToLower() + "&" +
                        "users[0][password]=" + user.password + "&" +
                        "users[0][idnumber]=" + user.idnumber;
           

            var response = MakeRequest(HttpVerb.POST, postData, MoodleFunctions.UPDATEUSER, null);

            JToken data = JToken.Parse(response);

            if (data is JArray)
            {
                return true;
            }
            else if (data is JObject)
            {
                if (data["exception"] == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }


            return true;

        }

        public bool EnrolUser(int roleid, MoodleUser user, MoodleCourse course)
        {

            string postData = @"enrolments[0][roleid]=" + roleid + "&" +
                                "enrolments[0][userid]=" + user.id + "&" +
                                "enrolments[0][courseid]=" + course.id;

            var response = MakeRequest(HttpVerb.POST, postData, MoodleFunctions.ENROLUSER, null);

            if (response == "null")
                return true;

            return false;
        }

        public bool UnenrolUser(int roleid, MoodleUser user, MoodleCourse course)
        {

            string postData = @"enrolments[0][roleid]=" + roleid + "&" +
                                "enrolments[0][userid]=" + user.id + "&" +
                                "enrolments[0][courseid]=" + course.id;

            var response = MakeRequest(HttpVerb.POST, postData, MoodleFunctions.UNENROLUSER, null);

            if (response == "null")
                return true;

            return false;
        }

      

        /// <summary>
        /// Get course in Moodle if exists
        /// </summary>
        /// <param name="shortname"></param>
        /// <returns>The course object if exists. Null if not exists</returns>
        public MoodleCourse getCourse(string shortname)
        {
            string postData = @"field=shortname&" +
                                "value=" + shortname;

            var response = MakeRequest(HttpVerb.GET, postData, MoodleFunctions.GETCOURSESBYFIELD, null);

            JObject objResult = JObject.Parse(response);

            if (objResult["courses"].Count() <= 0)
                return null;

            MoodleCourse course = new MoodleCourse();
            foreach (var c in objResult["courses"])
            {
                course.id = (int)c["id"];
                course.fullname = c["fullname"].ToString();
                course.shortname = c["shortname"].ToString();

            }

            return course;

        }
        public int CreateCourse(MoodleCourse c)
        {
            string postData = @"courses[0][fullname]=" + c.fullname.Replace(':', ' ').Replace('&', ' ') + "&" +
                                "courses[0][shortname]=" + c.shortname + "&" +
                                "courses[0][categoryid]=" + c.categoryid + "&" +
                                "courses[0][startdate]=" + c.startdate + "&" +
                                "courses[0][enddate]=" + c.enddate + "&" +
                                "courses[0][visible]=" + c.visible;

            var response = MakeRequest(HttpVerb.GET, postData, MoodleFunctions.COURSECREATE, null);

            JArray data = JArray.Parse(response);
            return (int)data[0]["id"];

            /*
            if (data is JArray)
            {
                return (int)data[0]["id"];
            }
            else if (data is JObject)
            {
                if (data["exception"] == null)
                {
                    return 1;
                }
                else
                {
                    Console.WriteLine(data["exception"]);
                    return 0;
                }
            }
            
            return 0;
                 */
        }





        public bool assignRole(int roleid, int userid, int contextid)
        {
            string postData = @"assignments[0][roleid]=" + roleid + "&" +
                                "assignments[0][userid]=" + userid + "&" +
                                "assignments[0][contextid]=" + contextid;
            var response = MakeRequest(HttpVerb.POST, postData, MoodleFunctions.COREROLEASSIGNROLES, null);

            if (response == "null")
            {
                return true;
            }

            JObject data = JObject.Parse(response);
            if (data is JObject)
            {
                if (data["exception"] == null)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine(data["exception"]);
                    return false;
                }
            }

            return false;

        }

        public bool unAssignRole(int roleid, int userid, int contextid)
        {
            string postData = @"unassignments[0][roleid]=" + roleid + "&" +
                                "unassignments[0][userid]=" + userid + "&" +
                                "unassignments[0][contextid]=" + contextid;
            var response = MakeRequest(HttpVerb.POST, postData, MoodleFunctions.COREROLEUNASSIGNROLES, null);

            if (response == "null")
            {
                return true;
            }

            JObject data = JObject.Parse(response);
            if (data is JObject)
            {
                if (data["exception"] == null)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine(data["exception"]);
                    return false;
                }
            }

            return false;
        }

        public bool ImportCourse(int importfrom, int importto)
        {

            string postData = @"importfrom=" + importfrom + "&" +
                                "importto=" + importto;


            var response = MakeRequest(HttpVerb.POST, postData, MoodleFunctions.IMPORTCOURSE, null);

            if (response == "null")
            {
                return true;
            }

            JObject data = JObject.Parse(response);
            if (data is JObject)
            {
                if (data["exception"] == null)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine(data["exception"]);
                    return false;
                }
            }

            return false;
        }

        public string MakeRequest(HttpVerb method, string parameters, string func, string postdata)
        {
            var apiPath = "?wstoken=" + this.active_moodle.token +
                                "&wsfunction=" + func +
                                "&moodlewsrestformat=json" +
                                "&" + parameters;

            /*
            // [ Parameter Format  ] ___=___  &
            // [ Parameter Example ] "members[0][groupid]=" + group.id + "&"
            var paramPairs = new List<KeyValuePair<string,string>>();
            foreach (var p in parameters.Split(new char[] { '&' }))
            {
                var tmp = p.Split(new char[] { '=' });
                paramPairs.Add(new KeyValuePair<string,string>(tmp[0], tmp[1]));
            }
            */

            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(this.active_moodle.webserviceurl);

                // For Post Request (Also you uncomment the previous comment block to convert the parameters)
                // var content = new FormUrlEncodedContent(paramPairs);
                // HttpResponseMessage result = await client.PostAsync(apiPath, content);

                HttpResponseMessage result = client.GetAsync(apiPath).GetAwaiter().GetResult();

                return result.Content.ReadAsStringAsync()
                                     .GetAwaiter().GetResult();
            }
        }

        public string MakeRequestOld(HttpVerb method, string parameters, string func, string postdata)
        {
            var requesttext = this.active_moodle.webserviceurl +
                                "?wstoken=" + this.active_moodle.token +
                                "&wsfunction=" + func +
                                "&moodlewsrestformat=json" +
                                "&" + parameters;

            var request = (HttpWebRequest)WebRequest.Create(requesttext);

            request.Method = method.ToString();
            request.ContentLength = 0;
            request.ContentType = "application/json";
            request.Timeout = 10000;
            if (!string.IsNullOrEmpty(postdata) && method == HttpVerb.POST)
            {
                var encoding = new UTF8Encoding();
                var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(postdata);
                request.ContentLength = bytes.Length;

                using (var writeStream = request.GetRequestStream())
                {
                    writeStream.Write(bytes, 0, bytes.Length);
                }
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var responseValue = string.Empty;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }

                // grab the response
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                        using (var reader = new StreamReader(responseStream))
                        {
                            responseValue = reader.ReadToEnd();
                        }
                }

                response.Close();
                return responseValue;
            }
        }


    }
}
