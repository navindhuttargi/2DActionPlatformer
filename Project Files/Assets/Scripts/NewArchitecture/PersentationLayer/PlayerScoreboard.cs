using ActionPlatformer.Gameplay;
using ActionPlatformer.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
namespace ActionPlatformer
{
	public class PlayerScoreboard : MonoBehaviourPunCallbacks
	{
		public GameObject PlayerOverviewEntryPrefab;

		private List<GameObject> playerListEntries;
		public Transform scoreboard;
		public Transform scoreboardParent;
		#region UNITY

		public void Awake()
		{
			playerListEntries = new List<GameObject>();
			scoreboard = gameObject.transform.GetChild(0).transform;
			int playerNumber = 0;
			foreach (Player p in PhotonNetwork.PlayerList)
			{
				GameObject entry = Instantiate(PlayerOverviewEntryPrefab, scoreboardParent);
				entry.transform.localScale = Vector3.one;

				Text[] texts = entry.GetComponentsInChildren<Text>();
				texts[0].text = (++playerNumber).ToString();
				texts[1].text = p.NickName;
				texts[2].text = p.GetScore().ToString();

				playerListEntries.Add(entry);
				//entry.GetComponent<Text>().text = string.Format("{0}\nScore: {1}\nLives: {2}", p.NickName, p.GetScore(), AsteroidsGame.PLAYER_MAX_LIVES);
			}
			scoreboard.gameObject.SetActive(false);
		}

		#endregion

		#region PUN CALLBACKS

		public override void OnPlayerLeftRoom(Player otherPlayer)
		{
			//Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
			//playerListEntries.Remove(otherPlayer.ActorNumber);
		}

		public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
			//GameObject entry;
			//if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
			//{
			//	entry.GetComponent<Text>().text = string.Format("{0}\nScore: {1}\nLives: {2}", targetPlayer.NickName, targetPlayer.GetScore(), targetPlayer.CustomProperties[AsteroidsGame.PLAYER_LIVES]);
			//}
			//GetScore();
		}
		public IEnumerator DisplayScore()
		{
			float timer = 4;
			scoreboard.gameObject.SetActive(true);
			while (timer > 3.5f)
			{
				timer -= Time.deltaTime;
				yield return 0;
			}
			GetScore();
			while (timer > 0)
			{
				timer -= Time.deltaTime;
				yield return 0;
			}
			scoreboard.gameObject.SetActive(false);
			GameplayManager.Instance.controllerFactory.GetInstance<IGameController>().OnEventTOGameManager(new GameEvent(Enums.GAMEPLAY_EVENT.PLAYER_RESPAWN));
		}
		private void GetScore()
		{
			Player[] players = PhotonNetwork.PlayerList;
			int[] score = new int[players.Length];
			for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
			{
				score[i] = PhotonNetwork.PlayerList[i].GetScore();
			}
			int[] tempScore = score;
			quickSort(score, 0, score.Length - 1);
			SortScore(players, score, tempScore);
		}
		private void SortScore(Player[] tempPlayer, int[] ascendingScore, int[] unSortedScore)
		{
			Player[] players = new Player[tempPlayer.Length];
			int[] sortedScore = new int[ascendingScore.Length];
			int playerCount = 0;
			for (int i = ascendingScore.Length - 1; i >= 0; i--)
			{
				for (int j = 0; j < unSortedScore.Length; j++)
				{
					if (ascendingScore[i] == unSortedScore[j])
					{
						players[playerCount] = tempPlayer[i];
						sortedScore[playerCount] = ascendingScore[i];
						playerCount++;
						break;
					}
				}
			}
			//playerStatParent.gameObject.SetActive(true);
			PrintScore(players, sortedScore);
			//StartCoroutine(WaitTillPlayerSpawn());
		}
		private void PrintScore(Player[] player, int[] score)
		{
			for (int i = 0; i < playerListEntries.Count; i++)
			{
				Text[] texts = playerListEntries[i].GetComponentsInChildren<Text>();
				texts[0].text = (i + 1).ToString();
				texts[1].text = player[i].NickName;
				texts[2].text = score[i].ToString();
			}
		}
		#endregion

		#region	QUICKSORT_LOGIC
		int partition(int[] arr, int low, int high)
		{
			int pivot = arr[high];
			int i = (low - 1);
			for (int j = low; j < high; j++)
			{
				if (arr[j] < pivot)
				{
					i++;
					int temp = arr[i];
					arr[i] = arr[j];
					arr[j] = temp;
				}
			}
			int temp1 = arr[i + 1];
			arr[i + 1] = arr[high];
			arr[high] = temp1;

			return i + 1;
		}
		void quickSort(int[] arr, int low, int high)
		{
			if (low < high)
			{
				int pi = partition(arr, low, high);

				quickSort(arr, low, pi - 1);
				quickSort(arr, pi + 1, high);
			}
		}
		#endregion
	}
}
