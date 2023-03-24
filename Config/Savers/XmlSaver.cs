using mToolkitPlatformComponentLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using System.IO;

namespace mToolkitFrameworkExtensions.Config.Savers
{
    /// <summary>
    /// Handles saving the configuration as an XML file.
    /// </summary>
    public class XmlSaver : mConfigSaver
    {
        public string ConfigFullPath { get { return $"{ConfigDirectory}\\{ConfigFileName}"; } }
        public readonly string ConfigDirectory;
        public readonly string ConfigFileName;
        private readonly XElement Config;
        private readonly XmlWriterSettings Settings;

        /// <summary>
        /// Initializes a new instance of the mToolConfigSaver class.
        /// </summary>
        /// <param name="configDirectory">The directory where the configuration will be saved.</param>
        /// <param name="config">The XElement representing the configuration.</param>
        public XmlSaver(string configDirectory, string filename, XElement config, XmlWriterSettings? settings = null)
        {
            ConfigDirectory = configDirectory;
            ConfigFileName = filename;
            Config = config;

            if (settings == null)
                settings = new XmlWriterSettings() { Indent = true };

            Settings = settings;
        }

        /// <summary>
        /// Saves the current configuration as an XML file to the ConfigDirectory, if it is not null.
        /// </summary>
        public void SaveConfig()
        {
            if (ConfigDirectory != null && Config != null)
            {
                Directory.CreateDirectory(ConfigDirectory);

                // Create a new XML writer for the specified file path
                using (XmlWriter writer = XmlWriter.Create($"{ConfigDirectory}\\{ConfigFileName}", Settings))
                {
                    // Write the XElement to the XML writer
                    Config.WriteTo(writer);
                }
            }
        }
    }
}
