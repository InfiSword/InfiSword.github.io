using System.Linq;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class RansomwareInfection : VirusInfection
{
    private Material virusMat;
    private const int UnlockCost = 15;

    private GameObject ransomUIObj;

    protected override void InitializeVisuals()
    {
        float effectIntensity = PreferenceData.EffectIntensity;
        virusMat = targetFile.GetComponent<File_Base>()._image.material;
        virusMat.EnableKeyword("HOLOGRAM_ON");
        virusMat.SetFloat("_HologramStripesAmount", 0.1f * effectIntensity);
        virusMat.SetFloat("_HologramUnmodAmount", 0.0f);
        virusMat.SetFloat("_HologramStripesSpeed", 4.5f * effectIntensity);
        virusMat.SetFloat("_HologramMinAlpha", 0.1f);
        virusMat.SetFloat("_HologramMaxAlpha", 0.75f);
        virusMat.SetColor("_HologramStripeColor", new Color(255.0f, 0.0f, 0.0f, 255.0f));
        virusMat.SetFloat("_HologramBlend", effectIntensity);

        virusMat.EnableKeyword("GLITCH_ON");
        virusMat.SetFloat("_GlitchAmount", 20.0f * effectIntensity);
        virusMat.SetFloat("_GlitchSize", effectIntensity);

        // 협박 텍스트 UI 생성 호출
        CreateRansomTextUI();
    }

    /// <summary>
    /// 파일 아이콘 위에 협박 텍스트를 띄우는 함수
    /// </summary>
    private void CreateRansomTextUI()
    {
        ransomUIObj = Instantiate(Resources.Load<GameObject>("Prefabs/UI/VirusMessage/RansomwareText"));
        ransomUIObj.transform.SetParent(targetFile.transform, false);
        ransomUIObj.gameObject.transform.localPosition = new Vector3(0, 0.1f, 0);

        Canvas canvas = ransomUIObj.GetComponent<Canvas>();
        canvas.sortingLayerName = "Corvered";
        canvas.sortingOrder = 100;

        TextMeshProUGUI textUI = ransomUIObj.GetComponent<TextMeshProUGUI>();
        Managers.Language.SetFormattedText(textUI, "INFECTION_RANSOM_PAY", UnlockCost);

        textUI.transform.DOScale(1.1f, 0.4f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
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
                HandleClick();
            }
        }
    }

    private void HandleClick()
    {
        if (Managers.nowPlayerData.money >= UnlockCost)
        {
            Managers.nowPlayerData.SetMoney(-UnlockCost);
            OnInfectionCleared();
            RemoveInfection(true);
        }
        else
        {
            Debug.Log($"[RansomwareInfection] 골드가 부족합니다! (현재: {Managers.nowPlayerData.money}, 필요: {UnlockCost})");

            // 돈이 부족할 때 클릭하면 텍스트가 강하게 흔들리는 연출 (피드백)
            if (ransomUIObj != null)
            {
                ransomUIObj.transform.DOComplete(); // 진행중인 흔들기 초기화
                ransomUIObj.transform.DOShakePosition(0.3f, new Vector3(0.5f * PreferenceData.ScreenShakeAmount, 0, 0), 20);
            }
        }
    }

    protected override void CleanupVisuals()
    {
        if (virusMat != null)
        {
            virusMat.DisableKeyword("HOLOGRAM_ON");
            virusMat.DisableKeyword("GLITCH_ON");

            virusMat.SetFloat("_HologramBlend", 0.0f);
            virusMat.SetFloat("_GlitchAmount", 0.0f);
        }

        // 바이러스가 해제(또는 파괴)될 때 생성했던 텍스트 UI와 애니메이션 깔끔하게 제거
        if (ransomUIObj != null)
        {
            ransomUIObj.transform.DOKill();
            Destroy(ransomUIObj);
        }
    }
}
