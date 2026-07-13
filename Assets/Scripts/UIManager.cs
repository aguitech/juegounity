using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text scoreText;
    public Text highScoreText;
    public Text gameOverScoreText;
    public Text gameOverHighText;
    public GameObject menuPanel;
    public GameObject gameplayPanel;
    public GameObject gameOverPanel;
    public Button playButton;
    public Button restartButton;
    public GameManager gm;

    void Start()
    {
        if (playButton != null) playButton.onClick.AddListener(OnPlayClicked);
        if (restartButton != null) restartButton.onClick.AddListener(OnRestartClicked);
        ShowMenu();
    }

    public void OnPlayClicked()
    {
        if (gm != null) gm.StartGame();
    }

    public void OnRestartClicked()
    {
        if (gm != null) gm.RestartGame();
    }

    public void ShowMenu()
    {
        if (menuPanel != null) menuPanel.SetActive(true);
        if (gameplayPanel != null) gameplayPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (highScoreText != null) highScoreText.text = "HI: " + Mathf.FloorToInt(gm.highScore).ToString();
    }

    public void ShowGameplay()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (gameplayPanel != null) gameplayPanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    public void ShowGameOver(float score, float highScore)
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (gameOverScoreText != null) gameOverScoreText.text = "SCORE: " + Mathf.FloorToInt(score).ToString();
        if (gameOverHighText != null) gameOverHighText.text = "HI: " + Mathf.FloorToInt(highScore).ToString();
    }

    public void UpdateScore(float score)
    {
        if (scoreText != null) scoreText.text = Mathf.FloorToInt(score).ToString();
    }
}