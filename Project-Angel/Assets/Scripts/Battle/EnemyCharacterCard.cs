using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemyCharacterCard : MonoBehaviour
{

    public TextMeshProUGUI nameTxt;
    public TextMeshProUGUI healthTxt;
    public Image healthBar;

    public void UpdateCard(BattleCharacter character)
    {

        nameTxt.text = character.charInfo.characterNickName;
        healthTxt.text = character.Health.ToString("00");
        healthBar.fillAmount = (float)character.Health / (float)character.charInfo.baseMaxHealth;

    }

}
