using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HomeView : BaseView
{
    [SerializeField] private RectTransform homeView = null;
    [SerializeField] private RectTransform mapsPanel = null;
    [SerializeField] private Text totalCoinText = null;
    [SerializeField] private Text speedText = null;
    [SerializeField] private Text upgradePriceText = null;
    [SerializeField] private Button upgradeButton = null;
    [SerializeField] private CanvasGroup coinEffectPrefab = null;

    private List<CanvasGroup> listCoinEffect = new List<CanvasGroup>();

    private CanvasGroup GetCoinEffect()
    {
        CanvasGroup coinEffect = listCoinEffect.Where(a => !a.gameObject.activeSelf).FirstOrDefault();
        if(coinEffect == null)
        {
            coinEffect = Instantiate(coinEffectPrefab, Vector3.zero, Quaternion.identity);
            coinEffect.transform.SetParent(transform);
            listCoinEffect.Add(coinEffect);
        }
        coinEffect.gameObject.SetActive(true);
        return coinEffect;
    }



    public override void OnShow()
    {
        mapsPanel.gameObject.SetActive(false);
        totalCoinText.text = ConvertCoin(PlayerDataController.CurrentCoin);
        speedText.text = System.Math.Round(PlayerDataController.CurrentSpeed,2).ToString();
        upgradePriceText.text = ConvertCoin(PlayerDataController.CoinToUpgrade);
        upgradeButton.interactable = PlayerDataController.CanUpgradeSpeed();
    }

    public override void OnHide()
    {
        gameObject.SetActive(false);
        foreach (CanvasGroup coinEffect in listCoinEffect) { coinEffect.gameObject.SetActive(false);}
    }

    public void OnClickMapDefaultButton()
    {
        IngameManager.SetTargetMapID(-1);
        LoadingView.SetTargetScene("Ingame");
        ViewManager.Instance.SetActiveView(ViewType.LoadingView);
    }    

    public void OnClickMapButton(int id)
    {
        IngameManager.SetTargetMapID(id);
        LoadingView.SetTargetScene("Ingame");
        ViewManager.Instance.SetActiveView(ViewType.LoadingView);
    }    

    public void OnClickMapsButton()
    {
        mapsPanel.gameObject.SetActive(true);
    }


    public void OnClickCloseMapsButton()
    {
        mapsPanel.gameObject.SetActive(false);
    }


    public void OnClickUpgradeButton()
    {
        PlayerDataController.UpgradeSpeed();
        totalCoinText.text = ConvertCoin(PlayerDataController.CurrentCoin);
        speedText.text = System.Math.Round(PlayerDataController.CurrentSpeed, 2).ToString();
        upgradePriceText.text = ConvertCoin(PlayerDataController.CoinToUpgrade);
        upgradeButton.interactable = PlayerDataController.CanUpgradeSpeed();
    }


    public void CreateCoinEffect(Vector2 screenPos)
    {
        Vector2 localPoint = Vector2.zero;
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(homeView, screenPos, null,out localPoint))
        {
            CanvasGroup coinEffect = GetCoinEffect();
            RectTransform coinTrans = coinEffect.GetComponent<RectTransform>();
            coinTrans.anchoredPosition = localPoint;
            StartCoroutine(CRMoveCoinEffect(coinEffect, coinTrans));
            PlayerDataController.UpdateCoin(100);

            totalCoinText.text = ConvertCoin(PlayerDataController.CurrentCoin);
            upgradeButton.interactable = PlayerDataController.CanUpgradeSpeed();
        }
    }



    private IEnumerator CRMoveCoinEffect(CanvasGroup coinEffect, RectTransform coinTrans)
    {
        float t = 0;
        float moveTime = 0.5f;
        Vector2 startPos = coinTrans.anchoredPosition;
        Vector2 endPos = startPos + Vector2.up * 300f;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            float factor = t / moveTime;
            coinTrans.anchoredPosition = Vector2.Lerp(startPos, endPos, factor);
            coinEffect.alpha = Mathf.Lerp(1f, 0f, factor);
            yield return null;
        }
        coinEffect.alpha = 1f;
        coinEffect.gameObject.SetActive(false);
    }


    private string ConvertCoin(float coin)
    {
        if (coin < 1000) { return System.Math.Round(coin, 2).ToString(); }
        else if (coin > 1000 && coin < 1000000)
        {
            float temp = (float)System.Math.Round(coin / 1000f, 2);
            return temp.ToString() + "K$";
        }
        else if (coin > 1000000 && coin < 1000000000)
        {
            float temp = (float)System.Math.Round(coin / 1000000f, 2);
            return temp.ToString() + "M$";
        }
        else
        {
            float temp = (float)System.Math.Round(coin / 1000000000f, 2);
            return temp.ToString() + "B$";
        }
    }
}
