using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    GameInit = 0,
    GameStart = 1,
    LevelFailed = 2,
    LevelCompleted = 3,
}


public enum PlayerState
{
    PlayerInit = 0,
    PlayerStart = 1,
    PlayerFinish = 2,
    PlayerFailed = 3,
    PlayerCompleted = 4,
}

public enum ViewType
{
    HomeView = 0,
    IngameView = 1,
    EndgameView = 2,
    LoadingView = 3,
}

public enum CatType
{
    BlackCat = 0,
    WhiteCat = 1,
}