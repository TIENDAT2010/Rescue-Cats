using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TsunamiController : MonoBehaviour
{
    [SerializeField] private Transform finishTrans = null;
    [SerializeField] private Transform oceneTrans = null;
    private float moveSpeed = 0f;
    public void SetSpeed(float speed)
    {
        moveSpeed = speed;
    }    
    public void StartMove()
    {
        StartCoroutine(CRMoveForward());
    }


    private IEnumerator CRMoveForward()
    {
        float t = 0;   
        Vector3 startPos = transform.position;
        Vector3 endPos = finishTrans.position;
        float moveTime = Vector3.Distance(endPos, startPos) / moveSpeed;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            float factor = t / moveTime;
            transform.position = Vector3.Lerp(startPos, endPos, factor);
            yield return null;
            ViewManager.Instance.IngameView.SetTsunamiPos(transform.position.z);
        }

        t = 0;
        startPos = transform.position;
        endPos = oceneTrans.position;
        moveTime = Vector3.Distance(endPos, startPos) / moveSpeed;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            float factor = t / moveTime;
            transform.position = Vector3.Lerp(startPos, endPos, factor);
            yield return null;
            ViewManager.Instance.IngameView.SetTsunamiPos(transform.position.z);
        }
    }   
}
