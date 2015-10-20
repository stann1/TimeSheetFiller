using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleClient.Properties;
using SeleniumWorker;

namespace ConsoleClient
{
    class Program
    {
        private const string JobCancelationText = "cancel";
        
        static void Main(string[] args)
        {
            Console.WriteLine("Starting automatic fill-in of time sheet. Press any key to proceed, or type 'cancel' to abort.");
            string command = Console.ReadLine();
            if (command == JobCancelationText)
            {
                Console.WriteLine("Job aborted.");
                return;
            }

            // input params ===============================
            string userName = Settings.Default.DefaultUser;
            Console.WriteLine("Enter password for user {0}", userName);
            string password = Console.ReadLine();

            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now.AddDays(1);
            WorkItem workItem = new WorkItem()
            {
                WorkTypeCode = 102,
                WorkHours = 8,
                CostUnitCode = Settings.Default.DefaultCostUnit,
                CostCenterCode = Settings.Default.DefaultCostCenter
            };
            // input params ===============================

            WebPortalWorker worker = new WebPortalWorker();
            worker.FillInPeriod(userName, password , start, end, workItem);
        }
    }
}
