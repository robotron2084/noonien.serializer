using com.enemyhideout.noonien.editor;
using com.enemyhideout.noonien.serializer;
using UnityEditor;
using UnityEngine;

namespace com.enemyhideout.noonien.serializer
{
  [CustomEditor(typeof(GraphDocument))]
  public class GraphDocumentInspector : Editor
  {
    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();
      var instance = (GraphDocument)target;
      NodeEditorCore.EditorForNode(instance.Root);
      if (GUILayout.Button("Open In Editor"))
      {
        NodeGraphEditor.Root = instance.Root;
        NodeGraphEditor wnd = EditorWindow.GetWindow<NodeGraphEditor>();

      }
    }
  }
}