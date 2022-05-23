using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts.Units;
using Assets.Scripts.Utility;

public class CastleMenu : MonoBehaviour
{

    public Castle Castle { get; set; }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }

    public void SetCastleProduction(UnitType type)
    {
        if (Castle.ProductionUnits.Contains(type))
        {
            transform.Find("ProductionUnit").GetComponent<Image>().sprite = Resources.Load("UnitSprites/" + type.SpriteName, typeof(Sprite)) as Sprite;
            Castle.CurrentProduction = type;
            UpdateProduction();
        }
    }

    public void SetCastle(Castle activeCas)
    {
        Castle = activeCas;
        UpdateProduction();
    }

    public void OpenBuyProductionMenu()
    {
        GameObject buyProductionPanel = transform.Find("BuyProductionPanel").gameObject;
        buyProductionPanel.GetComponent<BuyProductionPanel>().SetUnits(Castle.Owner.PlayerUnits);
        transform.Find("BuyProductionPanel").gameObject.SetActive(true);
    }


    public void SetCastleProduction(int pos)
    {
        if (Castle.ProductionUnits.Count > pos)
        {
            int i = 0;
            foreach (UnitType u in Castle.ProductionUnits)
            {
                if (i == pos)
                {
                    SetCastleProduction(u);
                }
                i++;
            }
        }
    }

    private void UpdateProduction()
    {
        GameObject prodUnit = transform.Find("ProductionUnit").gameObject;
        Image unitUI = prodUnit.GetComponent<Image>();
        if (Castle.CurrentProduction != null)
        {
            UnitType type = Castle.CurrentProduction;
            unitUI.sprite = Resources.Load("UnitSprites/" + type.SpriteName, typeof(Sprite)) as Sprite;
            // Set the stats of the unit in the stat panel
            // TODO set fields rather than entire text field
            Transform unitStats = transform.Find("UnitPanel");
            unitStats.Find("UnitStats").GetComponent<Text>().text = @"Name: " + type.Name + @"
Strength: " + type.Strength + @"
Hits: " + type.Hits + @"
Move:" + type.Speed;
        }
        else
        {
            unitUI.sprite = Resources.Load("UnitSprites/" + "EmptyUIPlaceholder", typeof(Sprite)) as Sprite;
        }

        Transform prodSel = transform.Find("ProductionSelection");
        int i = 0;
        foreach (UnitType u in Castle.ProductionUnits)
        {
            if (i == Castle.ProductionUnits.Count)
            {
                i++;
                break;
            }
            prodSel.GetChild(i).GetComponent<Image>().sprite = Resources.Load("UnitSprites/" + u.SpriteName, typeof(Sprite)) as Sprite;
            prodSel.GetChild(i).GetComponent<Button>().enabled = true;
            i++;
        }
        for (int j = i; j < Constants.ProductionSize; j++)
        {
            prodSel.GetChild(j).GetComponent<Image>().sprite = Resources.Load("UnitSprites/" + "EmptyUIPlaceholder", typeof(Sprite)) as Sprite;
            prodSel.GetChild(j).GetComponent<Button>().enabled = false;
        }
    }
}
