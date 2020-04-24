
using ActionPlatformer.UI;
using UnityEngine;

namespace ActionPlatformer
{
	public interface ISceneManager
	{
		void OnEventFromGameManager(GameEvent gameEvent);
	}
	public class SceneHandler : MonoBehaviour, ISceneHandler
	{
		public void OnEventFromGameManager(GameEvent gameEvent)
		{

		}

		public void OnInitialize()
		{

		}
	}
}
