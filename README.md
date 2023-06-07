# AOISs in AR
This is a visualization tool to annotate gaze data from augmented reality(AR) scenarios to perform AOI based eye-tracking analysis. 
The visualization tool consists of a gaze replay and timeline visualization that are linked together. Gaze data can be labeled in two ways using the two visualizations
The project includes a dataset that we collected from an AR pilot study.

## Gaze Replay
This visualization simulates the movement and gaze data of participants. Fixations can be annotated by performing spatial annotation. For this, an AOI cube is placed in the room so all fixations in the selected region are labeled with a specific AOI.
## Timeline Visualisierung
Fixations were extracted from the gaze data. In this visualization the fixations of the individual participants are shown, each fixation is represented by a thumbnail image.
Individual fixations can be annotated by clicking on one or more thumbnails. When a thumbnail is selected, the focused region is mapped in the gaze replay to show the fixation region.

## Setup
The Unity project contains two scenes:
- TimelineVisualization
- GazeReplay
First, open both scenes in Unity, then unload GazeReplay. When you start the active scene (TimelineVisualization), the GazeReplay scene is loaded automatically.


## Reference
Paper: Visual Gaze Labeling for Augmented Reality Studies
