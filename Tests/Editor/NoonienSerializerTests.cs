using System;
using System.Collections.Generic;
using com.enemyhideout.noonien;
using com.enemyhideout.noonien.serializer;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine;

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

    [Test]
    public void TestJsonSerializer()
    {
      var serializer = new NoonienSerializer(null);
      var node = CreateTestNode();

      var jsonStr = serializer.SerializeObject(node);
      Debug.Log(jsonStr);

      var node2 = serializer.DeserializeObject<Node>(jsonStr);
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
}