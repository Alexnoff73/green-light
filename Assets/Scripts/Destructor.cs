using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Destructor : MonoBehaviour
{

    public bool autoDestroy;

    public event EventHandler<EventArgs> Destroyed;

    void OnCollisionEnter(Collision col){
        if(col.gameObject.CompareTag("Destructor")){
            OnDestroyed();
            if (autoDestroy) Destroy(gameObject);
        }
    }

    private void OnDestroyed()
    {
        Destroyed?.Invoke(this, EventArgs.Empty);
    }
}