using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using static mToolkitFrameworkExtensions.XElementExt;

namespace mToolkitFrameworkExtensions
{
    public class XElementExt
    {
        public static object GetAttributeValue(XElement element, string attributeName)
        {
            return element.Attribute(attributeName)?.Value ?? string.Empty;
        }

        public static void SetAttributeValue(XElement element, string attributeName, string? value)
        {
            if (value != null)
            {
                XAttribute? name = element.Attribute(attributeName);
                if (name == null)
                    element.Add(new XAttribute(attributeName, value));
                else
                    name.Value = value;
            }
        }
        public static object GetElementValue(XElement element, string elementName)
        {
            return element.Element(elementName)?.Value ?? string.Empty;
        }

        public static void SetElementValue(XElement element, string elementName, string? value)
        {
            if (value != null)
            {
                XElement? name = element.Element(elementName);
                if (name == null)
                    element.Add(new XElement(elementName, value));
                else
                    name.Value = value;
            }
        }
    }

    public class XElementExtElementValue<T>
    {
        public T Value
        {
            get
            {
                return (T)XElementExt.GetElementValue(Current, Name);
            }
            set
            {
                XElementExt.SetElementValue(Current, Name, value?.ToString());
            }
        }

        private readonly XElement Root;
        private readonly XElement Current;
        private readonly string Name;

        public XElementExtElementValue(XElement root, string name, T value)
        {
            Root = root;
            Name = name;
            Current = new XElement(name, value);
            Root.Add(Current);
        }
    }

    public class XElementExtAttributeValue<T>
    {
        public T Value
        {
            get
            {
                return (T)XElementExt.GetAttributeValue(Root, Name);
            }
            set
            {
                XElementExt.SetAttributeValue(Root, Name, value?.ToString());
            }
        }

        private readonly XElement Root;
        private readonly string Name;

        public XElementExtAttributeValue(XElement root, string name, T value)
        {
            Root = root;
            Name = name;
            Value = value;
        }

        public static implicit operator T(XElementExtAttributeValue<T> myClass)
        {
            return myClass.Value;
        }
    }
}
