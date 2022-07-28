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
                    return ByteArrayToString(stream.ToArray());
                }
            }
            catch
            {
                // Log error
                throw;
            }
        }

        private static string ByteArrayToString (byte[] bytes)
        {
            return Encoding.Default.GetString(bytes);
        }
    }
}
