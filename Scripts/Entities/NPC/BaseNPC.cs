using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum NPCStatus {
    Idle,

    Looking,

    WalkingToPlayer,
    WalkingBackToPosition,
    Talking,
    HandlingAction,
    WaitingForPlayerInput,
    Inactive
}

public class BaseNPC : MonoBehaviour {

    /* References */

    [SerializeField] protected NPCScriptable npcScriptable;
    [SerializeField] protected GameObject _trigger;
    private NPCScriptable _defaultScriptable;

    /* Properties */
    [SerializeField] protected NPCStatus currentStatus;
    [SerializeField] protected int currentDialogue = 0;


    /* 
    <note> 
        Thoses functions are made to be overriden in the child classes.
        They act as the "default" behaviour of the NPC.
        And should be called in the child class using base.<functionName>()
    </note>
    */
    protected virtual IEnumerator handleAction(NPCScriptable.Dialogue dialogue){
        switch (dialogue.action){

            case NPCScriptable.Action.None: 
                yield return StartCoroutine(WaitNextDialogue());
                yield break;

            case NPCScriptable.Action.AskYesNo:
                // Trick to get the value returned by the coroutine without deriving from it
                yield return StartCoroutine(WaitForPlayerYesNo((isYes) => {
                    if (isYes) {
                        npcScriptable = dialogue.nextScriptable;
                        currentDialogue = 0;
                    }
                }));
                currentStatus = NPCStatus.Talking;
                yield break;

            case NPCScriptable.Action.EndDialogue: 
                GameManager.Instance.NextGameState();
                yield return StartCoroutine(EndDialogue());
                yield return new WaitForSeconds(5f);
                Reset();
                yield break;
            
            default: yield break;
        }
    }

    /* ============================== */
    
    void Awake(){
        currentStatus = NPCStatus.Idle;
        _defaultScriptable = npcScriptable; // Keep the original one in memory
    }

    /* Trigger */
    void OnTriggerEnter(Collider other){
        if (other.CompareTag("Player") && currentStatus != NPCStatus.Inactive){
            EnterDialogue();
        }
    }

    /* Dialogues */

    protected IEnumerator DisplayNextDialogue(NPCScriptable scriptable){
        if (currentDialogue >= scriptable.dialogues.Length) yield return null;
        if (currentStatus != NPCStatus.Talking) yield return null;

        // Retrieve the current dialogue
        NPCScriptable.Dialogue dialogue = scriptable.dialogues[currentDialogue++];

        // Show it on the UI
        string inputHighlight = $"<br><color=white><size=80%><%INPUT%></size></color>";
        if (dialogue.action == NPCScriptable.Action.None) inputHighlight = inputHighlight.Replace("%INPUT%", "SPACE");
        else if (dialogue.action == NPCScriptable.Action.AskYesNo) inputHighlight = inputHighlight.Replace("%INPUT%", "Y/N");
        else inputHighlight = "";

        GameManager.Instance.dialogueText.text = $"[{npcScriptable.npcName}] {dialogue.text} {inputHighlight}";
        Debug.Log($"[{npcScriptable.npcName}] >> {dialogue.text} ({dialogue.action})");

        // Handle the action
        yield return StartCoroutine(handleAction(dialogue));
    }

    protected IEnumerator WaitNextDialogue(){
        currentStatus = NPCStatus.WaitingForPlayerInput;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        yield return new WaitForSeconds(0.5f);
        currentStatus = NPCStatus.Talking;
    }

    protected IEnumerator WaitForPlayerYesNo(System.Action<bool> callback) {
        currentStatus = NPCStatus.WaitingForPlayerInput;
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Y)){
                callback(true);
                yield break;
            }else if (Input.GetKeyDown(KeyCode.N)){
                callback(false);
                yield break;
            }
            yield return null; 
        }
    }

    
    protected virtual void EnterDialogue(){
        GameManager.Instance.SwitchGameState("Dialogue");
        _trigger.SetActive(false);
        LookAtPlayer();
        currentStatus = NPCStatus.Talking;
    }
    
    protected virtual void Reset(){
        Debug.Log("Resetting NPC");
        currentDialogue = 0;
        npcScriptable = _defaultScriptable;
        _trigger.gameObject.SetActive(true);
    }

    protected IEnumerator EndDialogue(){
        currentStatus = NPCStatus.Looking;
        AudioManager.Instance.StopSound();
        // GameManager.Instance.NextGameState();
        yield return new WaitForSeconds(2.5f);
        GameManager.Instance.dialogueText.text = "";
    }

    /* Others functions */

    protected void LookAtPlayer(){
        // TODO: Make the head move, not the whole body (body must follow if head too much to the side)
        if (GameManager.Instance.Player == null) return;
        Vector3 destination = GameManager.Instance.Player.transform.position;
        destination.y = transform.position.y;
        transform.LookAt(destination);
    }

}