using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCPlane : NPCSpeaking {

    [SerializeField]
    private Transform _playerRespawnPoint;

    [SerializeField] private SFXScriptable _sfxStrangeScriptable;

    // ------------------------------

    override protected void EnterDialogue(){
        base.EnterDialogue();
        AudioManager.Instance.PlaySound(_sfxStrangeScriptable.sound, _sfxStrangeScriptable.volume, _sfxStrangeScriptable.minPitch);
        GameManager.Instance.Player.transform.LookAt(transform.position);
    }

    override protected IEnumerator handleAction(NPCScriptable.Dialogue dialogue){
        currentStatus = NPCStatus.HandlingAction;
        NPCScriptable.Action action = dialogue.action;
        
        switch (action)
        {
            
            case NPCScriptable.Action.ControlThePlane: 
                GameManager.Instance.SwitchGameState("PlaneRace");
                yield return new WaitForSeconds(1f);
                GameManager.Instance.Player.transform.position = GameManager.Instance.centerOfMap.position;
                // GameManager.Instance.Player.transform.LookAt(transform.position); 
                GameManager.Instance.dialogueText.text = "";
                break;
            default: yield return base.handleAction(dialogue); break;
        }
    }


    // ------------------------------
}