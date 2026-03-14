# 인디 게임 스타터 아키텍처

## 1. 이 문서의 목적

이 문서는 Unity 기반 인디 게임 프로젝트를 시작할 때 매번 재사용할 수 있는
`스타터 틀`을 정리한 문서다.

여기서 만들고 싶은 것은 "게임 규칙 완성본"이 아니다.

만들고 싶은 것은 아래처럼 거의 모든 프로젝트에서 반복되는 공통 배관이다.

- 부트스트랩
- DI 등록 구조
- 상태 전이
- 공통 서비스
- 옵션 / 사운드 / 저장 / UI / 씬 전환

즉, 이 문서는
"새 프로젝트를 시작할 때 처음 깔고 들어가는 공용 셸"
을 정의한다.

---

## 2. 핵심 방향

프로젝트는 크게 세 층으로 나눈다.

1. `Foundation`
   프로젝트를 움직이는 공통 기반 배관

2. `CommonModules`
   대부분의 인디 게임에서 거의 매번 들어가는 기능

3. `Game`
   이번 게임에서만 바뀌는 규칙, 콘텐츠, 장르 로직

이 경계가 깨지지 않으면, 다음 프로젝트로 이식하기 쉬워진다.

---

## 3. 무엇을 재사용할 것인가

거의 영구적으로 재사용하기 좋은 것:

- 시작 흐름
- DI 등록 방식
- 이벤트 버스
- 상태 머신
- 설정
- 저장 / 불러오기
- 오디오
- 씬 로딩
- 입력 래퍼
- 공통 UI 팝업 흐름
- 일시정지
- 로딩 화면
- 디버그 기능

공통 코어에 너무 깊게 넣지 말아야 할 것:

- 전투 규칙
- AI 행동 규칙
- 퀘스트 완료 규칙
- 인벤토리 의미 설계
- 성장 공식

이런 것들은 스타터 내부에 박아넣기보다
"나중에 꽂는 모듈"로 보는 게 맞다.

---

## 4. 추천 폴더 구조

```text
Assets/_Project
  /Foundation
    /Bootstrap
    /DI
    /Events
    /StateMachine
    /Services
    /SceneFlow
    /Persistence
    /UIFramework
    /Update
    /Utilities

  /CommonModules
    /Settings
    /Audio
    /SaveLoad
    /Input
    /Pause
    /Loading
    /Localization
    /Debug

  /Game
    /Installers
    /Features
    /Rules
    /Content
    /Configs

  /Presentation
    /UI
    /HUD
    /Audio
    /VFX

  /Scenes
  /Tests
```

정리하면:

- `Foundation`: 거의 안 바뀌는 틀
- `CommonModules`: 자주 재사용하는 기능 묶음
- `Game`: 이번 게임 전용 로직
- `Presentation`: 화면 표시와 연출

---

## 5. 프로젝트 시작 흐름

스타터는 보통 아래처럼 동작한다.

```text
Unity 씬 시작
-> GameBootstrapper 실행
-> Installer들이 서비스 등록
-> 공통 모듈 초기화
-> StateMachine이 BootState 진입
-> 설정/저장/프로필 로드
-> TitleState 진입
-> 시작 버튼 입력
-> LoadingState 진입
-> 게임 씬 로드
-> PlayState 진입
```

이 흐름이 매번 같아지면,
새 프로젝트를 시작할 때마다 구조를 다시 고민할 필요가 크게 줄어든다.

즉, 네가 원한 "초기에 그걸로 시작하는 느낌"이 맞다.
이 문서의 구조는 정확히 그 목적에 맞다.

---

## 6. 실제 클래스 구성

## 6-1. Bootstrap

### `GameBootstrapper : MonoBehaviour`

책임:

- 프로젝트 진입점
- 컨테이너 생성 시작
- Installer 실행
- 공통 서비스 초기화
- 상태 머신 시작

예시 역할:

```csharp
public sealed class GameBootstrapper : MonoBehaviour
{
    private IObjectResolver _resolver;
    private IGameStateMachine _stateMachine;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // 1. 컨테이너 생성
        // 2. 서비스 등록
        // 3. 상태 머신 Resolve
        // 4. BootState 진입
    }
}
```

