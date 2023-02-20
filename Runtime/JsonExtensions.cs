﻿using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace com.enemyhideout.noonien.serializer
{
  public static partial class JsonExtensions
  {
    public static JsonReader MoveToContent(this JsonReader reader)
    {
      if (reader.TokenType == JsonToken.None)
        reader.Read();
      while (reader.TokenType == JsonToken.Comment && reader.Read())
        ;
      return reader;
    }

    public static JToken RemoveFromLowestPossibleParent(this JToken node)
    {
      if (node == null)
        return null;
      var contained = node.AncestorsAndSelf().Where(t => t.Parent is JContainer && t.Parent.Type != JTokenType.Property).FirstOrDefault();
      if (contained != null)
        contained.Remove();
      // Also detach the node from its immediate containing property -- Remove() does not do this even though it seems like it should
      if (node.Parent is JProperty)
        ((JProperty)node.Parent).Value = null;
      return node;
    }
  }
}