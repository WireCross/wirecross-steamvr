using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameStats 
{
    private static int roundsCompleted;

    public static int RoundsCompleted
    {
        get
        {
            return roundsCompleted;
        }
        set
        {
            roundsCompleted = value;
        }
    }

}
