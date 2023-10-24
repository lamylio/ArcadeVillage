using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* 
<note>
    TODO: I'd like to change the way the events are handled, and instead of using a Singleton pattern
    I think it would be better to use ScriptableObjects even for the states, as event handlers.
    But I discovered this too late in the project, and I don't have time to refactor everything. 
</note>
*/
public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    /* 
    <note> 
        Ok, lets be honest, I could have used an enum here, but I wanted to try something different
        in order to learn more about Objects and List in C#.
    </note> */
    [SerializeField] private List<State> _states = new List<State>(){
        new State("MenuScreen", "BoatCutScene"),
        new State("BoatCutScene"),
        new State("WelcomeCutScene"),
        new State("SpawnRing", "PlaneRace"),
        new State("PlaneRace"),
        new State("Dialogue"),
        new State("FreeWalkPlayer")
    };

    [SerializeField] private string _initialState;
     
    public State CurrentGameState;
    
    private GameObject _player;
    public GameObject Player {
        get {
            if (_player == null){
                _player = GameObject.FindGameObjectWithTag("Player");
            }
            return _player;
        }
    }

    public TextMeshProUGUI dialogueText;

    public Transform centerOfMap;

    /* 
    <note> 
        Most of the implementations found in basic tutorials use a switch statement 
        I decided to use events instead, as I think it's better. Lemme know.
    </note> */
    public static event System.Action<State> OnGameStateChanged;

    void Awake(){
        if (Instance != null) Destroy(gameObject); 
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    void Start(){
        if (_initialState != null) SwitchGameState(_initialState);
        else SwitchGameState(_states[0]);
    }

    // ===================================================================

    public void NextGameState(){
        CurrentGameState = _states.Find(state => state.Is(CurrentGameState.NextState));
        OnGameStateChanged?.Invoke(CurrentGameState);
        Debug.LogWarning("<color=yellow>Switching to " + CurrentGameState.Name + "</color>");
    }
    public void SwitchGameState(State newState){
        CurrentGameState = newState;
        OnGameStateChanged?.Invoke(CurrentGameState);
        Debug.LogWarning("<color=orange>Switching to " + CurrentGameState.Name + "</color>");
    }

    public void SwitchGameState(string newStateName){
        State newState = _states.Find(state => state.Is(newStateName))  ;
        SwitchGameState(newState);
    }
}
