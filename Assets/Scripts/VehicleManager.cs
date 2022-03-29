using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using WaypointEditor;
using Random = UnityEngine.Random;

public class VehicleManager : MonoBehaviour
{
    private float speed = (float) 5.0;
    public Transform container;
    public Transform spawner;
    public Vehicle vehiclePrefab;
    private GameObject VehicleModel;
    public static List<Vehicle> m_Vehicles = new List<Vehicle>();
    public List<GameObject> vehiclePrefabsList;
    public List<Transform> spawnerList;
    public List<Transform> pointGettersList;

    [Range(2, 5)]
    public float delay = 2;

    private Coroutine m_SpawnRoutine;
    
    public event EventHandler<VehicleEvent> VehicleDestroyed;

    public void StartSpawning()
    {
        m_SpawnRoutine = StartCoroutine(SpawnRoutine());
    }
    
    public void StopSpawning()
    {
        StopCoroutine(m_SpawnRoutine);
    }
    
    IEnumerator SpawnRoutine()
    {
        Spawn();
        delay = Random.Range(2, 5);
        yield return new WaitForSeconds(delay);
        m_SpawnRoutine = StartCoroutine(SpawnRoutine());
    }

    static IEnumerator Wait(float waitTime){
        yield return new WaitForSeconds(waitTime);
    }
    
    private void Spawn()
    {
        int randomSpawner = Random.Range(0, spawnerList.Count);
        spawner = spawnerList[randomSpawner];
        Vehicle vehicle = Instantiate(vehiclePrefab, spawner.position, Quaternion.identity);
        vehicle.origin = spawner.name;
        vehicle.transform.parent = container.transform;

        int randomPath = Random.Range(0, spawner.childCount);
        vehicle.GetComponent<NavmeshPathFollower>().startingPath = spawner.GetChild(randomPath).GetComponent<WaypointPath>();
        vehicle.GetComponent<NavMeshAgent>().speed = speed;
        
        int randomPrefab = Random.Range(0, vehiclePrefabsList.Count-1);
        VehicleModel = Instantiate(vehiclePrefabsList[randomPrefab], vehicle.transform, false);

        vehicle.Destroyable.Destroyed += DestroyedHandler;
        
        m_Vehicles.Add(vehicle);
    }

    public class RestartCarsClass {
        public void RestartCars(string spawnerName)
        {
            foreach (Vehicle vehicle in m_Vehicles)
            {
                if (vehicle.origin == spawnerName)
                {
                    Wait(2);
                    vehicle.GetComponent<NavMeshAgent>().speed = 5;
                }
            }
        }
    }
    
    public void Clear()
    {
        foreach (Vehicle vehicle in m_Vehicles)
        {
            Destroy(vehicle.gameObject);
        }
        
        m_Vehicles.Clear();
    }
    
    private void DestroyedHandler(object sender, EventArgs args)
    {
        Destructor c = sender as Destructor;
        Vehicle vehicle = c.GetComponent<Vehicle>();
        
        OnVehicleDestroyed(vehicle);
    }

    private void OnVehicleDestroyed(Vehicle vehicle)
    {
        m_Vehicles.Remove(vehicle);
        VehicleDestroyed?.Invoke(this, new VehicleEvent(vehicle));
    }
}
