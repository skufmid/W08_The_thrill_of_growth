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

    private GameManager _game = new GameManager();
    private DataManager _data = new DataManager();
    private GameSceneManager _scene = new GameSceneManager();
    private UIManager _ui = new UIManager();
    private BattleManager _battle = new BattleManager();
    private SynergyManager _synergy = new SynergyManager();
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
    }

    private void Init()
    {
        Game.Init();
        Data.Init();
        Scene.Init();
        UI.Init();
        Battle.Init();
    }
}
