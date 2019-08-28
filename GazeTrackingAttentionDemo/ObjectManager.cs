using GazeTrackingAttentionDemo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeTrackingAttentionDemo
{
    class ObjectManager
    {
        public string userDir = "\\MMAD\\Subjects";
        public static User loadUser(String filePath)
        {
            String jsonData = File.ReadAllText(filePath);
            User u = JsonConvert.DeserializeObject<User>(jsonData);
            Console.WriteLine("Loaded user " + u.Id);
            return u;
        }

        public static void saveUser(User u)
        {
            String filePath = u.DirPath + "\\userData.json";
            String jsonData = JsonConvert.SerializeObject(u);
            System.IO.File.WriteAllText(filePath, jsonData);
        }

        public static User createUser(String id, String groupName, String groupPath)
        {
            User u = new User(id, groupName, groupPath);
            saveUser(u);
            return u;
        }

        //update test list
        public static void saveTest(String testListPath, Test test)
        {

            string filePath = testListPath;
            string jsonData;
            List<Test> testList;


            // Read existing json data
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "");
                testList = new List<Test>();
            }
            else
            {
                jsonData = File.ReadAllText(filePath);

                // De-serialize to object or create new list
                testList = JsonConvert.DeserializeObject<List<Test>>(jsonData)
                      ?? new List<Test>();
            }

            //check if already annotated
            bool alreadyAnnotated = false;
            int index = 0;
            foreach (Test t in testList)
            {
                if (Equals(test.Name, t.Name))
                {
                    index = testList.IndexOf(t);
                    alreadyAnnotated = true;
                }
            }

            if (alreadyAnnotated) //if already exists update
            {
                testList[index] = test;
            }
            else //else add to list
            {
                testList.Add(test);
            }

            // Update json data string and write to file
            jsonData = JsonConvert.SerializeObject(testList);
            System.IO.File.WriteAllText(filePath, jsonData);

        }

        //update test recording list
        public static void saveRecording(String recordingListPath, Recording recording)
        {

            string filePath = recordingListPath;
            string jsonData;
            List<Recording> recordingList;


            // Read existing json data
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "");
                recordingList = new List<Recording>();
            }
            else
            {
                jsonData = File.ReadAllText(filePath);

                // De-serialize to object or create new list
                recordingList = JsonConvert.DeserializeObject<List<Recording>>(jsonData)
                      ?? new List<Recording>();
            }

            //check if already annotated
            bool alreadyAnnotated = false;
            int index = 0;
            foreach (Recording r in recordingList)
            {
                if (Equals(recording.Index, r.Index))
                {
                    index = recordingList.IndexOf(r);
                    alreadyAnnotated = true;
                }
            }

            if (alreadyAnnotated) //if already exists update
            {
                recordingList[index] = recording;
            }
            else //else add to list
            {
                recordingList.Add(recording);
            }

            // Update json data string and write to file
            jsonData = JsonConvert.SerializeObject(recordingList);
            System.IO.File.WriteAllText(filePath, jsonData);
        }

        public static List<Test> loadTests()
        {
            string jsonData = File.ReadAllText(Session.currentUser.DirPath + @"\testList.json");

            // De-serialize to object or create new list
            return JsonConvert.DeserializeObject<List<Test>>(jsonData)
                  ?? new List<Test>();
        }

        public static List<Recording> loadRecordings()
        {
            string jsonData = File.ReadAllText(Session.currentUser.DirPath + @"\recordingList.json");

            // De-serialize to object or create new list
            return JsonConvert.DeserializeObject<List<Recording>>(jsonData)
                  ?? new List<Recording>();
        }

        public static void saveSession()
        {
            ObjectManager.saveUser(Session.currentUser);
            ObjectManager.saveTest(Session.currentUser.DirPath + @"\testList.json", Session.currentTest);
            ObjectManager.saveRecording(Session.currentTest.DataDir + @"\recordingList.json", Session.currentRecording);
        }

    }
}
