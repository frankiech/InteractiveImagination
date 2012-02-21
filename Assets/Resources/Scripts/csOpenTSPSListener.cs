/**
 * OpenTSPS + Unity3d Extension
 * Created by James George on 11/24/2010
 * 
 * This example is distributed under The MIT License
 *
 * Copyright (c) 2010-2011 James George
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TSPS;

public class csOpenTSPSListener : MonoBehaviour {

	private Dictionary<int,GameObject> peopleCubes = new Dictionary<int,GameObject>();
	private Person oldestPerson;
	private string objectToLookAt = "SKULL";
	// private Color[] lights = {Color.white, Color.red, Color.green, Color.blue};
		
	//game engine stuff for the example
	public Material	[] materials;
	public GameObject boundingPlane; //put the people on this plane
	public GameObject personMarker; //used to represent people moving about in our example

	// Use this for initialization
	void Start () {
		//unused in example	
		Camera.main.transform.position = new Vector3(0, 2, -5);
		Camera.main.transform.LookAt(GameObject.Find(objectToLookAt).transform);
	}
	
	// Update is called once per frame
	void Update () {
		//unused in example
		Debug.Log("update called");
		if (oldestPerson != null) {
			Debug.Log("oldestPerson.id = " + oldestPerson.id);
			Camera.main.transform.position = positionForPerson(oldestPerson);
			Camera.main.transform.LookAt(GameObject.Find(objectToLookAt).transform);
		}
	}
	
	public void PersonEntered(Person person){
		// Debug.Log(" person entered with ID " + person.id);
		GameObject personObject = (GameObject)Instantiate(personMarker, positionForPerson(person), Quaternion.identity);
		// personObject.renderer.material = materials[person.id % materials.Length];
		peopleCubes.Add(person.id,personObject);
		personObject.light.color = Color.white;
	}

	public void PersonUpdated(Person person) {
		// Debug.Log("Person updated with ID " + person.id + " and person.age: " + person.age);
		if(peopleCubes.ContainsKey(person.id)){
			GameObject cubeToMove = peopleCubes[person.id];
			cubeToMove.transform.position = positionForPerson(person);
			cubeToMove.transform.LookAt(GameObject.Find(objectToLookAt).transform);
		}
		if(oldestPerson == null) {
			Debug.Log("oldest person set to " + person.id);
			oldestPerson = person;
		}
		else if(person.id < oldestPerson.id) {
				oldestPerson = person;
		}
	}

	public void PersonWillLeave(Person person){
		// Debug.Log("Person leaving with ID " + person.id);
		if(peopleCubes.ContainsKey(person.id)){
			Debug.Log("Destroying cube");
			GameObject cubeToRemove = peopleCubes[person.id];
			peopleCubes.Remove(person.id);
			//delete it from the scene	
			Destroy(cubeToRemove);
			oldestPerson = null;
		}
	}
	
	//maps the OpenTSPS coordinate system into one that matches the size of the boundingPlane
	private Vector3 positionForPerson(Person person){
		Bounds meshBounds = boundingPlane.GetComponent<MeshFilter>().sharedMesh.bounds;
		float offset = 1.0f;
		return new Vector3( (float)(.5 - person.centroidX) * meshBounds.size.x - offset, 1.0f, (float)(person.centroidY - .5) * meshBounds.size.z - offset);
	}
	
	
}
