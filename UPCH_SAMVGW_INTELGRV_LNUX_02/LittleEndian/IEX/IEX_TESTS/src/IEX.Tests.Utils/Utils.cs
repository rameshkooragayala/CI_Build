using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IEX.Tests.Engine;

namespace IEX.Tests.Utils
{
    public static class Utils
    {
        /*Example of Util function. The call from the test will be Utils.Example(CL)
        public static void Example( _Platform p)
        {
            p.EA.DUMMY("Key", true);
        }

        //Example of Util function using 'this' parameter. The call from the test will be CL.Example()
        public static void Example(this _Platform p)
        {
            p.EA.DUMMY("Key", true);
        }
        */

        public static void SendResults(this TestsScheduler scheduler)
        {
            //Here you can implement post tests functionality like sending the results to QC.

            var status = scheduler.TestsGroupStatus;
            var cancelationReason = _Test.CancelationReason;
        }

        public static void SaveSnapshot(_Platform p, string snapshotName)
        {
            if (!Directory.Exists(p.EA.LogFilePath + "Tests Images"))
            {
                Directory.CreateDirectory(p.EA.LogFilePath + "Tests Images");
            }
            p.IEX.GetScreenCapture("", p.EA.LogFilePath + "Tests Images\\" + snapshotName + ".jpg", IEXGateway.ImageFormat.Jpeg);
            p.IEX.GetSnapshot("This Screen Needs To Be Validated Manualy");
        }
    }
}

