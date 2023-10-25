using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
    This class is a singleton that handles all the audio in the game.
    It has two audio sources, one for music and one for sound effects.

    Remark: this script is real bad, sorry lol
 */

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource _musicSource, _sfxSource;

    void Awake(){
        if (Instance != null) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        Instance = this;

        if (_musicSource == null) _musicSource = gameObject.AddComponent<AudioSource>();
        if (_sfxSource == null) _sfxSource = gameObject.AddComponent<AudioSource>();
    }
    
    // ===================================================================

    public void playMusic(AudioClip clip = null){
        if (clip){
            _musicSource.clip = clip;
        }
        _musicSource.Play();
    }

    public void stopMusic(){
        _musicSource.Stop();
    }

    public void pauseMusic(){
        _musicSource.Pause();
    }

    public void playSound(AudioClip clip, float volume = 1, float pitch=1){
        _sfxSource.pitch = pitch;
        _sfxSource.PlayOneShot(clip, volume);
    }

    public void playSoundCancelling(AudioClip clip, float volume = 1, float pitch = 1){
        _sfxSource.Stop();
        _sfxSource.clip = clip;
        _sfxSource.pitch = pitch;
        _sfxSource.volume = volume;
        _sfxSource.Play();
        _sfxSource.volume = 1;
    }

    public void playLocalizedSound(AudioClip clip, Vector3 position, float volume = 1, float pitch = 1){
        if (_sfxSource.isPlaying) return;
        _sfxSource.pitch = pitch;
        _sfxSource.transform.position = position;
        _sfxSource.PlayOneShot(clip, volume);
    }

    public void stopSound(){
        _sfxSource.Stop();
    }

    // ===================================================================

    public void setSFXLoop(bool loop){
        _sfxSource.loop = loop;
    }

    public void setMusicVolume(float volume){
        _musicSource.volume = volume;
    }
    public void sendMusicToBackground(){
        _musicSource.volume = 0.15f;
    }
    public void sendMusicToFront(){
        _musicSource.volume = 0.5f;
    }

    public void muteMusicForTime(float time){
        _musicSource.volume = 0f;
        Invoke("sendMusicToBackground", time);
    }

    public void changeMusicSpeed(float speed){
        _musicSource.pitch = speed;
    }

    public void changeSFXSpeed(float speed){
        _sfxSource.pitch = speed;
    }

}
