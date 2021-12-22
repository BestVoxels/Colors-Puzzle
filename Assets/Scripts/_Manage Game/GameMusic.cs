using UnityEngine;

public class GameMusic : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField]
    private Sound[] _sound;

    public static GameMusic Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            PlaySound(0);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(byte number)
    {
        _audioSource.clip = _sound[number].AudioClip;
        _audioSource.volume = _sound[number].Volume;

        _audioSource.Play();
    }

    public void MuteState(bool status) => _audioSource.mute = status;
}