﻿using System.Collections;

using UnityEngine;

namespace ActionPlatformer
{
	public class Remover : MonoBehaviour
	{
		public GameObject splash;
		public bool canDie;

		void OnTriggerEnter2D(Collider2D col)
		{
			if (!canDie)
				return;
			// If the player hits the trigger...
			if (col.gameObject.tag == "Player")
			{
				// .. stop the camera tracking the player
				GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().enabled = false;

				// .. stop the Health Bar following the player
				if (GameObject.FindGameObjectWithTag("HealthBar").activeSelf)
				{
					GameObject.FindGameObjectWithTag("HealthBar").SetActive(false);
				}

				// ... instantiate the splash where the player falls in.
				Instantiate(splash, col.transform.position, transform.rotation);
				// ... destroy the player.
				Destroy(col.gameObject);
				// ... reload the level.
				StartCoroutine("ReloadGame");
			}
			else
			{
				// ... instantiate the splash where the enemy falls in.
				Instantiate(splash, col.transform.position, transform.rotation);

				// Destroy the enemy.
				Destroy(col.gameObject);
			}
		}

		IEnumerator ReloadGame()
		{
			// ... pause briefly
			yield return new WaitForSeconds(2);
			// ... and then reload the level.
			UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex, UnityEngine.SceneManagement.LoadSceneMode.Single);
		}
	}
}
