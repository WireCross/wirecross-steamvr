using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine;

public class VignetteEffect : Emitter
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
        // load piano note sounds
        clips.Add(Resources.Load<AudioClip>("PianoNotes/a-note"));
        clips.Add(Resources.Load<AudioClip>("PianoNotes/b-note"));
        clips.Add(Resources.Load<AudioClip>("PianoNotes/c-note"));
        clips.Add(Resources.Load<AudioClip>("PianoNotes/d-note"));
        clips.Add(Resources.Load<AudioClip>("PianoNotes/e-note"));
        clips.Add(Resources.Load<AudioClip>("PianoNotes/f-note"));
        clips.Add(Resources.Load<AudioClip>("PianoNotes/g-note"));

        // select 3 random piano notes to use
        for(int i=0; i < 3; i++)
        {
            selected.Add(random.Next(0, clips.Count));
        }

        // setup audio output and manipulate our post processing
        source = gameObject.GetComponent<AudioSource>();
        volume = gameObject.GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out vig);

        NotifySetup();
        Debug.Log("NotifySetup");
    }

    // Update is called once per frame
    void Update()
    {
        // check if we played recently to prevent us from spamming noises
        // and changing colors constantly
        if (hasPlayedRecently)
        {
            // if we're below a threshold with vignette, we're done
            if(vig.intensity.value <= 0.001)
            {
                hasPlayedRecently = false;
                return;
            }

            // otherwise, step our intensity down
            float t = (Time.time - startTime) / duration;
            vig.intensity.value = (60f - Mathf.SmoothStep(0, 60f, t)) / 100f;
        }
        else
        {
            AudioClip clip = clips[selected[currentIdx]];
            Color color = associatedColors[currentIdx];

            // play sound and change color to associated color
            source.PlayOneShot(clip);
            vig.color.value = color;
            vig.intensity.value = 0.6f;

            // loop back to 0 if we're done with it
            if (++currentIdx >= 3)
            {
                currentIdx = 0;
            }

            // now start timing since our last invocation
            startTime = Time.time;
            hasPlayedRecently = true;
        }
    }

    public override void Next()
    {
        return;
    }

    public override void NotifySetup()
    {
        List<Color> colors = new List<Color>();
        List<Mesh> meshes = new List<Mesh>();
        
        // add our colors to pass back
        foreach (int idx in selected)
        {
            colors.Add(associatedColors[currentIdx]);
        }

        // use cylinders since sounds don't have shapes really 
        // (we could potentially implement shapes but we'd have to use a different impl of vignette)
        colors.Add(associatedColors[0]);
        colors.Add(associatedColors[1]);
        colors.Add(associatedColors[2]);

        for (int i=0; i < 3; i++)
        {
            meshes.Add(PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Cylinder));
        }

        // pass our answers back to input
        SetupInput.input.SetupAnswers(colors.ToArray(), meshes.ToArray(), new int[] { 0, 1, 2 });
    }

}
