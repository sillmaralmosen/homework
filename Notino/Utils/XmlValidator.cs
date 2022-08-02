using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Notino.Utils
{
    public static class XmlValidator
    {
        public static void XmlByXsd(byte[] bytes)
        {
            XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();

            xmlSchemaSet.Add("validace-schema", "validace.xsd");

            XmlReader reader = XmlReader.Create(new MemoryStream(bytes));
            XDocument source;
  
            source = XDocument.Load(reader);
  
            source.Validate(xmlSchemaSet, XmlByXsdValidationEventHandler!);
        }

        private static void XmlByXsdValidationEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    throw new Exception();
                default:
                    throw new ArgumentOutOfRangeException();
                case XmlSeverityType.Warning:
                    break;
            }
        }
    }
}
