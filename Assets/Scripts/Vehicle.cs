using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class Vehicle: MonoBehaviour
{
    public Destructor Destroyable { get; private set; }
    public VehicleData Data { get; private set; }
    private SpriteRenderer m_renderer;
    public string origin;
    //GameManager.PointsClass PointsClass = new GameManager.PointsClass();

    void Awake()
    {
        Destroyable = GetComponent<Destructor>();
        m_renderer = GetComponent<SpriteRenderer>();
    }

    void OnCollisionEnter(Collision col){
        if(col.gameObject.CompareTag("Vehicle") && origin != col.gameObject.GetComponent<Vehicle>().origin){
            Debug.Log("game ended");
            GameManager.StopGame();
        }
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log(gameObject.tag);
        if (col.gameObject.CompareTag("Stop"))
        {
            if (col.gameObject.GetComponentInParent<RoadLight>() == null || col.gameObject.GetComponentInParent<RoadLight>().pointLight.color != Color.green)
            {
                GetComponent<NavMeshAgent>().speed = 0;
            }
        }else if(col.gameObject.CompareTag("Point"))
        {
            Debug.Log("Points Passed");
            //PointsClass.IncrementPoints();
        }
    }
}