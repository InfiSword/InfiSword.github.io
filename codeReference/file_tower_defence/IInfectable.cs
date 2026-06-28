public interface IInfectable
{
    // 현재 감염된 상태인지 확인
    bool IsInfected { get; }

    // 스택 증가 (알약의 치료등, 기타 파일의 감염 효과 증폭등에 사용)
    void AddStack(int amount);

    // 스택 달성 시, 혹은 즉시 치료 스킬을 위한 함수
    void InvokeFullStack();
}