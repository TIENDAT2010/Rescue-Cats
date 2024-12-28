using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator animator = null;
    [SerializeField] private Transform endPos = null;
    [SerializeField] private Transform fOVObject = null;
    [SerializeField] private Transform arrowObject = null;
    [SerializeField] private Transform playerOnBoatPos = null;
    [SerializeField] private Transform finishObject = null;
    [SerializeField] private CharacterController characterController = null;
    [SerializeField] private CameraParentController cameraParentController = null;
    [SerializeField] private TsunamiController tsunamiController = null;
    [SerializeField] private LayerMask catLayer = new LayerMask();
    [SerializeField] private LayerMask tsunamiLayer = new LayerMask();
    [SerializeField] private List<Transform> listBlackCatPos = new List<Transform>();
    [SerializeField] private List<Transform> listWhiteCatPos = new List<Transform>();



    private JoystickController joystickController = null;
    private List<CatController> listWhiteCat = new List<CatController>();
    private List<CatController> listBlackCat = new List<CatController>();
    private float movementY = 0;
    private float blendSpeed = 0;
    private int blackCatIndex = -1;
    private int whiteCatIndex = -1;
    private bool startRun = false;
    private bool finishRun = false;
    private bool defeated = false;

    private void Start()
    {
        defeated = false;
        finishRun = false;
        fOVObject.gameObject.SetActive(false);
        arrowObject.gameObject.SetActive(false);
        joystickController = ViewManager.Instance.IngameView.JoystickController;
        cameraParentController.SetFollowTarget(transform);
    }

    private void Update()
    {
        if(startRun && finishRun == false && defeated == false)
        {
            if(characterController.enabled)
            {
                if (joystickController.JoystickDirection != Vector2.zero)
                {
                    //Rotate the player
                    Vector3 joystickDir = joystickController.JoystickDirection;
                    Vector3 playerDir = Vector3.zero;
                    playerDir.x = (joystickDir.normalized.x + transform.position.x);
                    playerDir.z = (joystickDir.normalized.y + transform.position.z);
                    playerDir.y = transform.position.y;
                    Vector3 targetDir = (playerDir - transform.position).normalized;
                    if (!targetDir.Equals(Vector3.zero))
                    {
                        Quaternion quaternion = Quaternion.LookRotation(targetDir, Vector3.up);
                        transform.rotation = Quaternion.Slerp(transform.rotation, quaternion, 3 * Time.deltaTime);
                    }

                    //Move the player
                    Vector3 movementDir = transform.forward;
                    if (!characterController.isGrounded) { movementY -= Time.deltaTime; }
                    else { movementY = 0f; }
                    movementDir.y = movementY;
                    characterController.Move(movementDir * PlayerDataController.CurrentSpeed * Time.deltaTime);

                    blendSpeed = Mathf.Clamp(blendSpeed + 5f * Time.deltaTime, 0, 1);
                    animator.SetFloat("Speed", blendSpeed);
                }
                else
                {
                    blendSpeed = Mathf.Clamp(blendSpeed - 5f * Time.deltaTime, 0, 1);
                    animator.SetFloat("Speed", blendSpeed);
                }


                CatController closestCat = PoolManager.Instance.FindClosestCat(transform.position);
                if (closestCat != null)
                {
                    float distance = Vector3.Distance(transform.position, closestCat.transform.position);
                    if (distance < 5f)
                    {
                        fOVObject.gameObject.SetActive(true);
                        arrowObject.gameObject.SetActive(false);
                    }
                    else
                    {
                        fOVObject.gameObject.SetActive(false);
                        arrowObject.gameObject.SetActive(true);
                        Vector3 dir = (closestCat.transform.position - transform.position).normalized;
                        arrowObject.forward = dir;
                    }
                }
                else
                {
                    fOVObject.gameObject.SetActive(false);
                    arrowObject.gameObject.SetActive(false);
                }


                //Check collide with the cat
                Collider[] catColider = Physics.OverlapSphere(transform.position, 5f, catLayer);
                if (catColider.Length > 0)
                {
                    foreach (Collider col in catColider)
                    {
                        Vector3 towardCat = (col.transform.position - transform.position).normalized;
                        if (Vector3.Angle(transform.forward, towardCat) <= 30)
                        {
                            CatController cat = PoolManager.Instance.FindCat(col.transform);
                            if (cat != null && cat.IsRescueByPlayer == false)
                            {
                                cat.RescuedByPlayer();
                                if (cat.IsRescueByPlayer)
                                {
                                    if (cat.CatType == CatType.WhiteCat)
                                    {
                                        listWhiteCat.Add(cat);
                                    }
                                    else
                                    {
                                        listBlackCat.Add(cat);
                                    }
                                }
                            }
                        }
                    }
                }


                //Check collide with the tsunami
                Collider[] tsunamiColider = Physics.OverlapSphere(transform.position, 0.5f, tsunamiLayer);
                if (tsunamiColider.Length > 0)
                {
                    PlayerDefeated();
                    IngameManager.Instance.GameFailed();
                }



                ViewManager.Instance.IngameView.SetPlayerPos(transform.position.z);
            }
        }
    }



    public void GameStart()
    {
        startRun = true;
        defeated = false;
        tsunamiController.StartMove();
    }


    public void PlayerDefeated()
    {
        defeated = true;
        blendSpeed = 0f;
        animator.SetFloat("Speed", blendSpeed);
        tsunamiController.StopAllCoroutines();
        joystickController.gameObject.SetActive(false);
        fOVObject.gameObject.SetActive(false);
    }



    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.collider.CompareTag("Finish") && finishRun == false)
        {
            finishRun = true;
            hit.gameObject.SetActive(false);
            characterController.enabled = false;
            joystickController.gameObject.SetActive(false);
            cameraParentController.OnPlayerHitFinish();
            finishObject.gameObject.SetActive(false);
            fOVObject.gameObject.SetActive(false);
            List<GameObject> list = PoolManager.Instance.GetAllObstacles();
            for(int i = 0; i < list.Count; i++)
            {
                list[i].gameObject.SetActive(false);
            }

            animator.SetFloat("Speed", 1f);
            StartCoroutine(CRMovePlayerToCenter());
            StartCoroutine(CRMovePlayerToEnd());
        }    
    }


    private IEnumerator CRMovePlayerToCenter()
    {
        float t = 0;
        float moveTime = 0.5f;
        float startX = transform.position.x;
        Vector3 starFoward = transform.forward;
        Vector3 endFoward = Vector3.forward;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            float factor = t / moveTime;
            transform.forward = Vector3.Lerp(starFoward, endFoward, factor);
            float newX = Mathf.Lerp(startX, 5, factor);
            Vector3 newPos = new Vector3(newX, 0f, transform.position.z);
            transform.position = newPos;
            yield return null;
            animator.SetFloat("Speed", 1f);
            if (arrowObject.gameObject.activeSelf) { arrowObject.gameObject.SetActive(false); }
        }
    }


    public Transform GetCatPos(CatType catType)
    {
        if(catType == CatType.BlackCat)
        {
            blackCatIndex++;
            return listBlackCatPos[blackCatIndex];
        }
        else
        {
            whiteCatIndex++;
            return listWhiteCatPos[whiteCatIndex];
        }    

    }

    private IEnumerator CRMovePlayerToEnd()
    {
        float t = 0;
   
        float startZ = transform.position.z;
        float endZ = endPos.position.z;
        float moveTime = (endZ - startZ) / (PlayerDataController.CurrentSpeed * 2f);
        bool isRotateCamera = false;
        while(t < moveTime)
        {
            t += Time.deltaTime;

            float factor = t / moveTime;
            float newZ = Mathf.Lerp(startZ, endZ, factor);
            Vector3 newPos = new Vector3(transform.position.x, 0f, newZ);
            transform.position = newPos;

            yield return null;

            if(Vector3.Distance(transform.position, endPos.position) < 45f && isRotateCamera == false)
            {
                isRotateCamera = true;
                cameraParentController.RotateToTop();
                ViewManager.Instance.IngameView.DisableTopPanel();
            }    
        }
        animator.SetFloat("Speed", 0);
        cameraParentController.SetFollowTarget(null);


        for(int i = listWhiteCat.Count -1; i >= 0 ; i--)
        {
            if (listWhiteCat[i].gameObject.activeSelf)
            {
                listWhiteCat[i].StopAllCoroutines();
                listWhiteCat[i].MoveToBoat();
                yield return new WaitForSeconds(1f);
            }
        }

        for (int i = 0; i < listBlackCat.Count; i++)
        {
            if (listBlackCat[i].gameObject.activeSelf)
            {
                listBlackCat[i].StopAllCoroutines();
            }
        }
        yield return new WaitForSeconds(0.25f);
        animator.SetBool("Jump", true);
        StartCoroutine(CRRotatePlayer());
        yield return StartCoroutine(CRJumpToBoat(playerOnBoatPos.position));
        animator.SetBool("Jump", false);

        for (int i = 0; i < listBlackCat.Count; i++)
        {
            if (listBlackCat[i].gameObject.activeSelf)
            {
                listBlackCat[i].MoveToBoat();
                yield return new WaitForSeconds(1f);
            }
        }

        yield return new WaitForSeconds(1f);

        BoatController boat = IngameManager.Instance.BoatTranform.GetComponent<BoatController>();
        boat.MoveBoat();
        cameraParentController.FollowTheBoat(boat.transform);
        yield return new WaitForSeconds(5f);
        IngameManager.Instance.GameCompleted();
    }


    private IEnumerator CRRotatePlayer()
    {
        float t = 0;
        float moveTime = 0.5f;
        Vector3 starVector = transform.forward;
        Vector3 endVector = playerOnBoatPos.forward;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            float factor = t / moveTime;
            transform.forward = Vector3.Lerp(starVector, endVector, factor);
            yield return null;
        }
    }

    private IEnumerator CRJumpToBoat(Vector3 targetPos)
    {
        List<Vector3> listPositions = new List<Vector3>();

        //Calculate the list position
        int movePoints = 80;
        Vector3 startPoint = transform.position;
        Vector3 midPoint = Vector3.Lerp(startPoint, targetPos, 0.5f) + Vector3.up * 6;
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

        transform.SetParent(IngameManager.Instance.BoatTranform);
    }

    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 from, Vector3 middle, Vector3 to)
    {
        return Mathf.Pow((1 - t), 2) * from + 2 * (1 - t) * t * middle + Mathf.Pow(t, 2) * to;
    }
}

