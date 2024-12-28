using System.Collections;
using UnityEngine;

public class CameraParentController : MonoBehaviour
{
    [SerializeField] private Transform cameraTrans = null;

    private Transform followTarget = null;
    private Vector3 velocity = Vector3.zero;
    private float smoothTime = 0.3f;


    private IEnumerator Start()
    {
        yield return new WaitForSeconds(5f);
        float t = 0;
        float movetime = 1f;
        Vector3 parentStartAngle = transform.eulerAngles;
        Vector3 parentEndAngle = Vector3.zero;
        Vector3 camStartAngle = cameraTrans.localEulerAngles;
        Vector3 camEndAngle = new Vector3(25f, 0f, 0f);
        Vector3 camStartPos = cameraTrans.localPosition;
        Vector3 camEndPos = new Vector3(0, 5f, -8f);
        while (t < movetime)
        {
            t += Time.deltaTime;
            float factor = t / movetime;
            transform.eulerAngles = Vector3.Lerp(parentStartAngle, parentEndAngle, factor);
            cameraTrans.localEulerAngles = Vector3.Lerp(camStartAngle, camEndAngle, factor);
            cameraTrans.localPosition = Vector3.Lerp(camStartPos, camEndPos, factor);
            yield return null;
        }


        //cameraTrans.localPosition = new Vector3(0, 5f, -8f);
        //cameraTrans.localEulerAngles = new Vector3(25f, 0f, 0f);
        yield return null;
    }

    private void Update()
    {
        if(followTarget != null)
        {
            Vector3 targetPosition = followTarget.position;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }


    public void SetFollowTarget(Transform target)
    {
        followTarget = target;
    }








    public void OnPlayerHitFinish()
    {
        StartCoroutine(CRRotateCamera());
    }
    private IEnumerator CRRotateCamera()
    {
        float t = 0;
        float moveTime = 2f;
        Vector3 camParentAngle = new Vector3(0, 180, 0);
        Vector3 camLocalAngle = new Vector3(10f, 0, 0);
        Vector3 camLocalPos = new Vector3(0, 2f, -7f);
        Vector3 starAngles = transform.eulerAngles;
        Vector3 starLocalPos = cameraTrans.localPosition;
        Vector3 starLocalAngles = cameraTrans.localEulerAngles;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            float factor = t / moveTime;
            transform.eulerAngles = Vector3.Lerp(starAngles, camParentAngle, factor);
            cameraTrans.localPosition = Vector3.Lerp(starLocalPos, camLocalPos, factor);
            cameraTrans.localEulerAngles = Vector3.Lerp(starLocalAngles, camLocalAngle, factor);
            yield return null;
        }
    }




    public void RotateToTop()
    {
        StartCoroutine(CRRotateToTop());
    }
    private IEnumerator CRRotateToTop()
    {
        float t = 0;
        float moveTime = 2f;
        Vector3 camParentAngle = new Vector3(0, 0, 0);
        Vector3 camLocalAngle = new Vector3(35f, 0, 0);
        Vector3 camLocalPos = new Vector3(0, 8f, -9f);
        Vector3 starAngles = transform.eulerAngles;
        Vector3 starLocalPos = cameraTrans.localPosition;
        Vector3 starLocalAngles = cameraTrans.localEulerAngles;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            float factor = t / moveTime;
            transform.eulerAngles = Vector3.Lerp(starAngles, camParentAngle, factor);
            cameraTrans.localPosition = Vector3.Lerp(starLocalPos, camLocalPos, factor);
            cameraTrans.localEulerAngles = Vector3.Lerp(starLocalAngles, camLocalAngle, factor);
            yield return null;
        }
    }    


    public void FollowTheBoat(Transform boatTrans)
    {
        followTarget = boatTrans;
        transform.position = boatTrans.position;
        transform.eulerAngles = new Vector3(0, 180, 0);
        cameraTrans.localEulerAngles = new Vector3(10f, 0, 0);
        cameraTrans.localPosition = new Vector3(0, 4, -10);
        StartCoroutine(CRWaitAndStopFollowTheBoat());
    }

    private IEnumerator CRWaitAndStopFollowTheBoat()
    {
        yield return new WaitForSeconds(5f);
        followTarget = null;
    }
}
