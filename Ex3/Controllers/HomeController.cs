using FlightSimulator.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Ex3.Controllers
{
    public class HomeController : Controller
    {
        static Dictionary<string, ConnectModel> models = new Dictionary<string, ConnectModel>();


        /// <summary>
        ///   <para>
        ///  Displays a dot where the plane at or load path from file.
        /// </para>
        /// </summary>
        /// <param name="ipOrFilename">The ip or filename.</param>
        /// <param name="portOrUpdateInterval">The port or update interval.</param>
        /// <returns>The view</returns>
        [HttpGet]
        public ActionResult display(string ipOrFilename, int portOrUpdateInterval)
        {
            IPAddress address;
            if (IPAddress.TryParse(ipOrFilename, out address) && ipOrFilename.Count(c => c == '.') == 3)
            {
                //Valid IP, with address containing the IP
                return View("~/Views/Home/display.cshtml");
            }
            else
            {
                return View("~/Views/Home/loadDisplay.cshtml");
            }
        }

        /// <summary>Displays the path.</summary>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        /// <param name="updateInterval">The update interval.</param>
        /// <returns>The view</returns>
        [HttpGet]
        public ActionResult displayPath(string ip, int port, int updateInterval)
        {
            return View();
        }


        /// <summary>Saves the animation.</summary>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        /// <param name="updateInterval">The update interval.</param>
        /// <param name="animationTime">The animation time.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>The view</returns>
        [HttpGet]
        public ActionResult saveAnimation(string ip, int port, int updateInterval, int animationTime, string filename)
        {
            return View();
        }

        /// <summary>Saves the file.</summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        [HttpPost]
        public string saveFile(string filename)
        {
            string content;
            using (var reader = new StreamReader(Request.InputStream))
                content = reader.ReadToEnd();
            string path = Server.MapPath("~/FlightData/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            
            // Write to file
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(String.Format("{0}{1}", path, filename)))
            {
                file.Write(content);
            }

            return "";
        }


        /// <summary>Loads the file.</summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        [HttpGet]
        public string loadFile(string filename)
        {
            string content;
            string path = Server.MapPath("~/FlightData/");

            // Read from file
            using (System.IO.StreamReader file =
            new System.IO.StreamReader(String.Format("{0}{1}", path, filename)))
            {
                content = file.ReadToEnd();
            }

            return content;
        }


        /// <summary>Gets the location.</summary>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        /// <returns>Lon,Lat</returns>
        [HttpGet]
        public string GetLocation(string ip, int port)
        {
            // Get model
            string key = String.Format("{0}:{1}", ip, port);
            ConnectModel model;

            if (models.ContainsKey(key))
            {
                model = models[key];
            }
            else
            {
                model = new ConnectModel(ip, port);
                models[key] = model;
            }

            // Wait for update event
            bool lonChanged = false;
            bool latChanged = false;


            AutoResetEvent waitHandle = new AutoResetEvent(false);
            // create and attach event handler for the "Completed" event
            EventHandler eventHandler = delegate (object sender, EventArgs e)
            {
                string propertyName = ((PropertyChangedEventArgs)e).PropertyName;
                if ((propertyName == "Lat" && !latChanged))
                {
                    waitHandle.Set();
                    latChanged = true;
                }
                else if (propertyName == "Lon" && !lonChanged)
                {
                    waitHandle.Set();
                    lonChanged = true;
                }
            };

            model.positionChanged += eventHandler;

            model.updatePosition();

            waitHandle.WaitOne();
            waitHandle.WaitOne();

            float lat = model.Lat;
            float lon = model.Lon;

            // Return the position
            return String.Format("{0},{1}", lon, lat);
        }


        /// <summary>Gets the information.</summary>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        /// <returns>Throtle,Rudder</returns>
        [HttpGet]
        public string GetInfo(string ip, int port)
        {
            // Load model
            string key = String.Format("{0}:{1}", ip, port);
            ConnectModel model;

            if (models.ContainsKey(key))
            {
                model = models[key];
            }
            else
            {
                model = new ConnectModel(ip, port);
                models[key] = model;
            }

            // Wait for update event
            bool rudderChanged = false;
            bool throttleChanged = false;

            AutoResetEvent waitHandle = new AutoResetEvent(false);
            // create and attach event handler for the "Completed" event
            EventHandler eventHandler = delegate (object sender, EventArgs e)
            {
                string propertyName = ((PropertyChangedEventArgs)e).PropertyName;
                if (propertyName == "Rudder" && !rudderChanged)
                {
                    waitHandle.Set();
                    rudderChanged = true;
                }
                else if (propertyName == "Throttle" && !throttleChanged)
                {
                    waitHandle.Set();
                    throttleChanged = true;
                }
            };

            model.positionChanged += eventHandler;

            model.updateRudder();
            model.updateThrottle();

            waitHandle.WaitOne();
            waitHandle.WaitOne();

            float rudder = model.Rudder;
            float throttle = model.Throttle;

            // Return the values
            return String.Format("{0},{1}", rudder, throttle);
        }

    }
}