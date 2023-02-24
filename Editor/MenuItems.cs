using System.IO;
using com.enemyhideout.noonien;
using com.enemyhideout.noonien.editor;
using com.enemyhideout.noonien.serializer;
using UnityEditor;
using UnityEngine;

namespace com.enemyhideout.noonien.serializer
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
        var graphAsset = ScriptableObject.CreateInstance<GraphDocument>();
        graphAsset.Json = jsonStr;
        ProjectWindowUtil.CreateAsset(graphAsset, filename);
      }
      else
      {
        Debug.Log($"Nope {obj}");
      }
    }
    [MenuItem("Assets/Noonien/Open Graph")]
    public static void OpenGraph()
    {
      var obj = (GraphDocument)Selection.activeObject;
      
      if (obj != null)
      {
        var graphAsset = obj;
        NodeGraphEditor.Root = graphAsset.Root;
        NodeGraphEditor wnd = EditorWindow.GetWindow<NodeGraphEditor>();
      }
    }

    
  }
}