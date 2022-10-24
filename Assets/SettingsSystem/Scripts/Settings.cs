using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Settings : MonoBehaviour
{
    public GameObject settingsPanel;
    public RectTransform vibrate;
    //public RectTransform sound;
    //public RectTransform music;

    private Player character;

	public Sprite vibrateOn;
    public Sprite vibrateOff;

    /*public Sprite soundOn;
    public Sprite soundOff;*/

    /*public Sprite musicOn;
    public Sprite musicOff;*/

    //public AudioSource backgroundMusic;

    private bool isOpenSettings = false;
    //public bool isOpenMusic = true;
    //public bool isOpenSound = true;
    public bool isOpenVibrate = true;

	private void Start()
	{
        /*vibrate.anchoredPosition = settingsTarget.anchoredPosition;
        sound.anchoredPosition = settingsTarget.anchoredPosition;*/
        isOpenSettings = false;
        settingsPanel.SetActive(false);
        character = Player.player;
	}

    public void OpenCloseSettings()
	{
        if (isOpenSettings)
		{
            /*vibrate.DOAnchorPos(settingsTarget.anchoredPosition, 0.5f);
            sound.DOAnchorPos(settingsTarget.anchoredPosition, 0.5f);*/
            settingsPanel.SetActive(false);
            isOpenSettings = false;
        }
        else
		{
            /*vibrate.DOAnchorPos(vibrateTarget.anchoredPosition, 0.5f);
            sound.DOAnchorPos(soundTarget.anchoredPosition, 0.5f);*/
            settingsPanel.SetActive(true);
            isOpenSettings = true;
        }
	}

    public void OpenCloseVibrate()
	{
        if (isOpenVibrate)
		{
            character.isVibrate = false;
            vibrate.GetComponent<Image>().sprite = vibrateOff;
            isOpenVibrate = false;
		}
        else
		{
            character.isVibrate = true;
            vibrate.GetComponent<Image>().sprite = vibrateOn;
            isOpenVibrate = true;
        }

    }

    /*public void OpenCloseSound()
    {
        if (isOpenSound)
        {
            character.isSoundEffect = false;
            sound.GetComponent<Image>().sprite = soundOff;
            isOpenSound = false;
        }
        else
        {
            character.isSoundEffect = true;
            sound.GetComponent<Image>().sprite = soundOn;
            isOpenSound = true;
        }
        //Save();
    }*/

    /*public void OpenCloseMusic()
    {
        if (isOpenMusic)
        {
            music.GetComponent<Image>().sprite = musicOff;
            backgroundMusic.volume = 0;
            isOpenMusic = false;
        }
        else
        {
            music.GetComponent<Image>().sprite = musicOn;
            backgroundMusic.volume = 0.1f;
            isOpenMusic = true;
        }
        //Save();
    }*/
    /*
    private void Save()
	{
        if (isOpenMusic) PlayerPrefs.SetInt("Music", 1);
        else PlayerPrefs.SetInt("Music", 0);
    }

	private void Load()
	{
        if (PlayerPrefs.GetInt("Music") == 1)
        {
            isOpenMusic = true;
            music.GetComponent<Image>().sprite = musicOn;
        }
        else
        {
            isOpenMusic = false;
            music.GetComponent<Image>().sprite = musicOn;
        }
    }*/
}
