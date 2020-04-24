using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ActionPlatformer
{
	/// <summary>
	/// loads the scene with given string
	/// </summary>
	public class SceneLoader : MonoBehaviour
	{
		// Start is called before the first frame update
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{

		}
		IEnumerator<float> StartLoadScene(GameEvent gameEvent, string sceneName, Action OnSuccess = null)
		{
			AsyncOperation asyncTask = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
			while (!asyncTask.isDone)
				yield return 0;
			if (OnSuccess != null)
				OnSuccess();

		}
	}
}
