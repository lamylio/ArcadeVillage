using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{

    // Singleton pattern
    public static GameManager Instance;

    // Ok, lets be honest, I could have used an enum here, but I wanted to try something different
    [SerializeField] private List<State> _states = new List<State>(){
        new State("MenuScreen", "BoatCutScene"),
        new State("BoatCutScene"),
        new State("WelcomeCutScene"),
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

    // Using triggers to change the game state instead of a case statement
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
