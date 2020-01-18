using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Wreckless/Character/Party")]
public class PartyInfo : CharacterInfo
{

    [Header("Party Character Info")]
    public int baseMaxMana;
    public int baseMaxTP;

}
