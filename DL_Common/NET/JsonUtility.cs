using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO;

namespace DL_CommonLibrary
{
    /// <summary>
    /// シリアライズに関係する汎用機能を提供するクラス 
    /// </summary>
    public static class JsonUtility
    {
        /// <summary>
        /// Json形式でファイル保存します
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static void SerializeToFile(string filePath, object obj)
        {
            string buf = Serialize(obj);
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.Write(buf);
                sw.Close();
            }
        }
        /// <summary>
        /// 任意のオブジェクトをJsonメッセージへシリアライズします。
        /// </summary>
        public static string Serialize(object obj)
        {
            using (var stream = new MemoryStream())
            {
                var setting = new DataContractJsonSerializerSettings()
                {
                    UseSimpleDictionaryFormat = true,
                };
                var serializer = new DataContractJsonSerializer(obj.GetType(), setting);
                serializer.WriteObject(stream, obj);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// Jsonファイルからオブジェクトへデシリアライズします。
        /// </summary>
        public static T DeserializeFromFile<T>(string filePath)
        {
            string buf = System.IO.File.ReadAllText(filePath);
            return Deserialize<T>(buf);
        }

        /// <summary>
        /// Jsonメッセージをオブジェクトへデシリアライズします。
        /// </summary>
        public static T Deserialize<T>(string message)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(message)))
            {
                var setting = new DataContractJsonSerializerSettings()
                {
                    UseSimpleDictionaryFormat = true,
                };
                var serializer = new DataContractJsonSerializer(typeof(T), setting);
                return (T)serializer.ReadObject(stream);
            }
        }



    }
}
