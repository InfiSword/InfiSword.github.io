using System.Collections.Generic;
using UnityEngine;

public class FileGrid : MonoBehaviour
{
    private File_Base fileUnit;
    public int GridX { get; private set; }
    public int GridY { get; private set; }
    public GameObject obstacleObject { get; private set; }

    [Header("Visuals")]
    [Tooltip("하이라이트의 기본 색상")]
    [SerializeField] private Color highlightColor = new Color(0.1f, 1f, 0.25f, 1f);
    
    [Tooltip("깜빡임 효과의 최소 알파값 (0~1)")]
    [SerializeField, Range(0f, 1f)] private float highlightMinAlpha = 0.35f;
    
    [Tooltip("깜빡임 효과의 최대 알파값 (0~1)")]
    [SerializeField, Range(0f, 1f)] private float highlightMaxAlpha = 1f;
    
    [Tooltip("하이라이트가 깜빡이는 속도")]
    [SerializeField] private float highlightBlinkSpeed = 3f;
    
    [Tooltip("쉐이더 내부 채우기 투명도")]
    [SerializeField, Range(0f, 1f)] private float highlightFillAlpha = 0.15f;
    
    [Tooltip("쉐이더 테두리 투명도")]
    [SerializeField, Range(0f, 1f)] private float highlightEdgeAlpha = 0.9f;
    
    [Tooltip("쉐이더 테두리 두께")]
    [SerializeField, Range(0.001f, 0.5f)] private float highlightEdgeWidth = 0.2f;
    
    [Tooltip("그리드 셀 크기 대비 하이라이트 크기 배율")]
    [SerializeField] private float highlightSizeMultiplier = 0.95f;
    
    [Tooltip("하이라이트에 사용할 쉐이더")]
    [SerializeField] private Shader highlightShader;

    // 모든 FileGrid 인스턴스가 공유하는 정적 리소스들 (메모리 절약)
    private static Sprite _generatedHighlightSprite;
    private static Material _sharedHighlightMaterial;
    private static MaterialPropertyBlock _sharedPropertyBlock;
    
    // 개별 그리드 인스턴스의 렌더러 참조
    private SpriteRenderer highlightRenderer;

    private readonly HashSet<File_Base> _activeBuffSources = new HashSet<File_Base>();
    private readonly HashSet<File_Base> _visibleBuffHighlightSources = new HashSet<File_Base>();

    public void Init(int _x, int _y)
    {
        fileUnit = null;
        GridX = _x;
        GridY = _y;
        obstacleObject = null;
        SetupHighlightRenderer();
        _activeBuffSources.Clear();
        _visibleBuffHighlightSources.Clear();
        SetBuffHighlightActive(false);
    }

    private void Update()
    {
        // 하이라이트가 활성화된 경우에만 깜빡임 효과 적용
        if (highlightRenderer == null || !highlightRenderer.gameObject.activeSelf)
            return;

        ApplyBlinkHighlightColor();
    }

    private void OnDestroy()
    {
        // 정적 리소스는 인스턴스 파괴 시 제거하지 않음
    }

    #region Highlight
    public void SyncHighlightSize()
    {
        SetupHighlightRenderer();

        if (highlightRenderer == null)
            return;

        highlightRenderer.transform.localScale = GetHighlightScale();
    }

    public void AddBuffHighlightSource(File_Base source)
    {
        if (source == null || !(source.file_Skill is Skill_BuffMain))
            return;

        _visibleBuffHighlightSources.Add(source);
        SetBuffHighlightActive(true);
    }

    public void RemoveBuffHighlightSource(File_Base source)
    {
        if (source == null)
            return;

        _visibleBuffHighlightSources.Remove(source);
        SetBuffHighlightActive(_visibleBuffHighlightSources.Count > 0);
    }

    private void SetBuffHighlightActive(bool active)
    {
        SyncHighlightSize();

        if (highlightRenderer != null)
        {
            highlightRenderer.gameObject.SetActive(active);

            if (active)
                ApplyBlinkHighlightColor();
        }
    }

