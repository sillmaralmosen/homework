using System;
using System.IO;
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
            try
            {
                source = XDocument.Load(reader);
            }
            catch (Exception)
            {
                throw;
            }

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

        public static bool IsValid(byte[] bytes)
        {
            try
            {
                using XmlReader reader = XmlReader.Create(new MemoryStream(bytes));
                new XmlDocument().Load(reader);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
