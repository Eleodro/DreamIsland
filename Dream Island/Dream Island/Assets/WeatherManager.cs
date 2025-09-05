using System;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    [Header("=== Weather Management ===")]
    [SerializeField] private int ticksBetweenWeather = 10;
    [SerializeField] private int weatherQueueSize = 5;
    private int currentWeatherTick = 0;
    [SerializeField] private Weather currentWeather = Weather.Sunny;

    public Weather CurrentWeather => currentWeather;
    private Queue<Weather> weatherQueue;

    [Header("=== Weather VFX ===")]
    [SerializeField] ParticleSystem rainParticles;
    [SerializeField] ParticleSystem snowParticles;
    [SerializeField] ParticleSystem lightningParticles;
    [SerializeField] ParticleSystem cloudParticles;

    [Header("=== Debug Options ===")]
    public bool forceRain = false;

    public static Action<WeatherManager, Queue<Weather>> OnWetherChange;

    private void Start()
    {
        rainParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        snowParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        lightningParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        cloudParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        FillWeatherQueue();
        ChangeWeather();
    }

    private void OnEnable()
    {
        GameManager.OnTick += Tick;
    }

    private void OnDisable()
    {
        GameManager.OnTick -= Tick;
    }

    private void Tick()
    {
        currentWeatherTick++;

        if (currentWeatherTick >= ticksBetweenWeather)
        {
            currentWeatherTick = 0;
            ChangeWeather();
        }
    }

    void FillWeatherQueue()
    {
        weatherQueue = new Queue<Weather>();

        for (int i = 0; i < weatherQueueSize; i++)
        {
            WeatherManager tempWeather = GetRandomWeather();

            weatherQueue.Enqueue(tempWeather);

            Debug.Log($"Weather is {tempWeather} at index {i}");
        }
    }

    private WeatherManager GetRandomWeather()
    {
        int randomWeather = 0;

        if (!forceRain) randomWeather = UnityEngine.Random.Range(0, (int)Weather.WEATHER_MAX + 1);
        else randomWeather = 2;

        return (Weather)randomWeather;
    }

    void ChangeWeather()
    {
        currentWeather = weatherQueue.Dequeue();
        weatherQueue.Enqueue(GetRandomWeather());

        OnWeatherChange?.Invoke(currentWeather, weatherQueue);

        switch (currentWeather)
        {
            case Weather.Sunny:
                rainParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                snowParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                lightningParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                cloudParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                break;
            case Weather.Cloudy:
                rainParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                snowParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                lightningParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                cloudParticles.Play();
                break;
            case Weather.Rain:
                rainParticles.Play();
                snowParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                lightningParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                cloudParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                break;
            case Weather.Lightning:
                rainParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                snowParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                lightningParticles.Play();
                cloudParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                break;
            case Weather.Snow:
                rainParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                snowParticles.Play();
                lightningParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                cloudParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                break;
            default:
                break;
        }
    }
}

public enum Weather
{
    Sunny = 0,
    Cloudy = 1,
    Rain = 2,
    Lightning = 3,
    Snow = 4,
    WEATHER_MAX = Snow
}