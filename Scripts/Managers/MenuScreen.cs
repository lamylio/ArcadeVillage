using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Playables;

using TMPro;


public class MainMenu : MonoBehaviour
{

    [SerializeField, Header("References")]
    private Animator _animator;
    [SerializeField] private TextMeshProUGUI _pressAnywhereText;

    [SerializeField] private GameObject _introductionCutScene, _introductionUI;

    [SerializeField] private AudioClip _menuMusic; 

    private bool hasStarted = false;

    void Awake(){
        // TODO: THIS HAS BEEN DELETED. REDO. 
        // _animator = GetComponent<Animator>();
    }

    void Start()
    {
        InvokeRepeating("BlinkPressKeyText", 0.5f, 1.5f);
        AudioManager.Instance.PlayMusic(_menuMusic);
    }

    void Update()
    {
        if (hasStarted) return;
        if (Input.GetKeyDown("space") || Input.GetMouseButtonDown(0)){
            StartCoroutine(SwitchToNextScene());
            hasStarted = true;
        }
    }

    // ===================================================================

    async void BlinkPressKeyText(){
        if (_pressAnywhereText == null) return;
        _pressAnywhereText?.gameObject.SetActive(false);  
        await Task.Delay(500);
        _pressAnywhereText?.gameObject.SetActive(true);
    }

    /* 
    <note>
        In the start, I used two separate scenes, but I decided to merge them into one in the end. 
        I haven't refactored the function name. 
    </note> 
    */
    IEnumerator SwitchToNextScene(){
        // _animator.SetTrigger("FadeIn");
        AudioManager.Instance.SendMusicToBackground();
        yield return new WaitForSeconds(1.5f);

        GameManager.Instance.NextGameState();
        _introductionCutScene.GetComponent<PlayableDirector>().Play();
        _introductionUI.SetActive(false);
    }
}
