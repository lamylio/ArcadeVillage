using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void PlayMusic(AudioClip clip = null){
        if (clip){
            _musicSource.clip = clip;
        }
        _musicSource.Play();
    }

    public void StopMusic(){
        _musicSource.Stop();
    }

    public void PauseMusic(){
        _musicSource.Pause();
    }

    public void PlaySound(AudioClip clip, float volume = 1, float pitch=1){
        _sfxSource.pitch = pitch;
        _sfxSource.PlayOneShot(clip, volume);
    }

    public void PlaySoundCancelling(AudioClip clip, float volume = 1, float pitch = 1){
        _sfxSource.Stop();
        _sfxSource.clip = clip;
        _sfxSource.pitch = pitch;
        _sfxSource.volume = volume;
        _sfxSource.Play();
        _sfxSource.volume = 1;
    }

    public void PlayLocalizedSound(AudioClip clip, Vector3 position, float volume = 1, float pitch = 1){
        if (_sfxSource.isPlaying) return;
        _sfxSource.pitch = pitch;
        _sfxSource.transform.position = position;
        _sfxSource.PlayOneShot(clip, volume);
    }

    public void StopSound(){
        _sfxSource.Stop();
    }

    // ===================================================================

    public void SetSFXLoop(bool loop){
        _sfxSource.loop = loop;
    }

    public void SetMusicVolume(float volume){
        _musicSource.volume = volume;
    }
    public void SendMusicToBackground(){
        _musicSource.volume = 0.15f;
    }
    public void SendMusicToFront(){
        _musicSource.volume = 0.5f;
    }

    public void MuteMusicForTime(float time){
        _musicSource.volume = 0f;
        Invoke("SendMusicToBackground", time);
    }

    public void ChangeMusicSpeed(float speed){
        _musicSource.pitch = speed;
    }

    public void ChangeSFXSpeed(float speed){
        _sfxSource.pitch = speed;
    }

}
