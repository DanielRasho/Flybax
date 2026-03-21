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
    [SerializeField] private GameObject WinScreen; 
    [SerializeField] private GameObject LooseScreen; 

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
        UpdateScores();
        player.OnPlayerHitDanger += LooseGame;
        player.OnPlayerOnGoal += WinGame;
    }

    private void OnDestroy()
    {
        player.OnPlayerHitDanger -= LooseGame;
        player.OnPlayerOnGoal -= WinGame;
    }

    void WinGame()
    {
        WinScreen.SetActive(true);
    }

    void LooseGame()
    {
        LooseScreen.SetActive(true);
    }

    void UpdateScores()
    {
        _bouncesScoreUI.text = "Bounces: " + _totalBounces.ToString();
        _studentsScoreUI.text = "Students: " + _totalStudents.ToString();
    }
}
