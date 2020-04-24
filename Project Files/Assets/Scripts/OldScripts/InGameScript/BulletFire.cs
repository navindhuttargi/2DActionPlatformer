using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;

namespace ActionPlatformer.Gameplay
{
	public class BulletFire : MonoBehaviour
	{
		public Player Owner { get; private set; }

		public void Start()
		{
			Destroy(gameObject, 3.0f);
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			PlayerHealth playerHealth = collision.transform.GetComponent<PlayerHealth>();
			if (playerHealth != null)
			{
				bool playerDied = playerHealth.SetHealth(50, collision.transform);
				if (playerDied)
				{
					Owner.AddScore(1);
				}
			}
			Destroy(gameObject);
		}
		public void InitializeBullet(Player owner/*, Vector3 originalDirection*/)
		{
			Owner = owner;
			//Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
			//rigidbody.velocity = originalDirection * 5;
		}
	}
}
