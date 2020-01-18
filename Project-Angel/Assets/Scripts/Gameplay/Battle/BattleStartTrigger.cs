using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStartTrigger : MonoBehaviour
{

    public WorldCharacter[] enemies;

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            BattleManager.Instance.InitBattle(enemies);
            this.enabled = false;
        }

    }

}
