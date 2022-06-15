using UnityEngine;
using UnityEngine.UI;

public class TopPanelUI : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var curPlayer = GameController.Instance.CurrentGameData.CurrentPlayer;
        if (curPlayer != null)
        {
            transform.Find("PlayerPanel").Find("PlayerName").GetComponent<Text>().text = curPlayer.Name;
            transform.Find("PlayerPanel").Find("GoldVal").GetComponent<Text>().text = "" + curPlayer.Gold;
        }
    }
}
