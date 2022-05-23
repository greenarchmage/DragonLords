using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Units;
using Assets.Scripts.Utility;
using TMPro;

/// <summary>
/// Panel for handling the display on the bottom of the screen
/// </summary>
public class BottomUI : MonoBehaviour
{
    private Stack Stack { get; set; }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Stack != null)
        {
            // TODO consider if this is to resource intensive
            UpdateMovement();
        }
    }

    public void SetSelectedStack(Stack st)
    {
        ClearSelectedStack();
        Stack = st;
        // get the stack visualization
        Transform stackPanel = transform.Find("StackUnits");
        int i = 0;
        foreach (Unit u in st.Units)
        {
            var unitObj = stackPanel.GetChild(i);
            unitObj.GetComponent<Image>().sprite = Resources.Load("UnitSprites/" + u.SpriteName, typeof(Sprite)) as Sprite;
            i++;
        }
    }

    private void UpdateMovement()
    {
        // get the stack visualization
        Transform stackPanel = transform.Find("StackUnits");
        int i = 0;
        foreach (Unit u in Stack.Units)
        {
            var unitObj = stackPanel.GetChild(i);
            unitObj.GetChild(0).GetComponent<TextMeshProUGUI>().text = u.RemainingMovement.ToString();
            i++;
        }
    }

    public void ClearSelectedStack()
    {
        Stack = null;
        Transform stackPanel = transform.Find("StackUnits");
        for (int i = 0; i < Constants.StackSize; i++)
        {
            var unitObj = stackPanel.GetChild(i);
            unitObj.GetComponent<Image>().sprite = null;
            unitObj.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        }
    }

}
