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
        ColorDifficulty.GetPrimaryColor(),
        ColorDifficulty.GetSecondaryColor(),
        ColorDifficulty.GetTertiaryColor()
    };

    private List<int> selected = new List<int>();
    private int currentIdx = 0;

    private PostProcessVolume volume;
    private AudioSource source;
    private Vignette vig;
    private float intensity = 0;
    private float duration = 2f;
    private float startTime;

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

        for(int i=0; i < 3; i++)
        {
            selected.Add(random.Next(0, clips.Count));
        }

        source = gameObject.GetComponent<AudioSource>();
        volume = gameObject.GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out vig);        
    }

    // Update is called once per frame
    void Update()
    {
        if (hasPlayedRecently)
        {
            if(vig.intensity.value <= 0.001)
            {
                hasPlayedRecently = false;
                return;
            }

            float t = (Time.time - startTime) / duration;
            vig.intensity.value = (60f - Mathf.SmoothStep(0, 60f, t)) / 100f;
        }
        else
        {
            AudioClip clip = clips[selected[currentIdx]];
            Color color = associatedColors[currentIdx];

            source.PlayOneShot(clip);
            vig.color.value = color;
            vig.intensity.value = 0.6f;

            if (++currentIdx >= 3)
            {
                currentIdx = 0;
            }

            startTime = Time.time;
            hasPlayedRecently = true;
        }
    }

}
