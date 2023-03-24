using Ionic.Zip;
using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Printing.IndexedProperties;
using System.Security.Policy;
using System.Text;
using System.Threading;

namespace mToolkitFrameworkExtensions
{
    public static class ZipExt
    {
        /// <summary>
        /// Represents the type of a zip entry.
        /// </summary>
        public enum ZipEntryType
        {
            File = 0,
            Directory = 1,
            Root = 2
        }

        /// <summary>
        /// Represents a zip entry, which can be a file or directory.
        /// </summary>
        public class ZipEntry
        {
            /// <summary>
            /// Gets or sets the name of the zip entry.
            /// </summary>
            public string Name { get; set; } = "";

            /// <summary>
            /// Gets or sets the type of the zip entry.
            /// </summary>
            public ZipEntryType Type { get; set; } = ZipEntryType.File;

            /// <summary>
            /// Gets or sets the file extension of the zip entry, if it is a file.
            /// </summary>
            public string? Ext { get; set; }

            /// <summary>
            /// Gets or sets the content of the zip entry, if it is a file.
            /// </summary>
            public byte[]? Content { get; set; }

            /// <summary>
            /// Initializes a new instance of the ZipEntry class as a file.
            /// </summary>
            /// <param name="name">The name of the file.</param>
            /// <param name="ext">The file extension.</param>
            /// <param name="content">The content of the file.</param>
            public ZipEntry(string name, string ext, string content)
            {
                Name = name;
                Type = ZipEntryType.File;
                Ext = ext;
                Content = UTF8Encoding.UTF8.GetBytes(content);
            }

            /// <summary>
            /// Initializes a new instance of the ZipEntry class as a file.
            /// </summary>
            /// <param name="name">The name of the file.</param>
            /// <param name="ext">The file extension.</param>
            /// <param name="content">The content of the file.</param>
            public ZipEntry(string name, string ext, byte[] content)
            {
                Name = name;
                Type = ZipEntryType.File;
                Ext = ext;
                Content = content;
            }

            /// <summary>
            /// Initializes a new instance of the ZipEntry class as a directory.
            /// </summary>
            /// <param name="name">The name of the directory.</param>
            public ZipEntry(string name)
            {
                Name = name;
                Type = ZipEntryType.Directory;
            }

            private List<ZipEntry> children = new List<ZipEntry>();

            /// <summary>
            /// Adds a child entry to the current entry.
            /// </summary>
            /// <param name="child">The child entry to add.</param>
            /// <param name="childs">Additional child entries to add (optional).</param>
            /// <returns>The current entry with the added child(ren).</returns>
            public ZipEntry Add(ZipEntry child, ZipEntry[]? childs = null)
            {
                if (childs != null && childs.Length > 0)
                    foreach (ZipEntry c in childs)
                        child.Add(c);

                children.Add(child);
                return this;
            }

            /// <summary>
            /// Adds a file entry to the current entry.
            /// </summary>
            /// <param name="name">The name of the file entry.</param>
            /// <param name="ext">The file extension.</param>
            /// <param name="content">The content of the file.</param>
            /// <returns>The current entry with the added file entry.</returns>
            public ZipEntry Add(string name, string ext, string content, out ZipEntry creation)
            {
                creation = new ZipEntry(name, ext, content);
                return Add(creation);
            }

            /// <summary>
            /// Adds a directory entry to the current entry.
            /// </summary>
            /// <param name="name">The name of the directory entry.</param>
            /// <param name="childs">The child entries to add to the directory (optional).</param>
            /// <returns>The current entry with the added directory entry.</returns>
            public ZipEntry Add(string name, out ZipEntry creation, params ZipEntry[] childs)
            {
                creation = new ZipEntry(name);
                return Add(creation, childs);
            }

            /// <summary>
            /// Generates the file or directory represented by the current entry.
            /// </summary>
            /// <param name="path">The base path where the file or directory should be generated.</param>
            public void Generate(string path)
            {
                if (Type == ZipEntryType.File)
                {
                    GenerateFile(path);
                }
                else if (Type == ZipEntryType.Directory || Type == ZipEntryType.Root)
                {
                    GenerateDirectory(path);
                }
            }

