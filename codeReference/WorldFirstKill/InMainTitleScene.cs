using UnityEngine;

public class InMainTitleScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Lobby;
        Managers.UI.ShowSceneUI<UI_MainTitle>();
    }

    public override void Clear()
    {
        Managers.Loading.Clear();
    }
}
