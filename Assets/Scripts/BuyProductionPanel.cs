using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;

public class BuyProductionPanel : MonoBehaviour {


  private List<Unit> AvailableUnits = new List<Unit>();
  private Unit unitToBuy;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  public void SetUnits(PriorityQueueMin<Unit> playerUnits)
  {
    Transform unitPanel = transform.Find("UnitPurchasePanel");
    int unitPlace = 0;
    foreach (Unit unit in playerUnits)
    {
      unitPanel.GetChild(unitPlace).GetComponent<Image>().sprite = Resources.Load("UnitSprites/" + unit.SpriteName, typeof(Sprite)) as Sprite;
      AvailableUnits.Add(unit);
      unitPlace++;
    }
    for(int i= unitPlace; i < 4; i++)
    {
      unitPanel.GetChild(i).GetComponent<Button>().interactable = false;
    }
  }

  /// <summary>
  /// GUI function. Sets the unit to be purchased
  /// </summary>
  /// <param name="unitPlace">spot in Unit browser</param>
  public void SelectUnit(int unitPlace)
  {
    Unit selectUnit = AvailableUnits[unitPlace];
    Transform unitPurTrans = transform.Find("UnitForPurchase");
    unitPurTrans.GetComponent<Image>().sprite = Resources.Load("UnitSprites/" + selectUnit.SpriteName, typeof(Sprite)) as Sprite;
    unitToBuy = selectUnit;

    // TODO set fields rather than entire text field
    // TODO set from unitType
    unitPurTrans.Find("UnitPurchaseStats").GetComponent<Text>().text = @"Price:
Name: " + selectUnit.Name + @"
Strength: " + selectUnit.Strength + @"
Hits: " + selectUnit.Hits + @"
Move:" + selectUnit.Speed;
  }
}
