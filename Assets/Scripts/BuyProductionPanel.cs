using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Units;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;

public class BuyProductionPanel : MonoBehaviour {


  private List<UnitType> AvailableUnitTypes = new List<UnitType>();
  private UnitType unitTypeToBuy;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  public void SetUnits(PriorityQueueMin<UnitType> playerUnits)
  {
    Transform unitPanel = transform.Find("UnitPurchasePanel");
    int unitPlace = 0;
    foreach (UnitType unit in playerUnits)
    {
      unitPanel.GetChild(unitPlace).GetComponent<Image>().sprite = Resources.Load("UnitSprites/" + unit.SpriteName, typeof(Sprite)) as Sprite;
      AvailableUnitTypes.Add(unit);
      unitPlace++;
    }
    for(int i= unitPlace; i < 4; i++)
    {
      unitPanel.GetChild(i).GetComponent<Button>().interactable = false;
    }
  }

  /// <summary>
  /// GUI function. Sets the unitType to be purchased
  /// </summary>
  /// <param name="unitPlace">spot in Unit browser</param>
  public void SelectUnit(int unitPlace)
  {
    UnitType selectUnitType = AvailableUnitTypes[unitPlace];
    Transform unitPurTrans = transform.Find("UnitForPurchase");
    unitPurTrans.GetComponent<Image>().sprite = Resources.Load("UnitSprites/" + selectUnitType.SpriteName, typeof(Sprite)) as Sprite;
    unitTypeToBuy = selectUnitType;

    // TODO set fields rather than entire text field
    unitPurTrans.Find("UnitPurchaseStats").GetComponent<Text>().text = @"Price: " + selectUnitType.Price + @"
Name: " + selectUnitType.Name + @"
Strength: " + selectUnitType.Strength + @"
Hits: " + selectUnitType.Hits + @"
Move:" + selectUnitType.Speed;
  }

  public void BuyProduction()
  {
    // get the player, check if there is enough gold, if true, set the production in the castle menu
    CastleMenu casMen = transform.parent.GetComponent<CastleMenu>();
    if(casMen.Castle.Owner.Gold >= unitTypeToBuy.Price)
    {
      casMen.Castle.ProductionUnits.Insert(unitTypeToBuy);
      casMen.Castle.Owner.Gold -= unitTypeToBuy.Price;
      casMen.SetCastleProduction(unitTypeToBuy);
      gameObject.SetActive(false);
    } else
    {
      // TODO handle not enough money
    }
  }

}
