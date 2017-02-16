using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopPanelUI : MonoBehaviour {

  private Player curPlayer;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(curPlayer != null)
    {
      transform.FindChild("PlayerPanel").FindChild("PlayerName").GetComponent<Text>().text = curPlayer.Name;
      transform.FindChild("PlayerPanel").FindChild("GoldVal").GetComponent<Text>().text = "" + curPlayer.Gold;
    }
	}

  public void SetCurrentPlayer(Player cur)
  {
    curPlayer = cur;
  }
}
