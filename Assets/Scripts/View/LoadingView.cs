using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingView : BaseView
{
    [SerializeField] private Text loadingText = null;
    [SerializeField] private Slider loadingSlider = null;

    private static string TargetScene = string.Empty;

    public override void OnShow()
    {
        loadingSlider.value = 0f;
        StartCoroutine(CRLoadScene());
    }

    public override void OnHide()
    {
        loadingSlider.value = 0f;
        gameObject.SetActive(false);
    }


    private IEnumerator CRLoadScene()
    {
        float loadingAmount = 0f;
        while (loadingAmount < 0.95f)
        {
            yield return new WaitForSeconds(0.01f);
            loadingAmount += 0.02f;
            SetLoadingAmount(loadingAmount);
        }

        AsyncOperation asyn = SceneManager.LoadSceneAsync(TargetScene);
        while (!asyn.isDone)
        {
            yield return null;
        }
    }


    /// <summary>
    /// Set the loading amount.
    /// </summary>
    /// <param name="amount"></param>
    public void SetLoadingAmount(float amount)
    {
        loadingSlider.value = amount;
        loadingText.text = System.Math.Round((amount / 1f) * 100f, 2).ToString() + "%";
    }

    public static void SetTargetScene(string scene)
    {
        TargetScene = scene;
    }    
}
