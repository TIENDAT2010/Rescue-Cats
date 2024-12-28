using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RoadController : MonoBehaviour
{
    [SerializeField] private List<Transform> listTranformObstacle = new List<Transform>();

    public List<Transform> GetListTranform()
    {
        return listTranformObstacle;
    }    
}
