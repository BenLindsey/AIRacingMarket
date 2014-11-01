using UnityEngine;
using System.Collections;

public class CarSounds : MonoBehaviour {

    public AudioClip engineDClip;
    public AudioClip skidClip;

    private AudioSource engineDSource;
    private AudioSource skidSource;

    private OurCar ourCar;

	// Use this for initialization
	void Start () {

        engineDSource = CreateAudioSource(engineDClip);
        skidSource = CreateAudioSource(skidClip);

        ourCar = gameObject.GetComponent<OurCar>();
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

        skidSource.volume = (ourCar.IsSkidding) ? 1 : 0;
	}
}
