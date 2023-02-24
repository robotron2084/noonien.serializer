using System.IO;
using com.enemyhideout.noonien.serializer;
using UnityEditor;
using UnityEngine;

namespace com.enemyhideout.noonien.serializer
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

  }
}