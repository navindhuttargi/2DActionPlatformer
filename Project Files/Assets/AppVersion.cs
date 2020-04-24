using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AppVersion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		UnityEngine.UI.Text Version=GetComponent<UnityEngine.UI.Text>();
		Version.text = "AppVersion:"+ Application.version;
	}
}
