using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] public List<AudioSource> backgroundMusic;
    [SerializeField] public AudioSource damageSound;
    public AudioSource currentDamageSound;
    public AudioSource currentBackgroundMusic;
    public bool isMusicOn;
    public bool isSoundOn = true;

    private void Awake()
    {
        bool entered = false;
        if (Instance != null)
        {
            Instance.currentDamageSound = Instantiate(damageSound);
            Instance.currentBackgroundMusic = Instantiate(backgroundMusic[SceneManager.GetActiveScene().buildIndex]);
            isMusicOn = Instance.isMusicOn;
            isSoundOn = Instance.isSoundOn;
            if (!Instance.isMusicOn) Instance.currentBackgroundMusic.Pause();
            entered = true;
        }

        if (FindObjectsOfType(GetType()).Length > 1)
        {
            if (!isMusicOn) Instance.currentBackgroundMusic.Pause();
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            Instance.currentDamageSound = Instantiate(damageSound);
            Instance.currentBackgroundMusic =
                Instantiate(backgroundMusic[SceneManager.GetActiveScene().buildIndex]);
            if (!Instance.isMusicOn) Instance.currentBackgroundMusic.Pause();
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
    }

    public void PlayDamageSound()
    {
        if (Instance.isSoundOn)
            Instance.currentDamageSound.Play();
    }

    public void InvertMusicBool()
    {
        Instance.isMusicOn = !Instance.isMusicOn;
        if (Instance.isMusicOn) Instance.currentBackgroundMusic.Play();
        else Instance.currentBackgroundMusic.Pause();
    }

    public void InvertSoundBool()
    {
        Instance.isSoundOn = !Instance.isSoundOn;
    }
}