using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine;

public class VignetteEffect : MonoBehaviour
{
    private System.Random random = new System.Random();
    private bool hasPlayedRecently = false;

    private List<AudioClip> clips = new List<AudioClip>();

    private Color[] associatedColors =
    {
        new Color(1f, 0.647f, 0f),
        new Color(1f, 1f, 0f),
        new Color(0.502f, 0f, 0.502f),
        new Color(0f, 0f, 0.54f),
        new Color(0.54f, 0f, 0f),
        new Color(0.04f, 0.196f, 0.125f),
        new Color(0f, 1f, 0f)
    };

    private PostProcessVolume volume;
    private AudioSource source;
    private Vignette vig;
    private float intensity = 0;

    // Start is called before the first frame update
    void Start()
    {
        clips.Add(Resources.Load<AudioClip>("PianoNotes/a-note"));
        clips.Add(Resources.Load<AudioClip>("PianoNotes/b-note"));
        clips.Add(Resources.Load<AudioClip>("PianoNotes/c-note"));
        clips.Add(Resources.Load<AudioClip>("PianoNotes/d-note"));
        clips.Add(Resources.Load<AudioClip>("PianoNotes/e-note"));
        clips.Add(Resources.Load<AudioClip>("PianoNotes/f-note"));
        clips.Add(Resources.Load<AudioClip>("PianoNotes/g-note"));

        source = gameObject.GetComponent<AudioSource>();
        volume = gameObject.GetComponent<PostProcessVolume>();
        Debug.Log(volume == null);
        Debug.Log(volume.profile == null);
        volume.profile.TryGetSettings(out vig);

        
    }

    // Update is called once per frame
    void Update()
    {
        if (hasPlayedRecently)
            return;
        
        int idx = random.Next(0, clips.Count);

        AudioClip clip = clips[idx];
        Color color = associatedColors[idx];

        source.PlayOneShot(clip);
        vig.color.value = color;

        hasPlayedRecently = true;
        StartCoroutine(ResetPlayedState());
    }

    IEnumerator ResetPlayedState()
    {
        vig.intensity.value = 0.6f;
        while(vig.intensity.value > 0f)
        {
            vig.intensity.value -= 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
        hasPlayedRecently = false;
    }

}
