using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI remainingMoveText;
    [SerializeField] private TextMeshProUGUI matchCountText;

    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject levelFailUI;
    [SerializeField] private GameObject levelSuccessUI;

    [SerializeField] private List<GameObject> uiList = new List<GameObject>();

    private GameManager _gameManager;

    private void Awake()
    {
        _gameManager = GameManager.Instance;
        uiList.Add(mainMenuUI);
        uiList.Add(inGameUI);
        uiList.Add(levelFailUI);
        uiList.Add(levelSuccessUI);
    }


    private void OnEnable()
    {
        EventManager.OnLevelStart += OnLevelStart;
        EventManager.OnLevelSuccess += OnLevelSuccess;
        EventManager.OnLevelFail += OnLevelFail;
        EventManager.OnFindMatch += OnFindMatch;
        EventManager.OnNextLevel += OnNextLevel;
        EventManager.OnStartGameEvent += OnStartGame;
        EventManager.OnMovedItem += OnMovedItem;
    }

    private void OnMovedItem()
    {
        remainingMoveText.text = _gameManager.remainingMoveCount.ToString();
    }

    private void StartUIGetter()
    {
        matchCountText.text = _gameManager.matchCount.ToString();
        remainingMoveText.text = _gameManager.remainingMoveCount.ToString();
    }

    private void OnStartGame()
    {
        ActivateUIElement(mainMenuUI);
        StartUIGetter();
    }

    private void OnLevelStart()
    {
        ActivateUIElement(inGameUI);
        StartUIGetter();
        levelText.text = "Level" + " " + PlayerPrefs.GetInt("CurrentLevel", 1);
    }

    private void OnNextLevel()
    {
        ActivateUIElement(inGameUI);
        StartUIGetter();
    }

    private void OnFindMatch()
    {
        matchCountText.text = _gameManager.matchCount.ToString();
    }

    private void OnLevelFail()
    {
        ActivateUIElement(levelFailUI);
    }

    private void OnLevelSuccess()
    {
        ActivateUIElement(levelSuccessUI);
    }

    private void ActivateUIElement(Object elementName)
    {
        foreach (var element in uiList)
        {
            if (element == elementName)
            {
                element.SetActive(true);
            }
            else
            {
                element.SetActive(false);
            }
        }
    }


    private void OnDisable()
    {
        EventManager.OnLevelStart -= OnLevelStart;
        EventManager.OnLevelSuccess -= OnLevelSuccess;
        EventManager.OnLevelFail -= OnLevelFail;
        EventManager.OnFindMatch -= OnFindMatch;
        EventManager.OnNextLevel -= OnNextLevel;
        EventManager.OnStartGameEvent -= OnStartGame;
    }
}