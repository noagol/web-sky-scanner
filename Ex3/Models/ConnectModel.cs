using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Ex3.Models;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace FlightSimulator.Model
{
    class ConnectModel
    {
        private float lon;
        private float lat;
        private float rudder;
        private float throttle;
        private string ip;
        private int port;
        private Thread thread;
        private Queue<UpdateCommand> commandsQueue;

        private bool shouldStop;

        public event EventHandler positionChanged;


        /// <summary>Initializes a new instance of the <see cref="ConnectModel"/> class.</summary>
        public ConnectModel(string ip, int port)
        {
            // Set stop boolean
            shouldStop = false;

            // Initialize commands queue
            commandsQueue = new Queue<UpdateCommand>();

            // Intialize members
            this.ip = ip;
            this.port = port;

            // Start sender thread
            thread = new Thread(() => connectToSimulator(ip, port));
            thread.Start();
        }

        /// <summary>Gets or sets the lon.</summary>
        /// <value>The lon.</value>
        public float Lon
        {
            get { return lon; }
            set
            {
                lon = value;
                positionChanged?.Invoke(this, new PropertyChangedEventArgs("Lon"));
            }
        }

        /// <summary>
        ///   <para>
        ///  Gets or sets the lat.
        /// </para>
        /// </summary>
        /// <value>The lat.</value>
        public float Lat
        {
            get { return lat; }
            set
            {
                lat = value;
                positionChanged?.Invoke(this, new PropertyChangedEventArgs("Lat"));
            }
        }


        /// <summary>Gets or sets the rudder.</summary>
        /// <value>The rudder.</value>
        public float Rudder
        {
            get { return rudder; }
            set
            {
                rudder = value;
                positionChanged?.Invoke(this, new PropertyChangedEventArgs("Rudder"));
            }
        }

        /// <summary>Gets or sets the throttle.</summary>
        /// <value>The throttle.</value>
        public float Throttle
        {
            get { return throttle; }
            set
            {
                throttle = value;
                positionChanged?.Invoke(this, new PropertyChangedEventArgs("Throttle"));
            }
        }


        /// <summary>Updates the rudder.</summary>
        public void updateRudder()
        {
            commandsQueue.Enqueue(new UpdateCommand("/controls/flight/rudder", value => Rudder = value));
        }


        /// <summary>Updates the position.</summary>
        public void updatePosition()
        {
            commandsQueue.Enqueue(new UpdateCommand("/position/latitude-deg", value => Lat = value));
            commandsQueue.Enqueue(new UpdateCommand("/position/longitude-deg", value => Lon = value));
        }


        /// <summary>Updates the throttle.</summary>
        public void updateThrottle()
        {
            commandsQueue.Enqueue(new UpdateCommand("/controls/engines/current-engine/throttle", value => Throttle = value));
        }

        /// <summary>Stops this instance.</summary>
        public void stop()
        {
            shouldStop = true;
            thread.Join();
        }

        /// <summary>Reads the until new line.</summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        private static string readUntilNewLine(BinaryReader reader)
        {
            char[] buffer = new char[1024];
            int i = 0;
            char last = '\0';

            // Read until new line character
            while (i < 1024 && last != '\n')
            {
                char input = reader.ReadChar();
                buffer[i] = input;
                last = buffer[i];
                i++;
            }

            return new string(buffer);
        }

        /// <summary>Updates the simulator.</summary>
        private void connectToSimulator(string ip, int port)
        {
            // Connect to client
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            TcpClient client = new TcpClient();
            client.Connect(ep);

            string inputLine;
            float val;
            // Run until stop is called
            while (!shouldStop)
            {
                // Wait for commands to enter queue
                if (commandsQueue.Count != 0)
                {
                    using (NetworkStream stream = new NetworkStream(client.Client, false))
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        while (commandsQueue.Count != 0)
                        {
                            // Dequeue command
                            UpdateCommand command = commandsQueue.Dequeue();
                            if (command != null)
                            {
                                // Change decoding to ASCII
                                byte[] data = System.Text.Encoding.ASCII.GetBytes(command.Command);

                                // Write to socket
                                writer.Write(data);
                                writer.Flush();

                                // Read response
                                inputLine = readUntilNewLine(reader).Trim();

                                Match match = Regex.Match(inputLine, @"[\d]+\.?[\d]*");

                                val = float.Parse(match.ToString());

                                // Assign value to property
                                command.Property(val);
                            }
                        }
                    }
                }
                Thread.Sleep(1000);
            }

            // Close client socket
            client.Close();
        }
    }
}
