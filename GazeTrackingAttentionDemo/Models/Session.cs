using GazeTrackingAttentionDemo.DeviceInteraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeTrackingAttentionDemo.Models
{
    class Session
    {
        public static User currentUser;
        public static Test currentTest;
        public static Recording currentRecording;
        public static DeviceInteractionHost dataRecorder;

        public static List<Test> testList;

        public static void newSession(User u)
        {
            endSession();
            currentUser = u;
            currentTest = null;
            currentRecording = null;
            dataRecorder = null;
            testList = new List<Test>();
        }

        public static void endSession()
        {

        }
    }
}
