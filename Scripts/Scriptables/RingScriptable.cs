using UnityEngine;


/* <note> WARNING : DEPRECIATED </note> */

// [CreateAssetMenu(fileName = "RingCourse_", menuName = "Scriptables/RingCourse")]
public class RingCourseScriptable : ScriptableObject
{

    // TODO: Possibility to increase difficulty
    
    public Difficulty difficulty; 

    public Vector3 startPosition, startRotation;

    public Ring[] rings;


    [System.Serializable]
    public struct Ring {

        /* <note> 
            A Ring (GameObject) will be invoked with a collider and a trigger. 
            The position and rotation of the ring is **relative** to the previous one.
        </note> */
        
        public Vector3 relativePosition;
        public Vector3 relativeRotation;
        public Size size;

        public Ring(Vector3 relativePosition, Vector3 relativeRotation, Size size){
            this.relativePosition = relativePosition;
            this.relativeRotation = relativeRotation;
            this.size = size;
        }

    }

    public enum Size {
        Small,
        Medium,
        Large
    }

    public enum Difficulty {
        Easy,
        Medium,
        Hard,
        Impossible
    }
}