    private void ApplyBlinkHighlightColor()
    {
        float blink = (Mathf.Sin(Time.time * highlightBlinkSpeed) + 1f) * 0.5f;
        float alpha = Mathf.Lerp(highlightMinAlpha, highlightMaxAlpha, blink);
        
        _sharedPropertyBlock ??= new MaterialPropertyBlock();
        
        _sharedPropertyBlock.SetColor("_Color", new Color(highlightColor.r, highlightColor.g, highlightColor.b, alpha));
        _sharedPropertyBlock.SetFloat("_FillAlpha", highlightFillAlpha);
        _sharedPropertyBlock.SetFloat("_EdgeAlpha", highlightEdgeAlpha);
        _sharedPropertyBlock.SetFloat("_EdgeWidth", highlightEdgeWidth);
        
        highlightRenderer.SetPropertyBlock(_sharedPropertyBlock);
    }

    private void SetupHighlightRenderer()
    {
        if (highlightRenderer != null)
            return;

        Transform existingHighlight = transform.Find("_BuffHighlight");
        GameObject highlightObject = existingHighlight != null
            ? existingHighlight.gameObject
            : new GameObject("_BuffHighlight");

        highlightObject.transform.SetParent(transform, false);
        highlightObject.transform.localPosition = Vector3.zero;
        highlightObject.transform.localRotation = Quaternion.identity;
        highlightObject.transform.localScale = Vector3.one;

        highlightRenderer = highlightObject.GetComponent<SpriteRenderer>();
        if (highlightRenderer == null)
            highlightRenderer = highlightObject.AddComponent<SpriteRenderer>();

        highlightRenderer.sprite = GetHighlightSprite();
        highlightRenderer.sharedMaterial = GetHighlightMaterial();
        highlightRenderer.drawMode = SpriteDrawMode.Simple;
        highlightRenderer.sortingLayerName = "Default";
        highlightRenderer.sortingOrder = 0;
        highlightRenderer.color = Color.white;
        highlightRenderer.transform.localScale = GetHighlightScale();
        highlightObject.SetActive(false);
    }

    private Vector3 GetHighlightScale()
    {
        if (Managers.GridMgr != null)
        {
            Vector2 cellSize = Managers.GridMgr.CellSize;
            if (cellSize.x > 0f && cellSize.y > 0f)
            {
                Vector3 parentScale = transform.lossyScale;
                float scaleX = Mathf.Approximately(parentScale.x, 0f) ? cellSize.x : cellSize.x / parentScale.x;
                float scaleY = Mathf.Approximately(parentScale.y, 0f) ? cellSize.y : cellSize.y / parentScale.y;
                scaleX *= highlightSizeMultiplier;
                scaleY *= highlightSizeMultiplier;
                return new Vector3(scaleX, scaleY, 1f);
            }
        }

        return Vector3.one;
    }

    private static Sprite GetHighlightSprite()
    {
        if (_generatedHighlightSprite != null)
            return _generatedHighlightSprite;

        Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        texture.name = "GeneratedBuffHighlight";
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        _generatedHighlightSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        _generatedHighlightSprite.name = "GeneratedBuffHighlight";
        return _generatedHighlightSprite;
    }

    private Material GetHighlightMaterial()
    {
        if (_sharedHighlightMaterial != null)
            return _sharedHighlightMaterial;

        if (highlightShader == null)
            return null;

        _sharedHighlightMaterial = new Material(highlightShader)
        {
            name = "SharedGridBuffHighlightMaterial"
        };
        return _sharedHighlightMaterial;
    }
    #endregion

    #region Unit
    public File_Base GetFileUnit() => fileUnit;

    public void SetFileUnit(File_Base unit, bool isReturn = false)
    {
        if (unit == null)
        {
            Debug.LogWarning("SetFileUnit: unit is null.");
            return;
        }

        if (!isReturn)
        {
            if (fileUnit != null && unit != fileUnit)
            {
                Debug.LogWarning($"SetFileUnit: grid ({GridX}, {GridY}) is already occupied.");
                return;
            }

            fileUnit = unit;
            unit.CurrentGrid = this;
            if (unit is Unit_Folder _folder && _folder.isZip)
            {
                foreach (var inFile in _folder.myFolderWin.GetInnerFiles)
                {
                    inFile.CurrentGrid = this;
                }
            }
        }

        ApplyGridBuffs(true);
        ApplyBuffActive(true);
    }

