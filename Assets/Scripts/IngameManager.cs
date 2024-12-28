using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class IngameManager : MonoBehaviour
{
    public static IngameManager Instance { get; private set; }

    [SerializeField] private Transform playerPos = null;
    [SerializeField] private Transform oceanObject = null;
    [SerializeField] private Transform boatTranform = null;
    [SerializeField] private Transform finishObject = null;
    [SerializeField] private TsunamiController tsunami = null;
    [SerializeField] private List<Transform> listWhiteCatPos = new List<Transform>();
    [SerializeField] private List<Transform> listBlackCatPos = new List<Transform>();
    [SerializeField] private Transform docJumpPos = null;


    private List<CatController> listWhiteCat = new List<CatController>();
    private List<CatController> listBlackCat = new List<CatController>();
    private GameState gameState = GameState.GameStart;
    public Transform DockJumpPos => docJumpPos;
    public float FinishzPos => finishObject.position.z;
    public GameState GameState => gameState;
    public Transform BoatTranform => boatTranform;
    public int TotalCats { get; private set; }

    private int indexWhiteCat = -1;
    private int indexBlackCat = -1;
    private LevelGameConfigSO levelGameConfig = null;
    private static int mapID = 1;

    private void Awake()
    {
        if (Instance != null)
        {
            Instance = null;
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        ViewManager.Instance.SetActiveView(ViewType.IngameView);
        if (mapID == -1)
        {
            tsunami.SetSpeed(7);
            GenerateMapDefault();
        }
        else
        {
            levelGameConfig = Resources.Load<LevelGameConfigSO>(mapID.ToString());
            tsunami.SetSpeed(levelGameConfig.tsunamiSpeed);
            GenerateMap();
        }     
    }

    private void GenerateMapDefault()
    {
        List<Vector3> listCatPos = new List<Vector3>();
        listCatPos.Add(new Vector3(8, 0, 10));
        listCatPos.Add(new Vector3(2, 0, 18));
        listCatPos.Add(new Vector3(7, 0, 32));
        listCatPos.Add(new Vector3(3, 0, 48));
        listCatPos.Add(new Vector3(8, 0, 58));
        listCatPos.Add(new Vector3(6, 0, 73));

        List<Vector3> listObstaclePos = new List<Vector3>();
        listObstaclePos.Add(new Vector3(8, 0, 18));
        listObstaclePos.Add(new Vector3(2, 0, 36));
        listObstaclePos.Add(new Vector3(9, 0, 52));
        listObstaclePos.Add(new Vector3(1, 0, 70));
        Vector3 obstacle02Pos = new Vector3(2, 0, 30);
        Vector3 obstacle03Pos = new Vector3(9, 0, 36);
        Vector3 obstacle04Pos = new Vector3(1, 0, 52);
        Vector3 obstacle05Pos = new Vector3(6, 0, 70);

        for (int i = 0; i < 7; i++)
        {
            RoadController road = PoolManager.Instance.GetRandomRoad();
            Vector3 pos = playerPos.position;
            pos.z += 10 + (i * 20);
            road.transform.position = pos;

            if (i == 3)
            {
                finishObject.transform.position = road.transform.position + road.transform.forward * 10;
            }
            if (i == 6)
            {
                oceanObject.transform.position = road.transform.position + road.transform.forward * 10;
            }
        }


        for (int i = 0; i < listCatPos.Count; i++)
        {
            Vector2 random = Random.insideUnitCircle;
            Vector3 randomPosition = new Vector3(random.x, 0f, random.y);
            randomPosition = listCatPos[i] + randomPosition * Random.Range(0f, 2f);
            CatController cat = PoolManager.Instance.GetRandomCat();
            cat.transform.position = randomPosition;
            if (cat.CatType == CatType.BlackCat) { listBlackCat.Add(cat); }
            else if (cat.CatType == CatType.WhiteCat) { listWhiteCat.Add(cat); }
        }


        for (int i = 0; i < listObstaclePos.Count; i++)
        {
            GameObject obstacle = PoolManager.Instance.GetRandomObstacle();
            obstacle.transform.position = new Vector3(Random.Range(1f, 9f), 0, listObstaclePos[i].z);
        }

        TotalCats = listBlackCat.Count + listWhiteCat.Count;
        ViewManager.Instance.IngameView.SetPlayerPos(playerPos.position.z);
    }



    private void GenerateMap()
    {
        List<Transform> listObstaclePos = new List<Transform>();
        Vector3 currentPos = playerPos.position + Vector3.forward * 10;

        for (int i = 0; i < 2; i++)
        {
            RoadController road = PoolManager.Instance.GetRandomRoad();   
            road.transform.position = currentPos;
            currentPos += Vector3.forward * 20;
        }

        for(int i = 0; i < levelGameConfig.RoadsNumber; i++)
        {
            RoadController road = PoolManager.Instance.GetRandomRoad();
            road.transform.position = currentPos;
            currentPos += Vector3.forward * 20;
            listObstaclePos.AddRange(road.GetListTranform());
        }  
        
        finishObject.transform.position = currentPos;

        for(int i = 0; i < 3; i ++)
        {
            RoadController road = PoolManager.Instance.GetRandomRoad();
            road.transform.position = currentPos;
            currentPos += Vector3.forward * 20;
        }    

        oceanObject.transform.position = currentPos + Vector3.back * 10;

        List<Vector3> listCatPos = new List<Vector3>();
        int totalCats = 0;
        if(levelGameConfig.RoadsNumber < 5) { totalCats = 5; }
        else { totalCats = levelGameConfig.RoadsNumber ; }
        totalCats = Mathf.Clamp(totalCats, 0, 12);
        for(int i = 0; i < totalCats; i++)
        {
            Vector3 pos = Vector3.zero;
            pos.x = Random.Range(1, 10);
            pos.y = 0;
            float minz = (levelGameConfig.RoadsNumber * 20 / totalCats * (i + 1)) - 4;
            float maxz = (levelGameConfig.RoadsNumber * 20 / totalCats * (i + 1));
            pos.z = Random.Range(minz,maxz);
            listCatPos.Add(pos);
        }    
      
        for (int i = 0; i < listCatPos.Count; i++)
        {
            Vector2 random = Random.insideUnitCircle;
            Vector3 randomPosition = new Vector3(random.x, 0f, random.y);
            randomPosition = listCatPos[i] + randomPosition * Random.Range(0f, 2f);

            CatController cat = new CatController();
            if(listBlackCat.Count == 6)
            {
                cat = PoolManager.Instance.GetRandomCat(CatType.WhiteCat);
            }    
            else if(listWhiteCat.Count == 6)
            {
                cat = PoolManager.Instance.GetRandomCat(CatType.BlackCat);
            }    
            else
            {
                cat = PoolManager.Instance.GetRandomCat();
            }    
            cat.transform.position = randomPosition;
            if (cat.CatType == CatType.BlackCat) { listBlackCat.Add(cat); }
            else if (cat.CatType == CatType.WhiteCat) { listWhiteCat.Add(cat); }
        }

        int minObstacle = 0; int maxObstacle = 0;
        if(levelGameConfig.mapID < 4)
        {
            minObstacle = 1; maxObstacle = 2;
        }
        else if(levelGameConfig.mapID < 7)
        {
            minObstacle = 2; maxObstacle = 3;
        }    
        else
        {
            minObstacle = 2; maxObstacle = 4;
        }    
        int totalObstacle = Random.Range(levelGameConfig.RoadsNumber * minObstacle, levelGameConfig.RoadsNumber * maxObstacle);


        List<Transform> result = new List<Transform>();
        while (result.Count < totalObstacle)
        {
            int randomIndex = Random.Range(0, listObstaclePos.Count);
            Transform randomPoint = listObstaclePos[randomIndex];
            float randomZ = randomPoint.position.z;

            if(result.Contains(randomPoint))
            {
                continue;
            }

            int zCount = 0;
            foreach (Transform p in result)
            {
                if(Mathf.Abs(p.position.z - randomZ) < 0.1)
                {
                    zCount++;
                }
            }

            if (zCount < 2)
            {
                result.Add(randomPoint);
                Transform spawnPoint = listObstaclePos[randomIndex];
                GameObject obstacle = PoolManager.Instance.GetRandomObstacle();
                obstacle.transform.position = spawnPoint.position;
            }
        }
      


        TotalCats = listBlackCat.Count + listWhiteCat.Count;
        ViewManager.Instance.IngameView.SetPlayerPos(playerPos.position.z);
    }   

    public Transform GetPosForWhiteCat()
    {
        indexWhiteCat++;
        return listWhiteCatPos[indexWhiteCat];
    }

    public Transform GetPosForBlackCat()
    {
        indexBlackCat++;
        return listBlackCatPos[indexBlackCat];
    }

    public void GameFailed()
    {
        gameState = GameState.LevelFailed;
        ViewManager.Instance.SetActiveView(ViewType.EndgameView);
    }    

    public void GameCompleted()
    {
        gameState = GameState.LevelCompleted;
        ViewManager.Instance.SetActiveView(ViewType.EndgameView);
    }        

    public int RescuedCats()
    {
        int count = 0;
        for (int i = 0; i < listBlackCat.Count; i++)
        {
            if (listBlackCat[i].gameObject.activeSelf) { count++; }
        }
        for (int i = 0; i < listWhiteCat.Count; i++)
        {
            if (listWhiteCat[i].gameObject.activeSelf) { count++; }
        }
        return count;
    }


    public static void SetTargetMapID(int id)
    {
        mapID = id;
    }
}
