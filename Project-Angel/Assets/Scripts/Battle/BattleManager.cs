using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{

    #region Variables

    public static BattleManager Instance;

    private List<BattleCharacter> characterList; //List of total characters spawned in
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

            if (_turnIndex == characterList.Count)
                _turnIndex = 0;

            NextTurn();
        }
    }

    public bool InBattle { get; private set; } //Is the player in battle?

    public Transform battleArenaParent; //Parent object of Battle Arena to spawn characters in
    public Camera battleCamera; //Camera used in battle

    public enum AttackType { Physical, Magical };

    public Restorables castHealth;

    #endregion

    #region Battle Setup

    public void InitBattle(WorldCharacter[] _enemies)
    {
        //Remove All unwanted character models if any still exists
        if (characterList != null)
            DeleteCharacters();

        //Initilize or Reinitilize all lists
        characterList = new List<BattleCharacter>();
        partyList = new List<BattleCharacter>();
        enemyList = new List<BattleCharacter>();

        //Spawn in new characters
        SpawnCharacters(_enemies);

        InBattle = true;

        BattleHUD.Instance.InitHUD(partyList);

        TurnIndex = 0;
    }

    //Deletes all characters still on the field
    private void DeleteCharacters()
    {
        if (characterList.Count > 0)
            foreach (BattleCharacter c in characterList)
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
            characterList.Add(character);
            partyList.Add(character);
            spawnIndex++;
        }

        spawnIndex = 0;

        foreach (WorldCharacter c in _enemies)
        {
            BattleCharacter character = Instantiate(c.battleCharacterPrefab, enemySpawns.GetChild(spawnIndex).position, enemySpawns.GetChild(spawnIndex).rotation, battleArenaParent).GetComponent<BattleCharacter>();
            character.Init();
            characterList.Add(character);
            enemyList.Add(character);
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

    public void Cast()
    {
        if ((characterList[TurnIndex] as BattleParty).currentStance == BattleParty.Stance.Defensive)
        {
            BattleHUD.Instance.UpdateMenu(BattleHUD.SelectionMenu.Character);
            BattleHUD.Instance.InitButtons(partyList);
        }
        else
        {
            BattleHUD.Instance.UpdateMenu(BattleHUD.SelectionMenu.Character);
            BattleHUD.Instance.InitButtons(enemyList, AttackType.Magical);
        }
    }

    public void Guard()
    {

        characterList[TurnIndex].isGaurding = true;
        characterList[TurnIndex].GetComponent<Animator>().SetBool("IsGuarding", true);
        BattleHUD.Instance.UpdateCharacterStats(characterList[TurnIndex], CharacterCard.CharacterStats.Guard);
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

    public int GetAllKnockedoutCharacters()
    {
        int knockedoutMembers = 0;

        foreach (BattleCharacter c in partyList)
        {
            if ((c as BattleParty).IsKnockedOut)
                knockedoutMembers++;
        }

        return knockedoutMembers;
    }

    public int CalculatePlayerDamage(AttackType type)
    {

        int damage = 0;

        //ToDo add leveled strength and magic

        if (type == AttackType.Physical)
            damage = characterList[TurnIndex].charInfo.baseStrength;
        else
            damage = characterList[TurnIndex].charInfo.baseMagic;

        if ((characterList[TurnIndex] as BattleParty).currentStance == BattleParty.Stance.Agressive)
            damage += Mathf.CeilToInt((float)damage / 5);
        else if ((characterList[TurnIndex] as BattleParty).currentStance == BattleParty.Stance.Defensive)
            damage -= Mathf.CeilToInt((float)damage / 5);

        return damage;

    }

    #endregion

    #region Enemy Turn

    private IEnumerator EnemyTurn()
    {

        yield return new WaitForSeconds(2.5f);

        BattleCharacter target = null;

        do
        {
            target = partyList[Random.Range(0, partyList.Count)];
        } while ((target as BattleParty).IsKnockedOut);

        StartCoroutine(DealDamage(target, AttackType.Physical));

    }

    private int CalculateEnemyDamage(AttackType type, BattleCharacter target)
    {

        int damage = 0;

        if (type == AttackType.Physical)
            damage = characterList[TurnIndex].charInfo.baseStrength;
        else
            damage = characterList[TurnIndex].charInfo.baseMagic;

        if ((characterList[TurnIndex] as BattleParty).currentStance == BattleParty.Stance.Agressive)
            damage -= Mathf.CeilToInt((float)damage / 5);
        else if ((characterList[TurnIndex] as BattleParty).currentStance == BattleParty.Stance.Defensive)
            damage += Mathf.CeilToInt((float)damage / 5);

        return damage;

    }

    #endregion

    #region Battle

    private void NextTurn()
    {

        if (enemyList.Count == 0)
        {
            BattleHUD.Instance.battleEndPanel.GetComponent<EndBattlePanel>().InitPanel(EndBattlePanel.BattleResult.Win);
            return;
        }



        if (GetAllKnockedoutCharacters() == partyList.Count)
        {
            BattleHUD.Instance.battleEndPanel.GetComponent<EndBattlePanel>().InitPanel(EndBattlePanel.BattleResult.Lose);
            return;
        }

        if (characterList[TurnIndex].charInfo is PartyInfo)
        {

            if ((characterList[TurnIndex] as BattleParty).IsKnockedOut)
                TurnIndex++;

            characterList[TurnIndex].isGaurding = false;
            characterList[TurnIndex].GetComponent<Animator>().SetBool("IsGuarding", false);
            BattleHUD.Instance.UpdateCharacterStats(characterList[TurnIndex], CharacterCard.CharacterStats.None);
            BattleHUD.Instance.UpdateMenu(BattleHUD.SelectionMenu.Main);
            BattleHUD.Instance.UpdateMainMenuName(characterList[TurnIndex]);
        }
        else
        {
            characterList[TurnIndex].isGaurding = false;
            characterList[TurnIndex].GetComponent<Animator>().SetBool("IsGuarding", false);
            BattleHUD.Instance.UpdateMenu(BattleHUD.SelectionMenu.None);
            StartCoroutine(EnemyTurn());
        }

    }

    public IEnumerator DealDamage(BattleCharacter character, AttackType type)
    {

        BattleHUD.Instance.UpdateMenu(BattleHUD.SelectionMenu.None);

        if (type == AttackType.Magical && characterList[TurnIndex] is BattleParty)
        {
            (characterList[TurnIndex] as BattleParty).Mana -= 5;
            BattleHUD.Instance.UpdateCard(characterList[TurnIndex]);
        }

        //Animation
        string animName = null;
        animName = (type == AttackType.Physical) ? "Attack" : "MagicAttack";
        characterList[TurnIndex].GetComponent<Animator>().SetTrigger(animName);
        AnimatorClipInfo[] currentClipInfo = characterList[TurnIndex].GetComponent<Animator>().GetCurrentAnimatorClipInfo(0);
        yield return new WaitForSeconds(currentClipInfo[0].clip.length);


        int damage = 0;

        if (characterList[TurnIndex].isGaurding == false)
        {
            if (characterList[TurnIndex] is BattleParty)
            {
                damage = CalculatePlayerDamage(type);
            }
            else
                damage = CalculateEnemyDamage(type, character);
        }
        else
        {
            character.GetComponent<Animator>().SetBool("IsGuarding", false);
            BattleHUD.Instance.UpdateCharacterStats(character, CharacterCard.CharacterStats.None);
            if (characterList[TurnIndex] is BattleParty)
            {
                damage = CalculatePlayerDamage(type) / 2;
            }
            else
                damage = characterList[TurnIndex].charInfo.baseStrength / 2;
            characterList[TurnIndex].isGaurding = false;
        }

        character.SendMessage("TakeDamage", damage);

        TurnIndex++;

    }

    public IEnumerator RestoreCharacter(BattleCharacter character, Restorables item)
    {

        BattleHUD.Instance.UpdateMenu(BattleHUD.SelectionMenu.None);

        characterList[TurnIndex].GetComponent<Animator>().SetTrigger("UseItem");
        AnimatorClipInfo[] currentClipInfo = characterList[TurnIndex].GetComponent<Animator>().GetCurrentAnimatorClipInfo(0);
        yield return new WaitForSeconds(currentClipInfo[0].clip.length);

        if (item.restoreType == Restorables.RestoreType.Health)
            character.AddHealth(item.restoreValue);
        else if (item.restoreType == Restorables.RestoreType.Mana)
            (character as BattleParty).AddMana(item.restoreValue);
        else if (item.restoreType == Restorables.RestoreType.Revive)
            (character as BattleParty).Revive(item.restoreValue);

        Inventory.Instance.RemoveItem(item as Item);

        TurnIndex++;

    }

    public void EndBattle()
    {
        InBattle = false;
    }

    public void RemoveEnemy(BattleCharacter character)
    {
        enemyList.Remove(character);
        characterList.Remove(character);
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

        if (!InBattle || PauseMenu.Instance.IsPaused)
            return;

        if (Input.GetKeyDown(KeyCode.Tab) && BattleHUD.Instance.selectionMenu == BattleHUD.SelectionMenu.Main)
        {
            BattleHUD.Instance.UpdateCharacterStance(characterList[TurnIndex]);

            string messageTxt = "Stance Changed\n";

            switch ((characterList[TurnIndex] as BattleParty).currentStance)
            {
                case BattleParty.Stance.Balanced:
                    messageTxt += "Balanced Stance";
                    break;
                case BattleParty.Stance.Agressive:
                    messageTxt += "Aggresive Stance";
                    break;
                case BattleParty.Stance.Defensive:
                    messageTxt += "Defensive Stance";
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
