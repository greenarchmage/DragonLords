using UnityEngine;
using System.Collections;

public class MainUI : MonoBehaviour {

  private static MainUI instance;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  public static MainUI GetUI()
  {
    if(instance == null)
    {
      instance = new MainUI();
    }
    return instance;
  }

  public void SetSelectedStack(Stack st)
  {
    for(int i = 0; i<st.Units.Count; i++)
    {

    }
  }
}
