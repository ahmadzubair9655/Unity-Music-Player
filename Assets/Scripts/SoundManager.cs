using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.IO;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{

    public static SoundManager Instance;
    DirectoryInfo MusicFolder;
    WWW myClip;
    string myPath;
    private GameObject[] listings;
    private int index = 0;
    public GameObject listingPrefab;
    public Transform listingsParent;
    public int songNumber = 0;
    public TextMeshProUGUI btn;

    public GameObject Play_Button;
    public GameObject Pause_Button;
    public GameObject Next_Button;
    public GameObject Previous_Button;
    public GameObject Mute_Button;
    public GameObject Unmute_Button;
    public GameObject Loop_Button;
    public GameObject Unloop_Button;
    public GameObject playMusic_Button;
    public GameObject Middle_Control_Panel;
    public GameObject NoMiddle_panel;
    public GameObject Panel;
    public AudioClip[] audioClips;

    AudioSource audioSource;

    public int currentTrack = 0;
    public Text songName;
    public Text timeText;
    int fullLength, playTime, sec, min;
    public Slider progressBar, Volume_Slider;
    public bool isPaused = false;
    // Use this for initialization


    public void Start()
    {

        Pause_Button.SetActive(false);
        Play_Button.SetActive(true);

        Mute_Button.SetActive(false);
        Unmute_Button.SetActive(true);

        Next_Button.SetActive(true);
        Previous_Button.SetActive(true);

        Loop_Button.SetActive(true);
        Unloop_Button.SetActive(false);


        Middle_Control_Panel.SetActive(false);
        NoMiddle_panel.SetActive(true);

        StartCoroutine(UpdateMusicLibrary());


        Volume_Slider.value = 0.5f;

    }


    void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
#if UNITY_ANDROID
		    myPath = "/storage/sdcard0/Music";
#endif
#if UNITY_STANDALONE
        myPath = "C:/Users/HUNNY/Music/ogg";
#endif
#if Unity_Editor
		    myPath = "Builds/Music";
#endif
        MusicFolder = new DirectoryInfo(myPath);

    }


    private IEnumerator UpdateMusicLibrary()
    {
        int length = MusicFolder.GetFiles().Length;



        audioClips = new AudioClip[length];
        listings = new GameObject[length];

        for (int i = 0; i < MusicFolder.GetFiles().Length; i++)
        {


            if (!(MusicFolder.GetFiles()[i].FullName.Contains("wav") || MusicFolder.GetFiles()[i].FullName.Contains("ogg")))
                continue;


#if UNITY_STANDALONE
            myClip = new WWW("file:///" + MusicFolder.GetFiles()[i].FullName);
#endif
#if Unity_Android
			    myClip = cashWWWInstance.GetCachedWWW("file:///" + MusicFolder.GetFiles()[i].FullName);
#endif
            songNumber++;
            GameObject obj = Instantiate(listingPrefab);

            obj.transform.SetParent(listingsParent);
            //   obj.transform.parent = listingsParent;

            obj.GetComponentInChildren<TextMeshProUGUI>().SetText(MusicFolder.GetFiles()[i].Name);
            obj.GetComponentInChildren<TextMeshProUGUI>().enableAutoSizing = true;


            obj.name = MusicFolder.GetFiles()[i].Name;

            obj.GetComponent<RectTransform>().localScale = Vector3.one;

            listings[i] = obj;

            audioClips[i] = myClip.GetAudioClip();
            audioClips[i].name = MusicFolder.GetFiles()[i].Name;
            while (audioClips[i].loadState != AudioDataLoadState.Loaded)
            {
                yield return null;
            }
        }
        audioSource.clip = audioClips[index];
        songName.text = audioClips[index].name;

    }

    public void SelectSongByName(string s)
    {
        string seting;
        for (int i = 0; i < audioClips.Length; i++)
        {


            seting = audioClips[i].name;

            if (s.Equals(seting))
            {

                playMine(i);
                break;


            }
        }


    }





    public void playMusicBtn()
    {
        play();
    }

    public void play()
    {

        Play_Button.SetActive(false);
        Pause_Button.SetActive(true);

        if (isPaused == true)
        {
            audioSource.UnPause();
            isPaused = false;
        }
        if (audioSource.isPlaying)
            return;

        currentTrack--;
        if (currentTrack < 0)
            currentTrack = audioClips.Length - 1;
        //  audioSource.Play();
        StartCoroutine("waitformusic");
    }

    IEnumerator waitformusic()
    {


        while (audioSource.isPlaying)
        {

            songName.text = audioSource.clip.name;
            playTime = (int)audioSource.time;
            yield return null;
        }

        next();
    }


    // Update is called once per frame
    void Update()
    {

        if (audioSource.isPlaying)
        {
            showTitle();
            setVol();
            showPlayTime();
            progressBar.maxValue = audioClips[currentTrack].length;
            progressBar.value = audioSource.time;
        }
    }


    public void playMine(int value)
    {
        audioSource.Stop();

        audioSource.clip = audioClips[value];
        audioSource.Play();

    }

    public void next()
    {
        audioSource.Stop();
        currentTrack++;
        if (currentTrack > audioClips.Length - 1)
            currentTrack = 0;
        audioSource.clip = audioClips[currentTrack];
        audioSource.Play();

        // show title

        StartCoroutine("waitformusic");
    }

    public void previous()
    {
        audioSource.Stop();
        currentTrack--;
        if (currentTrack < 0)
            currentTrack = audioClips.Length - 1;
        audioSource.clip = audioClips[currentTrack];
        audioSource.Play();

        // show title

        StartCoroutine("waitformusic");
    }

    public void pause()
    {

        Play_Button.SetActive(true);
        Pause_Button.SetActive(false);

        audioSource.Pause();
        isPaused = true;
        StopCoroutine("waitformusic");
    }

    public void mute()
    {
        Mute_Button.SetActive(false);
        Unmute_Button.SetActive(true);
        audioSource.mute = false;
    }

    public void unmute()
    {


        Mute_Button.SetActive(true);
        Unmute_Button.SetActive(false);
        audioSource.mute = true;
    }

    public void equalizer()
    {
        Middle_Control_Panel.SetActive(true);
        NoMiddle_panel.SetActive(false);
        HidePanel();

    }

    public void unequalizer()
    {
        Middle_Control_Panel.SetActive(false);
        NoMiddle_panel.SetActive(true);
        showPanel();
    }

    void HidePanel()
    {
        Panel.gameObject.SetActive(false);
    }

    void showPanel()
    {
        Panel.gameObject.SetActive(true);
    }

    void showTitle()
    {
        songName.text = audioSource.clip.name;
        fullLength = (int)audioSource.clip.length;
    }

    void showPlayTime()
    {
        sec = playTime % 60;
        min = (playTime / 60) % 60;

        timeText.text = min + ":" + sec.ToString("D2") + "/" + ((fullLength / 60) % 60) + ":" + (fullLength % 60).ToString("D2");
    }

    void setVol()
    {
        audioSource.volume = Volume_Slider.value;
    }


    public void quit()
    {
        Debug.Log("has quit");
        Application.Quit();
    }

    public void loopMusic()
    {
        Loop_Button.SetActive(false);
        Unloop_Button.SetActive(true);

        audioSource.loop = false;
    }

    public void unloopMusic()
    {
        Loop_Button.SetActive(true);
        Unloop_Button.SetActive(false);

        audioSource.loop = true;
    }



}

