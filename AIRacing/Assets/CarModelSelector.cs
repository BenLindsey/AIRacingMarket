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

        // TODO: Modify wheel size.

    }
}
