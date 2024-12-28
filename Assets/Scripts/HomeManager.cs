using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HomeManager : MonoBehaviour
{
    [SerializeField] private Transform roadTrans = null;
    [SerializeField] private Animator playerAnimator = null;

    private bool touched = false;
    private float blendSpeed = 0f;
    private float targetBlendSpeed = 0f;
    private float touchReset = 0.2f;

    private float roadSpeed = 0f;
    private float minRoadSpeed = 0f;
    private float maxRoadSpeed = 40f;

    private float velocity = 0f;

    private void Awake()
    {
        PlayerDataController.InitPlayerData();
    }

    private void Start()
    {
        ViewManager.Instance.SetActiveView(ViewType.HomeView);
    }

    private void Update()
    {
        //Move the road
        roadTrans.position += Vector3.back * roadSpeed * Time.deltaTime;
        Transform firstLane = roadTrans.GetChild(0);
        if (firstLane.position.z < -60f)
        {
            Vector3 updatedPos = roadTrans.GetChild(roadTrans.childCount - 1).localPosition + Vector3.forward * 20f;
            firstLane.localPosition = updatedPos;
            firstLane.SetAsLastSibling();
        }


        //Count the touch
        if(Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
        {
            touched = true;
        }
        if (Input.GetMouseButtonUp(0) && touched)
        {
            touched = false;
            touchReset = 1f;
            if(targetBlendSpeed < 0.35f)
            {
                targetBlendSpeed = 0.35f;
            }
            else
            {
                targetBlendSpeed = Mathf.Clamp(targetBlendSpeed + Time.deltaTime * 30f, 0f, 1f);
            }
            roadSpeed = Mathf.Clamp(roadSpeed + Time.deltaTime * 1000f, minRoadSpeed, maxRoadSpeed);


            ViewManager.Instance.HomeView.CreateCoinEffect(Input.mousePosition);
        }
        else
        {
            touchReset -= Time.deltaTime;
            if(touchReset < 0f)
            {
                targetBlendSpeed = Mathf.Clamp(targetBlendSpeed - Time.deltaTime * 0.25f, 0f, 1f);
                roadSpeed = Mathf.Clamp(roadSpeed - Time.deltaTime * 15f, minRoadSpeed, maxRoadSpeed);
            }
        }

        blendSpeed = Mathf.SmoothDamp(blendSpeed, targetBlendSpeed, ref velocity, 0.1f);
        playerAnimator.SetFloat("Speed", blendSpeed);
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
}
