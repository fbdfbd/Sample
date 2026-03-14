using UnityEngine;

public sealed class GameBootstrapper : MonoBehaviour
{
    private IObjectResolver _resolver;
    private IGameStateMachine _stateMachine;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // 컨테이너는 객체 등록과 조회를 담당하는 상자입니다.
        // 이 안에 공통 서비스와 상태 머신의 생성 정보가 들어갑니다.
        SimpleContainer container = new SimpleContainer();

        FoundationInstaller foundationInstaller = new FoundationInstaller();
        CommonModulesInstaller commonModulesInstaller = new CommonModulesInstaller();
        GameInstaller gameInstaller = new GameInstaller();

        foundationInstaller.Install(container);
        commonModulesInstaller.Install(container);
        gameInstaller.Install(container);

        // 같은 container 객체를 IObjectResolver 역할로 바라보는 것입니다.
        _resolver = container;

        // Resolve는 등록된 객체를 가져오거나, 필요하면 만들어서 가져오는 것
        _stateMachine = _resolver.Resolve<IGameStateMachine>();

        // 게임 시작 시 가장 먼저 BootState로 진입합니다.
        _stateMachine.Enter<BootState>();
    }
}
