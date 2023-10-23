using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class UnitPlayer : MonoBehaviour
{

    // References
    private PlayerInputs _inputs;
    private CharacterController _controller;
    private Animator _animator;
    private SFX _groundSFX;

    [Header("Movement")]
    private Vector3 currentMovement;
    [SerializeField] private bool _isWalking {
        get {
            return currentMovement.z != 0;
        }
    }

    private  bool _isRunning ;

    [SerializeField] private float _currentSpeed {
        get {
            return _isRunning ? _runSpeed : _walkSpeed;
        }
        set {
            _currentSpeed = value;
        }
    }

    [Header("Settings (speeds)")]
    [SerializeField] private float _walkSpeed = 4.0f;
    [SerializeField] private float _runSpeed = 8.0f;
    [SerializeField] private float _turnSpeed = 1.75f;

    // ===================================================================
    void Awake(){
        _inputs = new PlayerInputs();
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _groundSFX = GetComponent<SFX>();

        GameManager.OnGameStateChanged += onGameStateChanged;
    }

    void Start()
    {
        _inputs.Basic.Walk.started += onWalkInputs;
        _inputs.Basic.Walk.performed += onWalkInputs;
        _inputs.Basic.Walk.canceled += onWalkInputs;

        _inputs.Basic.Run.started += context => { 
            _isRunning = context.ReadValueAsButton();
        };

        _inputs.Basic.Run.canceled += context => { 
            _isRunning = context.ReadValueAsButton();
        };

    }

    void FixedUpdate()
    {       
        // Walking and rotation of the player (the camera follows the player, so no need to rotate the camera)
        if (_isWalking) _controller.SimpleMove(transform.TransformDirection(Vector3.forward) *   _currentSpeed * currentMovement.z);
        transform.Rotate(0, currentMovement.x * _turnSpeed, 0);

        // Animations
        _animator.SetBool("isWalking", _isWalking);
        _animator.SetBool("isRunning", _isRunning && _isWalking);
    }

    void OnDestroy() => GameManager.OnGameStateChanged -= onGameStateChanged;

    void OnEnable() => _inputs.Enable();
    void OnDisable() => _inputs.Disable();

    // ===================================================================

    void onWalkInputs(InputAction.CallbackContext context){
        Vector2 walkMovementInput = context.ReadValue<Vector2>();
        currentMovement = new Vector3(walkMovementInput.x, 0, walkMovementInput.y);
    }

    void onGameStateChanged(State newState){
        if (newState.Is("FreeWalkPlayer")) {
            _inputs.Enable();
            transform.GetChild(0).gameObject.SetActive(true);
        } else _inputs.Disable();

        if (newState.Is("PlaneRace")) transform.GetChild(0).gameObject.SetActive(false);
    }

    void OnControllerColliderHit(ControllerColliderHit hit){
        if (!_isWalking) return;
        SFXScriptable sfxs = hit.gameObject.TryGetComponent(out SFX sfx) ? sfx.SFXScriptable : _groundSFX.SFXScriptable;
        float pitch = _isRunning ? sfxs.maxPitch : sfxs.minPitch;
        AudioManager.Instance.PlayLocalizedSound(sfxs.sound, hit.point, sfxs.volume, pitch);
    }
}
