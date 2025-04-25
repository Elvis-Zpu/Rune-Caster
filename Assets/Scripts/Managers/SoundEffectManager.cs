using UnityEngine;
using System.Collections.Generic;

public class SoundEffectManager : MonoBehaviour
{
    [System.Serializable]
    public class SoundEffect
    {
        public string effectName;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.5f, 1.5f)] public float pitch = 1f;
    }
    
    [SerializeField] private List<SoundEffect> soundEffects = new List<SoundEffect>();
    [SerializeField] private int audioSourcePoolSize = 5;
    
    private Dictionary<string, SoundEffect> effectMap = new Dictionary<string, SoundEffect>();
    private AudioSource[] audioSourcePool;
    private int currentSourceIndex = 0;
    
    private void Awake()
    {
        // Initialize the dictionary
        foreach (var effect in soundEffects)
        {
            if (!string.IsNullOrEmpty(effect.effectName) && effect.clip != null)
            {
                effectMap[effect.effectName] = effect;
            }
        }
        
        // Set up audio source pool
        audioSourcePool = new AudioSource[audioSourcePoolSize];
        for (int i = 0; i < audioSourcePoolSize; i++)
        {
            GameObject sourceObj = new GameObject($"AudioSource_{i}");
            sourceObj.transform.parent = transform;
            audioSourcePool[i] = sourceObj.AddComponent<AudioSource>();
            audioSourcePool[i].playOnAwake = false;
        }
    }
    
    public void PlaySound(string effectName)
    {
        if (effectMap.TryGetValue(effectName, out SoundEffect effect))
        {
            AudioSource source = GetNextAudioSource();
            source.clip = effect.clip;
            source.volume = effect.volume;
            source.pitch = effect.pitch;
            source.Play();
        }
        else
        {
            Debug.LogWarning($"Sound effect '{effectName}' not found");
        }
    }
    
    public void PlaySoundAtPosition(string effectName, Vector3 position)
    {
        if (effectMap.TryGetValue(effectName, out SoundEffect effect))
        {
            AudioSource source = GetNextAudioSource();
            source.transform.position = position;
            source.clip = effect.clip;
            source.volume = effect.volume;
            source.pitch = effect.pitch;
            source.spatialBlend = 1f; // Full 3D
            source.Play();
        }
        else
        {
            Debug.LogWarning($"Sound effect '{effectName}' not found");
        }
    }
    
    private AudioSource GetNextAudioSource()
    {
        // Get the next available audio source in the pool
        AudioSource source = audioSourcePool[currentSourceIndex];
        currentSourceIndex = (currentSourceIndex + 1) % audioSourcePoolSize;
        return source;
    }
}
