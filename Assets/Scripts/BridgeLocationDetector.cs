using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeLocationDetector : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    //TELL CANVAS YOU HAVE ENTERED MARKET AREA
    private void OnTriggerEnter(Collider other)
    {
        Player.PlayerInventory otherInventory = null;
        otherInventory = other.GetComponent<Player.PlayerInventory>();
        if (otherInventory != null){
            other.GetComponent<Player.PlayerInventory>().myCanvas.EnteredMarketArea(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        
    }
    //TELL CANVAS YOU HAVE LEFT MARKET AREA
    private void OnTriggerExit(Collider other)
    {
        Player.PlayerInventory otherInventory = null;
        otherInventory = other.GetComponent<Player.PlayerInventory>();
        if (otherInventory != null){
            other.GetComponent<Player.PlayerInventory>().myCanvas.EnteredMarketArea(false);
        }
    }
}
