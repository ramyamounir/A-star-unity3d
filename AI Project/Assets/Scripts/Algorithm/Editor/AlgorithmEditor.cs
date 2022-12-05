using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Algorithm)), CanEditMultipleObjects]
public class AlgorithmEditor : Editor {

    private Algorithm A;
    private bool showCities;
    private bool showNodes;
    private bool showAdvanced;
    public enum Heuristics {shortLineDistance, fewestLinks  }
    public Heuristics heuristic;
	
    void OnEnable()
    {
        A = (Algorithm)target;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Space(20);

        GUILayout.BeginVertical("Box");
        A.ConnectionsPath = EditorGUILayout.TextField("Connection Path", A.ConnectionsPath);
        A.LocationsPath = EditorGUILayout.TextField("Locations Path", A.LocationsPath);

        EditorGUI.indentLevel++;
        showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Advanced Settings");
        if (showAdvanced)
        {
            A.citiesObj = (Transform)EditorGUILayout.ObjectField("Cities", A.citiesObj, typeof(Transform), true);
            A.connectionsObj = (Transform)EditorGUILayout.ObjectField("Connections", A.connectionsObj, typeof(Transform), true);
            A.canvasObj = (Transform)EditorGUILayout.ObjectField("Canvas", A.canvasObj, typeof(Transform), true);
            A.cityPrefab = (GameObject)EditorGUILayout.ObjectField("City Sprite Prefab", A.cityPrefab, typeof(GameObject), true);
            A.linePrefab = (GameObject)EditorGUILayout.ObjectField("Line Prefab", A.linePrefab, typeof(GameObject), true);
            A.textPrefab = (GameObject)EditorGUILayout.ObjectField("Text Prefab", A.textPrefab, typeof(GameObject), true);
            A.redPrefab = (Material)EditorGUILayout.ObjectField("Red Prefab", A.redPrefab, typeof(Material), true);
            A.bluePrefab = (Material)EditorGUILayout.ObjectField("Blue Prefab", A.bluePrefab, typeof(Material), true);
            A.blackCircle = (Sprite)EditorGUILayout.ObjectField("Black Sprite", A.blackCircle, typeof(Sprite), true);
            A.greenCircle = (Sprite)EditorGUILayout.ObjectField("Green Sprite", A.greenCircle, typeof(Sprite), true);
        }
        EditorGUI.indentLevel--;

        GUILayout.BeginHorizontal();
        if (A.Cities.Count > 0) if (GUILayout.Button("Clear Cities")) { A.Cities.Clear(); A.Nodes.Clear(); A.open.Clear(); A.closed.Clear(); A.result = "";A.routeFound = false; A.RemoveCitiesPrefabs(); return; }
        if (GUILayout.Button("Load Cities from TXT")) A.Populate();
        if (A.Cities.Count > 0) if (GUILayout.Button("Draw Cities")) { A.DrawCitiesPrefabs(); }
        GUILayout.EndHorizontal();


        EditorGUI.indentLevel++;
        if (A.Cities.Count > 0)
        {
            showCities = EditorGUILayout.Foldout(showCities, "Cities: " + A.Cities.Count);
            if (showCities)
            {
                for (int i = 0; i < A.Cities.Count; i++)
                {
                    GUILayout.BeginVertical("Box");

                    GUILayout.BeginHorizontal();
                    //GUILayout.Label("(" + A.Cities[i].location[0] + "," + A.Cities[i].location[1] + ") " + "City: " + A.Cities[i].name + " -> " + PrintConnections(i));
                    GUILayout.Label("(", GUILayout.Width(7));
                    A.Cities[i].location[0] = EditorGUILayout.IntField(A.Cities[i].location[0], GUILayout.Width(50));
                    GUILayout.Label(",", GUILayout.Width(7));
                    A.Cities[i].location[1] = EditorGUILayout.IntField(A.Cities[i].location[1], GUILayout.Width(50));
                    GUILayout.Label(") " + "City: " + A.Cities[i].name + " -> " + PrintConnections(i));


                    if (GUILayout.Button("Remove City", GUILayout.Width(200))) { A.RemoveCity(i); return;}
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                }
            }
        }
        GUILayout.EndVertical();
        EditorGUI.indentLevel--;


        GUILayout.Space(20);

        //Route
        GUILayout.BeginVertical("Box");
        GUILayout.BeginHorizontal();

        GUILayout.Label("Route");
        A.begin = GUILayout.TextField(A.begin);
        GUILayout.Label(" to ");
        A.end = GUILayout.TextField(A.end);

        GUILayout.EndHorizontal();


        //Heuristic
        heuristic = (Heuristics)EditorGUILayout.EnumPopup("Heuristic",heuristic);
        if (heuristic == Heuristics.shortLineDistance) A.evaluation = false;
        else if (heuristic == Heuristics.fewestLinks) A.evaluation = true;

        // Start Button
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("RESET")) { A.Nodes.Clear();  A.open.Clear(); A.closed.Clear(); A.result = ""; A.routeFound = false; A.begin = ""; A.end = ""; A.DrawCitiesPrefabs(); return; }
        if (GUILayout.Button("EVALUATE"))
        {
            Evaluation(true);
        }
        else if (GUILayout.Button("STEP"))
        {
            Evaluation(false);
        }
        GUILayout.EndHorizontal();

