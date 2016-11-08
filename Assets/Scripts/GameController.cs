using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

  public Camera main;

  private GameObject selectedUnit;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {

          RaycastHit2D hit = Physics2D.Raycast(main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
          if(hit)
          {
            selectedUnit = hit.collider.gameObject;
            Debug.Log(hit.collider.transform.name);
          }
        }

        if (Input.GetMouseButtonDown(1) && selectedUnit != null)
        {
            Vector3 pos = main.ScreenToWorldPoint(Input.mousePosition);
            pos.x = Mathf.Round(pos.x);
            pos.y = Mathf.Round(pos.y);
            pos.z = 0;
            selectedUnit.transform.position = pos;
        }
	}
}