나중에 VContainer를 붙여도
이 클래스는 그대로 "조립의 시작점" 역할을 하면 된다.

---

## 6-2. Installer 계층

Installer는 스타터의 핵심이다.
무엇을 등록할지 결정하지만, 게임 규칙을 직접 담지는 않는다.

### `IGameInstaller`

```csharp
public interface IGameInstaller
{
    void Install(IServiceCollection services);
}
```

추천 Installer:

- `FoundationInstaller`
- `CommonModulesInstaller`
- `GameInstaller`
- `PresentationInstaller`

각 역할:

- `FoundationInstaller`
  이벤트 버스, 상태 머신, 씬 서비스, UI 서비스, 업데이트 라우터 등록

- `CommonModulesInstaller`
  설정, 사운드, 저장, 입력, 로딩, 일시정지 등록

- `GameInstaller`
  이번 프로젝트 전용 기능 등록

- `PresentationInstaller`
  Presenter, Screen, View 연결 등록

이 구조의 장점은
다음 프로젝트로 넘어갈 때 `FoundationInstaller`, `CommonModulesInstaller`는 거의 그대로 쓰고,
`GameInstaller`만 바꾸면 된다는 점이다.

---

## 6-3. DI 계층

이 스타터는 "컨테이너 친화적"이어야 하지만
"특정 컨테이너 종속"이어서는 안 된다.

즉, 코드 전체가 VContainer API를 직접 도배하지 않게 하고
경계만 맞춰두는 방식이 좋다.

### `IServiceCollection`

역할:

- 시작 시 서비스 등록

예시:

```csharp
public interface IServiceCollection
{
    void RegisterSingleton<TService, TImplementation>()
        where TImplementation : TService;

    void RegisterSingleton<TService>(TService instance);

    void RegisterTransient<TService, TImplementation>()
        where TImplementation : TService;
}
```

### `IObjectResolver`

역할:

- 등록이 끝난 뒤 객체 가져오기

예시:

```csharp
public interface IObjectResolver
{
    T Resolve<T>();
}
```

이렇게 해두면:

- 지금은 간단한 직접 구현으로 시작 가능
- 나중엔 VContainer로 교체 가능
- 프로젝트 코드가 컨테이너 세부 구현에 덜 묶임

중요한 규칙:

- 컨테이너 사용은 `Bootstrap`, `Installer` 쪽에 몰아두기
- 게임 로직에서 `Resolve()` 남발하지 않기

이 규칙이 DI를 깔끔하게 쓰는 핵심이다.

---

## 6-4. DI를 쉽게 다시 설명

DI는 어렵게 보면 복잡해 보이는데,
실제로는 그냥 이 말이다.

"객체가 필요한 걸 스스로 만들지 말고, 바깥에서 넣어줘라"

안 좋은 예:

```csharp
public class OptionsPopup
{
    private AudioService _audio = new AudioService();
    private SaveService _save = new SaveService();
}
```

문제:

- UI가 서비스 생성까지 해버림
- 테스트 어려움
- 구현 교체 어려움
- 다른 프로젝트로 옮기기 어려움

좋은 예:

```csharp
public class OptionsPopupPresenter
{
    private readonly IAudioService _audioService;
    private readonly ISaveService _saveService;

    public OptionsPopupPresenter(
        IAudioService audioService,
        ISaveService saveService)
    {
        _audioService = audioService;
        _saveService = saveService;
    }
}
```

장점:

- 필요한 기능만 선언함
- 구현 생성 책임은 밖으로 빠짐
- 테스트용 가짜 서비스 넣기 쉬움
- VContainer와 아주 잘 맞음

즉, DI는
"new를 여기저기 박지 않고 조립은 한 곳에서 한다"
라고 이해하면 된다.

---

## 6-5. Event Bus

### `IEventBus`

역할:

- 상태 변화 알림 전파
- 모듈 간 느슨한 연결

예시:

```csharp
public interface IEventBus
{
    void Publish<TEvent>(TEvent evt);
    void Subscribe<TEvent>(Action<TEvent> listener);
    void Unsubscribe<TEvent>(Action<TEvent> listener);
}
```

