using UnityEngine;

public class Manager : MonoBehaviour
{
    private static Manager _instance;
    public static Manager Instance => _instance;

    public static GameManager Game => Instance._game;
    public static DataManager Data => Instance._data;
    public static GameSceneManager Scene => Instance._scene;
    public static UIManager UI => Instance._ui;
    public static BattleManager Battle => Instance._battle;
    public static SynergyManager Synergy => Instance._synergy;
    public static TooltipManager Tooltip => Instance._tooltip;

    private GameManager _game = new GameManager();
    private DataManager _data = new DataManager();
    private GameSceneManager _scene = new GameSceneManager();
    private UIManager _ui = new UIManager();
    private BattleManager _battle = new BattleManager();
    private SynergyManager _synergy = new SynergyManager();
    private TooltipManager _tooltip;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
        _data.ShowAll();
    }

    private void Init()
    {
        Game.Init();
        Data.Init();
        Scene.Init();
        Battle.Init();
        UI.Init();
        Manager.Synergy.CanvasInit(); // 반드시 먼저!
        Synergy.Init();
        GameObject tooltipPrefab = Resources.Load<GameObject>("UI/TooltipPanel");
        GameObject canvas = GameObject.Find("SynergyCanvas"); // 🎯 캔버스 오브젝트 찾기

        GameObject tooltipInstance = Object.Instantiate(tooltipPrefab, canvas.transform);
        tooltipInstance.transform.localPosition = Vector3.zero; // 또는 필요하면 초기 위치 지정

        _tooltip = tooltipInstance.GetComponent<TooltipManager>();

    }
    private void Start()
    {
        //_synergy.CanvasInit();
    }
}
