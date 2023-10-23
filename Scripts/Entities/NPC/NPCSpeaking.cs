using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


/* Class for WIP / unused NPCs that still need to be able to talk */
public class NPCSpeaking : BaseNPC {

    // ------------------------------

    void Start(){
        currentStatus = NPCStatus.Looking;
    }

    void Update()
    {
        switch (currentStatus)
        {                  
        
            case NPCStatus.Looking: LookAtPlayer(); break;
            case NPCStatus.Talking: StartCoroutine(DisplayNextDialogue(npcScriptable)); break;
        
            default: break;
        }
    }

}