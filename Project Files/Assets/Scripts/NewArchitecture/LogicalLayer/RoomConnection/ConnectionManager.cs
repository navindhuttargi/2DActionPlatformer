using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ActionPlatformer.UI
{
	public interface IConnectionManager
	{
		void OnInitialize();
		void OnLogin(string playerName);
		void OnJoinRandomRoom();
		void OnLeaveRoom();
		void OnCreateRoom(string roomName, InputField maxPlayers);
		bool CheckPlayersReady();
		void OnStartGame();
		void LocalPlayerPropertiesUpdated();
	}
	public class ConnectionManager : MonoBehaviourPunCallbacks, IConnectionManager
	{
		#region PRIVATE_MEMBERS
		private IGameController gameController;
		private IGameUI uiManager;

		[SerializeField] GameObject playerListEntryPrefab;
		[SerializeField] GameObject roomListEntryPrefab;
		[SerializeField] GameObject insideRoomPanel;
		[SerializeField] ContentSizeFitter content;

		private Dictionary<string, GameObject> roomListEntries;
		private Dictionary<int, GameObject> playerListEntries;
		private Dictionary<string, RoomInfo> cachedRoomList;
		#endregion
		#region UNITY_CORE
		private void Awake()
		{
			PhotonNetwork.AutomaticallySyncScene = true;
		}
		#endregion
		public void OnInitialize()
		{
			gameController = GameManager.Instance.controllerFactory.GetInstance<IGameController>();
			uiManager = GameManager.Instance.controllerFactory.GetInstance<IGameUI>();

			roomListEntries = new Dictionary<string, GameObject>();
			cachedRoomList = new Dictionary<string, RoomInfo>();
			insideRoomPanel = ((UIManager)uiManager).uiScreens["InsideRoomPanel"].gameObject;
			playerListEntryPrefab = Resources.Load<GameObject>("UIPrefabs/playerEntryList");
			roomListEntryPrefab = Resources.Load<GameObject>("UIPrefabs/roomListEntry");
			content = ((UIManager)uiManager).uiScreens["RoomListPanel"].GetComponentInChildren<ContentSizeFitter>();
		}

		#region EVENT_FROM_UI
		public void OnLogin(string playerName)
		{
			Debug.Log("OnLogin()");
			PhotonNetwork.LocalPlayer.NickName = playerName;
			PhotonNetwork.ConnectUsingSettings();
		}
		public void OnJoinRandomRoom()
		{
			PhotonNetwork.JoinRandomRoom();
		}
		public void OnLeaveRoom()
		{
			PhotonNetwork.LeaveRoom();
		}
		public bool CheckPlayersReady()
		{
			if (!PhotonNetwork.IsMasterClient)
				return false;
			foreach (Player p in PhotonNetwork.PlayerList)
			{
				object isPlayerReady;
				if (p.CustomProperties.TryGetValue(PlatformersGame.PLAYER_READY, out isPlayerReady))
				{
					if (!(bool)isPlayerReady)
						return false;
				}
				else
					return false;
			}
			return true;
		}
		public void OnCreateRoom(string roomName, InputField maxPlayersField)
		{
			roomName = (roomName.Equals(string.Empty)) ? "Room" + Random.Range(1000, 10000) : roomName;

			byte maxPlayers;
			byte.TryParse(maxPlayersField.text, out maxPlayers);
			maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 5);

			RoomOptions roomOptions = new RoomOptions { MaxPlayers = maxPlayers };
			PhotonNetwork.CreateRoom(roomName, roomOptions, null);
		}
		public void OnStartGame()
		{
			PhotonNetwork.CurrentRoom.IsOpen = false;
			PhotonNetwork.CurrentRoom.IsVisible = false;
			PhotonNetwork.LoadLevel("GameScene");
		}
		public void LocalPlayerPropertiesUpdated()
		{
			gameController.OnEventTOGameManager(new GameEvent(Enums.ROOM_EVENT.IS_PLAYER_READY));
		}
		#endregion
		#region PUN_CALLBACKS
		public override void OnConnectedToMaster()
		{
			Debug.Log("OnConnectedToMaster() PlayerName:" + PhotonNetwork.LocalPlayer.NickName);
			GameEvent gameEvent = new GameEvent(Enums.ROOM_EVENT.LOGGED_IN);
			gameController.OnEventTOGameManager(gameEvent);
		}
		public override void OnRoomListUpdate(List<RoomInfo> roomList)
		{
			ClearRoomListView();
			UpdateCachedRoomList(roomList);
			UpdateRoomListView();
		}
		public override void OnLeftLobby()
		{
			cachedRoomList.Clear();
			ClearRoomListView();
		}
		public override void OnCreateRoomFailed(short returnCode, string message)
		{
			gameController.OnEventTOGameManager(new GameEvent(Enums.ROOM_EVENT.ON_CREATE_ROOM_FAILED));
		}
		public override void OnJoinRoomFailed(short returnCode, string message)
		{
			gameController.OnEventTOGameManager(new GameEvent(Enums.ROOM_EVENT.ON_JOIN_ROOM_FAILED));
		}
		public override void OnJoinRandomFailed(short returnCode, string message)
		{
			Debug.Log("<color=red> OnJoinRandomFailed</color>");
			string roomName = "Room " + Random.Range(1000, 10000);

			RoomOptions options = new RoomOptions { MaxPlayers = 5 };

			PhotonNetwork.CreateRoom(roomName, options, null);
		}
		public override void OnJoinedRoom()
		{
			Debug.Log("<color=green> OnJoinedRoom()	RoomName:</color>" + PhotonNetwork.CurrentRoom.Name);

			gameController.OnEventTOGameManager(new GameEvent(Enums.ROOM_EVENT.ON_JOINED_ROOM));
			if (playerListEntries == null)
				playerListEntries = new Dictionary<int, GameObject>();

			foreach (Player p in PhotonNetwork.PlayerList)
			{
				GameObject entry = Instantiate(playerListEntryPrefab, insideRoomPanel.transform);
				entry.transform.localScale = Vector3.one;
				entry.GetComponent<PlayerEntryListM>().Initialize(p.ActorNumber, p.NickName);

				object isPlayerReady;
				Debug.Log(p.CustomProperties.TryGetValue(PlatformersGame.PLAYER_READY, out isPlayerReady));
				if (p.CustomProperties.TryGetValue(PlatformersGame.PLAYER_READY, out isPlayerReady))
				{
					entry.GetComponent<PlayerEntryListM>().SetPlayerReady((bool)isPlayerReady);
				}
				if (playerListEntries != null)
					playerListEntries.Add(p.ActorNumber, entry);
				else
					Debug.LogError("playerListEntries is null");
			}

			gameController.OnEventTOGameManager(new GameEvent(Enums.ROOM_EVENT.IS_PLAYER_READY));

			Hashtable props = new Hashtable
			{
				{
					PlatformersGame.PLAYER_LOADED_LEVEL, false
				}
			};
			PhotonNetwork.LocalPlayer.SetCustomProperties(props);
		}
		public override void OnLeftRoom()
		{
			foreach (GameObject entry in playerListEntries.Values)
				Destroy(entry);

			playerListEntries.Clear();
			playerListEntries = null;
			gameController.OnEventTOGameManager(new GameEvent(Enums.ROOM_EVENT.ON_LEFT_ROOM));
		}
		public override void OnPlayerEnteredRoom(Player newPlayer)
		{
			GameObject entry = Instantiate(playerListEntryPrefab, insideRoomPanel.transform);
			entry.transform.localScale = Vector3.one;
			entry.GetComponent<PlayerEntryListM>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);
			playerListEntries.Add(newPlayer.ActorNumber, entry);
			gameController.OnEventTOGameManager(new GameEvent(Enums.ROOM_EVENT.IS_PLAYER_READY));
		}
		public override void OnPlayerLeftRoom(Player otherPlayer)
		{
			Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
			playerListEntries.Remove(otherPlayer.ActorNumber);
			gameController.OnEventTOGameManager(new GameEvent(Enums.ROOM_EVENT.IS_PLAYER_READY));
		}
		public override void OnMasterClientSwitched(Player newMasterClient)
		{

			if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
				gameController.OnEventTOGameManager(new GameEvent(Enums.ROOM_EVENT.IS_PLAYER_READY));
		}
		public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
			if (playerListEntries == null)
			{
				playerListEntries = new Dictionary<int, GameObject>();
			}

			GameObject entry;
			if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
			{
				object isPlayerReady;
				if (changedProps.TryGetValue(PlatformersGame.PLAYER_READY, out isPlayerReady))
				{
					entry.GetComponent<PlayerEntryListM>().SetPlayerReady((bool)isPlayerReady);
				}
			}
			gameController.OnEventTOGameManager(new GameEvent(Enums.ROOM_EVENT.IS_PLAYER_READY));
		}
		private void ClearRoomListView()
		{
			foreach (GameObject entry in roomListEntries.Values)
				Destroy(entry);
			roomListEntries.Clear();
		}
		private void UpdateCachedRoomList(List<RoomInfo> roomlist)
		{
			foreach (RoomInfo info in roomlist)
			{
				//remove room from cached roo list if it got closed ,became invisible or was marked as removed
				if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
				{
					if (cachedRoomList.ContainsKey(info.Name))
					{
						cachedRoomList.Remove(info.Name);
					}
					continue;
				}
				//update cached room info
				if (cachedRoomList.ContainsKey(info.Name))
					cachedRoomList[info.Name] = info;
				else
					cachedRoomList.Add(info.Name, info);
			}
		}
		private void UpdateRoomListView()
		{
			foreach (RoomInfo info in cachedRoomList.Values)
			{
				GameObject entry = Instantiate(roomListEntryPrefab, content.gameObject.transform);
				entry.transform.localScale = Vector3.one;
				entry.GetComponent<RoomEntryListM>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers);
				roomListEntries.Add(info.Name, entry);
			}
		}
		#endregion
	}

}
