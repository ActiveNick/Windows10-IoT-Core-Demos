// Copyright (c) Microsoft. All rights reserved.
using System;
// Make sure to add the Windows IoT Extension SDK in project references
using Windows.Devices.Gpio;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace PushButton
{
    public sealed partial class MainPage : Page
    {
        private const int LED_PIN = 6;  // This is an output
        private const int BUTTON_PIN = 5;  // This is an input
        private GpioPin ledPin;
        private GpioPin buttonPin;
        private GpioPinValue ledPinValue = GpioPinValue.High;

        // These are used for the "on screen" LED in the UI
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);

        public MainPage()
        {
            InitializeComponent();
            InitGPIO();
        }

        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                GpioStatus.Text = "There is no GPIO controller on this device.";
                return;
            }

            buttonPin = gpio.OpenPin(BUTTON_PIN);
            ledPin = gpio.OpenPin(LED_PIN);

            // Initialize LED to the OFF state by first writing a HIGH value
            // We write HIGH because the LED is wired in a active LOW configuration
            ledPin.SetDriveMode(GpioPinDriveMode.Output);
            ledPin.Write(GpioPinValue.High);

            // Check if input pull-up resistors are supported, for more info
            // see https://en.wikipedia.org/wiki/Pull-up_resistor
            if (buttonPin.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                // Configures the GPIO pin as high impedance with a pull-up resistor
                // to the voltage charge connection (VCC).
                buttonPin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                // Configures the GPIO pin in floating mode, with high impedance
                buttonPin.SetDriveMode(GpioPinDriveMode.Input);

            // Set a debounce timeout to filter out switch bounce noise from a button press
            buttonPin.DebounceTimeout = TimeSpan.FromMilliseconds(50);

            // Register for the ValueChanged event so our buttonPin_ValueChanged 
            // function is called when the button is pressed
            buttonPin.ValueChanged += buttonPin_ValueChanged;

            GpioStatus.Text = "GPIO pins initialized correctly.";
        }

        private void buttonPin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            // Toggle the state of the LED every time the button is pressed
            if (e.Edge == GpioPinEdge.FallingEdge)
            {
                ledPinValue = (ledPinValue == GpioPinValue.Low) ?
                    GpioPinValue.High : GpioPinValue.Low;
                ledPin.Write(ledPinValue);
            }

            // Need to invoke UI updates on the UI thread because this event
            // handler gets invoked on a separate thread.
            var task = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                if (e.Edge == GpioPinEdge.FallingEdge)
                {
                    ledEllipse.Fill = (ledPinValue == GpioPinValue.Low) ? 
                        redBrush : grayBrush;
                    GpioStatus.Text = "Button Pressed";
                }
                else
                {
                    GpioStatus.Text = "Button Released";
                }
                // Use Speech Synthesis to read the GPIO status out loud
                ReadText(GpioStatus.Text);
            });
        }

        // Quickly adds Text-to-Speech to the app using Cortana's default voice
        private async void ReadText(string mytext)
        {
            //Reminder: You need to enable the Microphone capabilitiy in Windows Phone projects
            //Reminder: Add this namespace in your using statements
            //using Windows.Media.SpeechSynthesis;

            // The media object for controlling and playing audio.
            MediaElement mediaplayer = new MediaElement();

            // The object for controlling the speech synthesis engine (voice).
            using (var speech = new SpeechSynthesizer())
            {
                //Retrieve the first female voice
                //speech.Voice = SpeechSynthesizer.AllVoices
                //    .First(i => (i.Gender == VoiceGender.Female && i.Description.Contains("United States")));
                // Generate the audio stream from plain text.
                SpeechSynthesisStream stream = await speech.SynthesizeTextToStreamAsync(mytext);

                // Send the stream to the media object.
                mediaplayer.SetSource(stream, stream.ContentType);
                mediaplayer.Play();
            }
        }
    }
}
