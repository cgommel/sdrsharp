using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace SDRSharp.FrequencyManager
{
    public class SettingsPersister
    {
        private const string FreqMgrFilename = "frequencies.xml";

        private readonly string _settingsFolder;

        public SettingsPersister()
        {
            //_settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SDRSharp");
            _settingsFolder = Application.ExecutablePath;
        }

        public List<MemoryEntry> ReadStoredFrequencies()
        {
            var result = ReadObject<List<MemoryEntry>>(FreqMgrFilename);
            if (result != null)
            {
                result.Sort(delegate(MemoryEntry e1, MemoryEntry e2)
                {
                    return e1.Frequency.CompareTo(e2.Frequency);
                });
                return result;
            }
            return new List<MemoryEntry>();
        }

        public void PersistStoredFrequencies(List<MemoryEntry> entries)
        {
            WriteObject(entries, FreqMgrFilename);
        }

        private T ReadObject<T>(string fileName)
        {
            var filePath = Path.Combine(_settingsFolder, fileName);
            if (File.Exists(filePath))
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open))
                {
                    var ser = new XmlSerializer(typeof(T));
                    return (T)ser.Deserialize(fileStream);
                }
            }
            return default(T);
        }

        private void WriteObject<T>(T obj, string fileName)
        {
            var filePath = Path.Combine(_settingsFolder, fileName);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                var ser = new XmlSerializer(obj.GetType());
                ser.Serialize(fileStream, obj);
            }
        }
    }
}
