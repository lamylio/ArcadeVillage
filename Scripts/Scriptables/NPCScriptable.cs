using UnityEngine;


[CreateAssetMenu(fileName = "NPC_", menuName = "Scriptables/NPC")]
public class NPCScriptable : ScriptableObject
{
    public string npcName;
    public Dialogue[] dialogues;


    [System.Serializable]
    public struct Dialogue {
        public string text;
        public Action action;

        public NPCScriptable nextScriptable;

        public Dialogue(string text, Action action = Action.None, NPCScriptable nextScriptable = null){
            this.text = text;
            this.action = action;
            this.nextScriptable = nextScriptable;
        }
    }
    public enum Action {
        None,
        EndDialogue,
        AskYesNo,
        ControlThePlane,
        OpenShop, // TODO: Implement shop (not this time)
    }
}

