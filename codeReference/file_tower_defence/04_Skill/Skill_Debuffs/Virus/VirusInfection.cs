using UnityEngine;
using System.Linq;

public abstract class VirusInfection : MonoBehaviour
{
    protected File_Base targetFile;
    protected Vector3 originalLocalPos;  

    public File_Base TargetFile => targetFile;

    protected virtual void Start()
    {
        targetFile = GetComponent<File_Base>();

        if (targetFile == null)
        {
            Destroy(this);
            return;
        }

        // 감염 시 파일 잠금 및 위치 저장
        targetFile.FileState = FileStatus.locked;
        originalLocalPos = transform.localPosition;

        InitializeVisuals();
    }

    protected abstract void InitializeVisuals();

    /// <summary>
    /// 감염 강제 치료
    /// </summary>
    public virtual void ForceClear()
    {
        OnInfectionCleared();
        RemoveInfection(true);
    }

    protected virtual void OnInfectionCleared()
    {
        Debug.Log($"{this.GetType().Name} - {targetFile.name} 감염 해제 완료!");
    }

    protected virtual void RemoveInfection(bool recoverState)
    {
        transform.localPosition = originalLocalPos;

        if (recoverState && targetFile != null)
        {
            // 다른 바이러스가 남아있는지 확인
            VirusInfection[] otherViruses = targetFile.GetComponents<VirusInfection>();
            // 나 자신을 제외하고 다른 바이러스가 없어야 정상 상태로 복구
            bool hasOtherVirus = otherViruses.Any(v => v != this);

            if (!hasOtherVirus)
            {
                targetFile.FileState = FileStatus.normal;
            }
        }

        CleanupVisuals();
        Destroy(this);
    }

    protected abstract void CleanupVisuals();
}
