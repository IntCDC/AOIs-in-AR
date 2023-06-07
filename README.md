# AOIs in AR
This is a visualization tool to annotate gaze data from Augmented Reality (AR) scenarios to perform AOI based eye-tracking analysis. 
The visualization tool consists of a gaze replay and timeline visualization - linked together to provide spatial and image-based annotation.

![GUI](https://github.com/IntCDC/AOIs-in-AR/assets/128146104/ad081f48-f946-47af-9ab0-19a9f04b049c)


## Setup
Please install Unity 2020.3.24 to open the project.

The Unity project contains two scenes (located in Assets/Scenes):
1. TimelineVisualization
2. GazeReplay

First, open both scenes in Unity, then unload GazeReplay. In *File/Build Settings*, the order of the *Scenes in Build* should be as follows: **TimelineVisualization 0, GazeReplay 1**. When you start the active scene (TimelineVisualization), the GazeReplay scene is loaded automatically.


## Gaze Replay
This visualization simulates the movement and gaze data of participants. Fixations can be annotated by performing spatial annotation. For this, an AOI cube is placed in the room so all fixations in the selected region are labeled with a specific AOI.

![spatialAnnotation](https://github.com/IntCDC/AOIs-in-AR/assets/128146104/1f91c15e-e4f7-4b32-b722-b56d462c9ff1)

## Timeline Visualization
Fixations are extracted from gaze data. In this visualization the fixations of the individual participants are shown, each fixation is represented by a thumbnail image.
Individual fixations can be annotated by clicking on one or more thumbnails. When a thumbnail is selected, the focused region is mapped in the gaze replay to show the fixation region.

![fixationbasedAnnotation](https://github.com/IntCDC/AOIs-in-AR/assets/128146104/a8cb583c-8a65-4c9d-9f83-ba290fe6ff68)


## Data set
To conduct the pilot study, we used HoloLens2 - we created an AR scene in Unity and used the [ARETT package](https://github.com/AR-Eye-Tracking-Toolkit/ARETT) to gather eye-tracking data. In parallel, we did a video recording with the HoloLens. After the study, we extracted fixations from gaze data using the [ARETT-R package](https://github.com/AR-Eye-Tracking-Toolkit/ARETT-R-Package) and created a thumbnail image for each fixation from the videos. During the study, we also generated a spatial mapping of the environment and created a photogrammetry mesh to simulate the room in gaze replay. To avoid copyright infringement, we replaced the mesh with a basic cube. 
For more information about the study, please read our paper.

## Reference
Paper: Visual Gaze Labeling for Augmented Reality Studies
