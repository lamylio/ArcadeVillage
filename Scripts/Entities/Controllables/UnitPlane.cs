using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class UnitPlane : MonoBehaviour
{

    private PlayerInputs _inputs;

    // --- References ---
    [SerializeField, Header("References")] private Rigidbody _rigidbody;
    [SerializeField] private GameObject _camera, _propeller, _rearWheel;
    [SerializeField] private SFXScriptable _sfxEngineRunningScriptable;
    
    [SerializeField] private Transform _ring;

    // --- Settings ---
    [SerializeField, Header("Settings")]    
    private float _maxSpeed = 150f;
    [SerializeField]
    private float _pitchMultipler = 1f, _yawMultipler = 1f, _rollMultipler = 1f, _liftMultipler = 10f, _thrustDecreaseRatio = 0.09f, _thrustIncreaseRatio = 0.002f;


    // --- Inputs ---
    [SerializeField, Space(5), Header("Inputs and current values")]
    private bool _throttle;
    [SerializeField]
    private float _pitch, _yaw, _roll, _thrust = 1f, _speedkph;
    

    // --- Calculated values ---
    private float _maxThrust => _maxSpeed / 100f;
    private float _thrustDelta => _maxThrust * _thrustIncreaseRatio;
    private float _speed => _speedkph / 100f;

    // --- Others ---
    private LineRenderer _lineRenderer;
    private int _numSegments = 5;


    /* ------ Monobehaviour functions ------ */
   
    void Awake(){
        _inputs = new PlayerInputs();
        _rigidbody = GetComponent<Rigidbody>();
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = _numSegments + 1;

        GameManager.OnGameStateChanged += onGameStateChanged;
    }

    void Start()
    {
        _inputs.Plane.Flight.started += onFlightInputs;
        _inputs.Plane.Flight.performed += onFlightInputs;
        _inputs.Plane.Flight.canceled += onFlightInputs;

        _inputs.Plane.Throttle.started += context => { 
            _throttle = context.ReadValueAsButton();
        };

        _inputs.Plane.Throttle.canceled += context => { 
            _throttle = context.ReadValueAsButton();
        };

        // TODO: Brake system
        StartCoroutine(calculateSpeed()); 
    }

    void Update(){
        _thrust += _throttle ? _thrustDelta : -_thrustDelta * _thrustDecreaseRatio;
        _thrust = Mathf.Clamp(_thrust, 0f, _maxThrust);
    }

    void FixedUpdate(){

        if (!_inputs.Plane.Flight.enabled) return;

        /* 
        <note> 
            I could have use the .AddForce and Torque functions, 
            but then the controlls seems kinda weird and less reactive to the user inputs.
            So I decided to use the .Translate and .Rotate functions instead.

            Please excuse me for the bad simulation of the physics and controls, lol.
        </note>
        */

        float pitchTorque = _pitch * _pitchMultipler;
        float yawTorque = _yaw * _yawMultipler * -1f;
        float rollTorque = _roll * _rollMultipler * -1f; 
        float forwardForce = _thrust;
 
        // Force
        _rigidbody.transform.Translate(Vector3.forward * forwardForce);
        _rigidbody.transform.Translate(Vector3.up * _liftMultipler * _speed * Time.deltaTime);
        _rigidbody.transform.Translate(Vector3.down * 9.81f * Time.deltaTime, Space.World);
        if (!_throttle) _rigidbody.transform.Translate(Vector3.forward * _speed * 0.999f * Time.deltaTime);
        
        // Torque
        _rigidbody.transform.Rotate(Vector3.forward, rollTorque);
        _rigidbody.transform.Rotate(Vector3.right, pitchTorque);
        _rigidbody.transform.Rotate(Vector3.up, yawTorque);


        // Propeller animation
        _propeller.transform.Rotate(Vector3.forward, Time.deltaTime * _speedkph * 100f);

        // Wheel animation (deprecated)
        // _rearWheel.transform.localRotation = Quaternion.Euler(0, _yaw * 30f, 0); 

        // Sound based on speed
        float pitchRange = (_sfxEngineRunningScriptable.maxPitch - _sfxEngineRunningScriptable.minPitch);
        float pitch = _thrust * pitchRange +  _sfxEngineRunningScriptable.minPitch;
        AudioManager.Instance.changeSFXSpeed(pitch);

        drawProjection();
    }

    /* ----- Others functions ----- */

    void onFlightInputs(InputAction.CallbackContext context){
        Vector3 movementInput = context.ReadValue<Vector3>();
        _pitch = movementInput.y;
        _roll = movementInput.x; 
        _yaw = movementInput.z;
    }


    void onGameStateChanged(State newState){
        if (newState.Is("PlaneRace")){
            if (GameManager.Instance.Player != null) GameManager.Instance.Player.gameObject.SetActive(false);
            _inputs.Enable();
            _camera.SetActive(true);
            _lineRenderer.enabled = true;
            _rigidbody.constraints = RigidbodyConstraints.None;
            AudioManager.Instance.setSFXLoop(true);
            AudioManager.Instance.playSoundCancelling(_sfxEngineRunningScriptable.sound, 0.5f);
        } else {
            _inputs.Disable();
            _lineRenderer.enabled = false;
            AudioManager.Instance.stopSound();
            AudioManager.Instance.setSFXLoop(false);
            AudioManager.Instance.changeSFXSpeed(1f);
            if (GameManager.Instance.Player != null) GameManager.Instance.Player.gameObject.SetActive(true);
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            _rigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;
        }
    }

    IEnumerator calculateSpeed(){
        Vector3 previousPosition = transform.position;
        while(true){
            yield return new WaitForFixedUpdate();
            _speedkph = (transform.position - previousPosition).magnitude * 100f;
            _speedkph = Mathf.Clamp(_speedkph, 0f, _maxSpeed);
            previousPosition = transform.position;
        }
    }

    void drawProjection(){
        Vector3[] points = new Vector3[_numSegments + 1];
        Vector3 direction = _ring.position - _propeller.transform.position;
        
        // Calculate control points for the Bezier curve
        for (int i = 0; i <= _numSegments; i++){
            float t = i / (float) _numSegments;
            Vector3 midPoint = Vector3.Lerp(_propeller.transform.position, _ring.position, t);
            Vector3 perpendicularDirection = Vector3.Cross(direction, Vector3.up).normalized;
            Vector3 controlPoint = midPoint + perpendicularDirection * Mathf.Sin(t * Mathf.PI) * 2f; // Adjust the multiplier for curve smoothness
            points[i] = controlPoint;
        }
        _lineRenderer.SetPositions(points);
    }

}