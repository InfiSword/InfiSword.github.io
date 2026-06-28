using DG.Tweening;
using TMPro;
using UnityEngine;

public class WormInfection : VirusInfection
{
    [Header("Shake Settings")]
    private float requiredShakeAmount = 50f;
    private float shakeSensitivity = 1.0f;
    private float shakeVisualPower = 0.1f;

    private float currentShakeAmount = 0f;

    private Vector3 prevMouseWorldPos;
    private bool isDragging = false;

    private SpriteRenderer virusRender;
    private GameObject shakeTextObj;

    protected override void InitializeVisuals()
    {
        GameObject virusObj = new GameObject { name = "WORMVirus" };
        virusRender = virusObj.AddComponent<SpriteRenderer>();
        virusRender.sprite = Resources.Load<Sprite>("Sprite\\Virus\\웜 감염");
        virusRender.sortingLayerName = "Corvered";
        virusObj.transform.SetParent(targetFile.transform);
        virusRender.gameObject.transform.position = targetFile.transform.position;

        shakeTextObj = Instantiate(Resources.Load<GameObject>("Prefabs/UI/VirusMessage/WormShakeText"));
        shakeTextObj.transform.SetParent(targetFile.transform);
        shakeTextObj.transform.localPosition = new Vector3(0, 0.1f, 0);
        Canvas canvas = shakeTextObj.GetComponent<Canvas>();
        canvas.sortingLayerName = "Corvered";
        canvas.sortingOrder = 100;

        TextMeshProUGUI textUI = shakeTextObj.GetComponent<TextMeshProUGUI>();
        Managers.Language.SetText(textUI, "INFECTION_WORM_SHAKE");
        textUI.transform.DOScale(1.1f, 0.4f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

        Debug.Log($"[WormInfection] {targetFile.name} 웜 감염 완료! 드래그로 흔들어 해제 가능");
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D col = GetComponent<Collider2D>();
            if (col == null && targetFile != null)
            {
                col = targetFile.GetComponent<Collider2D>();
            }

            if (col != null && col.OverlapPoint(mouseWorldPos))
            {
                isDragging = true;
                prevMouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        if (isDragging)
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 currentMouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 worldDelta = currentMouseWorldPos - prevMouseWorldPos;
                prevMouseWorldPos = currentMouseWorldPos;

                float delta = worldDelta.magnitude;
                if (delta > 0)
                {
                    currentShakeAmount += delta * shakeSensitivity;

                    // 시각적 떨림 효과
                    ShakeEffect();

                    // 해제 조건 달성
                    if (currentShakeAmount >= requiredShakeAmount)
                    {
                        OnInfectionCleared();
                        RemoveInfection(true);
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                transform.localPosition = originalLocalPos;
            }
        }
    }

    private void ShakeEffect()
    {
        float x = Random.Range(-1f, 1f) * shakeVisualPower;
        float y = Random.Range(-1f, 1f) * shakeVisualPower;
        transform.localPosition = originalLocalPos + new Vector3(x, y, 0);
    }

    protected override void CleanupVisuals()
    {
        if (virusRender != null)
        {
            Destroy(virusRender.gameObject);
        }
        if (shakeTextObj != null)        
        {
            Destroy(shakeTextObj);
        }
    }
}
