﻿using GazeTrackingAttentionDemo.Models;
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
        //public static User loadUser(String filePath)
        //{
        //    String jsonData = File.ReadAllText(filePath);
        //    User u = JsonConvert.DeserializeObject<User>(jsonData);
        //    Console.WriteLine("Loaded user " + u.Id);
        //    return u;
        //}

        //public static void saveUser(User u)
        //{
        //    String filePath = u.DirPath + "\\userData.json";
        //    String jsonData = JsonConvert.SerializeObject(u);
        //    System.IO.File.WriteAllText(filePath, jsonData);
        //}

        public static List<User> loadUsers()
        {
            string jsonData = File.ReadAllText(@"C:\MMAD\Subjects\userList.json");

            // De-serialize to object or create new list
            return JsonConvert.DeserializeObject<List<User>>(jsonData)
                  ?? new List<User>();
        }

        public static void saveUser(User user)
        {
            string filePath = @"C:\MMAD\Subjects\userList.json";
            string jsonData;
            List<User> userList;


            // Read existing json data
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "");
                userList = new List<User>();
            }
            else
            {
                jsonData = File.ReadAllText(filePath);

                // De-serialize to object or create new list
                userList = JsonConvert.DeserializeObject<List<User>>(jsonData)
                      ?? new List<User>();
            }

            //check if already annotated
            bool alreadyAnnotated = false;
            int index = 0;
            foreach (User u in userList)
            {
                if (Equals(user.Id, u.Id))
                {
                    index = userList.IndexOf(u);
                    alreadyAnnotated = true;
                }
            }

            if (alreadyAnnotated) //if already exists update
            {
                userList[index] = user;
            }
            else //else add to list
            {
                userList.Add(user);
            }

            // Update json data string and write to file
            jsonData = JsonConvert.SerializeObject(userList, Formatting.Indented);
            System.IO.File.WriteAllText(filePath, jsonData);
        }

        //update test list
        public static void saveTest(Test test)
        {

            string filePath = Session.currentUser.DirPath + @"\testList.json";
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
            jsonData = JsonConvert.SerializeObject(testList, Formatting.Indented);
            System.IO.File.WriteAllText(filePath, jsonData);
        }

        //update test recording list
        public static void saveRecording(Recording recording)
        {

            string filePath = Session.currentTest.TestDir + @"\recordingList.json";
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
            jsonData = JsonConvert.SerializeObject(recordingList, Formatting.Indented);
            System.IO.File.WriteAllText(filePath, jsonData);
        }

        public static List<Test> loadTests(User u)
        {
            string jsonData = File.ReadAllText(u.DirPath + @"\testList.json");

            // De-serialize to object or create new list
            return JsonConvert.DeserializeObject<List<Test>>(jsonData)
                  ?? new List<Test>();
        }

        public static List<Recording> loadRecordings(Test t)
        {
            string jsonData = File.ReadAllText(t.TestDir + @"\recordingList.json");

            // De-serialize to object or create new list
            return JsonConvert.DeserializeObject<List<Recording>>(jsonData)
                  ?? new List<Recording>();
        }

        //load any existing annotations
        public static List<AOI> loadAois(Recording r)
        {
            string jsonData = File.ReadAllText(r.dataDir + @"\annotations.json");

            // De-serialize to object or create new list
            List<AOI> aois =  JsonConvert.DeserializeObject<List<AOI>>(jsonData)
                  ?? new List<AOI>();
            foreach(AOI a in aois)
            {
                a.revertPolygon();
            }
            return aois;
        }

        public static void saveSession()
        {
            ObjectManager.saveUser(Session.currentUser);
            ObjectManager.saveTest(Session.currentTest);
            if (Session.currentRecording != null)
            {
                ObjectManager.saveRecording(Session.currentRecording);
            }
        }

        public static List<Fixation> loadFixationsFromFile(string filePath)
        {
            List<Fixation> validFixations = new List<Fixation>();
            Fixation f = new Fixation();
            f.dataPos = new List<DataPoint>();
            

            string line;
            StreamReader file = new StreamReader(filePath);
            bool firstline = true;
            while ((line = file.ReadLine()) != null)
            {
                if (firstline)
                {
                    firstline = false;
                    continue;
                }
                string[] vals = line.Split(',');
                DataPoint dp = new DataPoint(float.Parse(vals[1]), float.Parse(vals[2]), float.Parse(vals[3]));
                //add val to list
                if (vals[0].Equals("Begin"))
                {
                    f.startPos = dp;
                }
                else if (vals[0].Equals("Data"))
                {
                    f.dataPos.Add(dp);
                }
                else if (vals[0].Equals("End"))
                {
                    f.endPos = dp;
                    f.completeFixation(validFixations.Count);
                    validFixations.Add(f);
                    f = new Fixation();
                    f.dataPos = new List<DataPoint>();
                }
            }

            file.Close();
           
            return validFixations;
        }

        public static List<Saccade> loadSaccades(List<Fixation> fixations)
        {
            List<Saccade> saccades = new List<Saccade>();
            Saccade s = new Saccade();
            int i = 0;
            foreach (Fixation f in fixations)
            {
                if (s.start == null)
                {
                    s.start = f;
                }
                else
                {
                    s.finish = f;
                    s.completeSaccade(i);
                    saccades.Add(s);
                    s = new Saccade();
                    s.start = f;
                    i++;
                }
            }
            return saccades;
        }

    }
}
