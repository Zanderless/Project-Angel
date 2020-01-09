using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{

    #region Variables

    public static BattleManager Instance;

    private List<BattleCharacter> spawnedInCharacters; //List of total characters spawned in
    private List<BattleCharacter> partyList; //List of party characters spawned in
    private List<BattleCharacter> enemyList; //List of enemy characters spawned in

    public Transform partySpawns; //Parent of spawn points for party
    public Transform enemySpawns; //Parent of spawn points for enemies;

    private int _turnIndex; //Determins who's turn it is based on spawnedInCharacters list
    private int TurnIndex
    {
        get { return _turnIndex; }
        set
        {

            _turnIndex = value;

            if (_turnIndex == spawnedInCharacters.Count)
                _turnIndex = 0;

            NextTurn();
        }
    }

    public bool InBattle { get; private set; } //Is the player in battle?

    public Transform battleArenaParent; //Parent object of Battle Arena to spawn characters in
    public Camera battleCamera; //Camera used in battle

    public enum AttackType { Physical, Magical};

    private Dictionary<BattleCharacter, bool> guardingDictionary = new Dictionary<BattleCharacter, bool>();

    #endregion

    #region Battle Setup

    public void InitBattle(WorldCharacter[] _enemies)
    {
        //Remove All unwanted character models if any still exists
        if (spawnedInCharacters != null)
            DeleteCharacters();

        //Initilize or Reinitilize all lists
        spawnedInCharacters = new List<BattleCharacter>();
        partyList = new List<BattleCharacter>();
        enemyList = new List<BattleCharacter>();
        guardingDictionary = new Dictionary<BattleCharacter, bool>();

        //Spawn in new characters
        SpawnCharacters(_enemies);

        InBattle = true;

        BattleHUD.Instance.InitHUD(partyList);

        TurnIndex = 0;
    }

    public Restorables castHealth;

    //Deletes all characters still on the field
    private void DeleteCharacters()
    {
        if (spawnedInCharacters.Count > 0)
            foreach (BattleCharacter c in spawnedInCharacters)
            {
                Destroy(c.gameObject);
            }
    }

    private void SpawnCharacters(WorldCharacter[] _enemies)
    {

        int spawnIndex = 0;

        foreach (WorldCharacter c in PartyManager.Instance.partyCharaters)
        {
            BattleCharacter character = Instantiate(c.battleCharacterPrefab, partySpawns.GetChild(spawnIndex).position, partySpawns.GetChild(spawnIndex).rotation, battleArenaParent).GetComponent<BattleCharacter>();
            character.Init();
            spawnedInCharacters.Add(character);
            partyList.Add(character);
            guardingDictionary.Add(character, false);
            spawnIndex++;
        }

        spawnIndex = 0;

        foreach (WorldCharacter c in _enemies)
        {
            BattleCharacter character = Instantiate(c.battleCharacterPrefab, enemySpawns.GetChild(spawnIndex).position, enemySpawns.GetChild(spawnIndex).rotation, battleArenaParent).GetComponent<BattleCharacter>();
            character.Init();
            spawnedInCharacters.Add(character);
            enemyList.Add(character);
            guardingDictionary.Add(character, false);
            spawnIndex++;
        }

    }

    #endregion

    #region Party Turn

    public void AttackWeapon()
    {

        BattleHUD.Instance.UpdateMenu(BattleHUD.SelectionMenu.Character);
        BattleHUD.Instance.InitButtons(enemyList, AttackType.Physical);

    }

    public void AttackCast()
    {
        if (BattleHUD.Instance.GetCharacterForm(spawnedInCharacters[TurnIndex]) == 0 ||
            BattleHUD.Instance.GetCharacterForm(spawnedInCharacters[TurnIndex]) == 1) {
            BattleHUD.Instance.UpdateMenu(BattleHUD.SelectionMenu.Character);
            BattleHUD.Instance.InitButtons(enemyList, AttackType.Magical);
        }
        else if (BattleHUD.Instance.GetCharacterForm(spawnedInCharacters[TurnIndex]) == 2)
        {
            BattleHUD.Instance.UpdateMenu(BattleHUD.SelectionMenu.Character);
            BattleHUD.Instance.InitButtons(partyList);
        }
    }

    public void Guard()
    {

        guardingDictionary[spawnedInCharacters[TurnIndex]] = true;
        spawnedInCharacters[TurnIndex].GetComponent<Animator>().SetBool("IsGuarding", true);
        BattleHUD.Instance.UpdateCharacterStats(spawnedInCharacters[TurnIndex], CharacterCard.CharacterStats.Guard);
        TurnIndex++;

    }

    public void Item()
    {
        BattleHUD.Instance.UpdateMenu(BattleHUD.SelectionMenu.Inventory);
        BattleHUD.Instance.InitButtons();
    }

    public void Conjure()
    {
        print("Conjure");
    }

    #endregion

    #region Enemy Turn

    private IEnumerator EnemyTurn()
    {

        yield return new WaitForSeconds(2.5f);

        BattleCharacter target = null;

        target = partyList[Random.Range(0, partyList.Count)];

        StartCoroutine(DealDamage(target, AttackType.Physical));

    }

    #endregion

    #region Battle

    private void NextTurn()
    {

        if (enemyList.Count == 0)
        {
            EndBattle();
            return;
        }

        if(spawnedInCharacters[TurnIndex].charInfo is PartyInfo)
        {

            if ((spawnedInCharacters[TurnIndex] as BattleParty).IsKnockedOut)
                TurnIndex++;

            guardingDictionary[spawnedInCharacters[TurnIndex]] = false;
            spawnedInCharacters[TurnIndex].GetComponent<Animator>().SetBool("IsGuarding", false);
            BattleHUD.Instance.UpdateCharacterStats(spawnedInCharacters[TurnIndex], CharacterCard.CharacterStats.None);
            BattleHUD.Instance.UpdateMenu(BattleHUD.SelectionMenu.Main);
            BattleHUD.Instance.UpdateMainMenuName(spawnedInCharacters[TurnIndex]);
        }
        else
        {
            guardingDictionary[spawnedInCharacters[TurnIndex]] = false;
            spawnedInCharacters[TurnIndex].GetComponent<Animator>().SetBool("IsGuarding", false);
            BattleHUD.Instance.UpdateMenu(BattleHUD.SelectionMenu.None);
            StartCoroutine(EnemyTurn());
        }

    }

    public IEnumerator DealDamage(BattleCharacter character, AttackType type)
    {

        string animName = null;

        animName = (type == AttackType.Physical) ? "Attack" : "MagicAttack";

        BattleHUD.Instance.UpdateMenu(BattleHUD.SelectionMenu.None);

        spawnedInCharacters[TurnIndex].GetComponent<Animator>().SetTrigger(animName);
        AnimatorClipInfo[] currentClipInfo = spawnedInCharacters[TurnIndex].GetComponent<Animator>().GetCurrentAnimatorClipInfo(0);
        yield return new WaitForSeconds(currentClipInfo[0].clip.length);

        if (guardingDictionary[character] == false)
            character.SendMessage("TakeDamage", spawnedInCharacters[TurnIndex].charInfo.baseStrength);
        else
        {
            character.GetComponent<Animator>().SetBool("IsGuarding", false);
            BattleHUD.Instance.UpdateCharacterStats(character, CharacterCard.CharacterStats.None);
            character.SendMessage("TakeDamage", spawnedInCharacters[TurnIndex].charInfo.baseStrength / 2);
            guardingDictionary[character] = false;
        }

        TurnIndex++;

    }

    public IEnumerator RestoreCharacter(BattleCharacter character, Restorables item)
    {

        BattleHUD.Instance.UpdateMenu(BattleHUD.SelectionMenu.None);

        spawnedInCharacters[TurnIndex].GetComponent<Animator>().SetTrigger("UseItem");
        AnimatorClipInfo[] currentClipInfo = spawnedInCharacters[TurnIndex].GetComponent<Animator>().GetCurrentAnimatorClipInfo(0);
        yield return new WaitForSeconds(currentClipInfo[0].clip.length);

        if (item.restoreType == Restorables.RestoreType.Health)
            character.GiveHealth(item.restoreValue);
        else if (item.restoreType == Restorables.RestoreType.Mana)
            (character as BattleParty).AddMana(item.restoreValue);
        else if(item.restoreType == Restorables.RestoreType.Revive)
            (character as BattleParty).Revive(item.restoreValue);

        Inventory.Instance.RemoveItem(item as Item);

        TurnIndex++;

    }

    private void EndBattle()
    {
        InBattle = false;
    }

    public void RemoveEnemy(BattleCharacter character)
    {

        enemyList.Remove(character);
        spawnedInCharacters.Remove(character);

    }

    #endregion

    public List<BattleCharacter> GetPartyList()
    {
        return partyList;
    }

    #region Unity Methods

    private void Update()
    {

        if (battleCamera.gameObject.activeSelf != InBattle)
            battleCamera.gameObject.SetActive(InBattle);

        if (!InBattle)
            return;

        if (Input.GetKeyDown(KeyCode.Tab) && BattleHUD.Instance.selectionMenu == BattleHUD.SelectionMenu.Main)
        {
            BattleHUD.Instance.UpdateCharacterForm(spawnedInCharacters[TurnIndex]);

            string messageTxt = "Form Changed\n";

            switch (BattleHUD.Instance.GetCharacterForm(spawnedInCharacters[TurnIndex]))
            {
                case 0:
                    messageTxt += "Balanced Form";
                    break;
                case 1:
                    messageTxt += "Aggresive Form";
                    break;
                case 2:
                    messageTxt += "Defensive Form";
                    break;
                default:
                    print("Uh Oh!");
                    break;
            }

            BattleMessageSystem.Instance.ShowMessage(messageTxt);

        }


    }

    private void Awake()
    {
        Instance = this;
    }

    #endregion

}
