using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class VirusInfectionAbility : MonoBehaviour
{
    private static readonly System.Type[] InfectionTypes = new System.Type[]
    {
        typeof(WormInfection),
        typeof(RansomwareInfection),
        // 새로운 감염 타입은 여기에 추가하면 됨
        // typeof(TrojanInfection),
        // typeof(SpywareInfection),
    };

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Unit_MyCom myCom = collision.GetComponent<Unit_MyCom>();
        if (myCom == null) return;

        // 내 컴퓨터에 도달했을 때 감염 시도
        InfectRandomFile();
    }

    private void InfectRandomFile()
    {
        if (InfectionTypes.Length == 0)
        {
            Debug.LogWarning("[VirusInfectionAbility] 등록된 감염 타입이 없습니다!");
            return;
        }

        List<File_Base> installedFiles = Managers.GridMgr.ActiveFiles;

        List<File_Base> targetCandidates = installedFiles
            .Where(file => 
                file != null &&
                !(file is Unit_MyCom) && 
                file.FileState == FileStatus.normal &&
                !file.IsInfected)  // 아예 감염되지 않은 파일만 선택
            .ToList();

        if (targetCandidates.Count > 0)
        {
            // 랜덤하게 감염 타입 선택
            System.Type selectedInfectionType = InfectionTypes[Random.Range(0, InfectionTypes.Length)];
            
            // 랜덤 대상 선정
            int randomIndex = Random.Range(0, targetCandidates.Count);
            File_Base target = targetCandidates[randomIndex];

            Component infectionComponent = target.gameObject.AddComponent(selectedInfectionType);
            Debug.Log($"[감염] {target.name}에 {selectedInfectionType.Name} 발생!");
        }
        else
        {
            Debug.Log($"[감염 실패] 감염시킬 수 있는 파일이 없습니다. (모든 파일이 이미 감염됨 또는 파일 없음)");
        }
    }
}

