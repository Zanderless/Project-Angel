using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{

    public static PartyManager Instance;

    public List<WorldCharacter> partyCharaters = new List<WorldCharacter>();

    private void Awake()
    {
        Instance = this;
    }

}
