using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum RingSize {
    Small,
    Medium,
    Large
}

public class Rings : MonoBehaviour {

    /* 
    <note>
        This script is attached to the parent object containing 3 rings, the small, the medium and the large one.
        It handles the generation of the course. Each time the plane goes through a ring, the next one is generated
        by activating the proper ring at a random position and rotation.

        TODO: tracking (path projection) from the plane to the next ring

    </note>
     */

    public float MIN_HEIGHT = 60f, MIN_DISTANCE = 30f, MAX_DISTANCE = 100f;

    [SerializeField, Header("Audio")] private SFXScriptable _sfxRingScriptable;
    [SerializeField] private SFXScriptable _sfxVictoryScriptable, _sfxLoseScriptable; 

    [SerializeField, Header("Course settings")] private int _amountOfRings = 3;
    [SerializeField] private int _currentRingCount = 0;
    [SerializeField] private int[] _ringSizes = { 16, 18, 20 };

    private MeshRenderer _meshRenderer;


    void Awake(){
        GameManager.OnGameStateChanged += onGameStateChanged;
    }

    void Start(){
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.enabled = false;
    }

    void onGameStateChanged(State newState){
        if (newState.Is("PlaneRace")){
            _meshRenderer.enabled = true;
            // InvokeRepeating("GenerateNextRing", 3f, 3f);
        } else {
            _meshRenderer.enabled = false;
            CancelInvoke("GenerateNextRing");
        }
    }

    /* ---------------------------------- */


    private void GenerateNextRing(){
        if(_currentRingCount++ >= _amountOfRings) {
            Debug.Log("Course completed");
            StartCoroutine(EndCourse());
        }else {
            SpawnRing(GetRandomPositionInAnnularRegion(MIN_DISTANCE, MAX_DISTANCE), GetRandomRotation(), Random.Range(0, _ringSizes.Length));
        }
    }

    private IEnumerator EndCourse(){
        GameManager.Instance.NextGameState();
        yield return new WaitForSeconds(1f);
        AudioManager.Instance.PlaySoundCancelling(_sfxVictoryScriptable.sound, _sfxVictoryScriptable.volume, _sfxVictoryScriptable.minPitch);
    }

    private Vector3 GetRandomPositionInAnnularRegion(float innerRadius, float outerRadius){
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

    private Quaternion GetRandomRotation(){
        return Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
    }

    private void SpawnRing(Vector3 position, Quaternion rotation, int type){
        type = Mathf.Clamp(type, 0, _ringSizes.Length - 1);
        int scale = _ringSizes[type];
        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = new Vector3(scale, 50, scale);
        Debug.Log($"Spawning ring {_currentRingCount} of size {type} at {position}");
    }

    void OnTriggerEnter(Collider other){
        // If name is Plane, then generate next ring
        if (other.name == "Plane"){
            GenerateNextRing();
            AudioManager.Instance.PlaySound(_sfxRingScriptable.sound, _sfxRingScriptable.volume, Random.Range(_sfxRingScriptable.minPitch, _sfxRingScriptable.maxPitch));
        }
    }


}