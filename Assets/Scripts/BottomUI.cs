using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Units;
using Assets.Scripts.Utility;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Panel for handling the display on the bottom of the screen
/// </summary>
public class BottomUI : MonoBehaviour
{
    // Handle a list of stacks for when more than one is present atop each other
    private List<Stack> Stacks { get; set; }
    //private Stack Stack { get; set; }
    // Use this for initialization
    void Start()
    {
        Stacks = new List<Stack>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Stacks != null && Stacks.Count > 0)
        {
            // TODO consider if this is to resource intensive
            UpdateMovement();
        }
    }

    public void SetSelectedStack(Stack st)
    {
        ClearSelectedStack();
        Stacks.Add(st);
        // get the stack visualization
        Transform stackPanel = transform.Find("StackUnits");
        int i = 0;
        foreach (Unit u in st.Units)
        {
            var unitObj = stackPanel.GetChild(i);
            // TODO show the correct sprite based on sprite index
            unitObj.GetComponent<Image>().sprite = Resources.Load("UnitSprites/" + u.SpriteName, typeof(Sprite)) as Sprite;
            i++;
        }
    }

    private void UpdateMovement()
    {
        // get the stack visualization
        Transform stackPanel = transform.Find("StackUnits");
        int i = 0;
        foreach(var stack in Stacks)
        {
            foreach (Unit u in stack.Units)
            {
                var unitObj = stackPanel.GetChild(i);
                unitObj.GetChild(0).GetComponent<TextMeshProUGUI>().text = u.RemainingMovement.ToString();
                i++;
            }
        }
    }

    public void ClearSelectedStack()
    {
        Stacks = new List<Stack>();
        Transform stackPanel = transform.Find("StackUnits");
        for (int i = 0; i < Constants.StackSize; i++)
        {
            var unitObj = stackPanel.GetChild(i);
            unitObj.GetComponent<Image>().sprite = null;
            unitObj.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        }
    }
}
