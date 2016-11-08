using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stack {

  public int StackSize { get; set; }
  public List<Unit> Units { get; set; }

  public Stack() { }
  public Stack(int stackSize)
  {
    StackSize = stackSize;
    Units = new List<Unit>(stackSize);
  }
}
