using System.IO;
using com.enemyhideout.noonien;
using com.enemyhideout.noonien.editor;
using com.enemyhideout.noonien.serializer;
using UnityEditor;
using UnityEngine;

namespace Editor
{
  public class MenuItems
  {
    [MenuItem("Assets/Noonien/Create Graph")]
    public static void CreateNode()
    {
      var obj = Selection.activeObject;
      if (FileUtils.IsAssetAFolder(obj))
      {
        var path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
        var serializer = new NoonienSerializer(null);
        var root = new Node(null, "Root");
        var jsonStr = serializer.SerializeObject(root);
        var filename = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(path, "Node.asset"));
        var textAsset = new TextAsset(jsonStr);
        ProjectWindowUtil.CreateAsset(textAsset, filename);
      }
      else
      {
        Debug.Log($"Nope {obj}");
      }
    }
    [MenuItem("Assets/Noonien/Open Graph")]
    public static void OpenGraph()
    {
      var obj = Selection.activeObject;
      if (FileUtils.IsAssetAGraph(obj))
      {
        var serializer = new NoonienSerializer(null);
        var textAsset = (TextAsset)obj;
        var graph = serializer.DeserializeObject<Node>(textAsset.text);
        NodeGraphEditor.Root = graph;
        NodeGraphEditor wnd = EditorWindow.GetWindow<NodeGraphEditor>();
        
        Debug.Log("Yep");
      }
      else
      {
        Debug.Log($"Nope {obj}");
      }
    }

    
  }
}