using System;
using System.Collections.Generic;

namespace NEPTONSync
{
    class Program
    {
        private static MoodleObj moodle;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            moodle = new MoodleObj("https://nepton.unic.ac.cy", "https://nepton.unic.ac.cy/webservice/rest/server.php", "e6ca6180321f0927a35d07a9f0c3843c", 2, "DL");
            OracleOperations oOperations = new OracleOperations(moodle);
            MoodleOperations mOperations = new MoodleOperations(moodle);

            while (true)
            {
                createNeptonAccount(oOperations, mOperations);
            }
        }

        private static void createNeptonAccount(OracleOperations oOperation, MoodleOperations mOperations)
        {
            List<OperationObjectStudent> operations = oOperation.getChanges();

            foreach (var operation in operations)
            {
                MoodleUser moodleUser = mOperations.getMoodleUser(operation.student_id.ToLower());

                //Lets check if course exists in Moodle before we do anything
                MoodleCourse course = mOperations.getCourse("nepton-01-s1-2020");


                if (moodleUser == null) //if the user does not exist in Moodle create him
                {
                    MoodleUser newUser = new MoodleUser();

                    newUser.username = operation.student_id;
                    newUser.firstname = operation.first_name;
                    newUser.lastname = operation.last_name;
                    newUser.idnumber = operation.student_id;
                    newUser.password = operation.password;
                    newUser.email = operation.e_mail;

                    var userid = mOperations.createUser(newUser);
                    if (userid != 0)
                    {

                        moodleUser = newUser;
                        moodleUser.id = userid;

                        Console.WriteLine($"{DateTime.Now}: User {operation.student_id} created");

                        //ENROL
                        if (mOperations.EnrolUser(5, moodleUser, course))
                        {
                            Console.WriteLine($"{DateTime.Now}: User {operation.student_id} enrolled in Course");

                            if (oOperation.UpdateRow(operation.id) == false)
                                Console.WriteLine($"{DateTime.Now}: Cannot update row {operation.id}");
                        }
                        else
                        {
                            Console.WriteLine($"{DateTime.Now}: User {operation.student_id} was NOT enrolled");
                            continue;
                        }

                    }
                }
                else
                {
                    Console.WriteLine($"{DateTime.Now}: User {operation.student_id} already exists, nothing to do");
                }
            }
        }
    }
}
