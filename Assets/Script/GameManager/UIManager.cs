using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject panelHome;
    [SerializeField] private GameObject panelHTP;
    [SerializeField] private GameObject panelLevels;

    [Header("UI Elements")]
    [SerializeField] private RectTransform logo;
    [SerializeField] private RectTransform ngButton;
    [SerializeField] private RectTransform cotiButton;
    [SerializeField] private RectTransform htpButton;
    [SerializeField] private RectTransform panelHTPRect;
    [SerializeField] private RectTransform panelLevelsRect;
    [SerializeField] private Button htpBtn;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button newGameBtn;
    [SerializeField] private Button continueBtn;
    [SerializeField] private Button homeButton;
    [SerializeField] private Button backToLevelButton;

    [Header("Level Buttons")]
    [SerializeField] private Button[] levelButtons;
    [SerializeField] private Color unlockedColor = Color.white;
    [SerializeField] private Color lockedColor = Color.gray;
    [SerializeField] private float levelButtonAnimDelay = 0.05f;
    [SerializeField] private float levelButtonScaleTime = 0.2f;

    [Header("References")]
    [SerializeField] private LevelManager levelManager;

    [Header("Animation Settings")]
    [SerializeField] private Vector2 logoStartPos = new Vector2(0, 800);
    [SerializeField] private Vector2 logoEndPos = new Vector2(0, 200);
    [SerializeField] private float logoMoveTime = 1f;
    [SerializeField] private float buttonDelay = 0.2f;
    [SerializeField] private float buttonScaleTime = 0.3f;
    [SerializeField] private float panelScaleTime = 0.3f;

    [Header("Fade")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    [SerializeField] private MapSpawner mapSpawner;

    [Header("Audio")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip buttonClickClip;

    private void Start()
    {
        panelHome.SetActive(true);
        panelHTP.SetActive(false);
        panelLevels.SetActive(false);

        logo.anchoredPosition = logoStartPos;

        ngButton.localScale = Vector3.zero;
        cotiButton.localScale = Vector3.zero;
        htpButton.localScale = Vector3.zero;
        panelHTPRect.localScale = Vector3.zero;

        backToLevelButton.onClick.AddListener(() =>
        {
            PlayButtonSound();
            OpenLevelPanel();
        });
        backToLevelButton.gameObject.SetActive(false);

        htpBtn.onClick.AddListener(() =>
        {
            PlayButtonSound();
            OpenHTPPanel();
        });

        exitButton.onClick.AddListener(() =>
        {
            PlayButtonSound();
            CloseHTPPanel();
        });

        newGameBtn.onClick.AddListener(() =>
        {
            PlayButtonSound();
            OnNewGameClicked();
        });

        continueBtn.onClick.AddListener(() =>
        {
            PlayButtonSound();
            OnContinueClicked();
        });

        homeButton.onClick.AddListener(() =>
        {
            PlayButtonSound();
            BackToHome();
        });

        int savedLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        if (savedLevel < 1)
        {
            continueBtn.interactable = false;
            ColorBlock colors = continueBtn.colors;
            colors.normalColor = lockedColor;
            continueBtn.colors = colors;
        }
        else
        {
            continueBtn.interactable = true;
            ColorBlock colors = continueBtn.colors;
            colors.normalColor = unlockedColor;
            continueBtn.colors = colors;
        }

        if (bgmSource != null && !bgmSource.isPlaying)
        {
            bgmSource.Play();
        }

        AnimateLogo();
    }

    private void AnimateLogo()
    {
        logo.DOAnchorPos(logoEndPos, logoMoveTime).SetEase(Ease.Linear).OnComplete(() =>
        {
            AnimateButtons();
        });
    }

    private void AnimateButtons()
    {
        ngButton.DOScale(Vector3.one, buttonScaleTime).SetEase(Ease.OutBack).SetDelay(0f);
        cotiButton.DOScale(Vector3.one, buttonScaleTime).SetEase(Ease.OutBack).SetDelay(buttonDelay);
        htpButton.DOScale(Vector3.one, buttonScaleTime).SetEase(Ease.OutBack).SetDelay(buttonDelay * 2);
    }

    private void OpenHTPPanel()
    {
        if (panelHTP.activeSelf) return;

        panelHTP.SetActive(true);
        panelHTPRect.localScale = Vector3.zero;
        panelHTPRect.DOScale(Vector3.one, panelScaleTime).SetEase(Ease.OutBack);
    }

    private void CloseHTPPanel()
    {
        panelHTPRect.DOScale(Vector3.zero, panelScaleTime).SetEase(Ease.InBack).OnComplete(() =>
        {
            panelHTP.SetActive(false);
        });
    }

    public void OpenLevelPanel()
    {
        panelHome.SetActive(false);
        panelLevels.SetActive(true);

        panelLevelsRect.localScale = Vector3.zero;
        panelLevelsRect.DOScale(Vector3.one, panelScaleTime).SetEase(Ease.OutBack);

        SetupLevelButtons();
        AnimateLevelButtons();
        backToLevelButton.gameObject.SetActive(false);
        if (mapSpawner != null)
        {
            mapSpawner.ClearMap();
        }
    }

    public void CloseLevelPanel()
    {
        panelLevelsRect.DOScale(Vector3.zero, panelScaleTime)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                panelLevels.SetActive(false);
            });
    }

    private void OnNewGameClicked()
    {
        levelManager.ResetProgress();
        OpenLevelPanel();
    }

    private void OnContinueClicked()
    {
        int savedLevel = PlayerPrefs.GetInt("CurrentLevel", 0);

        panelHTP.SetActive(false);
        FadeInAndLoadLevel(savedLevel);
        ShowBackToLevelButton(true);
    }

    private void BackToHome()
    {
        panelHome.SetActive(true);
        CloseLevelPanel();
        panelHTP.SetActive(false);
        UpdateContinueButtonState();
    }

    private void SetupLevelButtons()
    {
        int currentUnlockedLevel = PlayerPrefs.GetInt("CurrentLevel", 0);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            Button btn = levelButtons[i];
            bool unlocked = i <= currentUnlockedLevel;
            btn.interactable = unlocked;

            ColorBlock colors = btn.colors;
            colors.normalColor = unlocked ? unlockedColor : lockedColor;
            btn.colors = colors;

            btn.transform.localScale = Vector3.zero;
            btn.onClick.RemoveAllListeners();

            if (unlocked)
            {
                int levelIndex = i;
                btn.onClick.AddListener(() =>
                {
                    PlayButtonSound();
                    FadeInAndLoadLevel(levelIndex);
                    ShowBackToLevelButton(true);
                });
            }
        }
    }

    private void AnimateLevelButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            Transform btnTransform = levelButtons[i].transform;
            btnTransform.localScale = Vector3.zero;

            btnTransform.DOScale(Vector3.one, levelButtonScaleTime)
                .SetEase(Ease.OutBack)
                .SetDelay(i * levelButtonAnimDelay);
        }
    }

    public void ShowBackToLevelButton(bool show)
    {
        backToLevelButton.gameObject.SetActive(show);
    }

    public void FadeInAndLoadLevel(int levelIndex)
    {
        StartCoroutine(FadeInCoroutine(levelIndex));
    }

    private IEnumerator FadeInCoroutine(int levelIndex)
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.color = new Color(0, 0, 0, 0);
        yield return fadeImage.DOFade(1f, fadeDuration).WaitForCompletion();
        yield return new WaitForSeconds(0.3f);
        panelLevels.SetActive(false);
        panelHome.SetActive(false);
        levelManager.LoadLevel(levelIndex);
        fadeImage.color = new Color(0, 0, 0, 1);
        yield return fadeImage.DOFade(0f, fadeDuration).WaitForCompletion();
        fadeImage.gameObject.SetActive(false);
    }

    private void UpdateContinueButtonState()
    {
        int savedLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        if (savedLevel < 1)
        {
            continueBtn.interactable = false;
            ColorBlock colors = continueBtn.colors;
            colors.normalColor = lockedColor;
            continueBtn.colors = colors;
        }
        else
        {
            continueBtn.interactable = true;
            ColorBlock colors = continueBtn.colors;
            colors.normalColor = unlockedColor;
            continueBtn.colors = colors;
        }
    }

    private void PlayButtonSound()
    {
        if (sfxSource != null && buttonClickClip != null)
        {
            sfxSource.PlayOneShot(buttonClickClip);
        }
    }
}
