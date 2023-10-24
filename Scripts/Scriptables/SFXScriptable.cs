using UnityEngine;


[CreateAssetMenu(fileName = "SFX_", menuName = "Scriptables/SFX")]
public class SFXScriptable : ScriptableObject
{
    public AudioClip sound;
    public float volume = 1;
    public float minPitch = 1;
    public float maxPitch = 1;

    public bool loop = false;
}