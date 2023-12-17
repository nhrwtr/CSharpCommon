using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Common.IO
{
    /// <summary>
    /// XMLファイル読み込みのヘルパクラス
    /// </summary>
    public static class XmlFileHelper
    {
        /// <summary>
        /// XMLファイルから、XMLドキュメント(<see cref="XDocument"/>を生成して返す
        /// </summary>
        /// <param name="filePath">ファイル名・およびパス</param>
        /// <returns>XMLドキュメント</returns>
        public static XDocument ReadDocument(string filePath)
        {
            return XDocument.Load(filePath);
        }

        /// <summary>
        /// <see cref="XmlSerializer"/>を用いて、デシリアライズする
        /// </summary>
        /// <typeparam name="T">シリアライズする型</typeparam>
        /// <param name="filePath">ファイル名・およびパス</param>
        /// <returns>シリアライズされたオブジェクト</returns>
        public static T Serialize<T>(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("filePath", "null");
            }
            if (!File.Exists(filePath))
            {
                throw new ArgumentException(@"File does not exist", "filePath");
            }

            var serializer = new XmlSerializer(typeof(T));

            T instance = default;
            StreamReader sr;
            using (sr = new StreamReader(filePath, Encoding.UTF8))
            {
                object obj = serializer.Deserialize(sr);
                if (obj is T t)
                {
                    instance = t;
                }
            }

            return instance;
        }

        /// <summary>
        /// <see cref="DataContractSerializer"/>を用いて、デシリアライズする
        /// </summary>
        /// <typeparam name="T">シリアライズする型</typeparam>
        /// <param name="filePath">ファイル名・およびパス</param>
        /// <returns>シリアライズされたオブジェクト</returns>
        public static T ContractSerialize<T>(string filePath)
        {
            var serializer = new DataContractSerializer(typeof(T));

            T instance = default;

            XmlReader sr;
            using (sr = XmlReader.Create(filePath))
            {
                object obj = serializer.ReadObject(sr);
                if (obj is T t)
                {
                    instance = t;
                }
            }

            return instance;
        }
    }
}