    public void RemoveFileUnit()
    {
        if (fileUnit != null)
        {
            ApplyGridBuffs(false);
            ApplyBuffActive(false);

            fileUnit.CurrentGrid = null;
            if (fileUnit is Unit_Folder _folder && _folder.isZip)
            {
                foreach (var inFile in _folder.myFolderWin.GetInnerFiles)
                {
                    inFile.CurrentGrid = null;
                }
            }

            fileUnit = null;
        }
    }
    #endregion

    #region Buff
    public void AddBuffSource(File_Base source)
    {
        if (source == null)
            return;

        if (!_activeBuffSources.Contains(source))
        {
            _activeBuffSources.Add(source);
        }

        if (fileUnit != null)
        {
            ApplyBuffFromSource(fileUnit, source, true);
        }
    }

    public void RemoveBuffSource(File_Base source)
    {
        if (source == null)
            return;

        if (_activeBuffSources.Contains(source))
        {
            _activeBuffSources.Remove(source);
        }
        RemoveBuffHighlightSource(source);

        if (fileUnit != null)
        {
            ApplyBuffFromSource(fileUnit, source, false);
        }
    }

    private void ApplyBuffFromSource(File_Base target, File_Base source, bool apply)
    {
        if (target == null || source == null || target == source)
            return;

        if (source.CurrentGrid == this)
            return;

        Skill_BuffMain buffSkill = source.file_Skill as Skill_BuffMain;
        if (buffSkill == null)
            return;

        float amount = buffSkill.apply_Amount;
        float waitTickTime = buffSkill.waitTickTime;

        if (apply)
        {
            BuffStat _buffStat = new BuffStat(buffSkill.BuffType, source.GetInstanceID(), amount, buffSkill.duration, waitTickTime);
            target.ApplyBuff(_buffStat);
        }
        else
        {
            BuffStat _buffStat = new BuffStat(buffSkill.BuffType, source.GetInstanceID(), amount, buffSkill.duration, waitTickTime);
            target.RemoveBuff(_buffStat);
        }
    }

    private void ApplyGridBuffs(bool apply)
    {
        foreach (File_Base source in _activeBuffSources)
        {
            ApplyBuffFromSource(fileUnit, source, apply);
        }
    }

    private void ApplyBuffActive(bool apply)
    {
        if (fileUnit.file_Skill is Skill_BuffMain Buff)
        {
            Buff.ApplyBuffArea(apply);
        }

        if (fileUnit is Unit_Folder folder && folder.isZip == true)
        {
            foreach (var inFile in folder.myFolderWin.GetInnerFiles)
            {
                if (inFile.file_Skill is Skill_BuffMain innerBuffSkill)
                {
                    innerBuffSkill.ApplyBuffArea(apply);
                }
            }
        }
    }

    public void ZipApplyGirdBuffs()
    {
        ApplyGridBuffs(true);
        ApplyBuffActive(true);
    }

    public void ZipRemoveNonHPBuffs()
    {
        if (fileUnit == null) return;

        foreach (File_Base source in _activeBuffSources)
        {
            Skill_BuffMain buffSkill = source.file_Skill as Skill_BuffMain;
            if (buffSkill != null && buffSkill.BuffType != Define.BuffType.HP)
            {
                ApplyBuffFromSource(fileUnit, source, false);
            }
        }

        ApplyBuffActive(false);
    }

    public void ZipRemoveGridBuffs()
    {
        ApplyGridBuffs(false);
        ApplyBuffActive(false);
    }
    #endregion

    #region Obstacle
    public void SetObstacle(GameObject obstacle)
    {
        if (obstacle == null)
        {
            Debug.LogWarning("SetObstacle: obstacle is null.");
            return;
        }
        obstacleObject = obstacle;
    }

    public void MoveObstacle(FileGrid fileGrid)
    {
        if (obstacleObject != null)
        {
            fileGrid.SetObstacle(obstacleObject);
            obstacleObject = null;
        }
    }

    public void RemoveObstacle()
    {
        if (obstacleObject != null)
        {
            Destroy(obstacleObject);
            obstacleObject = null;
        }
    }
    #endregion
}
