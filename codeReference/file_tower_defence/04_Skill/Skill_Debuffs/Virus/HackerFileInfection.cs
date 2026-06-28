using UnityEngine;
using System.Collections;
using DG.Tweening;

public class HackerFileInfection : VirusInfection
{
    private float timer = 12f;
    [SerializeField] private float tickDamage = 10f;
    [SerializeField] private float tickInterval = 0.25f;
    private VirusBoss_Hacker ownerBoss;
    private Coroutine deleteCoroutine;
    private Coroutine tickDamageCoroutine;
    private int clickCount = 0;
    private bool isCleared = false;

    private SpriteRenderer virusRender;
    private GameObject gaugeUIObj;
    private UnityEngine.UI.Image gaugeFillImage;
    private float visualRatio = 1f;
    private Tween gaugeTween;

    private LineRenderer linePrefab;
    private LineRenderer line;

    public void SetOwner(VirusBoss_Hacker boss, LineRenderer prefab)
    {
        ownerBoss = boss;
        linePrefab = prefab;
    }

    protected override void Start()
    {
        base.Start();
        
        // 감염 게이지 설정
        targetFile.SetInfectionGauge(80f);
        targetFile.OnInfectionGaugeChanged += UpdateGaugeUI;

        deleteCoroutine = StartCoroutine(CoDeleteTimer());
        tickDamageCoroutine = StartCoroutine(CoTickDamage());
    }

    protected override void InitializeVisuals()
    {
        GameObject virusObj = new GameObject { name = "HackingVirus" };
        virusRender = virusObj.AddComponent<SpriteRenderer>();
        virusRender.sprite = Resources.Load<Sprite>("Sprite\\Virus\\Boss\\HACKER\\FileHackedCover");
        virusRender.sortingLayerName = "Corvered";
        virusObj.transform.SetParent(targetFile.transform);

        virusObj.transform.position = targetFile.transform.position;
        virusObj.transform.position += new Vector3(0, 0.2f, 0);

        if (targetFile != null && targetFile._image != null)
        {
            targetFile._image.color = Color.red;
        }

        CreateGaugeUI();

        if (ownerBoss != null && linePrefab != null)
        {
            line = Instantiate(linePrefab, ownerBoss.transform);
            line.positionCount = 2;
            line.startColor = Color.red;
            line.endColor = Color.red;
        }
    }

    private void CreateGaugeUI()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/UI/InfectionGuage");
        if (prefab != null && targetFile != null && targetFile._unitCanvas != null)
        {
            gaugeUIObj = Instantiate(prefab, targetFile._unitCanvas.transform);
            
            Transform fillTransform = gaugeUIObj.transform.Find("CurHPImage");
            if (fillTransform != null)
            {
                gaugeFillImage = fillTransform.GetComponent<UnityEngine.UI.Image>();
                if (gaugeFillImage != null)
                {
                    gaugeFillImage.material = Instantiate(gaugeFillImage.material);
                }
            }
        }
        
        UpdateGaugeUI(targetFile.currentInfectionGauge, targetFile.maxInfectionGauge);
    }

    private void UpdateGaugeUI(float current, float max)
    {
        if (gaugeFillImage != null && gaugeFillImage.material != null && max > 0)
        {
            float targetRatio = Mathf.Clamp01(current / max);
            gaugeFillImage.material.SetFloat("_segmentAmount", max / 10f);

            gaugeTween?.Kill();
            gaugeTween = DOTween.To(() => visualRatio, x => {
                visualRatio = x;
                gaugeFillImage.material.SetFloat("_offset", visualRatio - 0.5f);
            }, targetRatio, 0.5f).SetEase(Ease.OutQuad); 
        }
    }

    private IEnumerator CoDeleteTimer()
    {
        yield return new WaitForSeconds(timer);
        
        if (targetFile != null)
        {
            Debug.Log($"[HackerInfection] 시간 초과! {targetFile.name} 삭제");
            ownerBoss?.NotifyInfectionExpired(this);
            targetFile.DeleteThisFile();
        }
    }

    private IEnumerator CoTickDamage()
    {
        while (targetFile != null && targetFile.FileState != FileStatus.Dead)
        {
            yield return new WaitForSeconds(tickInterval);
            if (targetFile != null && targetFile.FileState != FileStatus.Dead)
            {
                targetFile.TakeDmg(tickDamage);
            }
        }
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
            return;

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
        if (isCleared)
            return;

        clickCount++;
        Debug.Log($"[HackerFileInfection] Click count: {clickCount}/5");

        if (targetFile != null)
        {
            UnitState.instance.FloatingText($"{clickCount}/5", targetFile.transform.position, false);
            
            float amount = targetFile.maxInfectionGauge / 5f;
            targetFile.ReduceInfectionGauge(amount);
        }
    }

    protected override void OnInfectionCleared()
    {
        isCleared = true;
        base.OnInfectionCleared();

        if (deleteCoroutine != null)
            StopCoroutine(deleteCoroutine);

        if (tickDamageCoroutine != null)
            StopCoroutine(tickDamageCoroutine);

        if (targetFile != null)
        {
            targetFile.OnInfectionGaugeChanged -= UpdateGaugeUI;
        }

        gaugeTween?.Kill();
        ownerBoss?.NotifyInfectionCleared(this);
    }

    protected override void CleanupVisuals()
    {
        if (targetFile != null && targetFile._image != null)
        {
            targetFile._image.color = Color.white;
        }

        if(virusRender != null)
            Destroy(virusRender.gameObject);

        if (gaugeUIObj != null)
            Destroy(gaugeUIObj);

        if (line != null)
        {
            Destroy(line.gameObject);
            line = null;
        }
    }

    private void LateUpdate()
    {
        if (line != null && targetFile != null && ownerBoss != null)
        {
            line.SetPosition(0, ownerBoss.transform.position);
            line.SetPosition(1, targetFile.transform.position);
        }
    }

    private void OnDestroy()
    {
        if (targetFile != null)
        {
            targetFile.OnInfectionGaugeChanged -= UpdateGaugeUI;
        }
        gaugeTween?.Kill();
        
        CleanupVisuals();

        if (!isCleared)
        {
            ownerBoss?.NotifyInfectionExpired(this);
        }
    }
}
