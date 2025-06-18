using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
    [SerializeField] private Button htpBtn;
    [SerializeField] private Button exitButton;

    [Header("Animation Settings")]
    [SerializeField] private Vector2 logoStartPos = new Vector2(0, 800);
    [SerializeField] private Vector2 logoEndPos = new Vector2(0, 200);
    [SerializeField] private float logoMoveTime = 1f;
    [SerializeField] private float buttonDelay = 0.2f;
    [SerializeField] private float buttonScaleTime = 0.3f;
    [SerializeField] private float panelScaleTime = 0.3f;

    private void Start()
    {
        // Ban đầu
        panelHome.SetActive(true);
        panelHTP.SetActive(false);
        panelLevels.SetActive(false);

        logo.anchoredPosition = logoStartPos;

        ngButton.localScale = Vector3.zero;
        cotiButton.localScale = Vector3.zero;
        htpButton.localScale = Vector3.zero;
        panelHTPRect.localScale = Vector3.zero;

        // Gán nút
        htpBtn.onClick.AddListener(OpenHTPPanel);
        exitButton.onClick.AddListener(CloseHTPPanel);

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
        Debug.Log("HTP Button Clicked"); // Thêm dòng này
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

    // Nếu cần thêm cho PanelLevels
    public void OpenLevelPanel()
    {
        panelHome.SetActive(false);
        panelLevels.SetActive(true);
    }
}
