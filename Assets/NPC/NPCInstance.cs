using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInstance
{
    // Start is called before the first frame update
    public GameObject gameObject;
    public LightSource lightSource;
    public NPCInstance(GameObject gameObject, LightSource lightSource){
        this.gameObject = gameObject;
        this.lightSource = lightSource;
    }
}
