using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossInfo : MonoBehaviour
{
    public int bossesKilled;

    public static BossInfo Instance;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (bossesKilled > 1)
        {
            PlayerController.Instance.score += (int)Mathf.Max(0, (60 - SaveData.Instance.time) * 5);
            SaveData.Instance.UpdateData(0);
            SaveData.Instance.StateData[3] =
                Mathf.Max((int)SaveData.Instance.StateData[3], PlayerController.Instance.score);
            SaveData.Instance.StateData[6] = Mathf.Min((float)SaveData.Instance.StateData[6],
                (float)SaveData.Instance.StateData[5]);
            SaveData.Instance.StateData[2] = 0;
            SaveData.Instance.StateData[4] = 10f;
            SaveData.Instance.StateData[5] = 0f;
            SaveData.Instance.StateData[1] = (int)SaveData.Instance.StateData[1] + 1;
            SaveData.Instance.SaveToFile();
            Destroy(SaveData.Instance);
            SceneManager.LoadScene(0);
        }
    }
}