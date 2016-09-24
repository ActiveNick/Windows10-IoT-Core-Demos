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
        // TO DO: Replace this with your own connection string, please don't use mine :)
        static string connectionString = "HostName=NickIoTLab.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=Y0XssoLOWrKHFGHqiILo18JJG/BdEfDguQHgECBmPIg=";

        static void Main(string[] args)
        {
            Console.WriteLine("Send Cloud-to-Device message\n");
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            // We'll expect an ACK message to confirm it was received by the device
            ReceiveFeedbackAsync();

            Console.WriteLine("Press any key to send a C2D message.");
            Console.ReadLine();
            SendCloudToDeviceMessageAsync().Wait();
            Console.ReadLine();
        }

        private async static Task SendCloudToDeviceMessageAsync()
        {
            // The message we send to the device
            var commandMessage = new Message(Encoding.ASCII.GetBytes("This is a Cloud to device message from Nick."));

            // Specify that we'll expect a full delivery acknowledgement from the IoT device
            commandMessage.Ack = DeliveryAcknowledgement.Full;
            // Instruct the IoT Hub to send the message to the device based on our ID
            await serviceClient.SendAsync("NickPi2-FEZHAT", commandMessage);
        }

        private async static void ReceiveFeedbackAsync()
        {
            var feedbackReceiver = serviceClient.GetFeedbackReceiver();

            Console.WriteLine("\nReceiving c2d feedback from service");
            while (true)
            {
                var feedbackBatch = await feedbackReceiver.ReceiveAsync();
                if (feedbackBatch == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received feedback: {0}", string.Join(", ", feedbackBatch.Records.Select(f => f.StatusCode)));
                Console.ResetColor();

                await feedbackReceiver.CompleteAsync(feedbackBatch);
            }
        }
    }
}
