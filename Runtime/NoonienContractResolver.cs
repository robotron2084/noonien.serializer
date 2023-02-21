using System;
using System.Collections.Generic;
using System.Reflection;
using com.enemyhideout.noonien;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace com.enemyhideout.noonien.serializer
{
  public class NoonienContractResolver : DefaultContractResolver
  {
    private object[] _constructorParams;
    private INotifyManager _notifyManager;

    public NoonienContractResolver(INotifyManager notifyManager)
    {
      _notifyManager = notifyManager;
      _constructorParams = new object[] { _notifyManager };
    }
      
    protected override JsonArrayContract CreateArrayContract(Type objectType)
    {
      var arrayContract = base.CreateArrayContract(objectType);
      if (objectType == typeof(NodeCollection))
      {
        arrayContract.DefaultCreator = () =>
        {
          return new Collection<Node>(null);
        };
      }

      if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Collection<>))
      {
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

    private Dictionary<Type, HashSet<string>> ignoredProperties = new Dictionary<Type, HashSet<string>>()
    {
      {typeof(Node), new HashSet<string>()
      {
        nameof(Node.ChildrenCount),
        nameof(Node.ElementsCount),
        nameof(Node.NotifyManager),
      }},
      {typeof(Element), new HashSet<string>()
      {
        nameof(Element.Parent),
        nameof(Element.Node),
        nameof(Element.Version),
        nameof(Element.Name)
      }}
    };

    private static bool IsPropertyIgnored(Type type, String propertyName, Dictionary<Type, HashSet<string>> propertyMap)
    {
      if (propertyMap.TryGetValue(type, out var hashSet))
      {
        return hashSet.Contains(propertyName);
      }

      return false;
    }
    
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
      JsonProperty property = base.CreateProperty(member, memberSerialization);
      if (IsPropertyIgnored(property.DeclaringType, property.PropertyName, ignoredProperties))
      {
        property.Ignored = true;
      }
      return property;
    }
  }
}