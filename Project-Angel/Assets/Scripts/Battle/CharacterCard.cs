using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterCard : MonoBehaviour
{

    [Header("Character Info")]
    public TextMeshProUGUI nameTxt;
    public Image characterPortrait;

    [Header("Health and Mana Text")]
    public TextMeshProUGUI healthTxt;
    public TextMeshProUGUI manaTxt;

    [Header("Health and Mana Bar")]
    public Image healthBar;
    public Image manaBar;

    [Header("Out and Guard Txt")]
    public GameObject outTxt;
    public GameObject guardTxt;

    [HideInInspector]
    public BattleCharacter character;

    public void InitCard(BattleCharacter _character)
    {
        character = _character;
        nameTxt.text = character.charInfo.characterNickName;
        SetGuardOutTxt(CharacterStats.None);
        UpdateCard();
    }

    public void UpdateCard()
    {

        //Health
        healthTxt.text = character.Health.ToString("00");
        healthBar.fillAmount = (float)character.Health / (float)character.charInfo.baseMaxHealth;

        //Mana
        manaTxt.text = (character as BattleParty).Mana.ToString("00");
        manaBar.fillAmount = (float)(character as BattleParty).Mana / (float)(character.charInfo as PartyInfo).baseMaxMana;

    }

    public enum CharacterStats { Out, Guard, None};

    public void SetGuardOutTxt(CharacterStats stats)
    {

        switch (stats)
        {
            case CharacterStats.None:
                outTxt.SetActive(false);
                guardTxt.SetActive(false);
                break;
            case CharacterStats.Out:
                outTxt.SetActive(true);
                guardTxt.SetActive(false);
                break;
            case CharacterStats.Guard:
                outTxt.SetActive(false);
                guardTxt.SetActive(true);
                break;
            default:
                print("Uh Oh!");
                break;
        }

    }

    public void Destroy()
    {
        GameObject.Destroy(this.gameObject);
    }

}
