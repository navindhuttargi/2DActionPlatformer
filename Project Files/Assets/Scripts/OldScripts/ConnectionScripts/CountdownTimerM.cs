// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CountdownTimer.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities,
// </copyright>
// <summary>
// This is a basic CountdownTimer. In order to start the timer, the MasterClient can add a certain entry to the Custom Room Properties,
// which contains the property's name 'StartTime' and the actual start time describing the moment, the timer has been started.
// To have a synchronized timer, the best practice is to use PhotonNetwork.Time.
// In order to subscribe to the CountdownTimerHasExpired event you can call CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;
// from Unity's OnEnable function for example. For unsubscribing simply call CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;.
// You can do this from Unity's OnDisable function for example.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using ActionPlatformer.UI;
using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ActionPlatformer.Gameplay
{
	/// <summary>
	/// This is a basic CountdownTimer. In order to start the timer, the MasterClient can add a certain entry to the Custom Room Properties,
	/// which contains the property's name 'StartTime' and the actual start time describing the moment, the timer has been started.
	/// To have a synchronized timer, the best practice is to use PhotonNetwork.Time.
	/// In order to subscribe to the CountdownTimerHasExpired event you can call CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;
	/// from Unity's OnEnable function for example. For unsubscribing simply call CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;.
	/// You can do this from Unity's OnDisable function for example.
	/// </summary>
	public class CountdownTimerM : MonoBehaviourPunCallbacks
	{
		public const string CountdownStartTime = "StartTime";

		/// <summary>
		/// OnCountdownTimerHasExpired delegate.
		/// </summary>
		//public delegate void CountdownTimerHasExpired();

		/// <summary>
		/// Called when the timer has expired.
		/// </summary>
		//public static Action OnCountdownTimerHasExpired;

		private bool isTimerRunning;

		private float startTime;

		[Header("Reference to a Text component for visualizing the countdown")]
		public Text Text;

		[Header("Countdown time in seconds")]
		public float Countdown = 5.0f;

		public void Start()
		{
			if (Text == null)
			{
				Debug.LogError("Reference to 'Text' is not set. Please set a valid reference.", this);
				return;
			}
		}
		public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
			object startTimeFromProps;

			if (propertiesThatChanged.TryGetValue(CountdownStartTime, out startTimeFromProps))
			{
				isTimerRunning = true;
				startTime = (float)startTimeFromProps;
				StartCoroutine(CountdownTimer());
			}
		}
		IEnumerator<float> CountdownTimer()
		{
			float countdown = Time.deltaTime;
			while (countdown > 0f)
			{
				float timer = (float)PhotonNetwork.Time - startTime;
				countdown = Countdown - timer;
				Text.text = string.Format("Game starts in {0} seconds", countdown.ToString("n2"));
				yield return 0;
			}
			Text.text = string.Empty;
			GameplayManager.Instance.controllerFactory.GetInstance<IGameController>().OnEventTOGameManager(new GameEvent(Enums.GAMEPLAY_EVENT.TIME_EXPIRED));
		}
	}
}
