using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public GameObject playerGameObj;
    private Player player;

    //public Canvas UI;

    public Text HPText;

    void Start()
    {
        player = playerGameObj.GetComponent<Player>();
    }

    void Update()
    {
        HPText.text = "HP: " + player.HP;
    }
}
