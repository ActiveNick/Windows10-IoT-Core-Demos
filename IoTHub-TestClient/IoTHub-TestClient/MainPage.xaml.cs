using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace IoTHub_TestClient
{
    /// <summary>
    /// Simple test page that connects to an Azure IoT Hub for device-to-cloud
    /// and cloud-to-device scenarios.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Send a string to an Azure IoT Hub.
        /// </summary>
        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string str = txtMsg.Text;
                await AzureIoTHub.SendDeviceToCloudMessageAsync(str);
            }
            catch (Exception ex)
            {
                await new Windows.UI.Popups.MessageDialog($"Oops, something went wrong: {ex.Message}").ShowAsync();
            }
        }

        /// <summary>
        /// Puts the app in an async "wait" mode, ready to receive incoming messages from the IoT Hub.
        /// </summary>
        private async void btnReceive_Click(object sender, RoutedEventArgs e)
        {
            string ret;
            ret = await AzureIoTHub.ReceiveCloudToDeviceMessageAsync();

            // Use the dispatcher to display the message on the main UI thread
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                lblReturn.Text = ret;
            });
        }
    }
}
