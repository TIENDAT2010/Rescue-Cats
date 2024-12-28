using System.Collections;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    public void MoveBoat()
    {
        transform.SetParent(null);
        StartCoroutine(CRRotate());
        StartCoroutine(CRMove());
    }

    private IEnumerator CRRotate()
    {
        float t = 0;
        float moveTime = 1.5f;
        Vector3 starVector = transform.forward;
        Vector3 endVector = Vector3.forward;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            float factor = t / moveTime;
            transform.forward = Vector3.Lerp(starVector, endVector, factor);
            yield return null;
        }
    }


    private IEnumerator CRMove()
    {
        float t = 0;
        float moveTime = 5f;
        while (t < moveTime)
        {
            t -= Time.deltaTime;
            transform.position = transform.position + transform.forward * 5 * Time.deltaTime;
            yield return null;
        }
    }
}
