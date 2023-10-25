using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public enum RingSize {
    Small,
    Medium,
    Large
}

/* 
<note>
    This script is attached to the parent object containing 3 rings, the small, the medium and the large one.
    It handles the generation of the course. Each time the plane goes through a ring, the next one is generated
    by activating the proper ring at a random position and rotation.

    TODO: Ring size and course difficulty + link with CoinManager (not implemented yet)
</note>
*/
public class Rings : MonoBehaviour {

    

    public float MIN_HEIGHT = 60f, MIN_DISTANCE = 30f, MAX_DISTANCE = 100f;

    [SerializeField, Header("Audio")] private SFXScriptable _sfxRingScriptable;
    [SerializeField] private SFXScriptable _sfxVictoryScriptable, _sfxLoseScriptable; 

    [SerializeField, Header("Course settings")] private int _amountOfRings = 3;
    [SerializeField] private int _currentRingCount = 0;
    [SerializeField] private int[] _ringSizes = { 16, 18, 20 };
    [SerializeField] private float _timeLimit = 90f;

    [SerializeField, Header("References")] private TextMeshProUGUI _countdown;
    [SerializeField] private GameObject _plane;

    private Vector3 _planeRespawnPosition;
    private Quaternion _planeRespawnRotation;

    private MeshRenderer _meshRenderer;
    private float _remainingTime;

    /* ------ Monobehaviour functions ------ */

    void Awake(){
        GameManager.OnGameStateChanged += onGameStateChanged;
    }

    void Start(){
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.enabled = false;
        _remainingTime = _timeLimit;
        _planeRespawnPosition = _plane.transform.position;
        _planeRespawnRotation = _plane.transform.rotation;
    }
    void LateUpdate(){
        if(_remainingTime <= 0f && _countdown.enabled) {
            _countdown.enabled = false;
            Debug.Log("Course failed");
            StartCoroutine(endCourse());
            return;
        }
        if(_currentRingCount > 0) _remainingTime -= Time.deltaTime;
        int minutes = Mathf.FloorToInt(_remainingTime / 60f);
        int seconds = Mathf.FloorToInt(_remainingTime % 60f);
        _countdown.text = $"{minutes:00}:{seconds:00}";
    }

    void OnTriggerEnter(Collider other){
        if (other.name == "Plane"){
            if (_remainingTime <= 0) return;
            generateNextRing();
            AudioManager.Instance.playSound(_sfxRingScriptable.sound, _sfxRingScriptable.volume, Random.Range(_sfxRingScriptable.minPitch, _sfxRingScriptable.maxPitch));
        }
    }

    /* ---------------------------------- */

    void onGameStateChanged(State newState){
        if (newState.Is("PlaneRace")){
            /* <note> The first ring is always the same, and already in position. Se we only need to enable the renderer </note> */
            _meshRenderer.enabled = true;
            _countdown.enabled = true;
            // InvokeRepeating("generateNextRing", 3f, 3f);
        } else {
            _meshRenderer.enabled = false;
            _countdown.enabled = false;
        }
    }

    private void generateNextRing(){
        if(_currentRingCount++ >= _amountOfRings) {
            Debug.Log("Course completed");
            StartCoroutine(endCourse(true));
        }else {
            spawnRing(getRandomPositionInAnnularRegion(MIN_DISTANCE, MAX_DISTANCE), getRandomRotation(), Random.Range(0, _ringSizes.Length));
        }
    }

    private IEnumerator endCourse(bool victory=false){
        GameManager.Instance.nextGameState();
        yield return new WaitForSeconds(1f);

        _plane.transform.position = _planeRespawnPosition;
        _plane.transform.rotation = _planeRespawnRotation;

        _currentRingCount = 0;
        _remainingTime = _timeLimit;
        SFXScriptable sfxs = victory ? _sfxVictoryScriptable : _sfxLoseScriptable;
        AudioManager.Instance.playSound(sfxs.sound, sfxs.volume, sfxs.minPitch);
    }

    private Vector3 getRandomPositionInAnnularRegion(float innerRadius, float outerRadius){
        /* 
        <note>
            This methods returns a random position within an annular region.
            That means, a random position within a circle of radius outerRadius, excluding a circle of radius innerRadius.
            So we have a ring that is not too close to the previous one, nor too far.

            Remark: my first idea was to select random position within the map border using the MeshCollider of the Skydome. This new way is cool.        
        </note>
         */
        float randomRadius = Random.Range(innerRadius, outerRadius);
        float randomAngle = Random.Range(0f, 360f);
        float randomX = randomRadius * Mathf.Cos(Mathf.Deg2Rad * randomAngle);
        float randomY = randomRadius * Mathf.Sin(Mathf.Deg2Rad * randomAngle);
        randomY = randomY < MIN_HEIGHT ? MIN_HEIGHT : randomY;
        float randomZ = Random.Range(-outerRadius, outerRadius);

        return new Vector3(randomX, randomY, randomZ);
    }

    private Quaternion getRandomRotation(){
        return Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
    }

    private void spawnRing(Vector3 position, Quaternion rotation, int type){
        type = Mathf.Clamp(type, 0, _ringSizes.Length - 1);
        int scale = _ringSizes[type];
        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = new Vector3(scale, 50, scale);
        Debug.Log($"Spawning ring {_currentRingCount} of size {type} at {position}");
    }

}