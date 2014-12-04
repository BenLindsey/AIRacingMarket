using UnityEngine;
using System.Collections;

public class TrackArea : MonoBehaviour {

    public bool DEBUG = true;

    public GameObject marker;

    private BezierSpline spline;

    public float start;
    public float end;

    public float cornerAmount;

    private GameObject startMarker;
    private GameObject endMarker;

    private int turnDirection;

	// Use this for initialization
	void Start () {
        if (DEBUG && marker != null) {
            startMarker = Instantiate(marker) as GameObject;
            endMarker = Instantiate(marker) as GameObject;
        }
	}

	// Update is called once per frame
	void Update () {
        if (DEBUG && spline != null && marker != null) {
            startMarker.transform.position = spline.GetPoint(start);
            endMarker.transform.position = spline.GetPoint(end);
        }
	}

    public void setSpline(BezierSpline spline) {
        this.spline = spline;
    }
}
