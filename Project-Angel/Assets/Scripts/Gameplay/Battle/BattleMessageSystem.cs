using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleMessageSystem : MonoBehaviour
{

    public static BattleMessageSystem Instance;

    private TextMeshProUGUI battleTxt;

    private float timer;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        battleTxt = GetComponent<TextMeshProUGUI>();
        battleTxt.enabled= false;
    }

    private void Update()
    {

        if (timer > 0)
            timer -= Time.deltaTime;
        else if(timer <= 0)
            battleTxt.enabled = false;

    }

    public void ShowMessage(string message)
    {
        timer = 1f;
        battleTxt.enabled = true;
        battleTxt.text = message;
    }

}
