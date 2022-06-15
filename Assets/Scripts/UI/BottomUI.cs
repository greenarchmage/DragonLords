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
    private Stack ActiveStack { get; set; }
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
        ActiveStack = st;
        Stacks.Add(st);
        UpdateUnitGUI();
    }

    private void UpdateUnitGUI()
    {
        ClearStackGUI();
        // get the stack visualization
        Transform stackPanel = transform.Find("StackUnits");
        int i = 0;
        foreach(var stack in Stacks)
        {
            foreach (Unit u in stack.Units)
            {
                var unitObj = stackPanel.GetChild(i);
                // TODO show the correct sprite based on sprite index
                unitObj.GetComponent<Image>().sprite = Resources.Load("UnitSprites/" + u.SpriteName, typeof(Sprite)) as Sprite;
                unitObj.GetChild(0).GetComponent<TextMeshProUGUI>().text = u.RemainingMovement.ToString();
                i++;
            }
        }
    }
    private void ClearStackGUI()
    {
        Transform stackPanel = transform.Find("StackUnits");
        for (int i = 0; i < Constants.StackSize; i++)
        {
            var unitObj = stackPanel.GetChild(i);
            unitObj.GetComponent<Image>().sprite = null;
            unitObj.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        }
    }

    private void UpdateMovement()
    {
        // Get all the stacks of the current stacks owner. If a stack passes over a stack add it temporaely
        var gameCon = GameController.Instance;
        var curPlayer = gameCon.CurrentGameData.CurrentPlayer;
        Stacks = new List<Stack>() { ActiveStack };
        foreach(var stack in gameCon.AllStacks)
        {
            if(stack.Owner.Name == curPlayer.Name)
            {
                // this is a stack to check
                Vector3 pos = stack.transform.position;
                pos.x = Mathf.Round(pos.x);
                pos.y = Mathf.Round(pos.y);
                pos.z = 0;
                var activePos = ActiveStack.transform.position;
                activePos.x = Mathf.Round(activePos.x);
                activePos.y = Mathf.Round(activePos.y);
                activePos.z = 0;
                if(pos == activePos && stack != ActiveStack && !Stacks.Contains(stack))
                {
                    Stacks.Add(stack);
                }
            }
        }
        UpdateUnitGUI();
    }

    public void ClearSelectedStack()
    {
        Stacks = new List<Stack>();
        ClearStackGUI();
    }
}
