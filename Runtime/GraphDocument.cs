using UnityEngine;

namespace com.enemyhideout.noonien.serializer
{
  public class GraphDocument : ScriptableObject
  {
    [HideInInspector][SerializeField]
    public string Json;


    [System.NonSerialized]
    private Node _root;
    
    public Node Root
    {
      get
      {
        if (_root == null)
        {
          var serializer = new NoonienSerializer(null);
          _root = serializer.DeserializeObject<Node>(Json); 
        }
        return _root;
      }
      set
      {
        _root = value;
        var serializer = new NoonienSerializer(null);
        Json = serializer.SerializeObject(_root);
      }
    }
  }
}