좋은 이벤트 예:

- `SettingsChangedEvent`
- `GamePausedEvent`
- `GameResumedEvent`
- `SceneLoadedEvent`
- `SaveCompletedEvent`
- `LanguageChangedEvent`

주의:

- 모든 호출을 이벤트로 만들 필요는 없다
- 직접 의존이 더 자연스러운 경우는 그냥 직접 호출이 낫다

이벤트는 "알림"에 쓰는 게 좋다.

---

## 6-6. State Flow

### `IGameState`

```csharp
public interface IGameState
{
    void Enter();
    void Exit();
}
```

### `IPayloadState<TPayload>`

```csharp
public interface IPayloadState<TPayload>
{
    void Enter(TPayload payload);
    void Exit();
}
```

### `IGameStateMachine`

```csharp
public interface IGameStateMachine
{
    void Enter<TState>() where TState : IGameState;
    void Enter<TState, TPayload>(TPayload payload)
        where TState : IPayloadState<TPayload>;
}
```

추천 기본 상태:

- `BootState`
- `TitleState`
- `LoadingState`
- `PlayState`
- `PauseState`
- `ResultState`

각 역할:

- `BootState`
  설정, 저장 데이터, 프로필, 공통 서비스 준비

- `TitleState`
  타이틀 화면 표시, 새 게임/이어하기 선택

- `LoadingState`
  로딩 화면 표시, 타겟 씬 로드

- `PlayState`
  실제 플레이 진행

- `PauseState`
  정지 상태와 일시정지 메뉴 관리

- `ResultState`
  종료 결과 화면

이 상태 머신은 재사용 가치가 매우 높다.

---

## 6-7. 공통 서비스 인터페이스

인디 스타터에서 특히 가치가 큰 서비스들이다.

### `ISettingsService`

역할:

- 옵션 데이터 보관
- 옵션 적용
- 옵션 저장

예시:

```csharp
public interface ISettingsService
{
    SettingsData Current { get; }
    void Apply(SettingsData data);
    void Save();
}
```

### `IAudioService`

역할:

- BGM / SFX 재생
- 볼륨 적용

예시:

```csharp
public interface IAudioService
{
    void PlayBgm(string id);
    void StopBgm();
    void PlaySfx(string id);
    void SetMasterVolume(float value);
    void SetBgmVolume(float value);
    void SetSfxVolume(float value);
}
```

### `ISaveService`

역할:

- 진행 저장
- 데이터 로드
- 슬롯 관리

예시:

```csharp
public interface ISaveService
{
    bool HasSave(string slotId);
    T Load<T>(string slotId) where T : new();
    void Save<T>(string slotId, T data);
    void Delete(string slotId);
}
```

### `ISceneService`

역할:

- 씬 전환
- 씬 로딩 제어

예시:

```csharp
public interface ISceneService
{
    UniTask LoadSceneAsync(string sceneName);
}
```

UniTask를 안 쓰면 `Task`로 바꾸거나 콜백 기반으로 시작해도 된다.

### `IUIService`

역할:

- 팝업 열기/닫기
- 공통 스크린 표시

예시:

```csharp
public interface IUIService
{
    void ShowScreen<TScreen>() where TScreen : IScreen;
    void HideScreen<TScreen>() where TScreen : IScreen;
    bool IsVisible<TScreen>() where TScreen : IScreen;
}
```

### `IInputService`

역할:

- 입력 모드 전환
- 입력 잠금

예시:

```csharp
public interface IInputService
{
    void EnableGameplay();
    void EnableUI();
    void DisableAll();
}
```

### `IPauseService`

역할:

- 일시정지
- 재개
- 상태 보관

예시:

```csharp
public interface IPauseService
{
    bool IsPaused { get; }
    void Pause();
    void Resume();
    void Toggle();
}
```

### `ILoadingService`

역할:

- 로딩 오버레이 표시/숨김

### `ILocalizationService`

역할:

- 언어 변경
- 텍스트 제공

### `IDebugService`

역할:

- 디버그 명령
- 치트
- 로그 도우미

---

## 7. CommonModules 구성

각 공통 모듈은 보통 아래 구성으로 생각하면 된다.

