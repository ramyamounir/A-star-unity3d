using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LocationChecker : MonoBehaviour {

    public Algorithm algo;

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {

        if (!algo) algo = GameObject.Find("A*").GetComponent<Algorithm>();
        CheckLocations();

    }

    public void CheckLocations()
    {
        int posX;
        int posY;
        int index;

        index = algo.CityIndex(gameObject.name);
        posX = algo.Cities[index].location[0];
        posY = algo.Cities[index].location[1];

        if ((transform.position.x != posX) || (transform.position.y != -posY))
        {
            algo.Cities[index].location[0] = (int)transform.position.x;
            algo.Cities[index].location[1] = -(int)transform.position.y;

        }
        
    }
}
