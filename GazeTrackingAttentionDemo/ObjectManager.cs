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

        //save/load list of tests

        //save/load list of recordings


    }
}
