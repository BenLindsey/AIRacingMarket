using UnityEngine;
using System.Collections;

public class CarModelSelector : MonoBehaviour {

    private IDictionary<string, Func<GameObject, Void>> carModels = new Dictionary<string, Func<GameObject, Void>>();

    public CarModelSelector() {
        carModels.add("Catamount",     createCatamount);
        carModels.add("Monster Truck", createMonsterTruck);
    }

    public void createCar(string carModel, out GameObject car) {
        carModels[carmodel](car);
    }

    private void createCatamount(GameObject car) {
        Debug.log("Car set to catamount");
        // Do nothing - default car.
    }

    private void createMonsterTruck(GameObject car) {
        Debug.log("Car set to monster truck");
        GameObject actualCar = car.transform.FindChild("OurCar");
        // Do stuff here.
    }
}
