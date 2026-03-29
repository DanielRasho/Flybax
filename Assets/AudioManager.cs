using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour

{

    [SerializeField] AudioMixer audioMixer;

    private AudioSource sfxAudio => GetComponents<AudioSource>()[0];

    private AudioSource ambienceAudio => GetComponents<AudioSource>()[1];

    public static AudioManager Instance;

    public float MasterVolume
    {
        get
        {
            float vol;
            audioMixer.GetFloat("MasterVolume", out vol);
            vol = (vol + 80.0f) / 80.0f;
            return vol;
        }
    }

    public float AmbienceVolume
    {
        get
        {
            float vol;
            audioMixer.GetFloat("AmbienceVolume", out vol);
            vol = (vol + 80.0f) / 80.0f;
            return vol;
        }
    }

    public float SFXVolume
    {
        get
        {
            float vol;
            audioMixer.GetFloat("SFXVolume", out vol);
            vol = (vol + 80.0f) / 80.0f;
            return vol;
        }
    }


    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxAudio.PlayOneShot(clip, 5.0f);
    }

    public void PlayAmbience(AudioClip clip)
    {
        ambienceAudio.Stop();
        ambienceAudio.clip = clip;

        ambienceAudio.Play();
    }

    public void StopAmbience()
    {
        ambienceAudio.Stop();
    }


    public void StopSFX()
    {
        sfxAudio.Stop();
    }

    public bool IsFXPlaying(AudioClip clip)
    {
        return ambienceAudio.clip == clip && ambienceAudio.isPlaying;
    }

    public void SetMasterVolume(float vol)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Lerp(-80f, 0f, vol));
    }

    public void SetAmbienceVolume(float vol)
    {
        audioMixer.SetFloat("AmbienceVolume", Mathf.Lerp(-80f, 0f, vol));
    }

    public void SetSFXVolume(float vol)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Lerp(-80f, 0f, vol));
    }


}
