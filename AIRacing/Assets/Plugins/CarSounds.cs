using UnityEngine;
using System.Collections;

public class CarSounds : MonoBehaviour {

    public AudioClip engineSound;
    public AudioClip skidSound;

    private AudioSource engineSource;
    private AudioSource skidSource;

	// Use this for initialization
	void Start () {

        engineSource = gameObject.AddComponent<AudioSource>();
        engineSource.loop = true;
        engineSource.clip = engineSound;
        engineSource.volume = 0;
        engineSource.Play();

        skidSource = gameObject.AddComponent<AudioSource>();
        skidSource.loop = true;
        skidSource.clip = skidSound;
        skidSource.volume = 0;
        skidSource.Play();
	}
	
	// Update is called once per frame
	void Update () {

        // Increase engine noise with speed.
        // TODO: Divide by a meaningful number.
        engineSource.volume = Mathf.Clamp01(rigidbody.velocity.magnitude / 200f);

        skidSource.volume = (isSkidding()) ? 1 : 0;
	}

    private bool isSkidding() {
        foreach (WheelCollider collider
            in gameObject.GetComponentsInChildren<WheelCollider>()) {

            WheelHit hit;
            if (collider.GetGroundHit(out hit)
                && (hit.forwardSlip > 2 || hit.sidewaysSlip > 2)) {
                return true;
            }
        }

        return false;
    }
}
