using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public int coinCount;
    public int coinsToFind;

    public Text coinText;


    // Start is called before the first frame update
    void Start()
    {
        coinCount = 0;
        coinsToFind = 2;
    }

    // Update is called once per frame
    void Update()
    {
        coinText.text = "Coins: " + coinCount;
    }

    public void AddCoin()
    {
        coinCount++;
    }

}
