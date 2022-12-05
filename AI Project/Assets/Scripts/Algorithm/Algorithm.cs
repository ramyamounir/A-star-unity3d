using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

[System.Serializable]
public class City
{
    public string name;
    public List<string> connections = new List<string>();
    public List<int> location = new List<int>();
}

[System.Serializable]
public class Node
{
    public List<int> previousNode = new List<int>();
    public City cityType;
    public bool status;
    public float distance;
    public float estimated_dist;
    public float total_dist;

}



public class Algorithm : MonoBehaviour {


    public string ConnectionsPath = "Assets/Resources/connections.txt";
    public string LocationsPath = "Assets/Resources/locations.txt";

    public List<string[]> locations = new List<string[]>();
    public List<string[]> connections = new List<string[]>();

    public List<City> Cities = new List<City>();
    public List<Node> Nodes = new List<Node>();

    public List<int> open = new List<int>();
    public List<int> closed = new List<int>();

    public string begin;
    public string end;
    public string result;
    public bool routeFound;
    public int finalNode;
    public Transform citiesObj;
    public Transform connectionsObj;
    public Transform canvasObj;
    public GameObject cityPrefab;
    public GameObject linePrefab;
    public GameObject textPrefab;
    public Material redPrefab;
    public Material bluePrefab;
    public Sprite blackCircle;
    public Sprite greenCircle;

    //false is short distance, True is fewer links
    public bool evaluation;   

	// Use this for initialization
	void Start ()
    {
        Populate();
        
        AStar();
	}
	
    public void AStar()
    {
        // for the first node
        Initiate();

        // Search until destination city is found
        while(!routeFound)
        {
            Expand(FindNext());
            UpdateNodes();
        }
        PrintResult(finalNode);
    }

    public void AStarStep()
    {
        // for the first node
        if (Nodes.Count < 1) { Initiate(); ShowCity(Nodes[0].cityType.name); }

        // After 1st node
        else
        {
            if (routeFound)
            {
                PrintResult(finalNode);
            }
            else if (!routeFound)
            {
                Expand(FindNext());
                UpdateNodes();
                ShowResults();
            }

        }
    }

    // Add details of the start city (first node)
    void Initiate()
    {
        Nodes.Add(new Node());
        Nodes[Nodes.Count - 1].previousNode.Add(-1);
        Nodes[Nodes.Count - 1].cityType = Cities[CityIndex(begin)];
        Nodes[Nodes.Count - 1].status = true;
        Nodes[Nodes.Count - 1].distance = 0f;
        Nodes[Nodes.Count - 1].estimated_dist = (evaluation) ? CheckLinks(Cities[CityIndex(begin)].name) : CalculateDist(begin, end);
        Nodes[Nodes.Count - 1].total_dist = Nodes[Nodes.Count - 1].distance + Nodes[Nodes.Count - 1].estimated_dist;

        open.Add(Nodes.Count - 1);
    }
    // Find the open node using heuristic to be expand next
    int FindNext()
    {
        int minIndex = -1;
        float minDist = 50000f;

        for (int i = 0; i < open.Count; i++)
        {
            if(Nodes[open[i]].total_dist < minDist)
            {
                minIndex = i;
                minDist = Nodes[open[i]].total_dist;
            }

        }
        return open[minIndex];
    }

    // Expand the node and add details of that node
    void Expand(int node)
    {
        Nodes[node].status = false;
        open.Remove(node);
        closed.Add(node);
        
        for (int i = 0; i < Nodes[node].cityType.connections.Count; i++)
        {
            Nodes.Add(new Node());
            Nodes[Nodes.Count - 1].previousNode.AddRange(Nodes[node].previousNode);
            Nodes[Nodes.Count - 1].previousNode.Add(node);
            Nodes[Nodes.Count - 1].cityType = Cities[CityIndex(Nodes[node].cityType.connections[i])];
            Nodes[Nodes.Count - 1].status = true;
            Nodes[Nodes.Count - 1].distance = (evaluation)? Nodes[node].distance + 1 : Nodes[node].distance + CalculateDist(Nodes[node].cityType.name, Nodes[node].cityType.connections[i]);
            Nodes[Nodes.Count - 1].estimated_dist = (evaluation) ? CheckLinks( Cities[CityIndex(Nodes[node].cityType.connections[i])].name) : CalculateDist(Nodes[node].cityType.connections[i], end);
            Nodes[Nodes.Count - 1].total_dist = Nodes[Nodes.Count - 1].distance + Nodes[Nodes.Count - 1].estimated_dist;

            open.Add(Nodes.Count - 1);
        }

    }

