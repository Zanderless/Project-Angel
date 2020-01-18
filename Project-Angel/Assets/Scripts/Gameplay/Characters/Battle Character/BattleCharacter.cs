using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacter : MonoBehaviour
{

    public CharacterInfo charInfo;

    public bool isGaurding;

    public enum Stance { Balanced, Agressive, Defensive };
    public Stance currentStance;

    private int _health;
    public int Health
    {
        get { return _health; }
        protected set
        {
            _health = value;
            _health = Mathf.Clamp(_health, 0, charInfo.baseMaxHealth);

            if (_health <= 0)
                Die();

        }
    }

    public virtual void TakeDamage(BattleCharacter opponent, BattleManager.AttackType attack)
    {

        int dmg;

        if (attack == BattleManager.AttackType.Physical)
            dmg = opponent.charInfo.characterWeapon.attackDamage;
        else
            dmg = opponent.charInfo.baseMagic;

        if (opponent.currentStance == Stance.Agressive)
            dmg += Mathf.CeilToInt((float)dmg / 5);
        else if(opponent.currentStance == Stance.Defensive)
            dmg -= Mathf.CeilToInt((float)dmg / 5);

        if(currentStance == Stance.Agressive)
            dmg += Mathf.CeilToInt((float)dmg / 5);
        else if(currentStance == Stance.Defensive)
            dmg -= Mathf.CeilToInt((float)dmg / 5);

        if (opponent is BattleParty)
            if ((opponent as BattleParty).IsEnraged)
                dmg += Mathf.CeilToInt((float)dmg / 5);

        if (isGaurding)
            dmg = Mathf.CeilToInt((float)dmg / 2);

        Health -= dmg;

        GetComponent<Animator>().SetTrigger("Damaged");
    }

    public virtual void AddHealth(int hp)
    {
        Health += hp;
    }

    public virtual void Die()
    {
        GetComponent<Animator>().SetTrigger("Died");
        BattleManager.Instance.RemoveEnemy(this);
        Destroy(this.gameObject, 3f);
    }

    public virtual void UpdateStatusEffects()
    {

    }

    public virtual void Init()
    {
        Health = charInfo.baseMaxHealth;
        isGaurding = false;
        currentStance = Stance.Balanced;
    }

}
