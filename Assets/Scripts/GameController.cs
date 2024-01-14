using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameState {
    None = 0,
    Idle = 1,
    Jumping = 2,
    Ending = 3
}
public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public static float score;
    public static float highestScore;
    [SerializeField] PlayerController playerController;
    [SerializeField] Ground ground;
    [SerializeField] Obstacle[] obstaclePrefabs;
    [SerializeField] float timeBtwSpawn = 3f;
    [SerializeField] float startTimeBtwSpawn = 3f;
    [SerializeField] private GameObject spawnPosition;
    [SerializeField] private Vector3 groundPos;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text highestScoreText;
    private List<Obstacle> obstacles;
    public float initialGameSpeed = 5f;
    public float gameSpeedIncrement = 0.1f;
    public float gameSpeed { get; private set; } 
    public GameState gameState;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            DestroyImmediate(gameObject);
        }
        obstacles = new List<Obstacle>();
        score = highestScore = 0;
    }

    private void Update() {
        if (gameState == GameState.Idle) {
            // input is jumping button
            if ((Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0)) && playerController.gameObject.transform.position.y <= groundPos.y) {
                gameState = GameState.Jumping;
            }

            // increase gamespeed over time and make the ground move to the left
            gameSpeed += gameSpeedIncrement * Time.deltaTime;
            ground.MovingLeft();
            for (int i = 0; i < obstacles.Count; ++i) {
                if (obstacles[i] == null)
                    continue;
                if (obstacles[i].OutOfBound())
                    Destroy(obstacles[i].gameObject);
                obstacles[i].MovingLeft();
            }

            // spawn obstacle after a fixed duration
            if (timeBtwSpawn <= 0) {
                Obstacle obs = SpawnObstacle();
                obstacles.Add(obs);
                timeBtwSpawn = startTimeBtwSpawn;
            } else {
                timeBtwSpawn -= Time.deltaTime;
            }

            // score
            score += gameSpeed * Time.deltaTime;
            scoreText.text = Mathf.FloorToInt(score).ToString("D5");
        } else if (gameState == GameState.Jumping) {
            // jumping action
            playerController.Jumping();
            gameState = GameState.Idle;
        } else if (gameState == GameState.Ending) {
            // stop the obstacles
            for (int i = 0; i < obstacles.Count; ++i) {
                if (obstacles[i] == null)
                    continue;
                obstacles[i].StopMoving();
            }
            
            // stop the player's animation
            playerController.StopMoving();

            // display gameover panel
            gameOverPanel.SetActive(true);

            // set the highest score
            highestScore = Mathf.Max(highestScore, score);
            highestScoreText.gameObject.SetActive(true);
            highestScoreText.text = "HI " + Mathf.FloorToInt(highestScore).ToString("D5");
        }
    }

    private void OnDestroy() {
        if (Instance == this)
            Instance = null;
    }

    private void Start() {
        NewGame();
    }

    private void NewGame() {
        gameSpeed = initialGameSpeed;
        gameState = GameState.Idle;
    }

    private Obstacle SpawnObstacle() {
        int r = Random.Range(0, obstaclePrefabs.Length);
        Vector3 pos = spawnPosition.transform.position;
        if (obstaclePrefabs[r].name == "Bird") 
            pos.y = -1.5f;
        Obstacle obs = Instantiate(obstaclePrefabs[r], pos, Quaternion.identity).GetComponent<Obstacle>();
        return obs;
    }

    public void Retry() {
        SceneManager.LoadScene("SampleScene");
    }
}
