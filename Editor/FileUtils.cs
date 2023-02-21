using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
  public class FileUtils
  {
    public static bool IsAssetAFolder(Object obj){
      string path = "";
         
      if (obj == null){
        return false;
      }
 
      path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
         
      if (path.Length > 0)
      {
        if (Directory.Exists(path))
        {
          return true;
        }
        else
        {
          return false;
        }
      }
 
      return false;
    }

    public static bool IsAssetAGraph(Object o)
    {
      if (o == null)
      {
        return false;
      }
      var path = AssetDatabase.GetAssetPath(o.GetInstanceID());
      if (Path.GetExtension(path) == ".asset")
      {
        return true;
      }

      return false;

    }
  }
}