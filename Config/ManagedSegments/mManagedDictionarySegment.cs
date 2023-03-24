using mToolkitPlatformComponentLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace mToolkitFrameworkExtensions.Config.ManagedSegments
{
    public class mManagedDictionarySegment
    {
        public  readonly string Name;
        private readonly mSegmentHandler Owner;
        private readonly mDictionarySegment? dictionary;

        public mManagedDictionarySegment(mSegmentHandler owner, string name)
        {
            Name = name;
            Owner = owner;
            dictionary = Owner.CreateSegment<mDictionarySegment>(name, Owner.Owner);
        }

        protected void Remove(string directory, string key)
        {
            dictionary?.RemoveElement(GetPath(directory, key));
        }

        protected string? Get(string directory, string key)
        {
            return dictionary?.Get(GetPath(directory, key));
        }

        protected void Put(string directory, string key, string value)
        {
            dictionary?.Put(GetPath(directory, key), value);
        }

        protected XElement? GetElement(string directory, string key)
        {
            return dictionary?.GetElement(GetPath(directory, key));
        }

        protected void PutElement(string directory, string key, XElement value)
        {
            dictionary?.PutElement(GetPath(directory, key), value);
        }

        protected string GetPath(string directory, string key)
        {
            return $"{directory}/{key}";
        }

        protected string GetDirectory(string path)
        {
            return path.Split('/')[0];
        }

        protected string[] GetAllKeys(string directory)
        {
            return dictionary?.GetAllElements()
                .Where(
                    el =>
                        el?.Attribute("key")?.Value.StartsWith($"{directory}/") ?? false)
                .Select(el =>
                {
                    string val = el?.Attribute("key")?.Value ?? string.Empty;
                    return val.Replace($"{directory}/", "");
                })
                .ToArray() ?? new string[] { $"ERROR GETTING KEYS {directory}" };
        }

        protected string[] GetAll(string directory)
        {
            return dictionary?.GetAllElements()
                .Where(
                    el =>
                        el?.Attribute("key")?.Value.StartsWith($"{directory}/") ?? false)
                .Select(el => el.Value)
                .ToArray() ?? new string[] { $"ERROR GETTING DIRECTORY {directory}" };
        }
    }
}
