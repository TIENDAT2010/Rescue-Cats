
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndgameView : BaseView
{
    [SerializeField] private GameObject completedPanel = null;
    [SerializeField] private GameObject gameOverPanel = null;
    [SerializeField] private Text catsRescuedText = null;
    [SerializeField] private Text coinText = null;

    private int coinBonus = 0;
    public override void OnShow()
    {
        if (IngameManager.Instance.GameState == GameState.LevelCompleted)
        {
            completedPanel.SetActive(true);
            gameOverPanel.SetActive(false);
            coinBonus = IngameManager.Instance.RescuedCats() * 10;
        }
        else if (IngameManager.Instance.GameState == GameState.LevelFailed)
        {
            completedPanel.SetActive(false);
            gameOverPanel.SetActive(true);
        }
        catsRescuedText.text = "Cats Rescued: " + IngameManager.Instance.RescuedCats().ToString() + "/" + IngameManager.Instance.TotalCats.ToString();   
        StartCoroutine(CRShowResultPanel()); 
    }

    public override void OnHide() { gameObject.SetActive(false); }


    private IEnumerator CRShowResultPanel()
    {
        coinText.text = "0";
        int t = 0;
        int coinReward = (IngameManager.Instance.GameState == GameState.LevelFailed ? 30 : 100) + coinBonus;
        while (t < coinReward)
        {
            t += 1;
            coinText.text = "+" + t.ToString();
            yield return new WaitForSeconds(0.05f);
        }
        PlayerDataController.UpdateCoin(coinReward);
    }

    public void OnClickHomeButton()
    {
        LoadingView.SetTargetScene("Home");
        ViewManager.Instance.SetActiveView(ViewType.LoadingView);
    }

    public void OnClickRetryButton()
    {
        LoadingView.SetTargetScene("Ingame");
        ViewManager.Instance.SetActiveView(ViewType.LoadingView);
    }
}




