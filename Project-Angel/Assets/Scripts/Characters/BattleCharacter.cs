using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacter : MonoBehaviour
{

    public CharacterInfo charInfo;

    public int level;
    public int exp;

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

    public virtual void TakeDamage(int dmg)
    {
        Health -= dmg;
        GetComponent<Animator>().SetTrigger("Damaged");
    }

    public virtual void GiveHealth(int hp)
    {
        Health += hp;
    }

    public virtual void Die()
    {
        GetComponent<Animator>().SetTrigger("Died");
        BattleManager.Instance.RemoveEnemy(this);
        Destroy(this.gameObject, 3f);
    }

    public virtual void Init()
    {
        Health = charInfo.baseMaxHealth;
    }

}
