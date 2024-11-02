using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialObjectInstance
{
    public GameObject gameObject;
    public float lightRadius;
    public float lightIntensity;
    public bool canOverbright;
    public Color lightColor;

    public SpecialObjectInstance(GameObject gameObject, float lightRadius, float lightIntensity, bool canOverbright, Color lightColor)
    {
        this.gameObject = gameObject;
        this.lightRadius = lightRadius;
        this.lightIntensity = lightIntensity;
        this.canOverbright = canOverbright;
        this.lightColor = lightColor;
    }
}

