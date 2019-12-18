using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Emitter : MonoBehaviour
{

    public abstract void NotifySetup();
    public abstract void Next();

}
