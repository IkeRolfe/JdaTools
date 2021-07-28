using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static System.Environment;

namespace JdaTools.ConfigurationManager
{
    public class XMLManager
    {
        public Configurations _configs;
        public List<ConnectionInfo> Connections => _configs.Connections;
        public void ReadConfigurationFile(string filePath)
        {
            try
            {
                var commonpath = GetFolderPath(SpecialFolder.CommonApplicationData);
                var path = Path.Combine(commonpath, "JdaStudio", filePath);
                _configs = ReadXML<Configurations>(path);
            }
            catch(Exception e)
            {
                _configs = new Configurations();
            }
            _configs.Connections.Add(new ConnectionInfo{
                Name = "New Connection",
                Default = _configs.Connections.Count == 0? true : false, //If there are no saved configs, make this new one the default
                Url = ""
            });
        }

        public async Task SetDefault(ConnectionInfo connection)
        {
            if (_configs.Connections.Contains(connection))
            {
                foreach (var con in _configs.Connections)
                {
                    if (con.Name == connection.Name && con.Url == connection.Url)
                    {
                        con.Default = true;
                    }
                    else
                    {
                        con.Default = false;
                    }
                }
            }
            else
            {
                _configs.Connections.Remove(_configs.Connections.Last());
                _configs.Connections.Add(connection);
            }
            
        }

        public async Task WriteConfigurationFile(string fileName)
        {
            //TODO: Better handling of new conn this is a work around for a "New Connection" profile always being created
            _configs.Connections.RemoveAll(c => string.IsNullOrEmpty(c.Url));
            WriteXML(_configs, fileName);
        }
        private static T ReadXML<T>(string filePath)
        {
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open))
                {
                    var xsz = new XmlSerializer(typeof(T));
                    return (T)xsz.Deserialize(stream);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw e;
            }
            return default;
        }

        private static bool WriteXML<T>(T classToSave, string fileName)
        {
            try
            {
                var commonpath = GetFolderPath(SpecialFolder.CommonApplicationData);
                var filePath = Path.Combine(commonpath, "JdaStudio");
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                using (FileStream stream = new FileStream(Path.Combine(filePath, fileName), FileMode.Create))
                {
                    var xsz = new XmlSerializer(typeof(T));
                    xsz.Serialize(stream, classToSave);
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw e;
                return false;
            }

        }
    }
}