- 서비스 인터페이스
- 서비스 구현
- 데이터 구조
- 관련 이벤트
- UI Presenter 또는 View
- Installer 등록 코드

예시:

```text
CommonModules/Settings
  SettingsData.cs
  ISettingsService.cs
  SettingsService.cs
  SettingsChangedEvent.cs
  OptionsPopupPresenter.cs
```

추천 공통 모듈:

### Settings

포함:

- `SettingsData`
- `ISettingsService`
- `SettingsService`
- `SettingsChangedEvent`
- `OptionsPopupPresenter`

### Audio

포함:

- `IAudioService`
- `UnityAudioService`
- 오디오 설정 데이터
- AudioMixer 연결부

### SaveLoad

포함:

- `ISaveService`
- `JsonSaveService`
- 프로필/슬롯 데이터

### Input

포함:

- `IInputService`
- `UnityInputService`
- 액션맵 전환기

### Pause

포함:

- `IPauseService`
- `PauseService`
- PauseMenuPresenter

### Loading

포함:

- `ILoadingService`
- `LoadingService`
- LoadingOverlayView

### Localization

포함:

- `ILocalizationService`
- 언어 데이터 공급자
- `LanguageChangedEvent`

### Debug

포함:

- `IDebugService`
- 명령 등록기
- 디버그 오버레이

---

## 8. Presentation 패턴

화면 쪽은 얇게 유지하는 게 좋다.
화면은 규칙을 결정하는 곳이 아니라
"표현"하는 곳이어야 한다.

좋은 분리:

- `View`
  Unity 오브젝트 참조 담당

- `Presenter`
  UI 입력 처리 담당

- `Service`
  실제 기능 제공 담당

예시 흐름:

```text
OptionsPopupView
-> 슬라이더 입력 전달
-> OptionsPopupPresenter
-> ISettingsService 호출
-> SettingsChangedEvent 발행
-> AudioService가 이벤트를 받아 실제 볼륨 적용
```

이 구조는 인디에서 매우 재사용성이 좋다.

---

## 9. 모듈 계약

나중에 기능 모듈을 붙일 계획이면
지금부터 공통 계약을 잡아두는 게 좋다.

### `IGameModule`

```csharp
public interface IGameModule
{
    void Register(IServiceCollection services);
    void Initialize(IObjectResolver resolver);
    void Shutdown();
}
```

이 구조로 얻는 장점:

1. 항상 켜져 있는 공통 모듈 관리 가능
2. 프로젝트별 선택 모듈 추가 가능

예:

- `SettingsModule`
- `AudioModule`
- `PauseModule`
- `DialogueModule`
- `InventoryModule`

앞의 몇 개는 거의 매 프로젝트에 들어갈 것이고,
뒤의 몇 개는 게임마다 다를 수 있다.

---

## 10. 실제 스타터 조합

인디 프로젝트에서 현실적으로 추천하는 스타터 세트는 아래다.

### 항상 포함

- `GameBootstrapper`
- `FoundationInstaller`
- `CommonModulesInstaller`
- `GameInstaller`
- `EventBus`
- `GameStateMachine`
- `SettingsService`
- `AudioService`
- `SaveService`
- `SceneService`
- `UIService`
- `InputService`
- `PauseService`
- `LoadingService`

### 초반부터 있으면 좋은 것

- `LocalizationService`
- `ProfileService`
- `DebugService`

### 스타터 코어에 강제로 넣지 말 것

- 전투 공식
- 인벤토리 규칙
- AI 행동 규칙
- 퀘스트 판정 규칙
- 성장 계산

이런 것들은 `Game` 쪽에 두는 게 맞다.

---

## 11. 의존 방향 예시

좋은 의존 방향:

```text
OptionsPopupPresenter
-> ISettingsService
-> ISaveService
-> IEventBus

AudioService
-> SettingsChangedEvent를 구독

GameInstaller
-> 프로젝트 전용 모듈 등록
```

좋지 않은 방향:

```text
OptionsPopupView
-> 직접 new AudioService()
-> 직접 JSON 저장
-> 직접 씬 오브젝트 수정
```

이 방향이 어그러지면 스타터가 이식 불가능해진다.

