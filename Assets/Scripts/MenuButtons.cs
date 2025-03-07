using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    [SerializeField] private Button[] loadButtons;
    [SerializeField] private Button[] newButtons;
    [SerializeField] private GameObject noMusicText;
    
    void Awake()
    {
        Input.backButtonLeavesApp = true;
    }

    private void Start()
    {
        FillButtons();
        noMusicText.SetActive(!AudioManager.Instance.isMusicOn);
    }

    private void FillButtons()
    {
        for (var i = 0; i < loadButtons.Length; i++)
        {
            string filePath = Path.Combine(Application.dataPath, "Saves", "" + i + ".bin");
            if (File.Exists(filePath))
            {
                FileStream fileStream = new FileStream(filePath, FileMode.Open);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                ArrayList playerData =  (ArrayList)binaryFormatter.Deserialize(fileStream);
                fileStream.Close();
                float time = (float)playerData[5];
                float timeInMs = time * 100;
                int miliseconds = (int)timeInMs % 100;
                int seconds = (int)time % 60;
                int minutes = (int)(time / 60.0);
                string totalTime = "" + minutes + ":" + seconds + ":" + miliseconds;
                time = (float)playerData[6];
                timeInMs = time * 100;
                miliseconds = (int)timeInMs % 100;
                seconds = (int)time % 60;
                minutes = (int)(time / 60.0);
                string totalBestTime = "" + minutes + ":" + seconds + ":" + miliseconds;
                if ((float)playerData[6] > float.MaxValue/2)
                {
                    totalBestTime = "No";
                }
                loadButtons[i].GetComponentInChildren<TextMeshProUGUI>().text =
                    newButtons[i].GetComponentInChildren<TextMeshProUGUI>().text =
                        "Current Level: "+ playerData[0] +", Games Completed: "+playerData[1]+"\n" +
                        "Score:"+ playerData[2] +", Highest Score: "+playerData[3]+", Health: "+playerData[4]+"\n" +
                        "Total time:"+totalTime+", Best total time: "+totalBestTime+"";
            }
            else
            {
                loadButtons[i].GetComponentInChildren<TextMeshProUGUI>().text =
                    newButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = "Empty";
                loadButtons[i].GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
                newButtons[i].GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
                loadButtons[i].transform.GetChild(1).gameObject.SetActive(false);
                newButtons[i].transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    public void OnStartGame()
    {
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
    }

    public void StartGameIfMobile()
    {
        if (Application.isMobilePlatform)
        {
            SceneManager.LoadScene(1);
        }
    }

    public void OnExit()
    {
        Application.Quit();
    }
    
    public void SwitchMusic()
    {
        AudioManager.Instance.InvertMusicBool();
        noMusicText.SetActive(!AudioManager.Instance.isMusicOn);
    }


}
