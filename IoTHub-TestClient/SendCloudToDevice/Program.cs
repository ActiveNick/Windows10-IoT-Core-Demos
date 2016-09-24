using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;

namespace SendCloudToDevice
{
    class Program
    {
        static ServiceClient serviceClient;
        static string connectionString = "HostName=NickIoTLab.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=Y0XssoLOWrKHFGHqiILo18JJG/BdEfDguQHgECBmPIg=";

        static void Main(string[] args)
        {
            Console.WriteLine("Send Cloud-to-Device message\n");
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);

            Console.WriteLine("Press any key to send a C2D message.");
            Console.ReadLine();
            SendCloudToDeviceMessageAsync().Wait();
            Console.ReadLine();
        }

        private async static Task SendCloudToDeviceMessageAsync()
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes("This is a Cloud to device message from Nick."));
            await serviceClient.SendAsync("NickPi2-FEZHAT", commandMessage);
        }
    }
}
