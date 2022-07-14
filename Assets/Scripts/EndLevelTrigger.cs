using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndLevelTrigger : MonoBehaviour
{
    private GameObject player;
    private GameObject EndGameCanvas;
    private int coins;
    private int coinsTotal;
    public Text coinsFound; 

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        EndGameCanvas = GameObject.Find("End_level_Canvas");
        EndGameCanvas.SetActive(false);
    }

    // Update is called once per frame

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            coins = player.GetComponent<PlayerInventory>().coinCount;
            coinsTotal = player.GetComponent<PlayerInventory>().coinsToFind;
            EndGameCanvas.SetActive(true);
            coinsFound.text = coins + "/" + coinsTotal;
            //player.SetActive(false);
        }


    }
}