            /// <summary>
            /// Creates a new file entry.
            /// </summary>
            /// <param name="name">The name of the file entry.</param>
            /// <param name="ext">The file extension.</param>
            /// <param name="content">The content of the file.</param>
            /// <returns>A new file entry instance.</returns>
            public static ZipEntry CreateFile(string name, string ext, string content)
            {
                return new ZipEntry(name, ext, content);
            }

            /// <summary>
            /// Creates a new directory entry.
            /// </summary>
            /// <param name="name">The name of the directory entry.</param>
            /// <param name="childs">The child entries to add to the directory (optional).</param>
            /// <returns>A new directory entry instance.</returns>
            public static ZipEntry CreateDirectory(string name, params ZipEntry[] childs)
            {
                ZipEntry entry = new ZipEntry(name);

                foreach (ZipEntry c in childs)
                    entry.children.Add(c);

                return entry;
            }

            /// <summary>
            /// Creates a new root entry.
            /// </summary>
            /// <returns>A new root entry instance.</returns>
            public static ZipEntry CreateRoot()
            {
                ZipEntry entry = new ZipEntry("Root");
                entry.Type = ZipEntryType.Root;

                return entry;
            }

            /// <summary>
            /// Generates the file represented by the current entry.
            /// </summary>
            /// <param name="path">The base path where the file should be generated.</param>
            private void GenerateFile(string path)
            {
                if (Content == null)
                    throw new Exception($"Cannot create File ZipEntry without content: {Name}.{Ext ?? string.Empty}");

                string filePath = $"{path}\\{Name}.{Ext}";

                using (FileStream file = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    file.Write(Content);
                }
            }

            /// <summary>
            /// Generates the directory represented by the current entry and its children.
            /// </summary>
            /// <param name="path">The base path where the directory should be generated.</param>
            private void GenerateDirectory(string path)
            {
                if (Type != ZipEntryType.Root)
                    path = $"{path}\\{Name}";

                Directory.CreateDirectory(path);
                int count = children.Count;

                for (int i = 0; i < count; i++)
                {
                    children[i].Generate(path);
                }
            }
        }

        /// <summary>
        /// Represents a structure for creating a zip file from a given ZipEntry.
        /// </summary>
        public class ZipStructure
        {
            // The root ZipEntry of the structure.
            private ZipEntry root;

            /// <summary>
            /// Initializes a new instance of the ZipStructure class with a root entry.
            /// </summary>
            /// <param name="entry">The root entry of the zip structure.</param>
            public ZipStructure(ZipEntry entry)
            {
                root = entry;
                root.Type = ZipEntryType.Root;
            }

            /// <summary>
            /// Creates a zip file from the ZipStructure.
            /// </summary>
            /// <param name="path">The path where the zip file should be saved.</param>
            /// <param name="filename">The name of the zip file without extension.</param>
            /// <param name="ext">The file extension (default is "zip").</param>
            public void CreateZip(string path, string filename, string ext = "zip")
            {
                string directoryPath = $"{path}\\Temp\\ZipExt\\{filename}";

                if (Directory.Exists(directoryPath))
                    Directory.Delete(directoryPath, true);

                DirectoryInfo tempDirectory = Directory.CreateDirectory(directoryPath);
                root.Generate(directoryPath);

                using (ZipFile zip = new ZipFile())
                {
                    string[] files = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);

                    foreach (string file in files)
                    {
                        FileInfo fi = new FileInfo(file);
                        string pathInZip = file.Replace(directoryPath, "").Replace(fi.Name, "");
                        zip.AddFile(file, pathInZip);
                    }

                    zip.Save($"{path}\\{filename}.{ext}");
                }

                Directory.Delete(tempDirectory.FullName, true);
            }

            /// <summary>
            /// Creates a zip file from the given ZipEntry.
            /// </summary>
            /// <param name="zip">The root ZipEntry of the zip structure.</param>
            /// <param name="path">The path where the zip file should be saved.</param>
            /// <param name="filename">The name of the zip file without extension.</param>
            /// <param name="ext">The file extension (default is "zip").</param>
            public static void Create(ZipEntry zip, string path, string filename, string ext = "zip")
            {
                ZipStructure structure = new ZipStructure(zip);
                structure.CreateZip(path, filename, ext);
            }
        }
    }
}
