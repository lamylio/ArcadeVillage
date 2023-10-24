using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
<note>

    The assets used to make the sky are not compatible with the Unity Lightning Skybox, so I had to make my own skydome.
    Otherwise I would have used the Skybox, which is much more realistic and handles the sun rotation automatically.
    Too bad. :(
</note>
*/

public class SkyManager : MonoBehaviour
{
    const float SECONDS_IN_DAY = 86400f, SUN_OFFSET = -180f, LIGHT_OFFSET = -90f, SKY_OFFSET = 0.5f, LIGHT_MIN_INTENSITY = 0.1f, LIGHT_MAX_INTENSITY = 0.8f;
    
    /* ------------------ */

    [SerializeField, Header("Time Settings"), Range(0, 24)] private float _startTime = 6f;
    [SerializeField] private float _timeScale = 100f;

    [Header("References"), SerializeField] private Light _light;
    [SerializeField]  private GameObject _skybox, _sun, _moon;
    [SerializeField] private TextMeshProUGUI _timeUI;

    /* ------------------ */

    private DateTime _currentDateTime;
    private MeshRenderer _skyboxRenderer;
    public float currentRatio => (float) _currentDateTime.TimeOfDay.TotalSeconds / SECONDS_IN_DAY;

    void Start()
    {
        _currentDateTime = DateTime.Now.Date + TimeSpan.FromHours(_startTime);
        _skyboxRenderer = _skybox.GetComponent<MeshRenderer>();
    }

    
    void Update()
    {
        // Update the time
        _currentDateTime = _currentDateTime.AddSeconds(Time.deltaTime * _timeScale);

        // Show it on the screen, if there is a UI element (for debug purposes)
        _timeUI?.SetText(_currentDateTime.ToString("HH:mm"));

        // Change the skybox offset
        float skyBoxValue = currentRatio + SKY_OFFSET;
        _skyboxRenderer.material.mainTextureOffset = new Vector2(skyBoxValue, 0);
    
        // Rotate the sun around the center of the map
        float sunValue = currentRatio * 360 + SUN_OFFSET;
        _sun.transform.rotation = Quaternion.Euler(sunValue, 0, 0);

        // Change the directional light rotation
        float lightValue = currentRatio * 360 + LIGHT_OFFSET;
        _light.transform.rotation = Quaternion.Euler(lightValue, 45, 0);

        // Change the intensity of the light, from 0.1 to 1 at noon, then back to 0.1
        float sinValue = (float) Math.Sin(currentRatio * Math.PI);
        _light.intensity = Math.Clamp(sinValue, LIGHT_MIN_INTENSITY, LIGHT_MAX_INTENSITY);

        // TODO: Moon ?
    }
}
