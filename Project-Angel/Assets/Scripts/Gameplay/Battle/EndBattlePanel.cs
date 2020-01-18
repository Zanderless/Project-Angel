using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndBattlePanel : MonoBehaviour
{

    public TextMeshProUGUI battleResultTxt;

    public string battleWinText, battleLoseText;

    public TextMeshProUGUI expTxt;

    private string expText = "Exp Earned: ";

    public enum BattleResult { Win, Lose};

    private bool canEndBattle;

    public void InitPanel(BattleResult result)
    {

        BattleHUD.Instance.UpdateMenu(BattleHUD.SelectionMenu.End);

        switch (result)
        {
            case BattleResult.Win:
                battleResultTxt.text = battleWinText;
                expTxt.text = expText + "120";
                break;
            case BattleResult.Lose:
                battleResultTxt.text = battleLoseText;
                expTxt.text = expText + "0";
                break;
            default:
                print("Uh Oh!");
                break;
        }

        canEndBattle = true;

    }

    private void Update()
    {
        if(canEndBattle && Input.GetKeyDown(KeyCode.Space))
        {
            canEndBattle = false;
            BattleManager.Instance.EndBattle();
        }
    }

}
