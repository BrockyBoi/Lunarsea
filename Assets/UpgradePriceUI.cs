using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePriceUI : MonoBehaviour
{

    public Text displayText;

    public void SetPrice(int price)
    {
        if (price != 0)
            displayText.text = "Coins: " + string.Format("{0:N0}", price);
        else displayText.text = "Max";
    }
}
