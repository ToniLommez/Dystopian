using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager_Init : MonoBehaviour
{
    public GameObject Player;
    public GameObject CameraObject;
    public MapGenerator MapGenerator;
    public GameObject MusicManager;
    public GameObject GameOverScreen;
    public GameObject VictoryScreen;
    public GameObject PauseScreen;
    public GameObject BossManager;
    public EnemyManager EnemyManager;
    public RageService RageService;

    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            // Armazene o nome da cena atual para retornar a ela após carregar o GameManager.
            string currentSceneName = SceneManager.GetActiveScene().name;

            // Carregue a cena "Init" em modo aditivo para manter a cena atual.
            SceneManager.LoadScene("Init", LoadSceneMode.Additive);

            // Depois de carregar a cena "Init", defina a cena atual de volta como a cena ativa.
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                if (scene.name == "Init")
                {
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(currentSceneName));
                    PutValues();
                }
            };
        }
        else
        {
            Enemy.AllEnemies = new();
            PutValues();
        }
    }

    public void PutValues()
    {
        GameManager.Instance.Player = Player;
        GameManager.Instance.CameraObject = CameraObject;
        GameManager.Instance.MapGenerator = MapGenerator;
        GameManager.Instance.MusicManager = MusicManager;
        GameManager.Instance.GameOverScreen = GameOverScreen;
        GameManager.Instance.VictoryScreen = VictoryScreen;
        GameManager.Instance.PauseScreen = PauseScreen;
        GameManager.Instance.BossManager = BossManager;
        GameManager.Instance.RageService = RageService;
        GameManager.Instance.EnemyManager = EnemyManager;
        GameManager.Instance.graphUpdateTime = 3f;
        GameManager.Instance.graphUpdateTime2 = 6f;
    }
}
