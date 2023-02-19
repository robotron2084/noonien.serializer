using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.enemyhideout.noonien;
using com.enemyhideout.noonien.serializer;
using UnityEngine;

namespace com.enemyhideout.noonien.serializer
{
  public class NoonienDocumentManager
  {

    public static NodeGraphDocument CreateDocument(Node rootNode)
    {
      var nodeGraphDoc = new NodeGraphDocument();
      var nodesNormalized = new List<Node>();
      NormalizeNodes(rootNode, nodesNormalized);
      Dictionary<Node, NodeDocument> nodesMap = ToNodeDocuments(nodesNormalized);
      var elementsNormalized = nodesNormalized.SelectMany(x => x.Elements).ToList();
      var elementsMap = ToElementDocuments(elementsNormalized);


      UpdateNodesWithParentIds(nodesMap);
      UpdateElementsWithParentIds(elementsMap, nodesMap);
      nodeGraphDoc.Nodes = nodesMap.Values.ToList();
      nodeGraphDoc.Elements = elementsMap.Values.ToList();
      return nodeGraphDoc;

    }

    private static void NormalizeNodes(Node node, List<Node> nodesList)
    {
      nodesList.Add(node);
      if (node.ChildrenCount == 0)
      {
        return;
      }
      
      foreach (var child in node.Children)
      {
        NormalizeNodes(child, nodesList);
      }
    }

    private static Dictionary<Node, NodeDocument> ToNodeDocuments(List<Node> nodesList)
    {
      var nodesMap = new Dictionary<Node, NodeDocument>();
      
      foreach (var node in nodesList)
      {
        NodeDocument nodeDoc = ToNodeDocument(node);
        nodesMap[node] = nodeDoc;
      }

      return nodesMap;
    }

    private static void UpdateNodesWithParentIds(Dictionary<Node, NodeDocument> nodesMap)
    {

      foreach (var kvp in nodesMap)
      {
        var nodeDoc = kvp.Value;
        if (kvp.Key.Parent != null)
        {
          var nodeParent = nodesMap[kvp.Key.Parent];
          nodeDoc.ParentId = nodeParent.Id;
        }
      }
      
    }

    private static NodeDocument ToNodeDocument(Node node)
    {
      var retVal = new NodeDocument
      {
        Name = node.Name,
        Id = Guid.NewGuid(),
      };
      return retVal;
    }
    
    private static Dictionary<Element, ElementDocument> ToElementDocuments(List<Element> elementsNormalized)
    {
      var elementsMap = new Dictionary<Element, ElementDocument>();
      foreach (var element in elementsNormalized)
      {
        ElementDocument elementDocument = ToElementDocument(element);
        elementsMap[element] = elementDocument;
      }

      return elementsMap;
    }
    
    private static void UpdateElementsWithParentIds(Dictionary<Element, ElementDocument> elementsMap, Dictionary<Node, NodeDocument> nodesMap)
    {
      foreach (var kvp in elementsMap)
      {
        kvp.Value.ParentId = nodesMap[kvp.Key.Parent].Id;
      }
    }

    private static ElementDocument ToElementDocument(Element node)
    {
      var retVal = new ElementDocument
      {
        Id = Guid.NewGuid(),
        Type = node.GetType().FullName
      };
      return retVal;
    }
  }
}
