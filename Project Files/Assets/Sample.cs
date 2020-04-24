using System;
using System.Collections.Generic;
using UnityEngine;

public class Sample : MonoBehaviour
{
	public GameObject GO;
	public int num = 2;
	public string actualkey = "any";
	public string key = "num";
	public Dictionary<string, int> number = new Dictionary<string, int>();
	public SamplePlayer[] samplePlayers;
	// Start is called before the first frame update
	void Start()
	{
		//number.Add(actualkey, 1);
		//int n;
		//if (number.TryGetValue(key, out n))
		//{
		//	print(num);
		//}
		//int[] score = new int[samplePlayers.Length];
		//for (int i = 0; i < samplePlayers.Length; i++)
		//{
		//	score[i] = samplePlayers[i].score;
		//}
		//int length = score.Length;
		//quickSort(score, 0, length - 1);
		//Debug.Log("sorted array ");
		//printArray(score, length);
	}
	[ContextMenu("Test")]
	public void Test()
	{
		int[] score = new int[samplePlayers.Length];
		for (int i = 0; i < samplePlayers.Length; i++)
		{
			score[i] = samplePlayers[i].score;
		}
		int length = score.Length;
		quickSort(score, 0, length - 1);
		Debug.Log("sorted array ");
		printArray(score, length - 1);
	}
	void printArray(int[] arr, int n)
	{
		//foreach (int score in arr)
		//{
		//	foreach (SamplePlayer p in samplePlayers)
		//	{
		//		if (score == p.score)
		//		{
		//			Debug.Log("PLAYER NAME:" + p.name + "	Score:" + p.score);
		//			break;
		//		}
		//	}
		//}
		for (int score = n; score >= 0; score--)
		{
			foreach (SamplePlayer p in samplePlayers)
			{
				if (arr[score] == p.score)
				{
					Debug.Log("PLAYER NAME:" + p.name + "	Score:" + p.score);
					break;
				}
			}
		}
	}
	int partition(int[] arr, int low, int high)
	{
		int pivot = arr[high];

		// index of smaller element 
		int i = (low - 1);
		for (int j = low; j < high; j++)
		{
			// If current element is smaller  
			// than the pivot 
			if (arr[j] < pivot)
			{
				i++;

				// swap arr[i] and arr[j] 
				int temp = arr[i];
				arr[i] = arr[j];
				arr[j] = temp;
			}
		}

		// swap arr[i+1] and arr[high] (or pivot) 
		int temp1 = arr[i + 1];
		arr[i + 1] = arr[high];
		arr[high] = temp1;

		return i + 1;
	}


	/* The main function that implements QuickSort() 
    arr[] --> Array to be sorted, 
    low --> Starting index, 
    high --> Ending index */
	void quickSort(int[] arr, int low, int high)
	{
		if (low < high)
		{

			/* pi is partitioning index, arr[pi] is  
            now at right place */
			int pi = partition(arr, low, high);

			// Recursively sort elements before 
			// partition and after partition 
			quickSort(arr, low, pi - 1);
			quickSort(arr, pi + 1, high);
		}
	}
}

[Serializable]
public class SamplePlayer
{
	public int score;
	public string name;
}
