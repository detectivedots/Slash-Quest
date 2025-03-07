using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class SaveData : MonoBehaviour
{
    [SerializeField] private int saveNumber;
    public bool isNew;
    public ArrayList StateData = new ArrayList() {1, 0, 0, 0, 10f, 0f, float.MaxValue};
    public float time;
    public static SaveData Instance;

    private void Awake()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                Destroy(this.gameObject);
            }
            else
            {
                if (Instance)
                {
                    Destroy(Instance.gameObject);
                    Instance = this;
                }
            }
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
    }

    public void SetSaveNumber(int number)
    {
        this.saveNumber = number;
    }

    public void CreateNew(bool isNew)
    {
        this.isNew = isNew;
    }

    public void LoadData()
    {
        string filePath = Path.Combine(Application.dataPath, "Saves", "" + saveNumber + ".bin");
        if (isNew)
        {
            StateData = new ArrayList() {1, 0, 0, 0, 10f, 0f, float.MaxValue};
        }
        else
        {
            if (File.Exists(filePath))
            {
                FileStream fileStream = new FileStream(filePath, FileMode.Open);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                StateData = (ArrayList)binaryFormatter.Deserialize(fileStream);
                fileStream.Close();
            }
        }
        SceneManager.LoadScene(Mathf.Max((int)StateData[0], 1));
    }

    public void UpdateData(int level)
    {
        StateData[0] = level;
        StateData[2] = PlayerController.Instance.score;
        StateData[4] = PlayerController.Instance.GetComponent<DamageControl>().healthPoints;
        StateData[5] = time + (float)StateData[5];
    }

    public void SaveToFile()
    {
        if (Application.isMobilePlatform) return;
        string directoryPath = Path.Combine(Application.dataPath, "Saves");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        string filePath = Path.Combine(Application.dataPath, "Saves", "" + saveNumber + ".bin");
        FileStream fileStream = new FileStream(filePath, FileMode.Create);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(fileStream, StateData);
        fileStream.Close();
    }
}
