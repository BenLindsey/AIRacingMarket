using UnityEngine;
using System.Collections;

public class CarSounds : MonoBehaviour {

    public AudioClip engineDClip;
    public AudioClip skidClip;

    private AudioSource engineDSource;
    private AudioSource skidSource;

	// Use this for initialization
	void Start () {

        engineDSource = CreateAudioSource(engineDClip);
        skidSource = CreateAudioSource(skidClip);
	}

    private AudioSource CreateAudioSource(AudioClip clip) {

        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.loop = true;
        source.clip = clip;
        source.volume = 0;
        source.Play();

        return source;
    }
	
	// Update is called once per frame
	void Update () {

        // Increase engine noise with speed.
        // TODO: Divide by a meaningful number.
        engineDSource.volume = Mathf.Clamp01(rigidbody.velocity.magnitude / 200f);

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
