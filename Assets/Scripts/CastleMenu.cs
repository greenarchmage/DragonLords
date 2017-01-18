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
    transform.Find("ProductionUnit").GetComponent<Image>().sprite = Resources.Load("UnitSprites/" + castle.ProductionUnits[pos].SpriteName, typeof(Sprite)) as Sprite;
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
    for (int i = 0; i<castle.ProductionUnits.Count; i++)
    {
      prodSel.GetChild(i).GetComponent<Image>().sprite = Resources.Load("UnitSprites/" + castle.ProductionUnits[i].SpriteName, typeof(Sprite)) as Sprite; 
    }
  }
}
