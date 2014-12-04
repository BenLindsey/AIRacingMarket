using UnityEngine;
using System.Collections;

public class TrackAreaManager : MonoBehaviour {

    public BezierSpline spline;

    public TrackArea[] trackAreas;

	// Use this for initialization
	void Start () {
        foreach (TrackArea trackArea in trackAreas) {
            trackArea.setSpline(spline);
        }
	}
	
	// Update is called once per frame
	void Update () {
	   
	}

    public TrackArea GetCurrentTrackArea(float amountRoundTrack) {

        foreach (TrackArea trackArea in trackAreas) {
            if (amountRoundTrack > trackArea.start && amountRoundTrack < trackArea.end) {
                return trackArea;
            }
        }

        return null;
    }

    public TrackArea GetNextTrackArea(float amountRoundTrack) {

        TrackArea closest = null;
        float closestDiff = 1;

        foreach (TrackArea trackArea in trackAreas) {
            float diff = trackArea.start - amountRoundTrack;

            if (diff < 0) {
                diff = 1 + diff;
            }

            if (closest == null || diff < closestDiff) {
                closest = trackArea;
                closestDiff = diff;
            }
        }

        return closest;
    }
}
