using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;

static class AzureIoTHub
{
    //
    // Note: this connection string is specific to the device "NickPi2-FEZHAT". To configure other devices,
    // see information on iothub-explorer at http://aka.ms/iothubgetstartedVSCS
    //
    const string deviceConnectionString = "HostName=NickIoTLab.azure-devices.net;DeviceId=NickPi2-FEZHAT;SharedAccessKey=vSMfOqde7kDycueME8274LIboBpeIsfKQmuUJahMBP0=";

    //
    // To monitor messages sent to device "NickPi2-FEZHAT" use iothub-explorer as follows:
    //    iothub-explorer HostName=NickIoTLab.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=rrnhLYK2lws2OzEeUAlcOnnhkOF2P+uQIcrV6k1XEd0= monitor-events "NickPi2-FEZHAT"
    //

    // Refer to http://aka.ms/azure-iot-hub-vs-cs-wiki for more information on Connected Service for Azure IoT Hub

    /// <summary>
    /// Sends a custom string message to an Azure IoT Hub
    /// </summary>
    public static async Task SendDeviceToCloudMessageAsync(string str)
    {
        var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Amqp);

        var message = new Message(Encoding.ASCII.GetBytes(str));

        await deviceClient.SendEventAsync(message);
    }

    /// <summary>
    /// Receives a message from an Azure IoT HUb using the service-assisted communication pattern 
    /// (aka Command & Control).
    /// </summary>
    public static async Task<string> ReceiveCloudToDeviceMessageAsync()
    {
        var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Amqp);

        while (true)
        {
            var receivedMessage = await deviceClient.ReceiveAsync();

            if (receivedMessage != null)
            {
                var messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                await deviceClient.CompleteAsync(receivedMessage);
                return messageData;
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}
