using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCPlane : NPCSpeaking {

    [SerializeField] private Transform _playerRespawnPoint;

    [SerializeField] private SFXScriptable _sfxStrangeScriptable;

    // ------------------------------

    override protected void enterDialogue(){
        base.enterDialogue();
        AudioManager.Instance.playSound(_sfxStrangeScriptable.sound, _sfxStrangeScriptable.volume, _sfxStrangeScriptable.minPitch);
        GameManager.Instance.Player.transform.LookAt(transform.position);
    }

    override protected IEnumerator handleAction(NPCScriptable.Dialogue dialogue){
        currentStatus = NPCStatus.HandlingAction;
        NPCScriptable.Action action = dialogue.action;
        
        switch (action)
        {
            
            case NPCScriptable.Action.ControlThePlane: 
                GameManager.Instance.switchGameState("PlaneRace");
                yield return new WaitForSeconds(1f);
                GameManager.Instance.Player.transform.position = GameManager.Instance.CenterOfMap.position; 
                yield return new WaitForSeconds(1f);
                GameManager.Instance.DialogueText.text = "";
                yield return new WaitForSeconds(10f);
                base.Reset();
                break;
            default: yield return base.handleAction(dialogue); break;
        }
    }


    // ------------------------------
}