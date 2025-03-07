using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Button = UnityEngine.UIElements.Button;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Slider healthBar;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject noSoundText;
    [SerializeField] private GameObject noMusicText;
    [SerializeField] public TextMeshProUGUI errorText;

    public float time;
    private DamageControl _damageControl;
    private SaveData _saveData;

    private void Start()
    {
        _damageControl = PlayerController.Instance.GetComponent<DamageControl>();
        healthBar.maxValue = PlayerController.Instance.GetComponent<DamageControl>().maxHealth;
        _saveData = FindObjectOfType<SaveData>();
        noMusicText.SetActive(!AudioManager.Instance.isMusicOn);
        noSoundText.SetActive(!AudioManager.Instance.isSoundOn);
        Input.backButtonLeavesApp = false;
    }

    void Update()
    {
        time = _saveData.time;
        scoreText.text = "Score: " + PlayerController.Instance.score;
        float timeInMs = time * 100;
        int miliseconds = (int)timeInMs % 100;
        int seconds = (int)time % 60;
        int minutes = (int)(time / 60.0);
        timeText.text = "Time: " + minutes + ":" + seconds + ":" + miliseconds;
        healthBar.value = _damageControl.healthPoints;
    }

    public void Pause()
    { 
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }
    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SwitchMusic()
    {
        AudioManager.Instance.InvertMusicBool();
        noMusicText.SetActive(!AudioManager.Instance.isMusicOn);
    }

    public void SwitchSound()
    {
        AudioManager.Instance.InvertSoundBool();
        noSoundText.SetActive(!AudioManager.Instance.isSoundOn);
    }
    
}
