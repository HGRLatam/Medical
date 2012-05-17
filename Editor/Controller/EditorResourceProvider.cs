﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Engine.Saving;
using Engine.Saving.XMLSaver;
using System.Xml;
using MyGUIPlugin;
using Logging;

namespace Medical
{
    public class EditorResourceProvider : ResourceProvider, IDisposable
    {
        private static XmlSaver xmlSaver = new XmlSaver();
        private String parentPath;
        private FileSystemWatcher fileWatcher;

        public EditorResourceProvider(String path)
        {
            this.parentPath = path;
            if (Directory.Exists(parentPath))
            {
                fileWatcher = new FileSystemWatcher(parentPath);
                fileWatcher.IncludeSubdirectories = true;
                fileWatcher.EnableRaisingEvents = true;
            }
        }

        public void Dispose()
        {
            fileWatcher.Dispose();
        }

        public Saveable openSaveable(String filename)
        {
            filename = Path.Combine(parentPath, filename);
            using (XmlTextReader xmlReader = new XmlTextReader(File.Open(filename, FileMode.Open, FileAccess.Read)))
            {
                return (Saveable)xmlSaver.restoreObject(xmlReader);
            }
        }

        public void saveSaveable(String filename, Saveable saveable)
        {
            filename = Path.Combine(parentPath, filename);
            using (Stream stream = File.Open(filename, FileMode.Create, FileAccess.Write))
            {
                using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.Default))
                {
                    writer.Formatting = Formatting.Indented;
                    EditorController.XmlSaver.saveObject(saveable, writer);
                }
            }
        }

        public Stream openFile(string filename)
        {
            return File.OpenRead(Path.Combine(parentPath, filename));
        }

        public void addStream(string filename, MemoryStream memoryStream)
        {
            using (FileStream fileStream = new FileStream(Path.Combine(parentPath, filename), FileMode.Create))
            {
                memoryStream.WriteTo(fileStream);
            }
        }

        public void addFile(string path)
        {
            String filename = Path.GetFileName(path);
            File.Copy(path, Path.Combine(parentPath, filename), true);
        }

        public void deleteFile(String filename)
        {
            File.Delete(Path.Combine(parentPath, filename));
        }

        public string[] listFiles(string pattern)
        {
            String[] files = Directory.GetFiles(parentPath, pattern, SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; ++i)
            {
                files[i] = files[i].Remove(0, parentPath.Length + 1);
            }
            return files;
        }

        public String[] listFiles(String pattern, String directory, bool recursive)
        {
            String[] files = Directory.GetFiles(Path.Combine(parentPath, directory), pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; ++i)
            {
                files[i] = files[i].Remove(0, parentPath.Length + 1);
            }
            return files;
        }

        public bool exists(string path)
        {
            if (!path.StartsWith(parentPath))
            {
                path = Path.Combine(parentPath, path);
            }
            return File.Exists(path);
        }

        public String getFullFilePath(String filename)
        {
            return Path.Combine(parentPath, filename);
        }

        public string BackingLocation
        {
            get
            {
                return parentPath;
            }
        }

        public event FileSystemEventHandler FileChanged
        {
            add
            {
                fileWatcher.Changed += value;
            }
            remove
            {
                fileWatcher.Changed -= value;
            }
        }

        public event FileSystemEventHandler FileDeleted
        {
            add
            {
                fileWatcher.Deleted += value;
            }
            remove
            {
                fileWatcher.Deleted -= value;
            }
        }

        public event FileSystemEventHandler FileCreated
        {
            add
            {
                fileWatcher.Created += value;
            }
            remove
            {
                fileWatcher.Created -= value;
            }
        }

        public event RenamedEventHandler FileRenamed
        {
            add
            {
                fileWatcher.Renamed += value;
            }
            remove
            {
                fileWatcher.Renamed -= value;
            }
        }
    }
}
