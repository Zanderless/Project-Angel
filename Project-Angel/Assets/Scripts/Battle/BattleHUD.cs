using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class BattleHUD : MonoBehaviour
{

    public static BattleHUD Instance;

    [Header("Character Cards")]
    public GameObject characterCardPrefab;
    public Transform playerCardPanel;
    private List<CharacterCard> characterCards;

    [Header("Selection Menus")]
    public GameObject buttonPrefab;
    public GameObject mainSelectionMenu;
    public GameObject characterSelectionMenu;
    public GameObject inventorySelectionMenu;
    private List<Wreckless.UI.Button> buttons;
    public enum SelectionMenu { None, Main, Character, Inventory };
    public SelectionMenu selectionMenu;
    private bool canSelect;
    private int selectionIndex;
    private Transform currentSelectionParent;

    [Header("Battle HUD")]
    public GameObject battleHUD;

    public void InitHUD(List<BattleCharacter> characters)
    {
        //Character Cards
        if (characterCards != null)
            DestroyExistingCards();

        characterCards = new List<CharacterCard>();

        SpawnNewCards(characters);

    }

    #region Character Cards

    private void DestroyExistingCards()
    {

        if (characterCards.Count > 0)
            foreach (CharacterCard card in characterCards)
            {
                card.Destroy();
            }

    }

    public void SpawnNewCards(List<BattleCharacter> characters)
    {

        foreach (BattleCharacter c in characters)
        {
            CharacterCard card = Instantiate(characterCardPrefab, playerCardPanel).GetComponent<CharacterCard>();
            card.InitCard(c);
            characterCards.Add(card);

        }

    }

    public void UpdateCard(BattleCharacter character)
    {

        foreach(CharacterCard card in characterCards)
        {
            if(card.character == character)
            {
                card.UpdateCard();
                return;
            }
        }

    }

    public void UpdateCharacterStats(BattleCharacter character, CharacterCard.CharacterStats stats)
    {
        foreach (CharacterCard card in characterCards)
        {
            if (card.character == character)
            {
                card.SetGuardOutTxt(stats);
                return;
            }
        }
    }

    #endregion

    #region Selection Buttons

    public void InitButtons(List<BattleCharacter> characters, BattleManager.AttackType type)
    {

        if (selectionMenu == SelectionMenu.Main)
            return;

        DestroyExistingButtons();

        buttons = new List<Wreckless.UI.Button>();

        SpawnNewButtons(characters, type);

        InitSelectionMenuSelect(currentSelectionParent);

    }
    public void InitButtons(ItemCount item)
    {

        if (selectionMenu == SelectionMenu.Main)
            return;

        DestroyExistingButtons();

        buttons = new List<Wreckless.UI.Button>();

        SpawnNewButtons(item);

        InitSelectionMenuSelect(currentSelectionParent);

    }

    public void InitButtons()
    {

        if (selectionMenu == SelectionMenu.Main)
            return;

        DestroyExistingButtons();

        buttons = new List<Wreckless.UI.Button>();

        SpawnNewButtons();

        InitSelectionMenuSelect(currentSelectionParent);

    }

    private void DestroyExistingButtons()
    {
        if (currentSelectionParent.Find("Selections").childCount > 0)
        {
            for (int i = currentSelectionParent.Find("Selections").childCount - 1; i >= 0; i--)
            {
                Destroy(currentSelectionParent.Find("Selections").GetChild(i).gameObject);
            }
        }
    }

    private void SpawnNewButtons(List<BattleCharacter> characters, BattleManager.AttackType type)
    {

        if (selectionMenu == SelectionMenu.Character)
        {
            foreach (BattleCharacter c in characters)
            {
                Transform obj = Instantiate(buttonPrefab, currentSelectionParent.Find("Selections")).transform;
                obj.GetComponent<Wreckless.UI.Button>().ButtonText = c.charInfo.characterNickName;
                obj.GetComponent<Wreckless.UI.Button>().onSelect.AddListener(delegate { StartCoroutine(BattleManager.Instance.DealDamage(c, type)); });
                buttons.Add(obj.GetComponent<Wreckless.UI.Button>());
            }
        }
    }

    private void SpawnNewButtons(ItemCount item)
    {

        if (selectionMenu == SelectionMenu.Character)
        {
            foreach (BattleCharacter c in BattleManager.Instance.GetPartyList())
            {
                Transform obj = Instantiate(buttonPrefab, currentSelectionParent.Find("Selections")).transform;
                obj.GetComponent<Wreckless.UI.Button>().ButtonText = c.charInfo.characterNickName;
                obj.GetComponent<Wreckless.UI.Button>().onSelect.AddListener(delegate { StartCoroutine(BattleManager.Instance.RestoreCharacter(c, (item.item as Restorables))); });
                buttons.Add(obj.GetComponent<Wreckless.UI.Button>());
            }
        }
    }

    private void SpawnNewButtons()
    {

        if (selectionMenu == SelectionMenu.Inventory)
        {
            List<ItemCount> items = Inventory.Instance.GetRestorableItems();

            foreach (ItemCount count in items)
            {
                Transform obj = Instantiate(buttonPrefab, currentSelectionParent.Find("Selections")).transform;
                obj.GetComponent<Wreckless.UI.Button>().ButtonText = count.item.itemName + " " + count.count;
                obj.GetComponent<Wreckless.UI.Button>().onSelect.AddListener(delegate { UpdateMenu(SelectionMenu.Character); });
                obj.GetComponent<Wreckless.UI.Button>().onSelect.AddListener(delegate { InitButtons(count); });
                buttons.Add(obj.GetComponent<Wreckless.UI.Button>());
            }
        }
    }

    #endregion

    #region Selection Menus

    public void UpdateMenu(SelectionMenu menu)
    {

        GameObject activeSelector = null;

        switch (menu)
        {
            case SelectionMenu.None:
                mainSelectionMenu.SetActive(false);
                characterSelectionMenu.SetActive(false);
                inventorySelectionMenu.SetActive(false);
                activeSelector = characterSelectionMenu;
                break;
            case SelectionMenu.Main:
                mainSelectionMenu.SetActive(true);
                characterSelectionMenu.SetActive(false);
                inventorySelectionMenu.SetActive(false);
                activeSelector = mainSelectionMenu;
                break;
            case SelectionMenu.Character:
                mainSelectionMenu.SetActive(false);
                characterSelectionMenu.SetActive(true);
                inventorySelectionMenu.SetActive(false);
                activeSelector = characterSelectionMenu;
                break;
            case SelectionMenu.Inventory:
                mainSelectionMenu.SetActive(false);
                characterSelectionMenu.SetActive(false);
                inventorySelectionMenu.SetActive(true);
                activeSelector = inventorySelectionMenu;
                break;
            default:
                print("Uh Oh!");
                break;
        }

        selectionMenu = menu;
        if (menu == SelectionMenu.Main)
            InitSelectionMenuSelect(activeSelector.transform.Find("Selections"));
        else
            currentSelectionParent = activeSelector.transform;

    }

    private void InitSelectionMenuSelect(Transform parent)
    {

        if (parent == null)
            return;

        if(selectionMenu == SelectionMenu.Main)
            buttons = parent.GetComponentsInChildren<Wreckless.UI.Button>().ToList();

        selectionIndex = 0;
        canSelect = true;

    }

    private void SelectButton()
    {

        if (!canSelect)
            return;

        if (Input.GetKeyDown(KeyCode.W))
            selectionIndex--;
        else if (Input.GetKeyDown(KeyCode.S))
            selectionIndex++;

        if (selectionIndex < 0)
            selectionIndex = buttons.Count - 1;
        else if (selectionIndex == buttons.Count)
            selectionIndex = 0;

        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].isSelected = false;
            if (i == selectionIndex)
                buttons[i].isSelected = true;
        }

        if (Input.GetKeyDown(KeyCode.Return))
            buttons[selectionIndex].Select();

    }

    public void UpdateMainMenuName(BattleCharacter character)
    {
        mainSelectionMenu.transform.Find("MenuTitle").GetComponent<TextMeshProUGUI>().text = character.charInfo.characterNickName;
    }

    #endregion

    private void Update()
    {
        if (battleHUD.activeSelf != BattleManager.Instance.InBattle)
            battleHUD.SetActive(BattleManager.Instance.InBattle);

        if (!BattleManager.Instance.InBattle)
            return;

        SelectButton();

    }

    private void Awake()
    {
        Instance = this;
    }
}
