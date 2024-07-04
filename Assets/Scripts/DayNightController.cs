using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DayNightController : MonoBehaviour
{
    [Header("Time")]
    [Tooltip("Day Length is in minute")] [SerializeField] private float _targetDayLength = 13f;
    public float targetDayLength => _targetDayLength;

    [SerializeField] [Range(0f, 1f)] private float _timeOfDay;
    public float timeOfDay => _timeOfDay;

    [SerializeField] private int _dayNumber = 0;
    public int dayNumber => _dayNumber;

    [SerializeField] private int _yearNumber = 0;
    public int yearNumber => _yearNumber;

    [SerializeField] private int _yearLength = 100;
    public int yearLength => _yearLength;
    
    private float _timeScale = 100f;
    private bool _pause = false;

    [Header("Sun Light")]
    [SerializeField] private Transform dailyRotation;

    [SerializeField] private Light sun;
    private float _intensity;

    [SerializeField] private float sunBaseIntensity = 1f;
    [SerializeField] private float sunVariation = 1.5f;
    [SerializeField] private Gradient sunColor;

    private void Update()
    {
        if (!_pause)
        {
            UpdateTimeScale();
            UpdateTime();            
        }
        AdjustSunRotation();
        SunIntensity();
        AdjustSunColor();
    }

    private void UpdateTimeScale()
    {
        // game * timeScale = reality  => timeScale = reality / game
        _timeScale = 24 / (_targetDayLength / 60);
    }

    private void UpdateTime()
    {
        // timeOfDay = secondPassed / secondInADay
        _timeOfDay += Time.deltaTime * _timeScale / 86400;
        if (_timeOfDay > 1f)
        {
            _dayNumber++;
            _timeOfDay -= 1;  // subtract it by one instead of setting it to zero to prevent loss of time.
            if (_dayNumber > _yearLength)
            {
                _yearNumber++;
                _dayNumber = 0;
            }
        }
    }

    private void AdjustSunRotation()
    {
        float sunAngle = _timeOfDay * 360f;
        dailyRotation.transform.localRotation = Quaternion.Euler(new Vector3(sunAngle, 0f, 0f));
    }

    private void SunIntensity()
    {
        _intensity = Vector3.Dot(sun.transform.forward, Vector3.down);
        _intensity = Mathf.Clamp01(_intensity);

        sun.intensity = _intensity * sunVariation + sunBaseIntensity;
    }

    private void AdjustSunColor()
    {
        sun.color = sunColor.Evaluate(_intensity);
    }
}
