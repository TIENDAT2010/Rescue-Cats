using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ViewManager : MonoBehaviour
{
    public static ViewManager Instance { get; private set; }

    [SerializeField] private HomeView homeViewPrefab = null;
    [SerializeField] private IngameView ingameViewPrefab = null;
    [SerializeField] private EndgameView endgameViewPrefab = null;
    [SerializeField] private LoadingView loadingViewPrefab = null;
    [SerializeField] private GameObject eventSystemPrefab = null;
    [SerializeField] private Canvas canvas = null;

    public HomeView HomeView { private set; get; }
    public IngameView IngameView { private set; get; }
    public EndgameView EndgameView { private set; get; }
    public LoadingView LoadingView { private set; get; }
    public Canvas Canvas => canvas;
    private BaseView currentView = null;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (transform.childCount == 0)
            {
                Instantiate(eventSystemPrefab, transform);

                HomeView = Instantiate(homeViewPrefab, transform, false);
                HomeView.transform.localScale = Vector3.one;
                HomeView.OnHide();

                IngameView = Instantiate(ingameViewPrefab, transform, false);
                IngameView.transform.localScale = Vector3.one;
                IngameView.OnHide();

                EndgameView = Instantiate(endgameViewPrefab, transform, false);
                EndgameView.transform.localScale = Vector3.one;
                EndgameView.OnHide();

                LoadingView = Instantiate(loadingViewPrefab, transform, false);
                LoadingView.transform.localScale = Vector3.one;
                LoadingView.OnHide();
            }
        }
    }



    /// <summary>
    /// Set the view active with given type.
    /// </summary>
    /// <param name="viewType"></param>
    public void SetActiveView(ViewType viewType)
    {
        if (currentView != null)
            currentView.OnHide();
        switch(viewType)
        {
            case ViewType.HomeView:
                HomeView.gameObject.SetActive(true);
                HomeView.OnShow();
                currentView = HomeView;
                break;
            case ViewType.IngameView:
                IngameView.gameObject.SetActive(true);
                IngameView.OnShow();
                currentView = IngameView;
                break;
            case ViewType.EndgameView:
                EndgameView.gameObject.SetActive(true);
                EndgameView.OnShow();
                currentView = EndgameView;
                break;
            case ViewType.LoadingView:
                LoadingView.gameObject.SetActive(true);
                LoadingView.OnShow();
                currentView = LoadingView;
                break;
        }
    }

}
