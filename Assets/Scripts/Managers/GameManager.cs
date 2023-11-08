using System;
using System.Collections;
using System.Collections.Generic;
using thrcnkndmr;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    
    public const string RemainingMoveCountKey = "RemainingMoveCount";
    public const string MatchCountKey = "MatchCount";
    public const string CurrentLevelKey = "CurrentLevel";
    
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
        EventManager.OnLevelFail += OnLevelFail;
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
        throw new NotImplementedException();
    }
    
    private void OnFindMatch()
    {
        matchCount--;
        CheckWinOrLose();

    }

    private void OnLevelFail()
    {
        Debug.Log("sun");
    }
    
    public void DecreaseMoveCount(int amount)
    {
        remainingMoveCount -= amount;
    }
    private void LoadGame()
    {
        
        matchCount = PlayerPrefs.GetInt(MatchCountKey, defaultMatchCount);
        remainingMoveCount = PlayerPrefs.GetInt(RemainingMoveCountKey, defaultRemainingMoveCount);
        currentLevel = PlayerPrefs.GetInt(CurrentLevelKey, 1);
    }

    private void LevelStatsSetter()
    {
        if (matchCount > 20)
        {
            matchCount -= 5;
        }
        remainingMoveCount += 50;
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
        EventManager.OnLevelStartInvoker();
    }
    
    
    private void CheckWinOrLose()
    {
        if (remainingMoveCount <= 0)
        {
            if (matchCount > 0)
            {
               
EventManager.OnLevelFailInvoker();            }
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
        EventManager.OnLevelFail -= OnLevelFail;
        EventManager.OnFindMatch -= OnFindMatch;
        EventManager.OnNextLevel -= OnNextLevel;
        EventManager.OnMovedItem -= OnMovedItem;

    }
}
