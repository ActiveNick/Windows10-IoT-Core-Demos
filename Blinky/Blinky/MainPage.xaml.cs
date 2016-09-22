using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Gpio;

namespace Blinky
{
    /// <summary>
    /// A Blinky project is the maker edition of "Hello World". It's a trivial project,
    /// but it does validate that you hardware, IDE and SDKs are setup properly.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // The pin # represents the GPIO #, *NOT* the physical pin count on the board
        private const int LED_PIN = 5;
        private GpioPin pin;
        private GpioPinValue pinValue;

        // Color brushes used for the on-screen UI only
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);

        public MainPage()
        {
            this.InitializeComponent();

            InitGPIO();
        }

        private void InitGPIO()
        {
            // Before initialize the GPIO controller in code, make sure to add the
            // Windows IoT Extensions for the UWP in your project references

            // Next, we check to see if the GpioController exists on the current execution platform
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Devices.Gpio.GpioController"))
            {
                var gpio = GpioController.GetDefault();

                // Let's make sure there actually is a GPIO controller
                if (gpio == null)
                {
                    pin = null;
                    lblStatus.Text = "There is no GPIO controller on this device.";
                    return;
                }

                pin = gpio.OpenPin(LED_PIN);
            }

            // No GPIO, no pin, then we exit, we're obviously not on an IoT device
            if (pin == null)
            {
                return;
            }

            // GPIO pins can be used as input or output. Writing to an LED requies and output
            pin.SetDriveMode(GpioPinDriveMode.Output);
            // Turn off the LED by default by seting a low voltage (0V) on the pin
            pinValue = GpioPinValue.Low; 
            pin.Write(pinValue);
        }

        private void btnBlink_Click(object sender, RoutedEventArgs e)
        {
            if (pinValue == GpioPinValue.High)
            {
                pinValue = GpioPinValue.Low; // No voltage, or 0V
                pin.Write(pinValue);
                LED.Fill = grayBrush;
            }
            else
            {
                pinValue = GpioPinValue.High; // Typically 5v or 3.3V depending on the device
                pin.Write(pinValue);
                LED.Fill = redBrush;
            }
        }
    }
}
