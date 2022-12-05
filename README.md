# A-star-unity3d


## Instructions for Running ##

1. Locate the project folder called "AI Project.
2. Download and install unity3D from this link. https://unity3d.com/
3. Open unity and click on "OPEN" from the top right corner
4. Open the project folder in unity. N.B. you need to locate the folder not a file inside the folder. If it gives you a warning about using different versions, click continue.
5. Locate the game object called "A*" inside the Hiearchy panel on the left, and left click on it.
6. The inspector panel on the right will show the UI under a component called "Algorithm (Script)".
7. Write the paths of the files in the appropriate text fields (use "/" between directories). N.B. you can use the files already in the resources folder if you do not wish to change the connections and locations files.
8. If you wish to check/remove cities, expand the dropdown menu in the 1st box called "cities" and use the "Remove city" button to remove the city you do not want. You can also change cities locations from there and redraw using the "Draw Cities" button.
9. Click on the "DRAW CITIES" button to draw the cities and connections on the GUI.
10. Enter the route and choose a Heuristic method in the 2nd box then click "EVALUATE". N.B. you can use the Nodes dropdown menu to see more information about all the nodes that were expanded.
11. If you click "STEP" instead of "EVALUATE", you will be able to go through the search algorithm step by step
12. The route should automatically show inside the bottom box of the UI and in the GUI.
13. You can change Route or Heuristic and Evaluate again.
14. if you wish to change the locations visually, open the Cities game object in the hierarchy on the left, find and click on the city you want, a handle will appear in the UI, drag the city to the place you want and click "Draw Cities" back in the A* gameobject.

**You do not need to run/play the game for the algorithm to function**

***Source code Can be found in Assets/Scripts/Algorithm/Algorithm.cs   and    Assets/Scripts/Algorithm/Editor/AlgorithmEditor.cs***

***Algorithm.cs is the main source code file containing the full algorithm, while AlgorithmEditor.cs contains all the UI related code***
