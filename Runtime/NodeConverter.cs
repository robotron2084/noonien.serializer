using System;
using com.enemyhideout.noonien;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace com.enemyhideout.noonien.serializer
{
  public class NodeConverter : JsonConverter
  {
    const string refProperty = "$ref";
    const string idProperty = "$id";

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      var contract = serializer.ContractResolver.ResolveContract(objectType);
      if (!(contract is JsonObjectContract))
      {
        throw new JsonSerializationException(string.Format("Invalid non-object contract type {0}", contract));
      }
      if (!(existingValue == null || existingValue is Node))
      {
        throw new JsonSerializationException(string.Format("Converter cannot read JSON with the specified existing value. {0} is required.", typeof(Node)));
      }

      if (reader.MoveToContent().TokenType == JsonToken.Null)
      {
        return null;
      }
      var obj = JObject.Load(reader);

      var refId = (string)obj[refProperty].RemoveFromLowestPossibleParent();
      var objId = (string)obj[idProperty].RemoveFromLowestPossibleParent();
      if (refId != null)
      {
        var reference = serializer.ReferenceResolver.ResolveReference(serializer, refId);
        if (reference != null)
        {
          return reference;
        }
      }

      var value = Create(objectType, (Node)existingValue, serializer, obj);

      if (objId != null)
      {
        // Add the empty array into the reference table BEFORE populating it, to handle recursive references.
        serializer.ReferenceResolver.AddReference(serializer, objId, value);
      }

      Populate(obj, value, serializer);

      return value;
    }
      
    protected virtual Node Create(Type objectType, Node existingValue, JsonSerializer serializer, JObject obj)
    {
      return existingValue ?? (Node)serializer.ContractResolver.ResolveContract(objectType).DefaultCreator();
    }
      
    protected void Populate(JObject obj, Node value, JsonSerializer serializer)
    {
      using (var reader = obj.CreateReader())
      {
        reader.Read();
        serializer.Populate(reader, value);
        foreach (var element in value.Elements)
        {
          element.Parent = value;
        }
      }
    }

    public override bool CanWrite
    {
      get => false;
    }

    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof(Node);
    }
  }
}