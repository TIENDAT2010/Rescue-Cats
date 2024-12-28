using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatController : MonoBehaviour
{
    [SerializeField] private CatType catType = CatType.BlackCat;
    [SerializeField] private Animator animator = null;
    [SerializeField] private LayerMask tsunami = new LayerMask();
    [SerializeField] private SphereCollider sphereCollider = null;
    [SerializeField] private Transform canvasTrans = null;
    [SerializeField] private Image sliderImage = null;
    private PlayerController player;
    private Vector3 velocity = Vector3.zero;
    private float blendSpeed = 0;

    public CatType CatType => catType;
    public bool IsRescueByPlayer { get; private set; }
    private float timeCount = 0;

    private void Start()
    {
        timeCount = 0;
        IsRescueByPlayer = false;
        sphereCollider.enabled = true;
        canvasTrans.gameObject.SetActive(false);
        player = FindFirstObjectByType<PlayerController>();
        animator.SetFloat("Speed", blendSpeed);
        StartCoroutine(CRCheckWithTsunami());
    }


    public void RescuedByPlayer()
    {
        if(IsRescueByPlayer == false)
        {
            timeCount += Time.deltaTime;

            
            if(canvasTrans.gameObject.activeSelf == false)
            {
                canvasTrans.gameObject.SetActive(true);
            }
            sliderImage.fillAmount = timeCount / 0.5f;


            if (timeCount > 0.5f)
            {
                IsRescueByPlayer = true;
                sphereCollider.enabled = false;
                canvasTrans.gameObject.SetActive(false);
                if (catType == CatType.WhiteCat)
                {
                    StartCoroutine(CRMoveToTopPos());
                }
                else
                {
                    StartCoroutine(CRFollowPlayer());
                }
            }
        }           
    }    


    private IEnumerator CRMoveToTopPos()
    {
        Transform followTranform = player.GetCatPos(catType);

        float t = 0;
        float moveTime = 2f;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            float factor = t / moveTime;
            transform.position = Vector3.Lerp(transform.position, followTranform.position, factor);
            transform.forward = Vector3.Lerp(transform.forward, followTranform.forward, factor);
            yield return null;
        }
        gameObject.transform.SetParent(player.transform);
    }    



    private IEnumerator CRFollowPlayer()
    {
        Transform followTranform = player.GetCatPos(catType);
        while(true)
        {
            blendSpeed = Mathf.Clamp(blendSpeed + 5f * Time.deltaTime, 0, 1);
            animator.SetFloat("Speed", blendSpeed);

            transform.position = Vector3.SmoothDamp(transform.position, followTranform.position, ref velocity, 0.15f);
            Vector3 lookDirection = (followTranform.position - transform.position).normalized;
            Quaternion lookQuaternion = Quaternion.LookRotation(lookDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookQuaternion, 15 * Time.deltaTime);

            yield return null;
        }    
    }    
    

    public void MoveToBoat()
    {
        transform.SetParent(null);
        Transform onDuckPos = IngameManager.Instance.DockJumpPos;
        Transform onBoatPos = null;
        if(catType == CatType.WhiteCat)
        {
            onBoatPos = IngameManager.Instance.GetPosForWhiteCat();
        }    
        else
        {
            onBoatPos = IngameManager.Instance.GetPosForBlackCat();
        }    
        StartCoroutine(CRMoveToBoat(onDuckPos, onBoatPos));
    }    

    private IEnumerator CRMoveToBoat(Transform onDuckPos, Transform onBoatPos)
    {
        if(catType == CatType.WhiteCat)
        {
            yield return StartCoroutine(CRJump(onDuckPos.position));
            yield return new WaitForSeconds(0.25f);
            StartCoroutine(CRJump(onBoatPos.position));
            StartCoroutine(CRRotateCatFoward(onBoatPos));
        }  
        else
        {
            yield return StartCoroutine(CRLerpCatToPos(onDuckPos.position));
            yield return new WaitForSeconds(0.25f);
            StartCoroutine(CRJump(onBoatPos.position));
            StartCoroutine(CRRotateCatFoward(onBoatPos));
        }
    }


    private IEnumerator CRLerpCatToPos(Vector3 targetpos)
    {
        float t = 0;
        float moveTime = 1f;
        Vector3 starVector = transform.position;
        Vector3 endVector = targetpos;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            float factor = t / moveTime;
            transform.position = Vector3.Lerp(starVector, endVector, factor);
            yield return null;
        }
    }

    private IEnumerator CRRotateCatFoward(Transform targetpos)
    {
        float t = 0;
        float moveTime = 0.5f;
        Vector3 starVector = transform.forward;
        Vector3 endVector = targetpos.forward;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            float factor = t / moveTime;
            transform.forward = Vector3.Lerp(starVector, endVector, factor);
            yield return null;
        }
        transform.SetParent(IngameManager.Instance.BoatTranform);
    }


    private IEnumerator CRJump(Vector3 targetPos)
    {
        List<Vector3> listPositions = new List<Vector3>();

        //Calculate the list position
        int movePoints = 60;
        Vector3 startPoint = transform.position;
        Vector3 midPoint = Vector3.Lerp(startPoint, targetPos, 0.5f) + Vector3.up * 4;
        listPositions.Add(transform.position);
        for (int i = 1; i <= movePoints; i++)
        {
            float t = i / (float)movePoints;
            listPositions.Add(CalculateQuadraticBezierPoint(t, startPoint, midPoint, targetPos));
        }

        //Moving player to each point
        for (int i = 0; i < listPositions.Count; i++)
        {
            transform.position = listPositions[i];
            yield return null;
        }
    }

    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 from, Vector3 middle, Vector3 to)
    {
        return Mathf.Pow((1 - t), 2) * from + 2 * (1 - t) * t * middle + Mathf.Pow(t, 2) * to;
    }


    private IEnumerator CRCheckWithTsunami()
    {
        while(gameObject.activeSelf)
        {
            //Check collide with the tsunami
            Collider[] catColider = Physics.OverlapSphere(transform.position, 0.2f, tsunami);
            if (catColider.Length > 0)
            {
                gameObject.SetActive(false);
            }
            yield return null;
        }
    }    
}
