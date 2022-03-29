using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.AI;

public class RoadLight : MonoBehaviour
{
    public Light pointLight;
    public string Spawner;
    private VehicleManager Vehicles { get; set; }
    VehicleManager.RestartCarsClass RestartCarsClass = new VehicleManager.RestartCarsClass();
    

    // Start is called before the first frame update
    void Start()
    {
        pointLight.color = Color.red;
    }

    void OnMouseDown()
    {
        if (pointLight.color == Color.red)
        {
            pointLight.color = Color.green;
            Vehicles = GetComponent<VehicleManager>();
            RestartCarsClass.RestartCars(Spawner);
        }
        else
        {
            pointLight.color = Color.red;
        }
    }
    
}
