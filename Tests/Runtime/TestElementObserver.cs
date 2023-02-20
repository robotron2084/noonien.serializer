using com.enemyhideout.noonien;
using UnityEngine;

namespace Tests.Runtime
{
  public class TestElementObserver : ElementObserver<NoonienSerializerRuntimeTests.TestElement>
  {
    public string TestValue;
    protected override void DataUpdated(NoonienSerializerRuntimeTests.TestElement element)
    {
      base.DataUpdated(element);
      TestValue = element.TestValue;
    }
  }
}