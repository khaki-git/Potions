using System.Text;
using Zorro.Core.Serizalization;

namespace Potions;

public class StringItemData : DataEntryValue
{
    public string Value;

    public override void SerializeValue(BinarySerializer serializer)
    {
        serializer.WriteString(Value, Encoding.ASCII);
    }

    public override void DeserializeValue(BinaryDeserializer deserializer)
    {
        Value = deserializer.ReadString(Encoding.ASCII);
    }

    public override string ToString()
    {
        return Value;
    }
}