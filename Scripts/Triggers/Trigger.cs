using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trigger : MonoBehaviour
{
    protected bool active = true;    
    protected abstract void Action(GameObject player);
    protected void Deactivate(){active = false;}

    void OnTriggerEnter(Collider other){
        if (!active) return;
        if (other.gameObject == GameManager.Instance.Player) Action(player: other.gameObject);
    }
}
