using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.IO;

namespace TouhouSpring
{
    public static class ExtSerialization
    {
        public static void XmlSerializeToXml<T>(T obj, string fileName)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate);
            ser.Serialize(fileStream, obj);
            fileStream.Close();
        }

        public static T XmlDeserializeFromXml<T>(string xml)
        {
            T result;
            XmlSerializer ser = new XmlSerializer(typeof(T));
            using (TextReader tr = new StringReader(xml))
            {
                result = (T)ser.Deserialize(tr);
            }
            return result;
        }

        public static T XmlDeserializeFromFilePath<T>(string fileName)
        {
            string xml = File.ReadAllText(fileName);
            return XmlDeserializeFromXml<T>(xml);
        }

        public static string DataContractSerializeToXml(object obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (StreamReader reader = new StreamReader(memoryStream))
            {
                DataContractSerializer serializer = new DataContractSerializer(obj.GetType());
                serializer.WriteObject(memoryStream, obj);
                memoryStream.Position = 0;
                return reader.ReadToEnd();
            }
        }

        public static object DataContractDeserializeFromXml(string xml, Type toType)
        {
            using (Stream stream = new MemoryStream())
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                DataContractSerializer deserializer = new DataContractSerializer(toType);
                return deserializer.ReadObject(stream);
            }
        }
    }
}
