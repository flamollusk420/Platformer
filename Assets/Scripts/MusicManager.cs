using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {
    public List<AudioClip> songs;
    public List<string> songNames;
    static AudioSource audioSource;
    public string songName;
    public string soundTestSongName;
    public int songNumber;
    public int defaultSongNumber;

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    void Update() {
        //CheckSong();
        if(!audioSource.isPlaying) {
            PlaySound(songs[songNumber - 1]);
        }
        songName = songNames[songNumber - 1];
    }

    public void CheckSong(int songNumberSet) {
        songNumber = songNumberSet;
    }

    public void PlaySound(AudioClip songToPlay) {
        audioSource.PlayOneShot(songToPlay);
    }
}
