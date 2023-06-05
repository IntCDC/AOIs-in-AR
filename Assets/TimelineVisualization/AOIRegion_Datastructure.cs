using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AOIRegion_Datastructure
{
    public List<AOI_region> list_AOIregions;
    public AOIRegion_Datastructure()
    {
    }
    
}

[Serializable]
public class AOI_region
{
    public string name;
    public Vector3 position;
    public Vector3 scale;
    public Quaternion rotation;
    public bool virtual_;
    public bool real;
    public List<string> IDsInAOIregion;

    public AOI_region()
    {
    }   
    public AOI_region(string name, Vector3 position, Vector3 scale, bool virtual_, bool real) {

        this.name = name;
        this.position = position;
        this.scale = scale;
        this.virtual_ = virtual_;
        this.real = real;
    }
}
