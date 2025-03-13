using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Saylo.Centrex.Application.Common.JsonSerializers;

public class DateTimeOffsetNullableJavascriptConverter : JsonConverter<DateTimeOffset?>
{
  private const string JavascriptDateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
  
  public override bool CanConvert(Type typeToConvert)
  {
    return typeToConvert == typeof(DateTimeOffset?);
  }

  public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    var value = reader.GetString()!;
    return DateTimeOffset.TryParseExact(value,
                                        JavascriptDateTimeFormat,
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.AssumeLocal,
                                        out var date)
             ? date
             : null;
  }

  public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
  {
    if (value is null) writer.WriteNullValue();
    var dateTimeString = value?.UtcDateTime.ToString(JavascriptDateTimeFormat)!;
    writer.WriteStringValue(dateTimeString);
  }
}