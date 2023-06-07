using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AOI_label_Datastructure
{
    public AOI_labels AOIlabels;

    public AOI_label_Datastructure()
    { }
    
}

[Serializable]
public class AOI_labels
{
    public List<AOIs> virtualAOIs;
    public List<AOIs> realAOIs;
    public AOI_labels()
    { }
}
[Serializable]
public class AOIs
{
    public string AOI_name;
    public Color color;
    public bool virtual_;
    public bool real;

    public AOIs()
    { }
    public AOIs(string AOI_name, Color color, bool virtual_, bool real)
    { 
        this.AOI_name = AOI_name;
        this.color = color;
        this.virtual_ = virtual_;
        this.real = real;
    }
}