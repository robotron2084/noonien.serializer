using System.Collections;
using System.Collections.Generic;
using com.enemyhideout.noonien;
using com.enemyhideout.noonien.serializer;
using NUnit.Framework;
using Tests.Runtime;
using UnityEngine;
using UnityEngine.TestTools;

public class NoonienSerializerRuntimeTests
{
    [UnityTest]
    public IEnumerator TestNotifyLifeCycle()
    {
      var controller = new GameObject("Controller");
      var notifyManager = controller.AddComponent<NotifyManager>();

      var go = new GameObject("NodeObserver");
      var provider = go.AddComponent<NodeProvider>();
      var observer = go.AddComponent<TestElementObserver>();

      var serializer = new NoonienSerializer(notifyManager);
      var node = CreateTestNode();
      var jsonStr = serializer.SerializeObject(node);
      var node2 = serializer.DeserializeObject<Node>(jsonStr);

      provider.Node = node2;
      var element = node2.Get<TestElement>();
      Assert.That(observer.TestValue, Is.EqualTo(element.TestValue));
      yield break;

    }
  
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
}
