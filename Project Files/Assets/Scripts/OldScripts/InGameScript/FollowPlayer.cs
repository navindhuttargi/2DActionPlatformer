using UnityEngine;

namespace ActionPlatformer
{
	public class FollowPlayer : MonoBehaviour
	{
		public Vector3 offset;          // The offset at which the Health Bar follows the player.

		public GameObject player;      // Reference to the player.

		void Update()
		{
			if (!GameObject.Find(Photon.Pun.PhotonNetwork.LocalPlayer.NickName))
				return;
			if (player == null)
				player = GameObject.Find(Photon.Pun.PhotonNetwork.LocalPlayer.NickName);
			else
				// Set the position to the player's position with the offset.
				transform.position = player.transform.position + offset;
		}
	}
}