    void UpdateNodes()
    {
        ////////////////////////////////////////////// CHECKS FOR PARENT/ PREVIOUSLY VISITED NODES
        for (int i = 0; i < closed.Count; i++)
        {
            for (int j = 0; j < open.Count; j++)
            {
                if (Nodes[open[j]].cityType.name == Nodes[closed[i]].cityType.name )
                {
                    Nodes[open[j]].status = false;
                    closed.Add(open[j]);
                    open.Remove(open[j]);
                }
            }
        }

        /////////////////////////////////       CHECKS TO SEE IF WE FOUND THE SHOREST ROUTE OR NOT YET

        for (int i = 0; i < open.Count; i++)
        {
            if (Nodes[open[i]].cityType.name == end)
            {

                routeFound = checkRoute(open[i]);
                if (routeFound) { finalNode = open[i]; }

            }
        }



    }
   // function to get links 1 for cities except end city
    int CheckLinks(string city)
    {
        if (city == end)
        {
            return 0;
        }

        return 1;
    }
    // Check for shortest route
    bool checkRoute(int index)
    {
        float Dist = Nodes[index].total_dist;

        for (int i = 0; i < open.Count; i++)
        {
            if (i == index) continue;
            if (Nodes[open[i]].total_dist < Dist) return false;
        }

        return true;
    }
    // Function for printing the final route
    void PrintResult(int Node)
    {
        DrawCitiesPrefabs();
        result = "";
        string connection = "";

        for (int i = 1; i < Nodes[Node].previousNode.Count; i++)
        {

            ShowCity(Nodes[Nodes[Node].previousNode[i]].cityType.name);
            ShowCity(Nodes[Node].cityType.name);

            connection = "";
            if (i < Nodes[Node].previousNode.Count-1) connection += Nodes[Nodes[Node].previousNode[i]].cityType.name + " to " + Nodes[Nodes[Node].previousNode[i+1]].cityType.name;
            else connection += Nodes[Nodes[Node].previousNode[i]].cityType.name + " to " + Nodes[Node].cityType.name;
            ShowConnection(connection);


            result += Nodes[ Nodes[Node].previousNode[i] ].cityType.name;
            result += " to ";

        }

        result += Nodes[Node].cityType.name;

    }

    // visual representation of the route
    void ShowResults()
    {
        DrawCitiesPrefabs();
        string connection = "";

        for (int i = 0; i < open.Count; i++)
        {
            for (int j = 1; j < Nodes[open[i]].previousNode.Count; j++)
            {
                ShowCity( Nodes[Nodes[open[i]].previousNode[j]].cityType.name );
                ShowCity(Nodes[open[i]].cityType.name);

                connection = "";
                if (j < Nodes[open[i]].previousNode.Count - 1) connection += Nodes[Nodes[open[i]].previousNode[j]].cityType.name + " to " + Nodes[Nodes[open[i]].previousNode[j + 1]].cityType.name;
                else connection += Nodes[Nodes[open[i]].previousNode[j]].cityType.name + " to " + Nodes[open[i]].cityType.name;
                ShowConnection(connection);
            }

        }

    }

    // Turns specified city to green
    void ShowCity(string city)
    {
        for (int i = 0; i < citiesObj.childCount; i++)
        {
            if (citiesObj.GetChild(i).gameObject.name == city)
            {
                citiesObj.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite =  greenCircle;
            }
        }

    }

    // turns connection to red
    void ShowConnection(string connection)
    {
        for (int i = 0; i < connectionsObj.childCount; i++)
        {
            if (connectionsObj.GetChild(i).gameObject.name == connection)
            {
                connectionsObj.GetChild(i).gameObject.GetComponent<LineRenderer>().sharedMaterial = redPrefab;
            }
        }
    }

    // Identify index of the city from city name
    public int CityIndex (string Name)
    {
        for (int i = 0; i < Cities.Count; i++)
        {
            if (Cities[i].name == Name) return i;
        }
        Debug.Log("City not found");
        return 1000;

    }

