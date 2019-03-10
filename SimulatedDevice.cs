// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// This application uses the Azure IoT Hub device SDK for .NET
// For samples see: https://github.com/Azure/azure-iot-sdk-csharp/tree/master/iothub/device/samples

using System;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace simulatedDevice
{
    class SimulatedDevice
    {
        private static DeviceClient s_deviceClient;

        // The device connection string to authenticate the device with your IoT hub.
        private const string s_connectionString = "HostName=rajasrifinaliothub.azure-devices.net;DeviceId=myOtherDevice;SharedAccessKey=+1yjRIxX7M2ebkln/gvUBoHYMuKTyDCLGAyMiZqwMrY=";

        // Async method to send simulated telemetry
        private static async void SendDeviceToCloudMessagesAsync()
        {
            // Initial telemetry values
            //double minTemperature = 20;
            //double minHumidity = 60;

            // blink rate
            int minBlinkRateThreshold = 1;
            int minBlinkRate = 9;

            // distance
            int minEyeDistanceThrehold = 20;
            int minEyeDistance = 30;

            Random rand = new Random();

            while (true)
            {
                //double currentTemperature = minTemperature + rand.NextDouble() * 15;
                //double currentHumidity = minHumidity + rand.NextDouble() * 20;

                // values collected for every 5 min.
                int currentBlinkRate = minBlinkRateThreshold + rand.Next(0,10); // generating a random value for current blinkrate
                int currentEyeDistance = minEyeDistanceThrehold + rand.Next(0,20);

                // Message to be sent to the IoT Hub with current blinkrate and current eye distance
                var sensorDataPoint = new {
                    blinkRate = currentBlinkRate,
                    eyeDistance = currentEyeDistance
                };
                var sensorDataPointString = JsonConvert.SerializeObject(sensorDataPoint);
                var sensorDataPointMessage = new Message(Encoding.ASCII.GetBytes(sensorDataPointString));

                sensorDataPointMessage.Properties.Add("blinkAlert", (currentBlinkRate < minBlinkRate) ? "true" : "false");
                sensorDataPointMessage.Properties.Add("eyeDistanceAlert", (currentEyeDistance < minEyeDistance) ? "true" : "false");

                await s_deviceClient.SendEventAsync(sensorDataPointMessage).ConfigureAwait(false);
                Console.WriteLine("Time: {0}: Sending data point: {1}", DateTime.Now, sensorDataPointString);


                // Create JSON message
                /* var telemetryDataPoint = new
                {
                    temperature = currentTemperature,
                    humidity = currentHumidity
                };*/


                //var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                //var message = new Message(Encoding.ASCII.GetBytes(messageString));

                // Add a custom application property to the message.
                // An IoT hub can filter on these properties without access to the message body.
                //message.Properties.Add("temperatureAlert", (currentTemperature > 30) ? "true" : "false");

                // Send the tlemetry message
                //await s_deviceClient.SendEventAsync(message).ConfigureAwait(false);
                //Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                // simulating data every 1 second
                await Task.Delay(1000).ConfigureAwait(false);
            }
        }

        private static void Main()
        {
            Console.WriteLine("Device simulation started. Please enter Ctrl-C to exit.\n");

            // Connect to the IoT hub using the MQTT protocol
            s_deviceClient = DeviceClient.CreateFromConnectionString(s_connectionString, TransportType.Mqtt);
            SendDeviceToCloudMessagesAsync();
            Console.ReadLine();
        }
    }
}
