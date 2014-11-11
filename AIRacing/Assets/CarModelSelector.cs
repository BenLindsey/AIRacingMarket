using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class CarModelSelector {

    private delegate void createCarDelegate();
    private IDictionary<string, createCarDelegate> carModels = new Dictionary<string, createCarDelegate>();
    private GameObject carObject;
    private OurCar car;

    public CarModelSelector() {
        carModels.Add("Catamount", createCatamount);
        carModels.Add("Monster Truck", createMonsterTruck);
    }

    public void createCar(string carModel, GameObject carObject) {
        this.carObject = carObject;
        car = carObject.GetComponentsInChildren<OurCar>(true)[0];
        carModels[carModel]();
    }

    private void createCatamount() {
        Debug.Log("Car set to catamount");
        // Do nothing - default car.
    }

    private void createMonsterTruck() {
        Debug.Log("Car set to monster truck");

        // Modify suspension.
        car.suspensionDistance = 1;
        car.suspensionFrontForce = 180;
        car.suspensionRearForce = 90;

        // Modify wheel size.
        List<Transform> wheels = new List<Transform>();
        getWheels(carObject.transform, wheels);

        foreach (Transform wheel in wheels) {
            wheel.localScale = new Vector3(3, 2, 2);
        }

        Vector3 com = car.transform.FindChild("CenterOfMass").localPosition;
        com.y = -2;
        car.SetCenterOfMass(com);
    }

    // Retrieves the wheels from the car object (actually the disc brakes).
    private void getWheels(Transform current, List<Transform> currentWheels) {
        if (current.name.Contains("DiscBrake")) {
            Debug.Log("Found wheel: " + current.name);
            currentWheels.Add(current);
        }

        foreach (Transform child in current) {
            getWheels(child, currentWheels);
        }
    }
}
