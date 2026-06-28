using UnityEngine;

/// <summary>
/// TODO: 해당 SystemMain가 맡을 일을 명확하게 생각하기
/// SERVER, CLOUD 의 경우 Atk에 두기에도 Buff에 두기에도 애매하여 새로운 분류를 만듬
/// Atk과 Buff는 파일이나 바이러스 같은 대상에게 발동한다는 느낌인데 이것들은 그거와 맞지 않는다고 생각
/// </summary> <summary>
public abstract class Skill_SystemMain : Skill_Main
{
    public float apply_Amount { get; protected set; }
    public float waitTickTime { get; protected set; }
    public float coolTime { get; protected set; }
    public float duration { get; protected set; }
    public float range { get; protected set; }

    public override void Init(BaseFileStat fileData)
    {
        this.myFile = GetComponent<File_Base>();
        this.myfileExtention = fileData.myExtension;
        SkillSetStat(fileData);
    }

    public override void SkillSetStat(BaseFileStat fileData)
    {
        if (!(fileData is SystemFileStat))
        {
            Debug.LogError($"Skill_SystemMain.SkillSetStat: 잘못된 파일 타입입니다. SystemFileStat이 필요하지만 {fileData.GetType().Name}이 전달되었습니다.");
            return;
        }

        this.apply_Amount = fileData.Atk;
        this.waitTickTime = fileData.WaitTickTime;
        this.coolTime = fileData.CoolTime;
        this.duration = fileData.Duration;
        this.range = fileData.Length;
    }

    protected abstract void Effect();
}
