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
    public class mListSegment : mToolConfigSegment
    {
        public mListSegment(XElement segment, mToolConfig config) : base(segment, config)
        {
        }

        public void Add(string value)
        {
            int index = 0;

            while (GetElement(index) != null)
            {
                index++;
            }

            XElement listValue = new XElement("listvalue", new XAttribute("index", index));
            listValue.Value = value;

            Owner.Add(listValue);
            Config.Saver?.SaveConfig();
        }

        public XElement[] GetAllElements()
        {
            return Owner.Elements().Where(el => el.Name == "listvalue").ToArray();
        }

        public string[] GetAll()
        {
            return GetAllElements().Select(el => el.Value).ToArray();
        }

        public void Add(int index, string value)
        {
            XElement? element = GetElement(index);

            if (element == null)
                return;

            element.Value = value;
            Config.Saver?.SaveConfig();
        }

        public string? Get(int index)
        {
            return GetElement(index)?.Value;
        }

        public XElement? GetElement(int index)
        {
            foreach (XElement item in Owner.Elements())
            {
                if (item.Name == "listvalue")
                {
                    int itemIndex = int.Parse(item.Attribute("index").Value);

                    if (itemIndex == index)
                        return item;
                }
            }

            return null;
        }
    }
}
