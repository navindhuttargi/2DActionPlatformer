using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ActionPlatformer
{
	public class PlayerOverviewPanel : MonoBehaviourPunCallbacks
	{
		public GameObject PlayerOverviewEntryPrefab;

		private Dictionary<int, GameObject> playerListEntries;

		#region UNITY

		public void Awake()
		{
			playerListEntries = new Dictionary<int, GameObject>();

			foreach (Player p in PhotonNetwork.PlayerList)
			{
				GameObject entry = Instantiate(PlayerOverviewEntryPrefab);
				entry.transform.SetParent(gameObject.transform);
				entry.transform.localScale = Vector3.one;
				entry.GetComponent<Text>().color = PlatformersGame.GetColor(p.GetPlayerNumber());
				entry.GetComponent<Text>().text = string.Format("{0}\nScore: {1}\nLives: {2}", p.NickName, p.GetScore(), PlatformersGame.PLAYER_MAX_LIVES);

				playerListEntries.Add(p.ActorNumber, entry);
			}
		}

		#endregion

		#region PUN CALLBACKS

		public override void OnPlayerLeftRoom(Player otherPlayer)
		{
			Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
			playerListEntries.Remove(otherPlayer.ActorNumber);
		}

		public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
			GameObject entry;
			if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
			{
				entry.GetComponent<Text>().text = string.Format("{0}\nScore: {1}\nLives: {2}", targetPlayer.NickName, targetPlayer.GetScore(), targetPlayer.CustomProperties[PlatformersGame.PLAYER_LIVES]);
			}
		}
		#endregion
	}
}