        // Show the Nodes
        EditorGUI.indentLevel++;
        if (A.Nodes.Count > 0)
        {
            showNodes = EditorGUILayout.Foldout(showNodes, "Nodes: " + A.Nodes.Count);
            if (showNodes)
            {
                for (int i = 0; i < A.Nodes.Count; i++)
                {
                    GUILayout.BeginVertical("Box");
                    GUILayout.Label("Node: " + i + ", City: " + A.Nodes[i].cityType.name + ", Dist: " + A.Nodes[i].distance + ", Total: " + A.Nodes[i].total_dist);
                    if (A.Nodes[i].status) GUILayout.Label("Open");
                    GUILayout.EndVertical();
                }

            }
        }
        EditorGUI.indentLevel--;
        GUILayout.EndVertical();

        GUILayout.Space(20);

        // Show the result
        ///////////////////////////////////
        GUILayout.BeginVertical("Box");
        GUILayout.Label(A.result);
        GUILayout.EndVertical();

        //////////////////////////////////

        //GUILayout.Space(100);
        //base.OnInspectorGUI();
    }

    string PrintConnections(int index)
    {
        string result = "";
        for (int i = 0; i < A.Cities[index].connections.Count; i++)
        {
            result += A.Cities[index].connections[i];
            result += " ";
        }

        return result;
    }

    void Evaluation (bool full)
    {
        int count = 0;
        int count1 = 0;


        if (A.Cities.Count == 0)
        {
            EditorUtility.DisplayDialog("Alert", "Please load the cities", "OK");
            return;

        }
        else
        {

            if (A.begin == "" && A.end == "")
            {

                EditorUtility.DisplayDialog("Alert", "TO and FROM Cities cannot be empty", "OK");
                return;

            }
            else if (A.begin == "" && A.end != "")
            {
                EditorUtility.DisplayDialog("Alert", "TO City cannot be empty", "OK");
                return;
            }
            else if (A.begin != "" && A.end == "")
            {
                EditorUtility.DisplayDialog("Alert", "FROM City cannot be empty", "OK");
                return;
            }
            else if (A.begin != "" && A.end != "")
            {
                for (int i = 0; i < A.Cities.Count; i++)
                {

                    if (A.begin != "" && A.end != "" && A.begin == A.Cities[i].name)
                    {
                        count++;
                    }
                    if (A.begin != "" && A.end != "" && A.end == A.Cities[i].name)
                    {
                        count1++;
                    }

                }

                if (A.begin != "" && A.end != "" && count == 0 && count1 == 0)
                {
                    EditorUtility.DisplayDialog("Alert", "TO and FROM Cities are not in City list", "OK");
                    return;
                }
                else if (A.begin != "" && count == 0 && count1 != 0)
                {
                    EditorUtility.DisplayDialog("Alert", "TO City is not in City list", "OK");
                    return;
                }
                else if (A.end != "" && count1 == 0 && count != 0)
                {
                    EditorUtility.DisplayDialog("Alert", "FROM City is not in City list", "OK");
                    return;
                }
                else if (count != 0 && count1 != 0)
                {
                    if (A.begin == A.end)
                    {

                        EditorUtility.DisplayDialog("Alert", "TO and FROM City cannot be the same", "OK");
                        return;
                    }
                }

            }



        }
        if (full)
        {
            if (A.Nodes.Count > 0) { A.Nodes.Clear(); A.open.Clear(); A.closed.Clear(); A.result = ""; A.routeFound = false; A.AStar(); }
            else A.AStar();
        }
        else if (!full)
        {
            A.AStarStep();
        }
        

    }
}
