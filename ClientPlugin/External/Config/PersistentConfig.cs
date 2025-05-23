﻿using System;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;
using System.Threading;
using VRage.Utils;

namespace ClientPlugin.External.Config
{
    // Ported from Torch's Persistent<T> class for compatibility of configuration files between targets and to work with IPluginLogger.
    // Simple class that manages saving <see cref="P:Torch.Persistent`1.Data" /> to disk using XML serialization.
    // Can automatically save on changes by implementing <see cref="T:System.ComponentModel.INotifyPropertyChanged" /> in the data class.
    /// <typeparam name="T">Data class type</typeparam>
    public class PersistentConfig<T> : IDisposable where T : class, INotifyPropertyChanged, new()
    {
        private T data;
        private Timer saveConfigTimer;
        private const int SaveDelay = 5000;

        private string Path { get; }

        public T Data
        {
            get => data;
            private set
            {
                if (data != null)
                    data.PropertyChanged -= OnPropertyChanged;

                data = value;
                data.PropertyChanged += OnPropertyChanged;
            }
        }

        ~PersistentConfig() => Dispose();

        private PersistentConfig(string path, T data = null)
        {
            Path = path;
            Data = data;
        }

        private void SaveLater()
        {
            if (saveConfigTimer == null)
                saveConfigTimer = new Timer(x => Save());

            saveConfigTimer.Change(SaveDelay, -1);
        }
        public void SaveNow()
        {
            Save();
            saveConfigTimer = null;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) => SaveLater();

        public static PersistentConfig<T> Load(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    using (var streamReader = File.OpenText(path))
                        return new PersistentConfig<T>(path, (T)xmlSerializer.Deserialize(streamReader));
                }
            }
            catch (Exception)
            {
                try
                {
                    var timestamp = DateTime.Now.ToString("yyyyMMdd-hhmmss");
                    var corruptedPath = $"{path}.corrupted.{timestamp}.txt";
                    File.Move(path, corruptedPath);
                }
                catch (Exception)
                {
                    // Ignored
                }
            }

            var config = new PersistentConfig<T>(path, new T());
            config.Save();
            return config;
        }

        private void Save(string path = null)
        {
            if (path == null)
                path = Path;

            // NOTE: There is a minimal chance of inconsistency here if the config data
            // is changed concurrently, but it is negligible in practice. Also, it would be
            // corrected by the next scheduled save operation after SaveDelay milliseconds.
            using (var text = File.CreateText(path))
                new XmlSerializer(typeof(T)).Serialize(text, Data);
        }

        public void Dispose()
        {
            try
            {
                if (Data is INotifyPropertyChanged d)
                    d.PropertyChanged -= OnPropertyChanged;

                saveConfigTimer?.Dispose();
                Save();
            }
            catch
            {
                // Ignored
            }
        }
    }
}
