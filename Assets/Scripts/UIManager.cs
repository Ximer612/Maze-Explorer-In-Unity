using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _coinsText, _objectiveText, _timeText, _scoreText, _highScoreText;
    [SerializeField] private UnityEngine.UI.Image HealthBar;
    [SerializeField] private int _coins,_currentScore, _highScore,_startScore;
    [SerializeField] private float _timePassed;

    private void Start()
    {
        MazeGenerator.OnMazeGenerated += Restart;
        MazeGenerator.OnMazeGenerated += SetHighScoreText;
        PlayerManager.OnKeyTaken += () => SetObjectiveText("Find the exit door!");
        PlayerManager.OnDead += () => SetCurrentScore(_startScore);
        PlayerManager.OnCoinTaken += () => { _coins++; _coinsText.SetText("Coins: " + _coins.ToString()); };
        PlayerManager.OnTakedDamage += (float damage) => { SetHealthBarValue(HealthBar.fillAmount - 0.1f); };
        
        SetHighScoreText();
        SetCurrentScore(_startScore);
        _timePassed = 0;
    }

    private void Update()
    {
        _timePassed += Time.deltaTime;
        SetTimerText(_timePassed);

        SetCurrentScore(_startScore - ((int)_timePassed) + _coins * 10);
    }

    void SaveScore()
    {
        if(_currentScore > _highScore)
        {
            PlayerPrefs.SetInt("HighScore", _currentScore);
        }
    }

    void Restart()
    {
        SaveScore();
        _timePassed = 0;
        SetCurrentScore(_startScore);
        SetObjectiveText("Find the key!");
        SetHealthBarValue(1f);
        _coins = 0;
        _coinsText.SetText("Coins: 0");

    }

    void SetHealthBarValue(float value)
    {
        HealthBar.fillAmount = value;
    }

    void SetTimerText(float value)
    {
        _timeText.SetText("Time: " + ((int)value));
    }

    void SetCurrentScore(int value)
    {
        _currentScore = value;
        _scoreText.SetText("Score: " + _currentScore);
    }

    void SetHighScoreText()
    {
        _highScore = PlayerPrefs.GetInt("HighScore");
        _highScoreText.SetText("Highscore: " + _highScore);
    }

    void SetObjectiveText(string value)
    {
        _objectiveText.SetText("Objective: "+value);
    }

}
