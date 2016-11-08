using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stack : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public int StackSize { get; set; }
    public List<Unit> Units { get; set; }

    public Stack() { }
    public Stack(int stackSize)
    {
        StackSize = stackSize;
        Units = new List<Unit>(stackSize);
    }
}
