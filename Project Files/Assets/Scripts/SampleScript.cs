using System.Collections.Generic;
using UnityEngine;

public class SampleScript : MonoBehaviour
{
	IService1 service1;
	IService2 service2;
	ServiceGiver serviceGiver = new ServiceGiver();
	// Start is called before the first frame update
	void Start()
	{
		service1 = serviceGiver.GetService<IService1>();
		service2 = serviceGiver.GetService<IService2>();
		service1.Service1Funcrion();
		service2.Service2Function();
	}
}
public interface IserviceGiver
{
	T GetService<T>();
}
public class ServiceGiver : IserviceGiver
{
	private Dictionary<object, object> keyValuePairs = new Dictionary<object, object>();
	public ServiceGiver()
	{
		keyValuePairs.Add(typeof(IService1), new Service1());
		keyValuePairs.Add(typeof(IService2), new Service2());
	}
	public Service1 service1;
	public Service2 service2;

	public T GetService<T>()
	{
		object tValue;
		if (keyValuePairs.TryGetValue(typeof(T), out tValue))
		{
			return (T)tValue;
		}
		return (T)tValue;
	}
}
public interface IService1
{
	void Service1Funcrion();
}
public class Service1 : IService1
{
	public void Service1Funcrion()
	{
		Debug.Log("Service1");
	}
}
public interface IService2
{
	void Service2Function();
}
public class Service2 : IService2
{
	public void Service2Function()
	{
		Debug.Log("Service2");
	}
}
