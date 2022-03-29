using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleEvent : EventArgs
{
    
    public Vehicle Vehicle { get; private set; }
    
    public VehicleEvent(Vehicle vehicle)
    {
        Vehicle = vehicle;
    }
}