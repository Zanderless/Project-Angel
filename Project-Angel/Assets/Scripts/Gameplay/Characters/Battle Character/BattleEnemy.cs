using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEnemy : BattleCharacter
{

    public override void TakeDamage(BattleCharacter opponent, BattleManager.AttackType attack)
    {
        base.TakeDamage(opponent, attack);
        BattleHUD.Instance.enemyCharacterCard.UpdateCard(this);
    }

}
