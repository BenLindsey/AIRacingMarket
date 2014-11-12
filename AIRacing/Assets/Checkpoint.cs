using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

    public int checkpointNumber;

    private HUD hud;

	// Use this for initialization
	void Start () {

        // Find the HUD where ever it is on the scene.
        hud = GameObject.FindObjectOfType<HUD>();
	}

    // Simply pass the event up to the HUD, which contains all update logic.
    public void OnTriggerEnter(Collider other) {
        hud.TriggerEnter(checkpointNumber, other);
    }
	
	// Update is called once per frame
	void Update () {
	}
}
