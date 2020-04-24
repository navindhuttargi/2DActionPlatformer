using ActionPlatformer.Gameplay;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
namespace ActionPlatformer
{

	/// <summary>
	/// generic Singleton state machine class derived from a monobehaviour.
	/// it handles all the basic functionality required for a state machine.
	/// it calls for a virtual RegisterStates method in Start,so that all the derived classes can register their states.
	/// it checks for goal state and changes to new state 
	/// it calls the OnEnter for each new state, before that it call for OnExit of previous state.
	/// it calls update on the current state for each frame.
	/// it all has a virtual OnStateCompleted method, should be called from each state on completion of its task.
	/// the derived classes are expected to override this method (OnStateCompleted) to handle state complete event and intiate the next state.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class GameStateMachine<T> : MonoBehaviourPunCallbacks where T : MonoBehaviourPunCallbacks
	{
		#region SINGLETON
		/// <summary>
		/// holds the static instance of singleton class.
		/// </summary>
		private static T _instance;

		/// <summary>
		/// lock just before checking for singleton instance 
		/// </summary>
		private static object _lock = new object();

		/// <summary>
		/// in case the application is quitting.
		/// </summary>
		private static bool _isApplicationQuitting = false;

		/// <summary>
		/// in case the object need to persist between multiple scenes.
		/// </summary>
		private static bool _doNotDestroyOnLoad = false;

		/// <summary>
		/// check if application is quitting.
		/// </summary>
		private static bool _checkForAppQuitting = true;

		/// <summary>
		/// public property to set and get check for applicaiton quitting.
		/// </summary>
		public static bool CheckForAppQuitting
		{
			get { return _checkForAppQuitting; }
			set { _checkForAppQuitting = value; }
		}

		/// <summary>
		/// public property to set and get for doDotDestory on loading a new scene.
		/// </summary>
		public static bool DoNotDestroyOnLoad
		{
			get
			{
				return _doNotDestroyOnLoad;
			}
			set
			{
				_doNotDestroyOnLoad = value;
				if (value)
					DontDestroyOnLoad(Instance);
			}
		}

		/// <summary>
		/// boolean flag to return if singleton instance is created.
		/// </summary>
		/// <returns></returns>
		public static bool IsInstatiated()
		{
			return _instance != null;
		}
		public static T Instance
		{
			get
			{
				if (_isApplicationQuitting && CheckForAppQuitting == true)
				{
					Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
									 "' already destroyed on application quit." +
									 " Won't create again - returning null.");
					return null;
				}

				lock (_lock)
				{
					if (_instance == null)
					{

						// searching for the existance of sametype of object in the current scene.
						_instance = (T)FindObjectOfType(typeof(T));

						if (FindObjectsOfType(typeof(T)).Length > 1)
						{
							Debug.LogWarning("[Singleton] Something went really wrong " +
										   " - there should never be more than 1 singleton!" +
										   " Reopenning the scene might fix it." + FindObjectOfType(typeof(T)).name);
							return _instance;
						}

						if (_instance == null)
						{
							//GameObject singleton = new GameObject();
							//_instance = singleton.AddComponent<T>();
							//singleton.name = "(singleton) " + typeof(T).ToString();

							//if (DoNotDestroyOnLoad == true)
							//    DontDestroyOnLoad(singleton);

							//						Debug.Log("[Singleton] An instance of " + typeof(T) + 
							//						          " is needed in the scene, so '" + singleton +
							//						          "' was created with DontDestroyOnLoad.");
						}
						else
						{
							//Debug.Log("[Singleton] Using instance already created: " +
							// _instance.gameObject.name);
						}
					}

					return _instance;
				}
			}
		}
		/// <summary>
		/// call just before getting destroyed.
		/// setting the singleton reference to null
		/// </summary>
		public virtual void OnDestroy()
		{
			_isApplicationQuitting = true;
			_instance = null;
		}

		#endregion SINGLETON

		#region STATES PARAMETERS AND REGISTRATON

		public Dictionary<uint, State> states = new Dictionary<uint, State>();

		protected uint _currentState = 0;
		public uint currentState { get { return _currentState; } set { _currentState = value; } }

		protected uint _goalState = 0;
		public uint goalState
		{ get { return _goalState; } set { _goalState = value; } }

		protected State managerCurrentState;

		protected void AddState(State state)
		{
			if (states.ContainsKey(state.ID))
			{
				Debug.Log("Adding +" + state.ToString() + "+State Same Multiple Times");
				return;
			}
			states.Add(state.ID, state);
		}
		protected virtual void RegisterStates()
		{
			states.Clear();
		}
		#endregion

		#region	CORE METHODS
		protected virtual void Start()
		{
			RegisterStates();
		}
		protected virtual void Update()
		{
			CheckForStateChange();
			if (managerCurrentState != null)
				managerCurrentState.OnUpdate();
		}
		void CheckForStateChange()
		{
			while (currentState != goalState)
			{
				uint tempState = goalState;

				if (managerCurrentState != null)
					managerCurrentState.OnExit();

				currentState = tempState;
				managerCurrentState = null;

				if (states.ContainsKey(currentState))
				{
					managerCurrentState = states[currentState];
					managerCurrentState.OnEnter();
				}
			}
		}
		public virtual void OnStateCompleted(State state, uint targetState = GameplayManager.INVALID_STATE)
		{
			if (targetState == GameplayManager.INVALID_STATE) return;
			else if (currentState == targetState) return;
			goalState = targetState;
		}
		#endregion
	}
}
