using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorDifficulty 
{
    private static System.Random random = new System.Random();
    private static Color primary;
    private static bool hasInitialized = false;

    public static Color Primary {
        get
        {
            if (!hasInitialized)
            {
                switch (random.Next(0, 3)) {
                    case 0:
                        primary = new Color(1, 0, 0);
                        break;
                    case 1:
                        primary = new Color(0, 1, 0);
                        break;
                    case 2:
                        primary = new Color(0, 0, 1);
                        break;
                }
                hasInitialized = true;
            }

            return primary;
        }
        set
        {
            primary = value;
        }
    }

    // 1f is easy, 0f is impossible
    private static float difficulty = 1;

    public static Color GetPrimaryColor(float alpha = 0.5f)
    {
        return new Color(Primary.r, Primary.g, Primary.b, alpha);
    }

    public static Color GetSecondaryColor(float alpha = 0.5f)
    {
        Color tertiary = GetTertiaryColor(alpha);
        Color primary = GetPrimaryColor(alpha);

        float dr = Mathf.Max(primary.r, tertiary.r) - Mathf.Min(primary.r, tertiary.r);
        float dg = Mathf.Max(primary.g, tertiary.g) - Mathf.Min(primary.g, tertiary.g);
        float db = Mathf.Max(primary.b, tertiary.b) - Mathf.Min(primary.b, tertiary.b);

        return new Color(Mathf.Min(primary.r, tertiary.r) + (dr / 2f),
                         Mathf.Min(primary.g, tertiary.g) + (dg / 2f),
                         Mathf.Min(primary.b, tertiary.b) + (db / 2f),
                         alpha);
    }

    public static Color GetTertiaryColor(float alpha = 0.5f)
    {
        return GetInverseColor(Primary, alpha);
    }

    public static float GetDifficulty()
    {
        return difficulty;
    }

    public static void IncrementDiffAndReset()
    {
        difficulty -= 0.1f;
        hasInitialized = false;
    }

    private static Color GetInverseColor(Color color, float alpha)
    {
        Color toRet = new Color(Math.Abs(GetDifficulty() - color.r),
                                Math.Abs(GetDifficulty() - color.g),
                                Math.Abs(GetDifficulty() - color.b),
                                alpha);
        return toRet;
    }

}
