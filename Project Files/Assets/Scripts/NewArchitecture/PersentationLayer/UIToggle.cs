using System;
using UnityEngine;
using UnityEngine.UI;

namespace ActionPlatformer
{
	public class UIToggle : MonoBehaviour
	{
		private Toggle toggle;

		//public delegate void OnToggle(Toggle toggle);

		//public static event OnToggle ToggleValueChanged;
		public static Action<Toggle> ToggleValueChanged;

		private void Start()
		{
			toggle = GetComponent<Toggle>();
			toggle.onValueChanged.AddListener(delegate { OnToggleValueChanged(); });
		}

		public void OnToggleValueChanged()
		{
			ToggleValueChanged?.Invoke(toggle);
		}
	}
}
