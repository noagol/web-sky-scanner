using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ex3.Models
{
    public class UpdateCommand
    {
        string command;
        Action<float> property;


        /// <summary>Initializes a new instance of the <see cref="UpdateCommand"/> class.</summary>
        /// <param name="path">The path.</param>
        /// <param name="property">The property.</param>
        public UpdateCommand(string path, Action<float> property)
        {
            Command = String.Format("get {0}\r\n", path);
            Property = property;
        }

        /// <summary>Gets the command.</summary>
        /// <value>The command.</value>
        public string Command
        {
            get { return command; }
            private set { command = value; }
        }

        /// <summary>Gets the property.</summary>
        /// <value>The property.</value>
        public Action<float> Property
        {
            get { return property; }
            private set { property = value; }
        }
    }
}