    // Calculate the distance between cities
    float CalculateDist(string from, string to)
    {
        float distX = Mathf.Abs(Cities[CityIndex(from)].location[0] - Cities[CityIndex(to)].location[0]);
        float distY = Mathf.Abs(Cities[CityIndex(from)].location[1] - Cities[CityIndex(to)].location[1]);
        float result = Mathf.Sqrt(Mathf.Pow(distX, 2f) + Mathf.Pow(distY, 2f));

        return result;
    }

    // Draw the cities on the plane
    public void DrawCitiesPrefabs()
    {
        RemoveCitiesPrefabs();

        GameObject city;
        GameObject Text;

        for (int i = 0; i < Cities.Count; i++)
        {
            city = Instantiate(cityPrefab, new Vector3(Cities[i].location[0], -Cities[i].location[1], -1), Quaternion.identity, citiesObj);
            city.name = Cities[i].name;

            Text = Instantiate(textPrefab, new Vector3(Cities[i].location[0], -Cities[i].location[1], -2), Quaternion.identity, canvasObj);
            Text.GetComponent<Text>().text = Cities[i].name;
            Text.name = Cities[i].name + " Text";
            
        }

        DrawConnections();

    }

    // Remove Cities and connections from GUI
    public void RemoveCitiesPrefabs()
    {
        for (int i = citiesObj.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(citiesObj.GetChild(i).gameObject);
        }

        for (int i = connectionsObj.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(connectionsObj.GetChild(i).gameObject);
        }

        for (int i = canvasObj.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(canvasObj.GetChild(i).gameObject);
        }

    }

    // Draw Connection between cities
    public void DrawConnections()
    {
        GameObject connection;
        string connectionName;
        Vector3 from, to;

        for (int i = 0; i < Cities.Count; i++)
        {
            for (int j = 0; j < Cities[i].connections.Count; j++)
            {
                connectionName = Cities[i].name + " to " +  Cities[i].connections[j];
                connection = Instantiate(linePrefab, Vector3.zero, Quaternion.identity, connectionsObj);
                connection.name = connectionName;
                from = new Vector3(Cities[i].location[0],-  Cities[i].location[1], -1);
                to = new Vector3(Cities[CityIndex(Cities[i].connections[j])].location[0], - Cities[CityIndex(Cities[i].connections[j])].location[1], -1);
                connection.GetComponent<LineRenderer>().SetPositions(new Vector3[] { from, to });


            }
        }

    }




    //Read the .txt file and store the value in the list

    public void ReadString(string path, List<string[]> list)
    {
        //Read the text from the file in the path
        StreamReader reader = new StreamReader(path);
        string[] result = new string[3];
        string mid = "";
        while (result[0] != "END")
        {
            mid = "";
            mid = reader.ReadLine();
            result = mid.Split(null);
            list.Add(result);
        }

        reader.Close();
    }
    // populate the cities with details from connection and location files
    public void Populate()
    {
        Cities.Clear();
        connections.Clear();
        locations.Clear();
        DrawCitiesPrefabs();

        ReadString(ConnectionsPath, connections);
        ReadString(LocationsPath, locations);

        int connectionIndex;

        for (int i = 0; i < locations.Count - 1; i++)
        {
            Cities.Add(new City());

            Cities[Cities.Count - 1].name = locations[i][0];
            Cities[Cities.Count - 1].location.Add(int.Parse(locations[i][1]));
            Cities[Cities.Count - 1].location.Add(int.Parse(locations[i][2]));


            connectionIndex = FindConnection(locations[i][0]);

            for (int j = 0; j < int.Parse(connections[connectionIndex][1]); j++)
            {
                Cities[Cities.Count - 1].connections.Add(connections[connectionIndex][j + 2]);
            }

        }

    }
    // Find number of connection of a city mentioned in connection file from cityname
    int FindConnection(string name)
    {
        for (int i = 0; i < connections.Count - 1; i++)
        {
            if (connections[i][0] == name) return i;
        }

        Debug.Log("City not matched");
        return 1000;
    }
    // Remove city from the City list
    public void RemoveCity(int index)
    {
        string cityName = Cities[index].name; 
        Cities.RemoveAt(index);

        for (int i = 0; i < Cities.Count; i++)
        {
            for (int j = Cities[i].connections.Count - 1; j >= 0  ; j--)
            {
                if (Cities[i].connections[j] == cityName)
                {
                    Cities[i].connections.RemoveAt(j);
                }
            }
        }

    }

}
