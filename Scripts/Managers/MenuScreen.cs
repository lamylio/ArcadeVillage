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
        _animator = GetComponent<Animator>();
        _introductionCutScene = transform.GetChild(0).gameObject;
        _introductionUI = transform.GetChild(1).gameObject;
    }

    void Start()
    {
        InvokeRepeating("BlinkPressKeyText", 0.5f, 1.5f);
        AudioManager.Instance.PlayMusic(_menuMusic);
    }

    void Update()
    {
        // Check if the player press space or click on the screen
        // If so, play the animation, then switch scene
        if (hasStarted) return;
        if (Input.GetKeyDown("space") || Input.GetMouseButtonDown(0)){
            StartCoroutine(SwitchToNextScene());
            hasStarted = true;
        }
    }

    // ===================================================================

    async void BlinkPressKeyText(){
        // Activate and deactivate the text
        if (_pressAnywhereText == null) return;
        _pressAnywhereText?.gameObject.SetActive(false);  
        await Task.Delay(500);
        _pressAnywhereText?.gameObject.SetActive(true);
    }

    IEnumerator SwitchToNextScene(){
        _animator.SetTrigger("FadeIn");
        AudioManager.Instance.SendMusicToBackground();
        yield return new WaitForSeconds(1.5f);

        GameManager.Instance.NextGameState();
        _introductionCutScene.GetComponent<PlayableDirector>().Play();
        _introductionUI.SetActive(false);
    }
}
