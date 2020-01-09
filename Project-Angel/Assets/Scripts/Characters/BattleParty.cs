using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleParty : BattleCharacter
{
    public bool IsKnockedOut { get; private set; }

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

    private int _aura;
    public int Aura
    {
        get { return _aura; }
        set
        {
            _aura = value;
            _aura = Mathf.Clamp(_aura, 0, (charInfo as PartyInfo).baseMaxAura);
        }
    }

    public override void TakeDamage(int dmg)
    {
        base.TakeDamage(dmg);
        BattleHUD.Instance.UpdateCard(this);
    }

    public override void GiveHealth(int hp)
    {
        base.GiveHealth(hp);
        BattleHUD.Instance.UpdateCard(this);
    }

    public void AddMana(int mana)
    {
        Mana += mana;
        BattleHUD.Instance.UpdateCard(this);
    }

    public void AddAura(int aura)
    {
        Aura += aura;
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

    public override void Init()
    {
        base.Init();
        Mana = (charInfo as PartyInfo).baseMaxMana;
    }

}
