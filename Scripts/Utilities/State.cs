[System.Serializable]
public class State {

    public string Name;
    private string _nextState;
    public string NextState {
        get {
            return _nextState;
        }
    }

    public State(string name, string nextState = "FreeWalkPlayer"){
        Name = name;
        _nextState = nextState;
    }

    public bool Is(string stateName){
        return Name.ToUpper() == stateName.ToUpper();
    }

}