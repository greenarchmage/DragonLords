using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stack : MonoBehaviour
{
  public Player Owner { get; set; }
  public int StackSize { get; set; }
  public List<Unit> Units { get { return units; } set { units = value; } }

  private List<Unit> units = new List<Unit>();

  // Use this for initialization
  void Start()
  {
  }

  // Update is called once per frame
  void Update()
  {

  }

  

  //public Stack() { }
  //public Stack(int stackSize)
  //{
  //  StackSize = stackSize;
  //  Units = new List<Unit>(stackSize);
  //}
}
