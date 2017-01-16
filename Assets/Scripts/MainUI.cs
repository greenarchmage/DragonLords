using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainUI : MonoBehaviour {
  
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  public void SetSelectedStack(Stack st)
  {
    Debug.Log(transform.FindChild("StackUnits"));
    // get the stack visualization
    Transform stackPanel = transform.FindChild("StackUnits");
    int i = 0;
    foreach(Unit u in st.Units)
    {
      stackPanel.GetChild(i).GetComponent<Image>().sprite = Resources.Load("UnitSprites/" + u.SpriteName, typeof(Sprite)) as Sprite;
      i++;
    }
  }

  public void ClearSelectedStack()
  {
    Transform stackPanel = transform.FindChild("StackUnits");
    // TODO make number in stack generic
    for(int i = 0; i < 8; i++)
    {
      stackPanel.GetChild(i).GetComponent<Image>().sprite = null;
    }
  }
}
