using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Wreckless/Items/Restorable")]
public class Restorables : Item
{
    public enum RestoreType { Health, Mana, Revive };

    [Header("Restorable Info")]
    public RestoreType restoreType;
    public int restoreValue;

}
