using System;
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
}