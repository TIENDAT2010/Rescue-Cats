using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [SerializeField] private RoadController[] roadPrefabs = null;
    [SerializeField] private CatController[] catPrefabs = null;
    [SerializeField] private GameObject[] obstaclePrefabs = null;

    private List<CatController> listCat = new List<CatController>();
    private List<GameObject> listObstacle = new List<GameObject>();

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



    public CatController FindCat(Transform trans)
    {
        foreach(CatController cat in listCat)
        {
            if(cat.transform.Equals(trans)) return cat; 
        }
        return null;
    }


    public CatController FindClosestCat(Vector3 pos)
    {
        float minDis = 10000;
        CatController result = null;
        foreach(CatController cat in listCat)
        {
            float dis = Vector3.Distance(pos, cat.transform.position);
            if(dis < minDis && cat.IsRescueByPlayer == false)
            {
                minDis = dis;
                result = cat;
            }
        }
        return result;
    }



    public RoadController GetRandomRoad()
    {
        int i = Random.Range(0, roadPrefabs.Length);
        RoadController road = Instantiate(roadPrefabs[i], Vector3.zero, Quaternion.identity);
        road.gameObject.SetActive(true);
        return road;
    }

    public CatController GetRandomCat()
    {
        int i = Random.Range(0, catPrefabs.Length);
        CatController cat = Instantiate(catPrefabs[i], Vector3.zero, Quaternion.identity);
        cat.gameObject.SetActive(true);
        listCat.Add(cat);
        return cat;
    }

    public CatController GetRandomCat(CatType catType)
    {
        CatController cat = new CatController();
        for (int i = 0; i < catPrefabs.Length; i++)
        {
            if (catPrefabs[i].CatType == catType)
            {
                cat = Instantiate(catPrefabs[i], Vector3.zero, Quaternion.identity);
            }    
        }
        cat.gameObject.SetActive(true);
        listCat.Add(cat);
        return cat;
    }

    public GameObject GetRandomObstacle()
    {
        int i = Random.Range(0, obstaclePrefabs.Length);
        GameObject obstacle = Instantiate(obstaclePrefabs[i], Vector3.zero, Quaternion.identity);
        obstacle.gameObject.SetActive(true);
        listObstacle.Add(obstacle);
        return obstacle;
    }

    public List<GameObject> GetAllObstacles()
    {
        return listObstacle;
    }    
}
