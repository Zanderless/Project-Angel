using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEnemy : BattleCharacter
{

    public override void TakeDamage(int dmg)
    {
        base.TakeDamage(dmg);
        BattleHUD.Instance.enemyCharacterCard.UpdateCard(this);
    }

}
