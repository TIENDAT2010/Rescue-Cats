using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    GameStart = 1,
    GamePause = 2,
    LevelFailed = 3,
    LevelCompleted = 4,
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