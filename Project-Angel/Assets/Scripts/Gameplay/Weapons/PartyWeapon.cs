using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Wreckless/Weapons/Party")]
public class PartyWeapon : Weapon
{
    public string weaponName;
    public Sprite weaponIcon;
    public int minLvlRequirement;

    public enum CharacterWeapon { Morgan, Titan, Bodie, Caroline, Summer};
    public CharacterWeapon characterWeapon;

}
