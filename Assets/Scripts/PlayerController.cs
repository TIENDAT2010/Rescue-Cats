using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [SerializeField] private Animator animator = null;
    [SerializeField] private Transform endPos = null;
    [SerializeField] private Transform fOVObject = null;
    [SerializeField] private Transform arrowObject = null;
    [SerializeField] private Transform playerOnBoatPos = null;
    [SerializeField] private CharacterController characterController = null;
    [SerializeField] private CameraParentController cameraParentController = null;
    [SerializeField] private LayerMask catLayer = new LayerMask();
    [SerializeField] private LayerMask tsunamiLayer = new LayerMask();
    [SerializeField] private List<Transform> listBlackCatPos = new List<Transform>();
    [SerializeField] private List<Transform> listWhiteCatPos = new List<Transform>();


    private PlayerState playerState = PlayerState.PlayerInit;
    private JoystickController joystickController = null;
    private List<CatController> listWhiteCat = new List<CatController>();
    private List<CatController> listBlackCat = new List<CatController>();
    private float movementY = 0;
    private float blendSpeed = 0;
    private int blackCatIndex = -1;
    private int whiteCatIndex = -1;


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
        playerState = PlayerState.PlayerInit;
        fOVObject.gameObject.SetActive(false);
        arrowObject.gameObject.SetActive(false);
        joystickController = ViewManager.Instance.IngameView.JoystickController;
        cameraParentController.SetFollowTarget(transform);
    }

    private void Update()
    {
        if(playerState == PlayerState.PlayerStart)
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
                PlayerFailed();
                IngameManager.Instance.GameFailed();
            }

            ViewManager.Instance.IngameView.SetPlayerPos(transform.position.z);
        }
    }



    public void PlayerStart()
    {
        playerState = PlayerState.PlayerStart;
    }

    public void PlayerCompleted()
    {
        playerState = PlayerState.PlayerCompleted;
        joystickController.gameObject.SetActive(false);
        fOVObject.gameObject.SetActive(false);
        arrowObject.gameObject.SetActive(false);
    }

    public void PlayerFailed()
    {
        playerState = PlayerState.PlayerFailed;
        blendSpeed = 0f;
        animator.SetFloat("Speed", blendSpeed);
        joystickController.gameObject.SetActive(false);
        fOVObject.gameObject.SetActive(false);
        arrowObject.gameObject.SetActive(false);
    }



    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.collider.CompareTag("Finish") && playerState == PlayerState.PlayerStart)
        {
            playerState = PlayerState.PlayerFinish;
            StartCoroutine(CROnPlayerFinish(hit.gameObject));
        }    
    }


    private IEnumerator CROnPlayerFinish(GameObject hit)
    {
        yield return null;

        ///Disable Object
        hit.gameObject.SetActive(false);
        characterController.enabled = false;
        joystickController.gameObject.SetActive(false);
        cameraParentController.OnPlayerHitFinish();
        fOVObject.gameObject.SetActive(false);
        arrowObject.gameObject.SetActive(false);
        List<GameObject> list = PoolManager.Instance.GetAllObstacles();
        for (int i = 0; i < list.Count; i++)
        {
            list[i].gameObject.SetActive(false);
        }



        ///Left Player To End Pos
        float t = 0;
        float moveTime = 10f;

        float startZ = transform.position.z;
        float endZ = endPos.position.z;

        float leftToCenter = 0.5f;
        float startX = transform.position.x;

        Vector3 starFoward = transform.forward;
        Vector3 endFoward = Vector3.forward;

        bool isRotateCamera = false;
        while (t < moveTime)
        {
            t += Time.deltaTime;

            float zFactor = t / moveTime;
            float newZ = Mathf.Lerp(startZ, endZ, zFactor);

            float xFactor = t / leftToCenter;
            float newX = Mathf.Lerp(startX, 5, xFactor);

            Vector3 newPos = new Vector3(newX, 0f, newZ);
            transform.position = newPos;

            transform.forward = Vector3.Lerp(starFoward, endFoward, xFactor);

            yield return null;

            blendSpeed = Mathf.Clamp(blendSpeed + 5f * Time.deltaTime, 0, 1);
            animator.SetFloat("Speed", blendSpeed);

            if (Vector3.Distance(transform.position, endPos.position) < 45f && isRotateCamera == false)
            {
                isRotateCamera = true;
                cameraParentController.RotateToTop();
                ViewManager.Instance.IngameView.DisableTopPanel();
            }
        }
        animator.SetFloat("Speed", 0);
        cameraParentController.SetFollowTarget(null);


        for (int i = listWhiteCat.Count - 1; i >= 0; i--)
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
        StartCoroutine(CRRotatePlayerOnBoat());
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




    private IEnumerator CRRotatePlayerOnBoat()
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









    /// <summary>
    /// Get Pos for Cats base on CatType.
    /// </summary>
    /// <param name="catType"></param>
    /// <returns></returns>
    public Transform GetCatPos(CatType catType)
    {
        if (catType == CatType.BlackCat)
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
}

