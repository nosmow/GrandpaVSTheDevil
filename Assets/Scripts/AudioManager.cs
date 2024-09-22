using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {  get; private set; } 

    [SerializeField] private AudioClip baseSound;
    [SerializeField] private AudioClip[] combats;

    private AudioSource audioSource;
    private Coroutine currentFadeCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void StartBaseSound(float fadeDuration = 1.0f)
    {
        if (currentFadeCoroutine != null) StopCoroutine(currentFadeCoroutine);
        currentFadeCoroutine = StartCoroutine(FadeToNewClip(baseSound, fadeDuration));
    }

    public void StartCombatSound(float fadeDuration = 1.0f)
    {
        if (currentFadeCoroutine != null) StopCoroutine(currentFadeCoroutine);
        int index = Random.Range(0, combats.Length);
        currentFadeCoroutine = StartCoroutine(FadeToNewClip(combats[index], fadeDuration));
    }

    private IEnumerator FadeToNewClip(AudioClip newClip, float fadeDuration)
    {
        if (audioSource.isPlaying)
        {
            // Fase 1: Fade out
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                audioSource.volume = 1 - (t / fadeDuration);
                yield return null;
            }
            audioSource.volume = 0;
            audioSource.Stop();
        }

        // Cambia al nuevo clip y lo inicia
        audioSource.clip = newClip;
        audioSource.Play();

        // Fase 2: Fade in
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = t / fadeDuration;
            yield return null;
        }
        audioSource.volume = 1;
    }
}
