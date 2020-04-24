using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


public class MyBehaviour : MonoBehaviour
{
	public Camera cam;
	void Start()
	{
		StartCoroutine(GetText());
	}

	IEnumerator GetText()
	{
		UnityWebRequest www = new UnityWebRequest("https://drive.google.com/uc?export=download&id=1ZLnLL_ZMrnKPxF8R54HiVB_SO19yriFO");
		www.downloadHandler = new DownloadHandlerBuffer();
		yield return www.SendWebRequest();

		if (www.isNetworkError || www.isHttpError)
		{
			Debug.Log(www.error);
		}
		else
		{
			// Show results as text
			Debug.Log(www.downloadHandler.text);
			string responce = www.downloadHandler.text;
			Data d = JsonUtility.FromJson<Data>(responce);
			Debug.Log(d.downloadlink + "	" + d.message + "	" + d.version);


			//Application.OpenURL(d.downloadlink);
			// Or retrieve results as binary data
			byte[] results = www.downloadHandler.data;
		}
	}
}
[System.Serializable]
public class Data
{
	public int version;
	public string downloadlink;
	public string message;
}
