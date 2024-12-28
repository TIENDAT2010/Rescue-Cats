using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameView : BaseView
{
    [SerializeField] RectTransform countDownPanel = null;
    [SerializeField] Text countDownText = null;
    [SerializeField] RectTransform topPanel = null;
    [SerializeField] RectTransform tsunamiTrans = null;
    [SerializeField] RectTransform playerTrans = null;
    [SerializeField] JoystickController joystickController = null;

    public JoystickController JoystickController => joystickController;

    public override void OnShow()
    {
        tsunamiTrans.anchoredPosition = Vector2.zero;
        StartCoroutine(CRCountDown());
        topPanel.gameObject.SetActive(true);
    }


    public override void OnHide()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator CRCountDown()
    {
        joystickController.gameObject.SetActive(false);
        countDownPanel.gameObject.SetActive(false);
        yield return new WaitForSeconds(6);
        countDownPanel.gameObject.SetActive(true);
        int countTime = 3; 
        while(countTime > 0)
        {
            countDownText.text = countTime.ToString();
            yield return new WaitForSeconds(1);
            countTime--;           
        }    
        FindFirstObjectByType<PlayerController>().GameStart();
        joystickController.gameObject.SetActive(true);
        countDownPanel.gameObject.SetActive(false);
    }    


    public void SetTsunamiPos(float zWorldPos)
    {
        float x = ((zWorldPos + 50) / (IngameManager.Instance.FinishzPos + 50)) * 800;
        x = Mathf.Clamp(x, 0, 800);
        tsunamiTrans.anchoredPosition = new Vector2(x, 0f);
    }

    public void SetPlayerPos(float zWorldPos)
    {
        float x = ((zWorldPos + 50) / (IngameManager.Instance.FinishzPos + 50)) * 800;
        x = Mathf.Clamp(x, 0, 800);
        playerTrans.anchoredPosition = new Vector2(x, 0f);
    }

    public void DisableTopPanel()
    {
        topPanel.gameObject.SetActive(false);
    }    
}
