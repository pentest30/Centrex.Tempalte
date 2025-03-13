using System.Globalization;
using System.Text.Json;

namespace Saylo.Centrex.Application.Common.JsonSerializers;

public class DateTimeOffsetJavascriptConverter : System.Text.Json.Serialization.JsonConverter<DateTimeOffset>
{
  private const string JavascriptDateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

  public override bool CanConvert(Type typeToConvert)
  {
    return typeToConvert == typeof(DateTimeOffset);
  }

  public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    var value = reader.GetString()!;
    return DateTimeOffset.ParseExact(value, JavascriptDateTimeFormat, CultureInfo.InvariantCulture);
  }

  public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
  {
    var dateTimeString = value.UtcDateTime.ToString(JavascriptDateTimeFormat);
    writer.WriteStringValue(dateTimeString);
  }
}