using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class TweenSample : MonoBehaviour
{
    public RectTransform UITarget;   // UI 타겟
    public Image UIImage;
    public GameObject ObjectTarget;  // 오브젝트 타겟

    public TMP_Text countText;

    public int currentValue = 0;
    public int addValue = 100;

    private int targetValue;

    public Color flashColor = Color.red;

    private Color originalColor;

    public CanvasGroup fadeTarget;

    public GameObject coinPrefab;

    void Start()
    {
        originalColor = UIImage.color;

        fadeTarget.alpha = 0;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayPunchUIScale();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayPunchObjectScale();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlayUIShake();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PlayCountUp();
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            PlayColorFlash();
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            PlayFade();
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            Vector3 dropPosition = transform.position + Vector3.up;

            Instantiate(
                coinPrefab,
                dropPosition,
                Quaternion.identity
            );
        }
    }

    public void PlayPunchUIScale()
    {
        if (UITarget == null) return;

        UITarget.DOKill();
        UITarget.localScale = Vector3.one;
        UITarget.DOPunchScale(Vector3.one * 0.3f, 0.25f, 8, 1.0f);
    }

    public void PlayPunchObjectScale()
    {
        if (ObjectTarget == null) return;

        ObjectTarget.transform.DOKill();
        ObjectTarget.transform.localScale = Vector3.one;
        ObjectTarget.transform.DOPunchScale(Vector3.one * 0.3f, 0.25f, 8, 1.0f);
    }

    public void PlayUIShake()
    {
        if (ObjectTarget == null) return;

        ObjectTarget.transform.DOKill(); // 이전 Tween 제거

        ObjectTarget.transform.DOShakePosition(
            0.3f, // 시간
            20f,  // 강도
            20,   // 진동 횟수
            90f   // 랜덤성
        );
    }
    public void PlayCountUp()
    {
        if (countText == null) return;

        targetValue += addValue; // 목표 숫자 증가

        DOTween.Kill("CountTween", true);

        DOTween.To(
            () => currentValue, // 현재 값
            value =>
            {
                currentValue = value;
                countText.text = currentValue.ToString();
            },
            targetValue, // 목표 값
            0.5f         // 시간
        )
        .SetEase(Ease.OutQuad)
        .SetId("CountTween");
    }
    public void PlayColorFlash()
    {
        if (UIImage == null) return;

        UIImage.DOKill();

        UIImage.color = originalColor;

        UIImage.DOColor(flashColor, 0.1f)
        .OnComplete(() =>
        {
            UIImage.DOColor(originalColor, 0.2f);
        });
    }
    public void PlayFade()
    {
        if (fadeTarget == null) return;

        fadeTarget.DOKill();
        fadeTarget.alpha = 0;

        Sequence seq = DOTween.Sequence();

        seq.Append(fadeTarget.DOFade(1, 0.2f));   // 나타남
        seq.AppendInterval(0.5f);                 // 유지
        seq.Append(fadeTarget.DOFade(0f, 0.3f)); // 사라짐
    }
}
