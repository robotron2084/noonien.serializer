using System;
using System.Collections.Generic;

namespace com.enemyhideout.noonien.serializer
{

  public class NodeDocument : IInstancedItem
  {
    public string Name;
    public Guid Id { get; set; }
    public Guid ParentId;
  }

  public class NodeGraphDocument
  {
    public List<NodeDocument> Nodes = new List<NodeDocument>();
    public List<ElementDocument> Elements = new List<ElementDocument>();
  }

  public class ElementDocument : IInstancedItem
  {
    public Guid Id { get; set; }
    public Guid ParentId;
    public string Type;
    public List<Property> Properties = new List<Property>();

    public void AddProperty(Property property)
    {
      Properties.Add(property);
    }
    
  }

  public abstract class Property
  {
    public string Key;
  }

  public class Property<T> : Property
  {
    public T Value;
    
    public Property(string key, T val)
    {
      Key = key;
      Value = val;
    }
  }
  
  public interface IInstancedItem
  {
    public Guid Id { get; set; }
  }


}