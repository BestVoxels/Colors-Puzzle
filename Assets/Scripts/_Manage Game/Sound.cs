using UnityEngine;

[System.Serializable]
public class Sound
{
    [SerializeField]
    private AudioClip _audioClip;
    public AudioClip AudioClip { get { return _audioClip; } }

    [SerializeField]
    [Range(0f,1f)]
    private float _volume;
    public float Volume { get { return _volume; } }
}
