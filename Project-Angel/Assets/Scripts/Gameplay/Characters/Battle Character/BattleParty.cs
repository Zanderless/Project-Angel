using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleParty : BattleCharacter
{
    public bool IsKnockedOut { get; private set; }
    public bool IsEnraged { get; private set; }
    public int enragedTurnsMax;
    public int enragedTurnsLeft;

    private int _mana;
    public int Mana
    {
        get { return _mana; }
        set
        {
            _mana = value;
            _mana = Mathf.Clamp(_mana, 0, (charInfo as PartyInfo).baseMaxMana);
        }
    }

    private int _tp;
    public int TP
    {
        get { return _tp; }
        set
        {
            _tp = value;
            _tp = Mathf.Clamp(_tp, 0, (charInfo as PartyInfo).baseMaxTP);
        }
    }

    public override void TakeDamage(BattleCharacter opponent, BattleManager.AttackType attack)
    {
        base.TakeDamage(opponent, attack);
        BattleHUD.Instance.UpdateCard(this);
    }

    public override void AddHealth(int hp)
    {
        base.AddHealth(hp);
        BattleHUD.Instance.UpdateCard(this);
    }

    public void AddMana(int mana)
    {
        Mana += mana;
        BattleHUD.Instance.UpdateCard(this);
    }

    public void TakeMana(int mana)
    {
        Mana -= mana;
        BattleHUD.Instance.UpdateCard(this);
    }

    public void AddTP(int tp)
    {
        TP += tp;
        BattleHUD.Instance.UpdateCard(this);
    }

    public void TakeTP(int tp)
    {
        TP -= tp;
        BattleHUD.Instance.UpdateCard(this);
    }

    public override void Die()
    {
        IsKnockedOut = true;
        GetComponent<Animator>().SetTrigger("Died");
        BattleHUD.Instance.UpdateCharacterStats(this, CharacterCard.CharacterStats.Out);
    }

    public void Revive(int healthRestore)
    {
        Health += healthRestore;
        IsKnockedOut = false;
        GetComponent<Animator>().SetTrigger("Revive");
        BattleHUD.Instance.UpdateCharacterStats(this, CharacterCard.CharacterStats.None);
        BattleHUD.Instance.UpdateCard(this);
    }

    public override void UpdateStatusEffects()
    {

        base.UpdateStatusEffects();

        enragedTurnsLeft--;

        if(enragedTurnsLeft == 0)
        {
            IsEnraged = false;
        }

        BattleHUD.Instance.UpdateCard(this);

    }

    public void EnrageCharacter()
    {
        IsEnraged = true;
        enragedTurnsLeft = enragedTurnsMax;
    }

    public override void Init()
    {
        base.Init();
        Mana = (charInfo as PartyInfo).baseMaxMana;
        TP = (charInfo as PartyInfo).baseMaxTP;
        enragedTurnsLeft = 0;
    }

}
