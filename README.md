# TactiScape VR

## Quick Details
**Description:** TactiScape VR is a tool that uses simulated tactility to allow users to model 3D landscapes with their hands and gestures in virtual reality. TactiScape presents a novel Free Form Vertex Displacement (FFVD) technique that allows for real time deformation of large scale meshes using gestures and hand positioning. The accompanying research explores the impact such simulated tactility can have on the experience oif a 3D sculpting system viewed through the lenses of intuitivity, efficiency, immersion, enjoyability, etc.

## For Submission
- [Research Paper](https://github.com/AdvaitU/tactiscape-vr/blob/main/AdvaitUkidve-TactiScapeVR-ThesisThesis.pdf) - Called TactiScape VR - Creating and Studying the Effects of Simulated Tactility In 3D Sculpting through Free Form Vertex Based Displacement of Landscape Meshes In Virtual Reality
- [Video Presentation](https://www.youtube.com/watch?v=_FOe3XUQhgI) - Quick 5 minute breakdown of the tool and accompanying research.
- [Personal Logs](./Personal_Logs/Personal_Logs.md) - Contains Personal Fortnightly Logs written during the development process as asked for the submission.
- [Scripts](./Scripts/) - This folder contains all the code used to make the tool. Each scipt is documented with code commenting.
- [Assets](./Assets/) - This folder contains all the prefabs needed to run the code with in Unity

## Other Files in the Repository

- [System Overview](./Scripts/SystemOverview.png) - Shows the System Overview and contains specific intructions to recreate the tool on your own using Unity.
- [Builds](./Builds/) contains all TactiScape 1.0 builds for Windows-Mac-Linux, WebGL, and Android standalone headsets (Andropid 4.0+ only)


## How to Test TactiScape VR

### Method 1: Download a Release
- Go to [this folder](./Builds/) and choose the directory to download according to your system.
- [Windows-Mac-Linux](./Builds/Windows-Mac-Linux) works on a desktop/laptop tethered to a VR Headset. Download the directory, unzip UnityPlayerdll.zip and place it in the root, and open tactiscape-vr.exe with a VR Headset connected and Link on.
- [WebGL](./Builds/WebGL) works on the web with your own local server. Download the directory and open tactiscape-vr-1.0.exe with a VR Headset connected and Link on.
- [Android](./Builds/Android) can be installed on a standalone VR Headset (like the Meta Quest 2) using the APK file.  [**Currently unavailable due to export issues**]

### Method 2 Get it from Itch.io
- Go to the itch.io project [here](https://advaitu.itch.io/tactiscape-vr).
- Download, unzip, and run 'tactiscape-vr.exe' with a VR Headset connected and Link on.

### Method 3 Build it Yourself
- Install Unity (Recommended version: 2022 and above)
- Create a new project using the 3D URP Pipeline
- Download and import the TactiScape Unity Prefab package from the Assets folder [here](./Assets/tactiscape1.0_prefabs.unitypackage)
- In the Unity Inspector, fill in the missing references. They are named in a way that allows for quick understanding. If you need help, please email ukidveadvait.work@gmail.com.
- Run in Debug Mode or Build for Windows and follow the Step 2 from Method 2


