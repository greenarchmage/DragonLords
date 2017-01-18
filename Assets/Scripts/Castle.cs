using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Castle : MonoBehaviour {

  public Unit CurrentProduction { get; set; }
  public Player Owner { get; set; }
  
  public List<Unit> ProductionUnits { get; set; }
  public List<Stack> Garrison { get; set; }
	// Use this for initialization
	void Start () {
    Garrison = new List<Stack>();
    ProductionUnits = new List<Unit>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnTriggerEnter2D(Collider2D other)
  {
    Debug.Log("Entering castle " + other.transform.name);
    Stack enterStack = other.gameObject.GetComponent<Stack>();
    if (enterStack != null)
    {
      if (enterStack.Owner == Owner)
      {
        Debug.Log("Add stack to castle");
        Garrison.Add(enterStack);
      }
    }
  }

  void OnTriggerExit2D(Collider2D other)
  {
    Debug.Log("Exiting castle " + other.transform.name);
    Stack enterStack = other.gameObject.GetComponent<Stack>();
    if (enterStack != null)
    {
      if (enterStack.Owner == Owner)
      {
        Debug.Log("Remove stack from castle");
        Garrison.Remove(enterStack);
      }
    }
  }
}
