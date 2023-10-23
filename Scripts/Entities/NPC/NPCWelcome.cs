using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCWelcome : BaseNPC {

    /* References */
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;

    /* Properties */
    [SerializeField] private Vector3 _finalRotation = new Vector3(0, 120, 0), _initialPosition;

    [SerializeField] private AudioClip _sfxEnter,  _sfxTalking, _sfxLeave;
    
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _initialPosition = transform.position;
        // currentStatus = NPCStatus.Inactive;
    }

    void Update()
    {
        switch (currentStatus)
        {                  
            case NPCStatus.WalkingToPlayer: RunToPlayer(); break;

            case NPCStatus.Talking: StartCoroutine(DisplayNextDialogue(npcScriptable)); break;
            
            case NPCStatus.WalkingBackToPosition: WalkBackToPosition(); break;

            default: break;
        }
    }


    override protected IEnumerator handleAction(NPCScriptable.Dialogue dialogue){
        currentStatus = NPCStatus.HandlingAction;
        NPCScriptable.Action action = dialogue.action;
        
        switch (action)
        {
            case NPCScriptable.Action.EndDialogue: 
                yield return StartCoroutine(base.EndDialogue());
                currentStatus = NPCStatus.WalkingBackToPosition; 
                AudioManager.Instance.PlayMusic();
                break;

            default: yield return base.handleAction(dialogue); break;
        }
    }

    override protected void EnterDialogue(){
        base.EnterDialogue();
        AudioManager.Instance.PauseMusic();
        AudioManager.Instance.PlaySound(_sfxEnter);
        currentStatus = NPCStatus.WalkingToPlayer;
        GameManager.Instance.Player.transform.LookAt(transform.position);
    }

    //  ===================================================================

    void MoveToDestination(Vector3 destination){
        _animator.SetBool("isWalking", true);
        _navMeshAgent.Move(Vector3.down * 0.05f * Time.deltaTime); // Ensure gravity even if the Rigidbody is frozen
        _navMeshAgent.SetDestination(destination);
        transform.LookAt(destination);
    }
    void RunToPlayer(){
        Debug.Assert(currentStatus == NPCStatus.WalkingToPlayer, "Wrong state, can't walk to player");

        Vector3 destination = GameManager.Instance.Player.transform.position;
        MoveToDestination(destination);

        // Change agent settings to make it run, and stop not too close to the player
        _navMeshAgent.speed = 6f;
        _navMeshAgent.stoppingDistance = 7.5f;

        // If the player is close enough, stop and start talking
        float distance = Vector3.Distance(transform.position, destination);
        if (distance <= _navMeshAgent.stoppingDistance){
            currentStatus = NPCStatus.Talking;
            AudioManager.Instance.PlaySound(_sfxTalking);
            _animator.SetBool("isWalking", false);
            _animator.SetBool("isRunning", false);
        }
    }

    void WalkBackToPosition(){
        Debug.Assert(currentStatus == NPCStatus.WalkingBackToPosition, "Wrong state, can't walk back to position");

        // Makes the NPC walk back but slower (by walking)
        _navMeshAgent.stoppingDistance = 0.5f;
        _navMeshAgent.speed = 2.5f;

        MoveToDestination(_initialPosition);
        // If the NPC is close enough to the initial position, stop and desactivate
        if (_navMeshAgent.remainingDistance <= 0){
            transform.rotation = Quaternion.Euler(_finalRotation.x, _finalRotation.y, _finalRotation.z);
            _animator.SetBool("isWalking", false);
            currentStatus = NPCStatus.Inactive;
        }
    }
}