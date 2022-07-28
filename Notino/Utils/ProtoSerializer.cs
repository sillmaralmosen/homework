using ProtoBuf;

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
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }
}
