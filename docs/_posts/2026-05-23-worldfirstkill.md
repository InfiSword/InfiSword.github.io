layout: single

title: "PROJECT REPORT // WORLDFIRSTKILL"

excerpt: "리플렉션 기반 동적 파싱 및 완전한 상태 복원 시스템"

categories: \[Project]

tags: \[Data Structure, Server, C#, System Design]

toc: true

toc\_sticky: true



서버(구글 스프레드시트) 기반 게임 데이터 관리와, Seed/Token을 활용한 완벽한 상태 복원 시스템입니다.



1\. 리플렉션(Reflection) 기반 CSV 동적 파싱



새로운 게임 데이터(아이템, 몬스터 등)가 추가될 때마다 매핑 코드를 작성하는 수고를 덜기 위해, 리플렉션을 활용한 자동 파싱 시스템을 구축했습니다.



public List<T> parseCSV<T>(string csvData) where T : BaseData, new()

{

&#x20;   Type myType = typeof(T);

&#x20;   FieldInfo\[] myFieldInfo = myType.GetFields();



&#x20;   // 1. CSV 첫 줄 분리 및 필드 매핑

&#x20;   // 2. 정규식을 활용한 SmartSplit (따옴표 내 쉼표 안전 처리)

&#x20;   // 3. Activator로 새 객체 생성 및 타입 자동 변환 할당

&#x20;   

&#x20;   return csvDataList;

}





타입 안전성 보장: 제네릭 제약 조건(where T : BaseData) 활용



타입 자동 변환: 리플렉션으로 필드 타입을 확인하여 int, float, string 등 알맞은 형태로 캐스팅



2\. 서버 다운로드 및 로컬 캐싱 (버전 관리)



앱 시작 시 불필요한 다운로드를 줄이기 위해 자체 버전 관리 로직을 도입했습니다.



메타 시트 우선 다운: 각 CSV의 서버 최신 버전 번호를 조회합니다.



버전 비교: 로컬 버전 != 서버 버전일 경우에만 해당 CSV를 비동기 다운로드.



로컬 캐싱: 변경이 없는 데이터는 로컬 텍스트 파일을 그대로 읽어들여 트래픽을 아낍니다.



3\. Seed / Token 시스템 (RNG 완전 복원)



게임을 세이브하고 로드했을 때, 랜덤 시드가 꼬여 보상이나 맵 생성 결과가 달라지는 것을 막기 위한 핵심 시스템입니다.

단순히 BaseSeed만 저장하는 것이 아니라, 각 시스템(Domain)별로 난수가 몇 번 호출되었는지 사용량까지 추적합니다.



💡 Token 인코딩 다이어그램

BaseSeed (10자리) + Domain 개수 (3자리) + \[Domain ID (3자리) + 사용량 (12자리)] \* N

{: .notice--success }



public static string BuildSeedToken()

{

&#x20;   int baseSeed = GameSeed.BaseSeed;  

&#x20;   Dictionary<GameSeed.Domain, ulong> usage = GameSeed.GetUsageSnapshot();  



&#x20;   // 고정 폭(Fixed-Width) 인코딩으로 안전한 문자열 생성

&#x20;   string token = Pad(EncodeIntToUInt(baseSeed), BASE\_SEED\_WIDTH);

&#x20;   // ... Domain 사용량 순회 및 패딩 추가 로직



&#x20;   return token;

}





저장된 토큰은 SaveLoadManager를 통해 다시 디코딩되어 게임의 난수 생성기를 완벽히 동일한 상태로 복원해 냅니다.



4\. 비동기 로딩 시스템 (UI 블로킹 방지)



async/await 및 UnityWebRequest를 활용해 대량의 데이터 파싱 시 메인 스레드가 멈추는 프리징(Freezing) 현상을 해결했습니다.

무거운 파싱 작업은 Task.Run()을 통해 백그라운드 스레드로 넘기고, UI 업데이트는 UnityMainThreadDispatcher를 사용해 안전하게 메인 큐로 전달합니다.

