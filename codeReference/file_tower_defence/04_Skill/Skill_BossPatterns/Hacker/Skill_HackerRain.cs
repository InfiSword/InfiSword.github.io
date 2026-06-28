using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Skill_HackerRain : Skill_BossMain
{
    [Header("Settings")]
    [SerializeField] private float fingerSnapCooldown = 4f;
    [SerializeField] private float rainBaseSpawnInterval = 0.25f;
    [SerializeField] private float rainMinSpawnInterval = 0.1f;

    [Header("References")]
    private Animator animator;
    private GameObject bossWindowImg;
    private GameObject errorCodePrefab;
    private Transform topSpawnPoint;
    private Transform bottomSpawnPoint;
    private List<HackerErrorCode> errorCodePool;
    
    // Removed maliciousCodes array. Text generation is now handled in HackerErrorCode.cs.

    private bool isCodeDescending = true;
    private bool canStartScaling = false;
    private Vector3 originalScale;
    private Vector3 originalLocalPos;

    public void Setup(Animator animator, GameObject window, GameObject prefab, Transform top, Transform bottom, List<HackerErrorCode> pool, Vector3 origScale, Vector3 origPos)
    {
        this.animator = animator;
        this.bossWindowImg = window;
        this.errorCodePrefab = prefab;
        this.topSpawnPoint = top;
        this.bottomSpawnPoint = bottom;
        this.errorCodePool = pool;
        this.originalScale = origScale;
        this.originalLocalPos = origPos;
    }

    public override void StartPattern()
    {
        if (isRunning) return;
        isRunning = true;
        StartCoroutine(Pattern_ErrorCodeRain());
    }

    private IEnumerator Pattern_ErrorCodeRain()
    {
        isCodeDescending = true;
        canStartScaling = false;
        Debug.Log("[HackerSkill] Pattern 2 시작: 오류 코드 비");

        animator.Play("DomainExpansion_Start");
        yield return new WaitUntil(() => canStartScaling);

        yield return StartCoroutine(CoScaleWindow(true));
        animator.Play("DomainExpansion");

        float duration = 15f;
        float elapsed = 0f;
        float spawnTimer = 0f;
        float fingerSnapTimer = 0f;

        while (elapsed < duration && isRunning)
        {
            float dt = Time.deltaTime;
            elapsed += dt;
            spawnTimer += dt;
            fingerSnapTimer += dt;

            int currentInfections = Managers.GridMgr.ActiveFiles.Count(f => f != null && f.IsInfected);
            float spawnInterval = Mathf.Max(rainMinSpawnInterval, rainBaseSpawnInterval - (currentInfections * 0.05f));

            if (spawnTimer >= spawnInterval)
            {
                SpawnErrorCode();
                spawnTimer = 0f;
            }

            if (fingerSnapTimer >= fingerSnapCooldown)
            {
                if (animator != null) animator.Play("DomainExpansion_FingerSnap");
                fingerSnapTimer = 0f;
            }
            yield return null;
        }

        if (bossWindowImg != null) yield return StartCoroutine(CoScaleWindow(false));
        FinishPattern();
    }

    private void SpawnErrorCode()
    {
        if (errorCodePrefab == null || (topSpawnPoint == null && bottomSpawnPoint == null))
            return;
        
        HackerErrorCode script = errorCodePool.FirstOrDefault(x => !x.gameObject.activeSelf);
        if (script == null) return;

        float currentDir = isCodeDescending ? 1f : -1f;
        Transform targetPoint = isCodeDescending ? topSpawnPoint : bottomSpawnPoint;
        if (targetPoint == null) return;

        BoxCollider2D targetCollider = targetPoint.GetComponent<BoxCollider2D>();
        if (targetCollider == null) return;

        Bounds bounds = targetCollider.bounds;
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);
        Vector3 spawnWorldPos = new Vector3(randomX, randomY, 0f);

        script.Init(5, spawnWorldPos, currentDir);
    }

    public void OnStartWindowScale() => canStartScaling = true;

    public void OnFingerSnap()
    {
        isCodeDescending = !isCodeDescending;
        float dir = isCodeDescending ? 1f : -1f;
        foreach (var code in errorCodePool)
        {
            if (code != null && code.gameObject.activeInHierarchy) 
                code.SetDirection(dir);
        }
    }

    private IEnumerator CoScaleWindow(bool fillScreen)
    {
        SpriteRenderer sr = bossWindowImg.GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        Vector3 targetScale;
        Vector3 targetLocalPos;

        if (fillScreen)
        {
            float height = Camera.main.orthographicSize * 2.0f; 
            float width = height * Camera.main.aspect;
            height *= 1.6f;
            width *= 1.1f;

            if (sr.sprite != null)
            {
                Vector2 spriteSize = sr.sprite.bounds.size;
                Vector3 parentWorldScale = owner.transform.lossyScale;
                targetScale = new Vector3(width / (spriteSize.x * parentWorldScale.x), height / (spriteSize.y * parentWorldScale.y), 1f);
            }
            else targetScale = Vector3.one;

            Vector3 worldCenter = Camera.main.transform.position;
            worldCenter.z = 0f;
            worldCenter.y += height * 0.15f;
            targetLocalPos = owner.transform.InverseTransformPoint(worldCenter);
        }
        else
        {
            targetScale = originalScale;
            targetLocalPos = originalLocalPos;
        }

        Vector3 startScale = bossWindowImg.transform.localScale;
        Vector3 startLocalPos = bossWindowImg.transform.localPosition;
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            bossWindowImg.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            bossWindowImg.transform.localPosition = Vector3.Lerp(startLocalPos, targetLocalPos, t);
            yield return null;
        }
        bossWindowImg.transform.localScale = targetScale;
        bossWindowImg.transform.localPosition = targetLocalPos;
    }
}
