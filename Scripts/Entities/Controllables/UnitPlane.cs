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

    // --- Settings ---
    [SerializeField, Header("Settings")]    
    private float _maxSpeed = 150f;
    [SerializeField]
    private float _pitchMultipler = 1f, _yawMultipler = 1f, _rollMultipler = 1f, _liftMultipler = 10f, _thrustDecreaseRatio = 0.09f;


    // --- Inputs ---
    [SerializeField, Space(5), Header("Inputs and current values")]
    private bool _throttle;
    [SerializeField]
    private float _pitch, _yaw, _roll, _thrust = 1f, _speedkph;
    

    // --- Calculated values ---
    private float _maxThrust => _maxSpeed / 100f;
    private float _thrustDelta => _maxThrust / 1000f;
    private float _speed => _speedkph / 100f;
    
    void Awake(){
        _inputs = new PlayerInputs();
        _rigidbody = GetComponent<Rigidbody>();

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
        StartCoroutine(CalculateSpeed()); 
    }

    void Update(){
        _thrust += _throttle ? _thrustDelta : -_thrustDelta * _thrustDecreaseRatio;
        _thrust = Mathf.Clamp(_thrust, 0f, _maxThrust);
    }

    void FixedUpdate(){

        /* 
        <Note> 
            I could have use the .AddForce and Torque functions, 
            but then the controlls seems kinda weird and less reactive to the user inputs.
            So I decided to use the .Translate and .Rotate functions instead.
        </Note>
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

        // Rotate the rear wheel with a maximum of -30 and +30
        // _rearWheel.transform.localRotation = Quaternion.Euler(0, _yaw * 30f, 0); 
    }

    void onFlightInputs(InputAction.CallbackContext context){
        Vector3 movementInput = context.ReadValue<Vector3>();
        _pitch = movementInput.y;
        _roll = movementInput.x; 
        _yaw = movementInput.z;
    }


    void onGameStateChanged(State newState){
        if (newState.Is("PlaneRace")){
            GameManager.Instance.Player.gameObject.SetActive(false);
            _inputs.Enable();
            _camera.SetActive(true);
        } else {
            _inputs.Disable();
            // _camera.SetActive(false);
            if (GameManager.Instance.Player != null) GameManager.Instance.Player.gameObject.SetActive(true);
        }
    }

    IEnumerator CalculateSpeed(){
        Vector3 previousPosition = transform.position;
        while(true){
            yield return new WaitForFixedUpdate();
            _speedkph = (transform.position - previousPosition).magnitude * 100f;
            previousPosition = transform.position;
        }
    }


}