---

## 12. DI가 실전에서 어떻게 느껴져야 하나

DI는 "여기저기 Resolve하는 마법"이 아니다.

이 스타터에서 DI는 아래처럼 느껴지면 된다.

1. Installer가 서비스 등록
2. Bootstrap이 컨테이너 생성
3. 상태, Presenter, 서비스가 필요한 의존성을 바깥에서 받음

예시:

```csharp
public sealed class OptionsPopupPresenter
{
    private readonly ISettingsService _settingsService;
    private readonly IEventBus _eventBus;

    public OptionsPopupPresenter(
        ISettingsService settingsService,
        IEventBus eventBus)
    {
        _settingsService = settingsService;
        _eventBus = eventBus;
    }

    public void ChangeMasterVolume(float value)
    {
        var data = _settingsService.Current;
        data.MasterVolume = value;
        _settingsService.Apply(data);
        _settingsService.Save();
    }
}
```

포인트:

- Presenter가 서비스 생성 안 함
- 필요한 것만 받음
- 구현이 아니라 계약에 의존함

이게 DI의 핵심이다.

나중에 VContainer를 붙이면
이 구조가 생성자 주입과 아주 자연스럽게 이어진다.

---

## 13. Service Locator 사용 규칙

Service Locator를 쓰고 싶다면
정말 짧은 줄에 묶어서 써야 한다.

허용하기 좋은 곳:

- Bootstrap 진입점
- Unity 생명주기 기반 연결 오브젝트
- 레거시 브리지

피해야 하는 곳:

- 게임 규칙
- Presenter
- Feature 서비스
- 도메인 로직

이유:

모든 곳에서 `ServiceLocator.Get<T>()`를 쓰기 시작하면
의존성이 눈에 안 보이고,
결국 전역 의존성 스파게티가 된다.

기본은 DI,
Service Locator는 예외용으로만 쓰는 게 좋다.

---

## 14. 새 프로젝트 시작용 최소 체크리스트

새 게임을 이 스타터로 시작한다면
최소한 처음에 있어야 하는 것은 아래다.

### Foundation

- `GameBootstrapper`
- `IServiceCollection`
- `IObjectResolver`
- `IEventBus`
- `IGameState`
- `IGameStateMachine`

### 공통 서비스

- `ISettingsService`
- `IAudioService`
- `ISaveService`
- `ISceneService`
- `IUIService`
- `IInputService`
- `IPauseService`
- `ILoadingService`

### 기본 상태

- `BootState`
- `TitleState`
- `LoadingState`
- `PlayState`

### Installer

- `FoundationInstaller`
- `CommonModulesInstaller`
- `GameInstaller`

이 정도면 프로젝트를 깨끗하게 시작하기에 충분하다.

---

## 15. 너한테 맞는 결론

지금 네가 원하는 건 "모든 장르를 완성해 주는 프레임워크"가 아니라
"게임 시작할 때 매번 꺼내 쓰는 공용 스타터 셸"이다.

그 관점에서는 지금 방향이 맞다.

정리하면:

- 재사용 가능한 시작 틀을 만든다
- 서비스는 인터페이스 기반으로 간다
- VContainer는 나중에 붙여도 된다
- 옵션, 사운드, 저장, 입력, 씬, 로딩, 일시정지 같은 공통 모듈은 스타터에 넣는다
- 전투나 퀘스트 같은 규칙은 게임 전용 레이어로 뺀다

즉,
"로직을 다 미리 만드는 것"이 아니라
"로직이 나중에 편하게 들어올 구조를 먼저 만드는 것"
이 맞다.

---

## 16. 다음 단계

이 문서 다음 단계는 설계 설명에서 끝내지 않고,
실제 코드 뼈대를 만드는 것이다.

추천 순서:

1. 폴더 구조 생성
2. 서비스 인터페이스 생성
3. EventBus 구현
4. GameStateMachine 구현
5. Bootstrapper 구현
6. Settings / Audio / Save 기본 서비스 구현
7. Title / Loading / Play 상태 연결

여기까지 만들면,
정말로 "다음 프로젝트에서 시작점으로 쓰는 스타터"가 된다.
