using ProtoBuf;
using System.Xml.Serialization;

namespace Homework.Model
{
    [XmlRoot("root")]
    [ProtoContract]
    public class Document
    {
        [ProtoMember(1)]
        [XmlElement("title")]
        public string Title { get; set; }
        [ProtoMember(2)]
        [XmlElement("text")]
        public string Text { get; set; }
    }
}
