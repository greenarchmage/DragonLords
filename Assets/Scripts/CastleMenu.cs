using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CastleMenu : MonoBehaviour {

  private Castle castle;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  public void CloseMenu()
  {
    gameObject.SetActive(false);
  }

  public void SetCastleProduction(int pos)
  {
    if(castle.ProductionUnits.Count > pos)
    {
      int i = 0;
      foreach(Unit u in castle.ProductionUnits)
      {
        if(i == pos)
        {
          transform.Find("ProductionUnit").GetComponent<Image>().sprite = Resources.Load("UnitSprites/" + u.SpriteName, typeof(Sprite)) as Sprite;
        }
        i++;
      }
    }
  }

  public void SetCastle(Castle activeCas)
  {
    castle = activeCas;
    GameObject prodUnit = transform.Find("ProductionUnit").gameObject;
    Image unitUI = prodUnit.GetComponent<Image>();
    if (castle.CurrentProduction != null)
    {
      unitUI.sprite = Resources.Load("UnitSprites/" + castle.CurrentProduction.SpriteName, typeof(Sprite)) as Sprite;
    } else
    {
      unitUI.sprite = Resources.Load("UnitSprites/" + "EmptyUIPlaceholder", typeof(Sprite)) as Sprite;
    }
    Transform prodSel = transform.Find("ProductionSelection");
    int i = 0;
    foreach(Unit u in castle.ProductionUnits)
    {
      if(i == castle.ProductionUnits.Count)
      {
        break;
      }
      prodSel.GetChild(i).GetComponent<Image>().sprite = Resources.Load("UnitSprites/" + u.SpriteName, typeof(Sprite)) as Sprite;
      i++;
    }
  }

  public void OpenBuyProductionMenu()
  {
    GameObject buyProductionPanel = transform.Find("BuyProductionPanel").gameObject;
    buyProductionPanel.GetComponent<BuyProductionPanel>().SetUnits(castle.Owner.PlayerUnits);
    transform.Find("BuyProductionPanel").gameObject.SetActive(true);
  }

  public void BuyProduction()
  {
    // get the selected unit from the panel, subtract its value from the player gold, add it to the castle buy queue

    transform.Find("BuyProductionPanel").gameObject.SetActive(false);
  }
}
