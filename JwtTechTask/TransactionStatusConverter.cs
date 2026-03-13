using JwtTechTask;
using System.Text.Json;
using System.Text.Json.Serialization;

public class TransactionStatusConverter : JsonConverter<TransactionStatus>
{
    public override TransactionStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();

            if (Enum.TryParse<TransactionStatus>(value, true, out var result))
                return result;

            return TransactionStatus.Unknown;
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            var intValue = reader.GetInt32();

            if (Enum.IsDefined(typeof(TransactionStatus), intValue))
                return (TransactionStatus)intValue;

            return TransactionStatus.Unknown;
        }

        return TransactionStatus.Unknown;
    }

    public override void Write(Utf8JsonWriter writer, TransactionStatus value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}