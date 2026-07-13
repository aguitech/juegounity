using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState { Menu, Playing, GameOver }

    [Header("Game State")]
    public GameState currentState = GameState.Menu;
    public float score = 0;
    public float highScore = 0;
    public float speed = 12f;
    public float speedIncrease = 0.3f;
    public float maxSpeed = 40f;

    [Header("References")]
    public PlayerController player;
    public SpawnManager spawnManager;
    public UIManager ui;
    public GameObject ground;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        highScore = PlayerPrefs.GetFloat("CyberDrifter_HighScore", 0);
    }

    void Start()
    {
        currentState = GameState.Menu;
        Time.timeScale = 1f;
        if (ui != null) ui.ShowMenu();
    }

    public void StartGame()
    {
        currentState = GameState.Playing;
        score = 0;
        speed = 12f;
        Time.timeScale = 1f;

        if (player != null)
        {
            player.ResetPlayer();
            player.StartRunning();
        }
        if (spawnManager != null) spawnManager.StartSpawning();
        if (ui != null) ui.ShowGameplay();
    }

    // Wrapper llamado desde HTML overlay button via JS: SendMessage('GameManager', 'StartGameFromJS')
    public void StartGameFromJS()
    {
        StartGame();
    }

    // Wrapper llamado desde HTML overlay para restart
    public void RestartGameFromJS()
    {
        RestartGame();
    }

    public void GameOver()
    {
        currentState = GameState.GameOver;
        Time.timeScale = 0.4f; // efecto cámara lenta

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetFloat("CyberDrifter_HighScore", highScore);
        }

        if (ui != null) ui.ShowGameOver(score, highScore);

        if (player != null) player.Die();

        // Notificar al HTML overlay para mostrar restart button
        #if UNITY_WEBGL && !UNITY_EDITOR
        try {
            // Application.ExternalEval ejecuta JS en el navegador
            Application.ExternalEval(string.Format(
                "if(window.parent!==window){{window.parent.postMessage({{type:'gameover',score:{0},hi:{1}}},'*');}}else{{window.postMessage({{type:'gameover',score:{0},hi:{1}}},'*');}}",
                Mathf.FloorToInt(score),
                Mathf.FloorToInt(highScore)
            ));
        } catch(System.Exception e) { Debug.Log("ExternalEval failed: " + e.Message); }
        #endif
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AddScore(float amount)
    {
        score += amount;
        if (ui != null) ui.UpdateScore(score);
    }

    void Update()
    {
        if (currentState == GameState.Playing)
        {
            score += Time.deltaTime * 10;
            if (ui != null) ui.UpdateScore(score);

            if (speed < maxSpeed)
            {
                speed += speedIncrease * Time.deltaTime;
            }

            // Mover suelo para efecto de velocidad
            if (ground != null)
            {
                ground.transform.Translate(Vector3.back * speed * Time.deltaTime);
            }
        }
    }
}