using UnityEngine;
using System.Collections;

public class CarSounds : MonoBehaviour {

    public AudioClip engineSound;

    private AudioSource engine;

	// Use this for initialization
	void Start () {

        engine = gameObject.AddComponent<AudioSource>();
        engine.loop = true;
        engine.clip = engineSound;
        engine.volume = 0;
        engine.Play();
	}
	
	// Update is called once per frame
	void Update () {

        // Increase engine noise with speed.
        // TODO: Divide by a meaningful number.
        engine.volume = Mathf.Clamp01(rigidbody.velocity.magnitude / 100f);
	}
}
