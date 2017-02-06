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
  }

  public void SelectUnit(int unitPlace)
  {
    Unit selectUnit = AvailableUnits[unitPlace];
    transform.Find("UnitForPurchase").GetComponent<Image>().sprite = Resources.Load("UnitSprites/" + selectUnit.SpriteName, typeof(Sprite)) as Sprite;
    unitToBuy = selectUnit;
  }
}
