using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundCotroller : MonoBehaviour {
    public static SoundCotroller controller;
    public static bool soundInPlace;

    public Dictionary<string, AudioClip> sounds;
    public AudioClip Song;



    public AudioClip Effect;


    AudioSource music;
    AudioSource effects;


	// Use this for initialization
	void Awake () {
        if (soundInPlace)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(this);
            soundInPlace = true;


            controller = this;
            music = gameObject.AddComponent<AudioSource>();
            effects = gameObject.AddComponent<AudioSource>();
            sounds = new Dictionary<string, AudioClip>();


            /*sounds.Add("Village", Village);
            sounds.Add("Castle", Castle);
            sounds.Add("Cave", Cave);
            sounds.Add("Houses", Houses);
            sounds.Add("Library", Library);
            sounds.Add("Plains", Plains);
            sounds.Add("Princess", Princess);
            sounds.Add("Shop", Shop);
            sounds.Add("Cottage", Cottage);

            sounds.Add("Swing", Swing);
            sounds.Add("Slime", Slime);
            sounds.Add("SlimeDeath", SlimeDeath);
            sounds.Add("SpitterDeath", SpitterDeath);
            sounds.Add("SpitterFire", SpitterFire);
            sounds.Add("FireballDeflect", FireballDeflect);
            sounds.Add("Boop", Boop);
            sounds.Add("Grass", Grass);
            */
        }
    }
    
    void Start()
    {
        

    }

    public void PlayMusic(string s)
    {
        music.clip = sounds[s];
        music.loop = true;
        music.Play();
    }

    public void setVolume(float f) {
        music.volume = f;
    }

    public void PlayEffect(string s)
    {
        effects.PlayOneShot(sounds[s]);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
