using Newtonsoft.Json;

namespace com.enemyhideout.noonien.serializer
{
  public class NoonienSerializer
  {
    private readonly INotifyManager _notifyManager;

    private JsonSerializerSettings _jsonSettings;

    public NoonienSerializer(INotifyManager notifyManager)
    {
      _notifyManager = notifyManager;
      _jsonSettings = new JsonSerializerSettings
      {
        TypeNameHandling = TypeNameHandling.Auto,
        NullValueHandling = NullValueHandling.Ignore,
        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        ContractResolver = new NoonienContractResolver(notifyManager)
      };
    }

    public T DeserializeObject<T>(string jsonStr)
    {
      T obj = JsonConvert.DeserializeObject<T>(jsonStr, _jsonSettings);
      return obj;
    }

    public string SerializeObject(object obj)
    {
      string jsonStr = JsonConvert.SerializeObject(obj, Formatting.Indented, _jsonSettings);
      return jsonStr;
    }

  }
}