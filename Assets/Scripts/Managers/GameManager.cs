using thrcnkndmr;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
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


    private void OnEnable()
    {
        EventManager.OnLevelStart += OnLevelStart;
        EventManager.OnLevelSuccess += OnLevelSuccess;
        EventManager.OnFindMatch += OnFindMatch;
        EventManager.OnNextLevel += OnNextLevel;
        EventManager.OnMovedItem += OnMovedItem;
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
    }

    private void OnLevelSuccess()
    {
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


    public void DecreaseMoveCount(int amount)
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
    }
}