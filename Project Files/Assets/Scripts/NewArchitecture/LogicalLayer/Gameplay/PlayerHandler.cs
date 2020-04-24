using ActionPlatformer.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
namespace ActionPlatformer.Gameplay
{

	public class PlayerHandler : MonoBehaviourPunCallbacks, ISceneHandler
	{
		[SerializeField]
		Transform[] spawnPositions;
		[SerializeField]
		GameObject player;
		IGameController gameController;
		public override void OnEnable()
		{
			base.OnEnable();
			//CountdownTimerM.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;
		}
		public override void OnDisable()
		{
			base.OnDisable();
			//CountdownTimerM.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;
		}
		#region PHOTON_CALLBACK_METHODS
		public override void OnDisconnected(DisconnectCause cause)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene("NewMenuScene");
		}

		public override void OnLeftRoom()
		{
			PhotonNetwork.Disconnect();
		}

		public override void OnMasterClientSwitched(Player newMasterClient)
		{
			if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
			{
				//
			}
		}

		public override void OnPlayerLeftRoom(Player otherPlayer)
		{
			CheckEndOfGame();
		}

		public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
			if (changedProps.ContainsKey(PlatformersGame.PLAYER_LIVES))
			{
				CheckEndOfGame();
				return;
			}

			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}

			if (changedProps.ContainsKey(PlatformersGame.PLAYER_LOADED_LEVEL))
			{
				if (CheckAllPlayerLoadedLevel())
				{
					Hashtable props = new Hashtable
					{
						{
							CountdownTimerM.CountdownStartTime, (float) PhotonNetwork.Time
						}
					};
					PhotonNetwork.CurrentRoom.SetCustomProperties(props);
				}
			}
		}
		#endregion

		public void OnInitialize()
		{
			GameObject positionsParent = GameObject.Find("Platform").transform.GetChild(0).gameObject;
			spawnPositions = new Transform[positionsParent.transform.childCount];

			for (int i = 0; i < positionsParent.transform.childCount; i++)
			{
				spawnPositions[i] = positionsParent.transform.GetChild(i);
			}
			gameController = GameplayManager.Instance.controllerFactory.GetInstance<IGameController>();
		}


		private bool CheckAllPlayerLoadedLevel()
		{
			foreach (Player p in PhotonNetwork.PlayerList)
			{
				object playerLoadedLevel;

				if (p.CustomProperties.TryGetValue(PlatformersGame.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
				{
					if ((bool)playerLoadedLevel)
					{
						continue;
					}
				}

				return false;
			}

			return true;
		}

		private void CheckEndOfGame()
		{
			bool allDestroyed = true;

			foreach (Player p in PhotonNetwork.PlayerList)
			{
				object lives;
				if (p.CustomProperties.TryGetValue(PlatformersGame.PLAYER_LIVES, out lives))
				{
					if ((int)lives > 0)
					{
						allDestroyed = false;
						break;
					}
				}
			}

			if (allDestroyed)
			{
				if (PhotonNetwork.IsMasterClient)
				{
					StopAllCoroutines();
				}

				string winner = "";
				int score = -1;

				foreach (Player p in PhotonNetwork.PlayerList)
				{
					if (p.GetScore() > score)
					{
						winner = p.NickName;
						score = p.GetScore();
					}
				}

				StartCoroutine(EndOfGame(winner, score));
			}
		}
		private IEnumerator EndOfGame(string winner, int score)
		{
			float timer = 5.0f;

			while (timer > 0.0f)
			{
				//display winner
				//InfoText.text = string.Format("Player {0} won with {1} points.\n\n\nReturning to login screen in {2} seconds.", winner, score, timer.ToString("n2"));

				yield return new WaitForEndOfFrame();

				timer -= Time.deltaTime;
			}

			PhotonNetwork.LeaveRoom();
		}

		void InitialPlayerSpawn()
		{
			int position = PhotonNetwork.LocalPlayer.GetPlayerNumber();
			player = PhotonNetwork.Instantiate("hero", spawnPositions[position].position, Quaternion.identity, 0);
			GameObject playerCam = Instantiate(Resources.Load("PlayerCamera")) as GameObject;
			player.name = PhotonNetwork.LocalPlayer.NickName;
			playerCam.GetComponent<CameraFollow>().player = player;
		}
		void PlayerRespawn()
		{
			int position = Random.Range(0, spawnPositions.Length);
			player.transform.position = spawnPositions[position].position;
		}


		private void OnCountdownTimerIsExpired()
		{
			InitialPlayerSpawn();
		}


		public void OnEventFromGameManager(GameEvent gameEvent)
		{
			switch (gameEvent.gamePlayEvent)
			{
				case Enums.GAMEPLAY_EVENT.TIME_EXPIRED:
					OnCountdownTimerIsExpired();
					break;
				case Enums.GAMEPLAY_EVENT.PLAYER_RESPAWN:
					PlayerRespawn();
					break;
				case Enums.GAMEPLAY_EVENT.GAMEOVER:
					break;
			}
		}
	}
}
