# BA_UnityDotsPerformance

## Setup:
1. Install Unity Hub (+ atleast Unity Version 2019.4.1f1)
2. Add the Folder '/DOTS_PerformanceDemo' as project
3. Start the project via Unity Hub
4. Select 'File'>'Open Scene'
5. Choose '/DOTS_PerformanceDemo/Scenes/SampleScene.unity'

## Usage:
Select the GameManager object in the Hierarchy and via Inspector select the desired options in the component 'Game Manager (Script)':
* Spawn Amount: number of spheres to be spawned
* Usage Type: Classic / Jobs / ECS+Jobs
* Do Pseudo Calculations: add some additional calculations to the movement to simulate more challenging tasks
