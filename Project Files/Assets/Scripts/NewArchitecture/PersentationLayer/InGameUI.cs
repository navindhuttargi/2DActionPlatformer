using UnityEngine;
using UnityEngine.UI;

namespace ActionPlatformer
{
	public class InGameUI : MonoBehaviour
	{
		private void OnEnable()
		{
			UIToggle.ToggleValueChanged += OnToggleValueChanged;
		}
		// Start is called before the first frame update
		void Start()
		{

		}

		void OnToggleValueChanged(Toggle toggle)
		{
			switch (toggle.name)
			{
				case "TextChatToggle":
					bool isActive = toggle.isOn;
					toggle.transform.parent.GetChild(2).gameObject.SetActive(isActive);
					break;
			}
		}

		private void OnDisable()
		{
			UIToggle.ToggleValueChanged -= OnToggleValueChanged;
		}
	}
}
