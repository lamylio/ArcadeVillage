using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCPlane : NPCSpeaking {

    [SerializeField]
    private Transform _playerRespawnPoint;

    // ------------------------------

    override protected IEnumerator handleAction(NPCScriptable.Dialogue dialogue){
        currentStatus = NPCStatus.HandlingAction;
        NPCScriptable.Action action = dialogue.action;
        
        switch (action)
        {
            
            case NPCScriptable.Action.ControlThePlane: 
                if (action == NPCScriptable.Action.ControlThePlane) {
                    GameManager.Instance.SwitchGameState("PlaneRace");
                    GameManager.Instance.Player.transform.position = GameManager.Instance.centerOfMap.position;
                    GameManager.Instance.Player.transform.LookAt(transform.position);
                }
                break;
            case NPCScriptable.Action.EndDialogue: 
                yield return StartCoroutine(base.EndDialogue());
                yield return new WaitForSeconds(5f); base.Reset();
                break;

            default: yield return base.handleAction(dialogue); break;
        }
    }


    // ------------------------------
}