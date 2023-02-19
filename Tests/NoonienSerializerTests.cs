using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using com.enemyhideout.noonien;
using com.enemyhideout.noonien.serializer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Tests
{
  public class NoonienSerializerTests
  {
    public class TestObject
    {
      public string Foo;
    }

    public class TestElement : Element
    {
      private string _testValue;
      public string TestValue
      {
        get
        {
          return _testValue;
        }
        set
        {
          SetProperty(value, ref _testValue);
        }
      }
      
      private TestObject _obj;
      public TestObject Obj
      {
        get
        {
          return _obj;
        }
        set
        {
          SetProperty(value, ref _obj);
        }
      }
    }
    
    public class TestElementWithNodes : Element
    {
      private Node _testNode;
      public Node TestNode
      {
        get
        {
          return _testNode;
        }
        set
        {
          SetProperty(value, ref _testNode);
        }
      }

      public Collection<TestElement> TestElements;
    }

    Node CreateTestNode()
    {
      var node = new Node(null);
      var element = node.AddElement<TestElement>();
      element.TestValue = "Hello";
      element.Obj = new TestObject() { Foo = "Fooooooo" };
      var child = node.AddNewChild("Child");
      var elementWithNodes = child.AddElement<TestElementWithNodes>();
      elementWithNodes.TestElements = new Collection<TestElement>(null);
      elementWithNodes.TestElements.Add(element);
      return node;

    }
    
    static KnownTypesBinder knownTypesBinder = new KnownTypesBinder
    {
      KnownTypes = new List<Type> { typeof(TestElement), typeof(NodeCollection), typeof(List<>) }
    };

    private static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
    {
      TypeNameHandling = TypeNameHandling.Auto,
      NullValueHandling = NullValueHandling.Ignore,
      PreserveReferencesHandling = PreserveReferencesHandling.Objects,
      ContractResolver = new ShouldSerializeContractResolver()
    };
    
    public class ShouldSerializeContractResolver : DefaultContractResolver
    {

      private object[] _constructorParams;
      private INotifyManager _notifyManager;

      public ShouldSerializeContractResolver()
      {
        _constructorParams = new object[] { null };
      }
      
      protected override JsonArrayContract CreateArrayContract(Type objectType)
      {
        var arrayContract = base.CreateArrayContract(objectType);
        //Debug.Log($"array: {objectType}");
        if (objectType == typeof(NodeCollection))
        {
          //Debug.Log("Found array type of collection!");
          arrayContract.DefaultCreator = () =>
          {
            return new Collection<Node>(null);
          };
        }

        if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Collection<>))
        {
          // Debug.Log($"generic creator for {objectType}");
          // Get the constructor that passes a notify manager in.
          Type[] types = new Type[1];
          types[0] = typeof(INotifyManager);
          ConstructorInfo constructorInfoObj = objectType.GetConstructor(
            BindingFlags.Instance | BindingFlags.Public, null,
            CallingConventions.HasThis, types, null);
          arrayContract.DefaultCreator = () => constructorInfoObj.Invoke(_constructorParams);
        }
        return arrayContract;
      }

      protected override JsonObjectContract CreateObjectContract(Type objectType)
      {
        JsonObjectContract contract = base.CreateObjectContract(objectType);
        if (objectType == typeof(Node))
        {
          contract.Converter = new NodeConverter();
          contract.DefaultCreator = () => new Node(_notifyManager);
        }
        
        return contract;
      }
      
      protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
      {
        JsonProperty property = base.CreateProperty(member, memberSerialization);

        if (property.DeclaringType == typeof(Node) && property.PropertyName == "ChildrenCount")
        {
          property.Ignored = true;
        }
        if (property.DeclaringType == typeof(Node) && property.PropertyName == "NotifyManager")
        {
          property.Ignored = true;
        }
        if (property.DeclaringType == typeof(Element) && property.PropertyName == "Parent")
        {
          property.Ignored = true;
        }
        if (property.DeclaringType == typeof(Element) && property.PropertyName == "Node")
        {
          property.Ignored = true;
        }
        if (property.DeclaringType == typeof(Element) && property.PropertyName == "Version")
        {
          property.Ignored = true;
        }
        if (property.DeclaringType == typeof(Element) && property.PropertyName == "Name")
        {
          property.Ignored = true;
        }

        
        return property;
      }
    }

    const string refProperty = "$ref";
    const string idProperty = "$id";
    public class NodeConverter : JsonConverter
    {

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
    
    

    [Test]
    public void TestJsonSerializer()
    {
      var node = CreateTestNode();
      string jsonStr = JsonConvert.SerializeObject(node, Formatting.Indented, _jsonSettings);

      Debug.Log(jsonStr);
      
      var node2 = JsonConvert.DeserializeObject<Node>(jsonStr, _jsonSettings);
      Assert.That(node2.Name, Is.EqualTo(node.Name));
      Assert.That(node2.ChildrenCount, Is.EqualTo(1));
      var child2 = node2.Children[0];
      Assert.That(child2.Parent, Is.EqualTo(node2));
      
      var element2 = node2.Get<TestElement>();
      Assert.That(element2, Is.Not.Null);
      Assert.That(node2.ElementsCount, Is.EqualTo(1));
      Assert.That(element2.Parent, Is.EqualTo(node2));

      var element2WithNodes = child2.GetElement<TestElementWithNodes>();
      Assert.That(element2WithNodes, Is.Not.Null);
      Assert.That(element2WithNodes.TestElements, Is.Not.Null);
      Assert.That(element2WithNodes.TestNode, Is.Null);

    }
  }
  
  public class KnownTypesBinder : ISerializationBinder
  {
    public IList<Type> KnownTypes { get; set; }

    public Type BindToType(string assemblyName, string typeName)
    {
      return KnownTypes.SingleOrDefault(t => t.Name == typeName);
    }

    public void BindToName(Type serializedType, out string assemblyName, out string typeName)
    {
      assemblyName = null;
      typeName = serializedType.Name;
    }
  }
  
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