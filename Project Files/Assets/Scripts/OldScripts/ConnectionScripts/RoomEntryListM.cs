using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
namespace ActionPlatformer
{
	public class RoomEntryListM : MonoBehaviour
	{
		[Header("UI Reference")]
		public Text roomNameText;
		public Text roomCapacity;
		public Button joinRoomButton;

		private string roomName;

		public void Start()
		{
			joinRoomButton.onClick.AddListener(() =>
			{
				if (PhotonNetwork.InLobby)
				{
					PhotonNetwork.LeaveLobby();
				}

				PhotonNetwork.JoinRoom(roomName);
			});
		}

		public void Initialize(string name, byte currentPlayers, byte maxPlayers)
		{
			roomName = name;

			roomNameText.text = name;
			roomCapacity.text = currentPlayers + " / " + maxPlayers;
		}

	}
}
