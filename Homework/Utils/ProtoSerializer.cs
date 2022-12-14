using System.Text;
using ProtoBuf;

namespace Homework.Utils;

public class ProtoSerializer
{
    public static string ProtoSerialize<T>(T record) where T : class
    {
        using (var stream = new MemoryStream())
        {
            Serializer.Serialize(stream, record);
            var ttt = stream.ToArray();
            return Encoding.Default.GetString(stream.ToArray());
        }
    }
}