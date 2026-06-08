using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace NtbSoft.ERP.Libs
{
    [Serializable]
    [XmlType(TypeName = "KeyValue")]
    public class KeyValuePaird<K, V>
    {

        public K Key

        { get; set; }

        public V Value

        { get; set; }

        public KeyValuePaird() { ;}

        public KeyValuePaird(K key, V val)
        {

            Key = key;

            Value = val;

        }

    }
}
