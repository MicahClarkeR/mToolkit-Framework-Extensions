using mToolkitPlatformComponentLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static mToolkitPlatformComponentLibrary.mToolConfig;

namespace mToolkitFrameworkExtensions.Config
{
    /// <summary>
    /// Represents a segment of type dictionary in the tool configuration file.
    /// </summary>
    public class mDictionarySegment : mToolConfigSegment
    {
        public mDictionarySegment(XElement segment, mToolConfig config) : base(segment, config)
        {
        }

        /// <summary>
        /// Puts a new key-value pair in the dictionary segment, or replaces an existing value if the key already exists.
        /// </summary>
        /// <param name="key">The key of the pair.</param>
        /// <param name="value">The value of the pair.</param>
        public void Put(string key, string value)
        {
            int index = 0;
            XElement? listValue = GetElement(key);

            if (listValue != null)
            {
                listValue.Remove();
            }

            listValue = new XElement("dictionaryvalue", new XAttribute("key", key));
            listValue.Value = value;

            Owner.Add(listValue);
            Config.Saver?.SaveConfig();
        }

        /// <summary>
        /// Gets all the dictionary values as a list of XElement objects.
        /// </summary>
        /// <returns>An array of XElement objects that represent all the values in the dictionary segment.</returns>
        public XElement[] GetAllElements()
        {
            return Owner.Elements().Where(el => el.Name == "dictionaryvalue").ToArray();
        }

        /// <summary>
        /// Gets all the dictionary values as an array of strings.
        /// </summary>
        /// <returns>An array of strings that represent all the values in the dictionary segment.</returns>
        public string[] GetAll()
        {
            return GetAllElements().Select(el => el.Value).ToArray();
        }

        /// <summary>
        /// Gets all the dictionary keys as an array of strings.
        /// </summary>
        /// <returns>An array of strings that represent all the keys in the dictionary segment.</returns>
        public string[] GetAllKeys()
        {
            return GetAllElements().Select(el => el.Attribute("key").Value).ToArray();
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <returns>The value associated with the specified key, or null if the key is not found.</returns>
        public string? Get(string key)
        {
            return GetElement(key)?.Value;
        }

        /// <summary>
        /// Puts a new key-value pair in the dictionary segment as an XElement object.
        /// </summary>
        /// <param name="key">The key of the pair.</param>
        /// <param name="value">The XElement object that represents the value of the pair.</param>
        public void PutElement(string key, XElement value)
        {
            Owner.Add(new XElement("dictionaryvalue", new XAttribute("key", key), value));
        }

        /// <summary>
        /// Removes the key-value pair associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the pair to remove.</param>
        public void RemoveElement(string key)
        {
            Owner.Elements().Single(el => el.Name.ToString().ToLower() == "dictionaryvalue" && el.Attribute("key")?.Value == key)?.Remove();
            Config.Saver?.SaveConfig();
        }

        /// <summary>
        /// Gets the XElement object that represents the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <returns>The XElement object that represents the value associated with the specified key, or null if the key is not found.</returns>
        public XElement? GetElement(string key)
        {
            foreach (XElement item in Owner.Elements())
            {
                if (item.Name == "dictionaryvalue")
                {
                    string itemIndex = item.Attribute("key").Value;

                    if (itemIndex == key)
                        return item;
                }
            }

            return null;
        }
    }

}
