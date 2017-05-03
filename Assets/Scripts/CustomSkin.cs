using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSkin : MonoBehaviour
{
	[SerializeField]
	int price;

	[SerializeField]
	Sprite sprite;

	public int GetPrice ()
	{
		return price;
	}

	public Sprite GetSprite ()
	{
		return sprite;
	}

	public static int SortByPrice (CustomSkin cs1, CustomSkin cs2)
	{
		return cs1.price.CompareTo (cs2.price);
	}
}

