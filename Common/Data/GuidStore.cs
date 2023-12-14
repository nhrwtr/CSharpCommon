using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using Common.Extensions;

namespace Common.Data
{
    /// <summary>
    /// globally unique identifier (GUID) generate and managed GUID list class.
    /// </summary>
    public class GuidStore
    {
        private static readonly HashSet<Guid> _created = new HashSet<Guid>();

        /// <summary>
        /// Generates a <see cref="Guid"/> structure. 
        /// </summary>
        /// <returns><see cref="Guid"/> structure</returns>
        public Guid NewGuid()
        {
            Guid guid;
            while (true)
            {
                guid = Guid.NewGuid();
                if (_created.Add(guid))
                {
                    break;
                }
            }
            return guid;
        }

        /// <summary>
        /// clear all items from GUID manage list.
        /// </summary>
        public void Clear()
        {
            _created.Clear();
        }

        /// <summary>
        /// remove from GUID manage list.
        /// </summary>
        /// <param name="guid">GUID structure</param>
        /// <returns>true: successed | false: failued</returns>
        public bool Remove(Guid guid)
        {
            return _created.Remove(guid);
        }

        /// <summary>
        /// add item for GUID manage list.
        /// </summary>
        /// <param name="guid">GUID structure</param>
        /// <returns>true: successed | false: failued</returns>
        public bool Add(Guid guid)
        {
            return Add(in guid);
        }

        /// <summary>
        /// add item for Guid manage list.
        /// </summary>
        /// <param name="input">GUID</param>
        /// <returns>true: successed | false: failued</returns>
        public bool Add(string input)
        {
            if (!Guid.TryParse(input, out Guid guid))
            {
                return false;
            }

            return Add(in guid);
        }

        /// <summary>
        /// parse GUID string list.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="delimiter"></param>
        public void From(string content, string delimiter = ",")
        {
            string[] arr = content.Split(StringSplitOptions.RemoveEmptyEntries, delimiter);

            foreach (var i in arr)
            {
                Add(i);
            }
        }

        public void From(DataTable dt, string columnName)
        {
            foreach(DataRow row in dt.Rows)
            {
                string guid = row[columnName] as string;
                Add(guid);
            }
        }

        public void From(XmlReader xml, string name, XmlNodeType nameNodeType = XmlNodeType.Element)
        {
            int targetDepth = -1;

            while(xml.Read())
            {
                switch(xml.NodeType)
                {
                    case XmlNodeType.Element:
                        // Only nodes at the same depth are explored.
                        if (targetDepth < 0 || targetDepth == xml.Depth) {

                            if (nameNodeType == XmlNodeType.Element && name == xml.LocalName)
                            {
                                // element
                                if (xml.HasValue)
                                {
                                    Add(xml.Value);
                                    if (targetDepth < 0)
                                    {
                                        // depth is setting
                                        targetDepth = xml.Depth;
                                    }
                                }
                            }
                            else if (nameNodeType == XmlNodeType.Attribute)
                            {
                                // attribute
                                if (xml.HasAttributes)
                                {
                                    while (xml.MoveToNextAttribute())
                                    {
                                        if (name == xml.Name && xml.HasValue)
                                        {
                                            Add(xml.Value);
                                            if (targetDepth < 0)
                                            {
                                                // depth is setting
                                                targetDepth = xml.Depth;
                                            }
                                            break;
                                        }
                                    }
                                }
                                xml.MoveToElement();
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public IEnumerable<Guid> Export()
        {
            foreach(var i in _created)
            {
                yield return i;
            }
        }

        /// <summary>
        /// GUID manage list to string.
        /// </summary>
        /// <param name="delimiter"></param>
        public string ToString(string delimiter)
        {
            var sb = new StringBuilder();
            foreach (var i in _created)
            {
                sb.Append(i.ToString());
                sb.Append(delimiter);
            }
            return sb.ToString();
        }

        /// <summary>
        /// GUID manage list to string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(",");
        }

        private bool Add(in Guid guid)
        {
            bool defined = _created.Contains(guid);
            if (!defined)
            {
                _created.Add(guid);
            }

            // no defined is add success, defined is no success.
            return !defined;
        }
    }
}
