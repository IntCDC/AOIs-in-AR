# AOIs in AR
This is a visualization tool to annotate gaze data from Augmented Reality (AR) scenarios to perform AOI based eye-tracking analysis. 
The visualization tool consists of a gaze replay and timeline visualization - linked together to provide spatial and image-based annotation.

![GUI](https://github.com/IntCDC/AOIs-in-AR/assets/128146104/448472f9-657a-482e-8744-34fb410c789d)

## Setup
Please install Unity 2020.3.24 to open the project.

The Unity project contains two scenes (located in Assets/Scenes):
1. TimelineVisualization
2. GazeReplay

First, drag both scenes into the hierarchy window, then unload GazeReplay. In *File/Build Settings*, the order of the *Scenes in Build* should be as follows: **TimelineVisualization 0, GazeReplay 1**. When you start the active scene (TimelineVisualization), the GazeReplay scene will be loaded as well.

## Structure
The project consists of an Assets, Packages and ProjectSettings folder. The Assets folder contains the necessary scripts, prefabs and data. Important scripts and data are listed below.

<pre>
<code>
.
└── Scenes
    |── TimelineVisualization  # Unity scene for Timeline Visualization
    |── GazeReplay  # Unity scene for Gaze Replay
└── Scripts
    |── AOI_Manager  # the code to create AOI cubes in gaze replay
    |── FileHandler  # the code to save annotated gaze data in json file
    |── Frame  # code that contains fixation information
    |── FrameAnnotator  # the code to handle fixation annotation in timeline visualization
    |── dataHandler  # the code to extract gaze data from the csv files
└── Streaming Assets
    |── frames  # thumbnail images of the fixations for each participant
    |── study_data  # fixation data of each participant
    |── RScript  # R code to extract fixations from gaze data
</code>
</pre>

## Gaze Replay
This visualization simulates the movement and gaze data of participants. Fixations can be annotated by performing spatial annotation. For this, an AOI cube is placed in the room so all fixations in the selected region are labeled with a specific AOI.

![spatialAnnotation](https://github.com/IntCDC/AOIs-in-AR/assets/128146104/83b23629-73a0-4794-bb79-12c71f1e6ec8)

## Timeline Visualization
Fixations are extracted from gaze data. In this visualization the fixations of the individual participants are shown, each fixation is represented by a thumbnail image.
Individual fixations can be annotated by clicking on one or more thumbnails. When a thumbnail is selected, the focused region is mapped in the gaze replay to show the fixation region.

![fixationbasedAnnotation](https://github.com/IntCDC/AOIs-in-AR/assets/128146104/110f6d02-8bc3-47e6-8a6d-ca2199e62b3d)


## Dataset
To conduct the pilot study, we used HoloLens2 - we created an AR scene in Unity and used the [ARETT package](https://github.com/AR-Eye-Tracking-Toolkit/ARETT) to gather eye-tracking data. In parallel, we did a video recording with the HoloLens. After the study, we extracted fixations from gaze data using the [ARETT-R package](https://github.com/AR-Eye-Tracking-Toolkit/ARETT-R-Package) and created a thumbnail image for each fixation from the videos. During the study, we also generated a spatial mapping of the environment and created a photogrammetry mesh to simulate the room in gaze replay. To avoid copyright infringement, we replaced the mesh with a basic cube. 
For more information about the study, please read our paper.

## Reference
Paper: Visual Gaze Labeling for Augmented Reality Studies
