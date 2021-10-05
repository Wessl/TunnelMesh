using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

// This class is a gigantic mess. TODO: Clean up
public class AirplaneController : MonoBehaviour
{
    // Exhaust particles
    [SerializeField] private ParticleSystem exhaustFumes;
    // Sound
    [SerializeField] private AudioSource airplaneSoundPlayer;
    public AudioClip shootSoundZap;
    private float _sfxVolume;
    // Plane movement variables
    public float forwardSpeed = 25f, strafeSpeed = 7.5f, hoverSpeed = 5f;
    public float startSpeed;
    private float _activeForwardSpeed, _activeStrafeSpeed, _activeHoverSpeed;
    private float _forwardAcceleration = 2.5f, _strafeAcceleration = 2f, _hoverAcceleration = 2f;
    public float lookRateSpeed = 90f;
    // Store script-wide variables relating to mouse, roll input and screen coordinates
    private Vector2 lookInput, screenCenter, mouseDistance;
    private float rollInput;
    public float rollSpeed = 90f, rollAcceleration = 3.5f;

    public float incSpeedAmountPerUnitOfTime;

    public Material engineExhaustRendererMat;
    public string engineLightStrRef;

    public GameManager gameManager;
    

    void Start()
    {
        _activeForwardSpeed = startSpeed;
        screenCenter.x = Screen.width / 2f;
        screenCenter.y = Screen.height / 2f;
        Cursor.lockState = CursorLockMode.Confined;
        InvokeRepeating("IncreaseSpeed", 0f, 0.5f);
        
    }

    void Update()
    {
        // Store some mouse input data, vector to mouse pos from center of screen
        lookInput = Input.mousePosition;
        mouseDistance = (lookInput - screenCenter) / screenCenter;
        mouseDistance = Vector2.ClampMagnitude(mouseDistance, 1f);

        // Rolling from side to side causes rotation along the Z axis
        rollInput = Mathf.Lerp(rollInput, Input.GetAxisRaw("Roll"), rollAcceleration * Time.deltaTime);
        // Align plane towards mouse position & apply potential roll input
        transform.Rotate(-mouseDistance.y * lookRateSpeed * Time.deltaTime, mouseDistance.x * lookRateSpeed * Time.deltaTime, rollInput * rollSpeed * Time.deltaTime, Space.Self);

        //_activeForwardSpeed = Mathf.Lerp(_activeForwardSpeed, Input.GetAxisRaw("Vertical") * forwardSpeed, _forwardAcceleration * Time.deltaTime);
        _activeStrafeSpeed = Mathf.Lerp(_activeStrafeSpeed, Input.GetAxisRaw("Horizontal") * strafeSpeed, _strafeAcceleration * Time.deltaTime);
        _activeHoverSpeed = Mathf.Lerp(_activeHoverSpeed, Input.GetAxisRaw("Hover") * hoverSpeed, _hoverAcceleration * Time.deltaTime);
        
        transform.position += _activeForwardSpeed * Time.deltaTime * transform.forward;
        transform.position += _activeStrafeSpeed * Time.deltaTime * transform.right;
        transform.position += _activeHoverSpeed * Time.deltaTime * transform.up;
    }

    private void IncreaseSpeed()
    {
        _activeForwardSpeed += incSpeedAmountPerUnitOfTime;
        engineExhaustRendererMat.SetFloat(engineLightStrRef, _activeForwardSpeed / 100);
        gameManager.HandleSpeedIncrease(_activeForwardSpeed);
    }
}