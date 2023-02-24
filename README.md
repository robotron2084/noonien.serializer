# noonien.serializer
A serialization mechanism to save and edit noonien graphs.

This is highly experimental.
TODO:
  * Add UI to allow graphs to be edited.
    * Somehow add serialization as a service...possibly via ServiceLocator?
    * Add ability to add/remove nodes.
    * Add ability to add/remove Elements.
      * Requires assembly reflection or something to find all Element types.
    * Add items to Collection<T>.
      * Need a Collection<T> editor of some sort.
    * Save nodes...automatically? Via a save button?
      * Need an OnChanged handler for the node so that it can be scheduled for save actions
  * Docs

DONE:
* Switched to scriptable object to allow simpler integration with Unity-ness. Created a custom inspector that allows the item to open in the graph editor.
* Inject INotifyManager into graph.
  * Remove yaml/messagepack experiments.
  * Very basic graphs can be serialized and deserialized to JSON.
  *   * Clean up these files and make it into an actual thing that could be used.

