using ProtoBuf;
using System.Text;

namespace Notino.Utils
{
    public class ProtoSerializer
    {
        public static string ProtoSerialize<T>(T record) where T : class
        {
            if (null == record) return null;

            try
            {
                using (var stream = new MemoryStream())
                {
                    Serializer.Serialize(stream, record);
                    var ttt = stream.ToArray();
                    return Encoding.Default.GetString(stream.ToArray());
                }
            }
            catch(Exception ex)
            {
                throw new Exception("serializace Proto3",ex);
            }
        }

    }
}
