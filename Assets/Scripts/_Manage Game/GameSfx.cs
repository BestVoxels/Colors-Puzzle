using UnityEngine;

public class GameSfx : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField]
    private Sound[] _sound;

    public static GameSfx Instance { get; private set; }
    private void Awake() => Instance = this;

    public void PlaySound(byte number)
    {
        if (SfxButton.CanPlaySfx == true)
        {
            _audioSource.volume = _sound[number].Volume;

            _audioSource.PlayOneShot(_sound[number].AudioClip);
        }
    }
}
