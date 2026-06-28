using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Skill_Survivor : Skill_BossMain
{
    public enum SurvivorState { Ready, Play, Destroyed }
    private SurvivorState currentState = SurvivorState.Ready;

    [Header("Prefabs")]
    [SerializeField] private GameObject clonePrefab;
    
    [Header("Settings")]
    public float patternDuration = 40f;
    public float warpInterval = 20f;
    public float spawnInterval = 1.5f;
    public int maxVirusCount = 30;

    private List<SurvivorClone> clones = new List<SurvivorClone>();
    private SurvivorClone realClone;

    private float elapsedTime = 0f;
    private float lastWarpTime = 0f;
    private float lastSpawnTime = 0f;

    public override void Init(Unit_VirusBase owner)
    {
        base.Init(owner);
    }

    public override void StartPattern()
    {
        if (isRunning) return;
        isRunning = true;
        SetState(SurvivorState.Ready);
    }

    private void Update()
    {
        if (isRunning && currentState == SurvivorState.Play)
        {
            UpdatePattern();
        }
    }

    public void SetState(SurvivorState newState)
    {
        currentState = newState;
        switch (currentState)
        {
            case SurvivorState.Ready:
                StartCoroutine(SequenceReady());
                break;
            case SurvivorState.Play:
                elapsedTime = 0f;
                lastWarpTime = 0f;
                lastSpawnTime = 0f;
                break;
            case SurvivorState.Destroyed:
                FinishPattern();
                Cleanup();
                break;
        }
    }

    private IEnumerator SequenceReady()
    {
        try
        {
            SpawnClones();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Skill_Survivor] Init Error: {e.Message}");
            SetState(SurvivorState.Destroyed);
            yield break;
        }

        yield return new WaitForSeconds(1.0f);
        SetState(SurvivorState.Play);
    }

    private void SpawnClones()
    {
        FileGridManager gridMgr = Managers.GridMgr;
        int width = gridMgr.GridWidth;
        int height = gridMgr.GridHeight;

        // 8 directions boundary coords
        Vector2Int[] boundaryCoords = new Vector2Int[]
        {
            new Vector2Int(0, 0),               // Bottom-Left
            new Vector2Int(width / 2, 0),       // Bottom-Center
            new Vector2Int(width - 1, 0),       // Bottom-Right
            new Vector2Int(0, height / 2),      // Left-Center
            new Vector2Int(width - 1, height / 2), // Right-Center
            new Vector2Int(0, height - 1),      // Top-Left
            new Vector2Int(width / 2, height - 1), // Top-Center
            new Vector2Int(width - 1, height - 1)  // Top-Right
        };

        foreach (var coord in boundaryCoords)
        {
            Vector2 worldPos = gridMgr.gridArray[coord.x, coord.y].transform.position;
            GameObject go = Instantiate(clonePrefab, worldPos, Quaternion.identity, transform);
            SurvivorClone clone = go.GetComponent<SurvivorClone>();
            clone.Init(this, false);
            clones.Add(clone);
        }

        WarpRealBoss();
    }

    private void WarpRealBoss()
    {
        if (clones.Count == 0) return;

        if (realClone != null) realClone.SetReal(false);

        realClone = clones[UnityEngine.Random.Range(0, clones.Count)];
        realClone.SetReal(true);
        lastWarpTime = elapsedTime;
    }

    private void UpdatePattern()
    {
        elapsedTime += Time.deltaTime;

        // Warp Logic
        if (elapsedTime - lastWarpTime >= warpInterval)
        {
            WarpRealBoss();
        }

        // Spawn Logic
        if (elapsedTime - lastSpawnTime >= spawnInterval)
        {
            TrySpawnVirus();
            lastSpawnTime = elapsedTime;
        }

        // Timer end logic
        if (elapsedTime >= patternDuration)
        {
            SetState(SurvivorState.Destroyed);
        }
    }

    private void TrySpawnVirus()
    {
        if (Managers.Pool.virusSpawner.AliveVirusCount >= maxVirusCount) return;

        SurvivorClone spawnSource = clones[UnityEngine.Random.Range(0, clones.Count)];
        
        float rand = UnityEngine.Random.value;
        Define.VirusType type;
        if (rand < 0.5f) type = Define.VirusType.Virus_Normal;
        else if (rand < 0.7f) type = Define.VirusType.Virus_Worm;
        else if (rand < 0.9f) type = Define.VirusType.Virus_Ransomware;
        else type = Define.VirusType.Virus_Trojan;

        SpawnVirusAt(spawnSource.transform.position, type);
    }

    private void SpawnVirusAt(Vector2 position, Define.VirusType type)
    {
        GameObject go = Managers.Pool.virusSpawner.SpawnVirus(type, position);
        if (go == null) return;

        SurvivorVirusAI ai = go.GetOrAddComponent<SurvivorVirusAI>();
        ai.Init(type);
    }

    public void OnRealBossHit(float damage)
    {
        if (owner != null) owner.TakeDmg(damage);
    }

    public override void StopPattern()
    {
        base.StopPattern();
        SetState(SurvivorState.Destroyed);
    }

    private void Cleanup()
    {
        foreach (var clone in clones)
        {
            if (clone != null) Destroy(clone.gameObject);
        }
        clones.Clear();
        realClone = null;
    }
}
