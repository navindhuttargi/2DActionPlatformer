using ActionPlatformer.Gameplay;
using ActionPlatformer.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace ActionPlatformer
{
	public interface IControllerFactory
	{
		T GetInstance<T>();
	}
	public class ControllerFactory : IControllerFactory
	{
		protected Dictionary<object, object> instances = new Dictionary<object, object>();
		protected Type[] allTypes;
		public ControllerFactory(IGameUI _gameUI = null, IGameController _gameController = null)
		{
			if (_gameUI != null && _gameController != null)
			{
				if (_gameUI != null && _gameController != null)
				{
					instances.Add(typeof(IGameUI), _gameUI);
					instances.Add(typeof(IGameController), _gameController);
				}
			}
		}

		public virtual T GetInstance<T>()
		{
			object value;
			if (instances.TryGetValue(typeof(T), out value))
			{
				return (T)value;
			}
			else
			{
				//try
				//{
				//	foreach (Type t in allTypes)
				//	{
				//		if (t.BaseType == typeof(MonoBehaviour) || t.BaseType == typeof(Photon.Pun.MonoBehaviourPunCallbacks))
				//		{
				//			try
				//			{
				//				value = GameManager.FindObjectOfType(t);
				//			}
				//			catch (Exception e)
				//			{
				//				Debug.LogException(e);
				//			}
				//			break;
				//		}
				//	}
				//}
				//catch (Exception e)
				//{
				//	Debug.LogException(e, GameManager.Instance);
				//}
				//var types = AppDomain.CurrentDomain.GetAssemblies()
				//.SelectMany(s => s.GetTypes())
				//.Where(p => typeof(T).IsAssignableFrom(p));
				//allTypes = types.ToArray();
				//foreach (Type item in allTypes)
				//{
				//	if (item == typeof(MonoBehaviour) || item.BaseType == typeof(Photon.Pun.MonoBehaviourPunCallbacks))
				//	{
				//		try
				//		{
				//			value = GameManager.FindObjectOfType(item);
				//		}
				//		catch (Exception e)
				//		{
				//			Debug.LogException(e);
				//		}
				//		break;
				//	}
				//}
				//if (value != null)
				//	instances.Add(typeof(T), value);
				string tp = typeof(T).ToString();
				switch (tp)
				{
					case "ActionPlatformer.UI.IConnectionManager":
						value = MonoBehaviour.FindObjectOfType<ConnectionManager>();
						break;
					case "ActionPlatformer.UI.ISceneHandler":
						value = MonoBehaviour.FindObjectOfType<SceneHandler>();
						break;
				}
				if (value != null)
					instances.Add(typeof(T), value);
				else
					Debug.LogError("not implemented exception");
			}
			return (T)value;
		}
	}
	public class InGameControllerFactory : ControllerFactory
	{
		public InGameControllerFactory(IGameUI gameplayUIManager, IGameController gameController)
		{
			var q = from t in Assembly.GetExecutingAssembly().GetTypes()
					where t.IsClass && t.Namespace == gameplayUIManager.GetType().Namespace
					select t;
			allTypes = q.ToArray();
			instances.Add(typeof(IGameUI), gameplayUIManager);
			instances.Add(typeof(IGameController), gameController);
		}
		public override T GetInstance<T>()
		{
			object value;
			if (instances.TryGetValue(typeof(T), out value))
			{
				return (T)value;
			}
			else
			{
				string tp = typeof(T).ToString();
				switch (tp)
				{
					case "ActionPlatformer.UI.IConnectionManager":
						value = MonoBehaviour.FindObjectOfType<ConnectionManager>();
						break;
					case "ActionPlatformer.UI.ISceneHandler":
						value = MonoBehaviour.FindObjectOfType<PlayerHandler>();
						break;
				}
				if (value != null)
					instances.Add(typeof(T), value);
				else
					Debug.LogError("not implemented exception");
			}
			return (T)value;
		}
	}
}
