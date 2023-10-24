[System.Serializable]
public class State {

    public string Name;
    public string NextState;
    
    public State(string name, string nextState = "FreeWalkPlayer"){
        Name = name;
        NextState = nextState;
    }

    public bool Is(string stateName){
        return Name.ToUpper() == stateName.ToUpper();
    }

}