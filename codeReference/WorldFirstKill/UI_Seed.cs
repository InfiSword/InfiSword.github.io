using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Seed : UI_Popup
{
    enum Buttons
    {
        SeedRandomButton,
        StartButton
    }

    enum Inputs
    {
        SeedInput,
    }

    enum Texts
    {
        SeedHintText,
    }

    private TMP_InputField _seedInput;
    private TMP_Text _seedHint;

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<TMP_InputField>(typeof(Inputs));
        Bind<TMP_Text>(typeof(Texts));

        _seedInput = Get<TMP_InputField>((int)Inputs.SeedInput);
        _seedHint = Get<TMP_Text>((int)Texts.SeedHintText);

        GetButton((int)Buttons.StartButton).onClick.AddListener(() =>
        {
            string input = _seedInput != null ? _seedInput.text : string.Empty;

            if (!Util.TryParseSeed(input, out long seed))
            {
                if (_seedHint != null) 
                    _seedHint.text = "유효한 64비트 정수를 입력하세요 (예: -1234, 4294966062)";
                return;
            }

            GameSeed.Init(seed);
            Debug.Log($"Click_Start with Seed: {seed}");
            
            Managers.IsNewGame = true;
            Managers.Scene.LoadScene(Define.Scene.Game);
        });


        GetButton((int)Buttons.SeedRandomButton).onClick.AddListener(() =>
        {
            Debug.Log("Click_Random");
            long seed = Util.GenerateRandomBaseSeed();
            if (_seedInput != null) _seedInput.text = seed.ToString();
            if (_seedHint != null) _seedHint.text = "랜덤 Seed 생성 완료";
        });

    }

    public override void ClosePopupUI()
    {
        base.ClosePopupUI();
    }

}
