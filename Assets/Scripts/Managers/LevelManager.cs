using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    
    private int _totalBounces; // Number of bounces of the ball on walls.
    private int _totalStudents; // Number of the students the ball was passed to.

    [SerializeField] private playerMovement player;
    [FormerlySerializedAs("BouncesScoreUI")] [SerializeField] private TMP_Text _bouncesScoreUI; 
    [FormerlySerializedAs("StudentsScoreUI")] [SerializeField] private TMP_Text _studentsScoreUI;
    [SerializeField] AudioClip ambienceClip;
    [SerializeField] private GameObject WinScreen; 
    [SerializeField] private GameObject LooseScreen; 

    private bool _gameEnded = false;

    private void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }   
    }

    void Start()
    {
        AudioManager.Instance.PlayAmbience(ambienceClip);
        Time.timeScale = 1f;
        UpdateScores();

        if (player != null)
        {
            player.OnPlayerHitDanger += LooseGame;
            player.OnPlayerOnGoal += WinGame;
            player.OnPlayerHitNewStudent += IncreaseStudentCount;
            player.OnBallBounce += IncreaseBounceCount;
        }
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnPlayerHitDanger -= LooseGame;
            player.OnPlayerOnGoal -= WinGame;
            player.OnPlayerHitNewStudent -= IncreaseStudentCount;
            player.OnBallBounce -= IncreaseBounceCount;
        }
    }

    void WinGame()
    {
        if (_gameEnded) return;
        _gameEnded = true;

        WinScreen.SetActive(true);

        if (player != null)
            player.enabled = false;

        Time.timeScale = 0f;
    }

    void LooseGame()
    {
        if (_gameEnded) return;
        _gameEnded = true;

        LooseScreen.SetActive(true);

        if (player != null)
            player.enabled = false;

        Time.timeScale = 0f;
    }

    void IncreaseStudentCount()
    {
        _totalStudents++;
        UpdateScores();
    }
    
    void IncreaseBounceCount()
    {
        _totalBounces++;
        UpdateScores();
    }

    void UpdateScores()
    {
        _bouncesScoreUI.text = "Bounces: " + _totalBounces.ToString();
        _studentsScoreUI.text = "Students: " + _totalStudents.ToString();
    }
}
