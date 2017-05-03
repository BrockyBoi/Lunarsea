using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSkinController : MonoBehaviour
{
	public static CustomSkinController controller;
	public List<CustomSkin> boatSkins = new List<CustomSkin> ();
	public List<CustomSkin> moonSkins = new List<CustomSkin> ();

	Sprite chosenBoatSprite;
	Sprite chosenMoonSprite;

	List<CustomSkin> currentList;

	void Awake ()
	{
		if (controller == null)
			controller = this;
		else if (controller != this)
			Destroy (gameObject);

		DontDestroyOnLoad (this);
	}

	// Use this for initialization
	void Start ()
	{
		Sort (boatSkins);
		Sort (moonSkins);
	}

	public void SetCurrentList (List<CustomSkin> list)
	{
		currentList = list;
	}

	public void ClickOnSprite (int sprite)
	{
		CustomSkin skin = currentList [sprite];
		if (CoinController.controller.getCoinNum () >= skin.GetPrice ()) {
			BuySprite (skin);
		}
	}

	public Sprite GetBoatSprite ()
	{
		return chosenBoatSprite;
	}

	public Sprite GetMoonSprite ()
	{
		return chosenMoonSprite;
	}

	void BuySprite (CustomSkin skin)
	{
		CoinController.controller.MakePurchase (skin.GetPrice ());

		Sprite boughtSkin = skin.GetSprite ();
		if (currentList == boatSkins)
			chosenBoatSprite = boughtSkin;
		else
			chosenMoonSprite = boughtSkin;

		PlayerInfo.controller.Save ();
	}

	void Sort (List<CustomSkin> list)
	{
		list.Sort (CustomSkin.SortByPrice);
	}
}


