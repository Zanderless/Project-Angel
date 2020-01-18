using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Wreckless/Character/Default")]
public class CharacterInfo : ScriptableObject
{

    [Header("Character Info")]
    public string characterFullName;
    public string characterNickName;
    public Sprite characterPortrait; 

    [Header("Character Stats")]
    public int baseMaxHealth;
    public int baseMagic;
    public enum ElementalType
    {
        None,
        Water,
        Fire,
        Lightning,
        Ice,
        Earth,
        Death,
        Life
    }
    public ElementalType elementalType;

    public Weapon characterWeapon;

    public void SetNewWeapon(Weapon newWeapon)
    {
        characterWeapon = newWeapon;
    }

}
