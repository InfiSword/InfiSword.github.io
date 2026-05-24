using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Loading : UI_Popup
{
    enum GameObjects
    {
        LoadingPanel
    }

    enum Texts
    {
        LoadingText,
        ProgressText
    }

    enum Images
    {
        LoadingBar_Fill
    }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        Bind<TMP_Text>(typeof(Texts));
        Bind<Image>(typeof(Images));

        GetImage((int)Images.LoadingBar_Fill).type = Image.Type.Filled;
        GetImage((int)Images.LoadingBar_Fill).fillMethod = Image.FillMethod.Horizontal;
        GetImage((int)Images.LoadingBar_Fill).fillAmount = 0;

        DontDestroyOnLoad(gameObject);
    }

    public void SetProgress(float progress, string message)
    {
        GetImage((int)Images.LoadingBar_Fill).fillAmount = Mathf.Clamp01(progress);
        GetText((int)Texts.ProgressText).text = $"{(Mathf.Clamp01(progress) * 100):F0}%";
        GetText((int)Texts.LoadingText).text = message;
    }
}
