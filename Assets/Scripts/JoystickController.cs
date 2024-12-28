using UnityEngine;

public class JoystickController : MonoBehaviour
{
    [SerializeField] private RectTransform ingameViewRect = null;
    [SerializeField] private RectTransform joystickRect = null;
    [SerializeField] private RectTransform centerRect = null;
    [SerializeField] private RectTransform borderRect = null;
    [SerializeField] private GameObject joystick = null;


    public Vector2 JoystickDirection { get; private set; } 

    private void Start()
    {
        joystick.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            joystick.SetActive(true);
            Vector3 mousePos = Input.mousePosition;
            Vector2 targetPos = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(ingameViewRect, mousePos, null, out targetPos);
            joystickRect.anchoredPosition = targetPos;
            JoystickDirection = Vector2.zero;
        }    
        if(Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Vector2 targetPos = Vector2.zero;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickRect, mousePos, null, out targetPos);

            Vector2 direction = (targetPos - borderRect.anchoredPosition);

            float mag = Mathf.Clamp(direction.magnitude, 0f, 90f);
            centerRect.anchoredPosition = direction.normalized * mag;

            JoystickDirection = direction.normalized;

        }       
        if(Input.GetMouseButtonUp(0))
        {
            JoystickDirection = Vector2.zero;
            joystick.SetActive(false);
        }
    }

}
