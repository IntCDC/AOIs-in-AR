using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]

public class ListFrame
{
    public List<FrameDataStructure> framesData;

    public ListFrame()
    {
    }
}
[Serializable]
public class FrameDataStructure
{
    public string participant;
    public List<Frames> frames = new List<Frames>();

    public FrameDataStructure()
    {
    }

    public FrameDataStructure(string name, List<Frames> frames)
    {
        participant = name;
        this.frames = frames;

    }
}
[Serializable]
public class Frames
{
    public string frame;
    public string ID;
    public FrameData frameData;
    public int frame_index;

    public Frames()
    {
    }

    public Frames(string frame, string ID, int frame_index, FrameData frameData)
    {
        this.frame = frame;
        this.ID = ID;
        this.frameData = frameData;
        this.frame_index = frame_index;
    }
}

[Serializable]
public class FrameData
{
    public float timestamp;
    public float fixation_duration;
    public Vector3 gazePoint;
    public Vector3 gazeScreenPoint;
    public string AOI;
    public bool is_annotated;
    public bool real;
    public bool virtual_;
    public FrameData(float timestamp, float fixation_duration, Vector3 gazePoint, Vector3 gazeScreenPoint, string AOI, bool is_annotated, bool virtual_, bool real)
    {
        this.timestamp = timestamp;
        this.fixation_duration = fixation_duration;
        this.gazePoint = gazePoint;
        this.gazeScreenPoint = gazeScreenPoint;
        this.AOI = AOI;
        this.is_annotated = is_annotated;
        this.virtual_ = virtual_;
        this.real = real;
    }
}