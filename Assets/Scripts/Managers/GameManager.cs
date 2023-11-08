using System;
using thrcnkndmr;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public enum GameState
    {
        MainMenu,
        InPlay,
        SuccessFail
    }

    public GameState gameState;

    private const string RemainingMoveCountKey = "RemainingMoveCount";
    private const string MatchCountKey = "MatchCount";
    private const string CurrentLevelKey = "CurrentLevel";

    public int remainingMoveCount;
    public int matchCount;


    [SerializeField] private int defaultMatchCount = 200;
    [SerializeField] private int defaultRemainingMoveCount = 50;
    [SerializeField] private int currentLevel = 1;

    public int DefaultMatchCount => defaultMatchCount;
    public int DefaultRemainingMove => defaultRemainingMoveCount;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void OnEnable()
    {
        EventManager.OnLevelStart += OnLevelStart;
        EventManager.OnStartGameEvent += OnStartGameEvent;
        EventManager.OnLevelSuccess += OnLevelSuccess;
        EventManager.OnFindMatch += OnFindMatch;
        EventManager.OnNextLevel += OnNextLevel;
        EventManager.OnMovedItem += OnMovedItem;
        EventManager.OnLevelFail += OnLevelFail;
    }

    private void OnLevelFail()
    {
        gameState = GameState.SuccessFail;
    }

    private void OnStartGameEvent()
    {
        gameState = GameState.MainMenu;
    }

    private void OnMovedItem()
    {
        DecreaseMoveCount(1);
        CheckWinOrLose();
    }

    private void OnNextLevel()
    {
        currentLevel++;
        LevelStatsSetter();
        SaveGame();
    }

    private void OnLevelStart()
    {
        LoadGame();
        gameState = GameState.InPlay;
    }

    private void OnLevelSuccess()
    {
        gameState = GameState.SuccessFail;

        if (remainingMoveCount > 20)
        {
            remainingMoveCount -= 5;
        }

        matchCount += 50;
        SaveGame();
    }

    private void OnFindMatch()
    {
        matchCount--;
        CheckWinOrLose();
    }


    private void DecreaseMoveCount(int amount)
    {
        remainingMoveCount -= amount;
    }

    private void LoadGame()
    {
        currentLevel = PlayerPrefs.GetInt(CurrentLevelKey, 1);
        LevelStatsSetter();
    }

    private void LevelStatsSetter()
    {
        remainingMoveCount = defaultRemainingMoveCount;
        matchCount = defaultMatchCount + (currentLevel - 1) * 50;
    }

    private void SaveGame()
    {
        PlayerPrefs.SetInt(RemainingMoveCountKey, remainingMoveCount);
        PlayerPrefs.SetInt(MatchCountKey, matchCount);
        PlayerPrefs.SetInt(CurrentLevelKey, currentLevel);
        PlayerPrefs.Save();
    }

    public void StartGameButton()
    {
        EventManager.OnLevelStartInvoker();
    }

    public void NextLevelButton()
    {
        EventManager.OnNextLevelInvoker();
    }

    public void RestartGameButton()
    {
        EventManager.OnRestartLevelInvoker();
    }


    private void CheckWinOrLose()
    {
        if (remainingMoveCount <= 0)
        {
            if (matchCount > 0)
            {
                EventManager.OnLevelFailInvoker();
            }
            else
            {
                EventManager.OnLevelSuccessInvoker();
            }
        }
        else if (matchCount <= 0)
        {
            EventManager.OnLevelSuccessInvoker();
        }
    }

    private void OnDisable()
    {
        EventManager.OnLevelStart -= OnLevelStart;
        EventManager.OnLevelSuccess -= OnLevelSuccess;
        EventManager.OnFindMatch -= OnFindMatch;
        EventManager.OnNextLevel -= OnNextLevel;
        EventManager.OnMovedItem -= OnMovedItem;
        EventManager.OnStartGameEvent -= OnStartGameEvent;
        EventManager.OnLevelFail -= OnLevelFail;
    }
}