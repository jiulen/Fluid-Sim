using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeartSimulation : MonoBehaviour
{

    [Header("Text")]
    public int currentDisplayedBPM;
    public TMP_Text BPMDisplayText;

    [Header("DontFill")]
    public int beatsPerMinute = 60; // Default BPM

    public float timeBetweenBeats; // Time between each beat
    public float timer; // Timer to keep track of time
    private float minBPM = 25, maxBPM = 300f;

    [Header("PredefinedValues")]
    public float severeHypoPotassium = 1;
    public float severeHyperPotassium = 8;
    public float severeHypoCalcium = 1;
    public float severeHyperCalcium = 4;


    [Header("Heartbeat animation")]
    public Animator HeartAnimator;
    public AnimationClip HeartbeatAnimation;

    [Header("Sliders")]
    public Slider PotassiumSlider;
    public Slider CalciumSlider;
    public TMP_Text PotassiumSliderValue, CalciumSliderValue;
    public float CurrentPotassium, CurrentCalcium;

    [Header("Effects")]
    public GameObject EffectPrefab;
    public Transform EffectParent;

    public TMP_Text CurrentPotassiumInducedEffect;

    [Header("Danger")]
    public float DangerWarningTimer;
    public bool isInDanger = false;
    public GameObject FibrillationWarning;
    bool dead = false;
    public TMP_Text FibrillationText;

    [Header("Visuals")]
    public Toggle SeeThroughToggle, VeinsToggle, ElectricToggle;
    public Material HeartFront;
    public GameObject[] Veins;
    public Material ElectricSignalMaterial, StandardMaterial;
    public GameObject Heart;

    [Header("EKG")]
    public EKGRenderer ekgRenderer;

    public void ToggleShader()
    {
        // Toggle between shaders based on the toggle state
        if (ElectricToggle.isOn)
        {
            // Use the material with the custom pulsing shader
            Heart. GetComponent<SkinnedMeshRenderer>().material = ElectricSignalMaterial;
        }
        else
        {
            // Use the material with the standard shader
            Heart. GetComponent<SkinnedMeshRenderer>().material = StandardMaterial;
        }
    }
    public void UpdateSeeThrough()
    {
        if(SeeThroughToggle.isOn)
        {
            HeartFront.SetColor("_Color", new Color(0,0,0,0));
        }
        else HeartFront.SetColor("_Color", new Color(1,1,1,1));
    }

    public void UpdateVeins()
    {
        foreach(GameObject GO in Veins)
        {
            GO.SetActive(VeinsToggle.isOn);
        }
    }

    public void UpdateNutrientValues()
    {
        PotassiumSliderValue.text = PotassiumSlider.value.ToString("F2");
        CalciumSliderValue.text = CalciumSlider.value.ToString("F2");

        CurrentPotassium = PotassiumSlider.value;
        CurrentCalcium = CalciumSlider.value;

        CheckPotassiumLevel();
    }

    public void AddPotassiumInducedEffect(string s, Color? color = null, bool danger = false)
    {
        if (CurrentPotassiumInducedEffect && CurrentPotassiumInducedEffect.text != s)
        {
            Destroy(CurrentPotassiumInducedEffect.gameObject);
        }

        if (!CurrentPotassiumInducedEffect)
        {
            GameObject GO = Instantiate(EffectPrefab);
            GO.transform.SetParent(EffectParent, false);
            CurrentPotassiumInducedEffect = GO.GetComponent<TMP_Text>();
            CurrentPotassiumInducedEffect.text = s;
            if(color != null)
            {
                CurrentPotassiumInducedEffect.color = (Color)color;
            }
            isInDanger = danger;
        }
    }

    public void RemovePotassiumInducedEffect()
    {
        if (CurrentPotassiumInducedEffect) Destroy(CurrentPotassiumInducedEffect.gameObject);
    }

    public void CheckPotassiumLevel()
    {
        if (dead) return;
        float currentPotassium = PotassiumSlider.value;

        // Calculate the normalized value for potassium between hypo and hyper ranges
        float normalizedPotassium = Mathf.InverseLerp(severeHypoPotassium, severeHyperPotassium, currentPotassium);

        if (normalizedPotassium < 0.25f)
        {
            AddPotassiumInducedEffect("Severe Hypokalemia", Color.red, danger: true); // If potassium value is close to severe hypo
        }
        else if (normalizedPotassium < 0.4f)
        {
            AddPotassiumInducedEffect("Hypokalemia"); // If potassium value is close to hypo
        }
        else if (normalizedPotassium > 0.75f)
        {
            AddPotassiumInducedEffect("Severe Hyperkalemia", Color.red, danger: true); // If potassium value is close to severe hyper
        }
        else if (normalizedPotassium > 0.6f)
        {
            AddPotassiumInducedEffect("Hyperkalemia"); // If potassium value is close to hyper
        }
        else
        {
            RemovePotassiumInducedEffect();
        }

        float adjustedBPM = Mathf.Lerp(minBPM, maxBPM, Mathf.InverseLerp(severeHypoPotassium, severeHyperPotassium, CurrentPotassium));
        beatsPerMinute = (int)adjustedBPM;
    }
    void Start()
    {
        // Calculate time between beats based on BPM
        timeBetweenBeats = 60f / beatsPerMinute;
        timer = 0f; // Initialize timer
        UpdateNutrientValues();
    }


    public void UpdateDangers()
    {
        if (!dead)
        {
            if (isInDanger)
            {
                DangerWarningTimer += Time.deltaTime;
                if (DangerWarningTimer > 2.5f)
                {
                    float normalizedPotassium = Mathf.InverseLerp(severeHypoPotassium, severeHyperPotassium, CurrentPotassium);
                    if (normalizedPotassium > 0.65f)
                    {
                        Dead("Fibrillation has occured", "Hyperkalemia induced ventricular fibrillation");
                        ekgRenderer.fibrillation = true;
                    }
                    else if (normalizedPotassium < 0.45f)
                        Dead("Cardiac arrest has occured", "Hypokalemia induced cardiac arrest");
                }
            }
            else
            {
                DangerWarningTimer -= Time.deltaTime * 3;
                if (DangerWarningTimer < 0) DangerWarningTimer = 0;
            }
        }
    }

    public void Dead(string causeOfDeath, string descOfDeath)
    {
        dead = true;
        HeartAnimator.SetBool("Fibrillation", true);
        AddPotassiumInducedEffect(descOfDeath, Color.red);
        FibrillationWarning.SetActive(true);
        FibrillationText.text = causeOfDeath;
        ElectricSignalMaterial.SetFloat("_RandomPulsing", 1);
    }

    public void Shock()
    {
        isInDanger = false;
        DangerWarningTimer = 0;
        ScreenController.Instance.StartWhitePulse();
        HeartAnimator.SetBool("Fibrillation", false);
        dead = false;
        beatsPerMinute = 60;
        PotassiumSlider.value = 4.00f;
        UpdateNutrientValues();
        FibrillationWarning.SetActive(false);
        ekgRenderer.fibrillation = false;
        ElectricSignalMaterial.SetFloat("_RandomPulsing", 0);
    }

    public void UpdateAnimationSpeed()
    {
        float minSpeed = 1.0f;
        float maxSpeed = 3.0f;

        // Clamp the beatsPerMinute within the range of minBPM and maxBPM
        float clampedBPM = Mathf.Clamp(beatsPerMinute, minBPM, maxBPM);

        // Calculate the normalized speed within the range of minSpeed and maxSpeed
        float normalizedSpeed = Mathf.InverseLerp(minBPM, maxBPM, clampedBPM);

        // Map the normalized speed to the desired speed range (1.0 to 3.0)
        float newSpeed = Mathf.Lerp(minSpeed, maxSpeed, normalizedSpeed);

        // Set the animator speed based on the calculated speed
        HeartAnimator.speed = newSpeed;
    }

    public void UpdateHeartTimer()
    {
        // Increment the timer
        timer += Time.deltaTime;

        // Check if the time exceeds the time between beats
        if (timer >= timeBetweenBeats)
        {
            // Reset timer
            timer = 0f;

            // Call a function to simulate a heartbeat
            SimulateHeartbeat();
        }
    }

    public void UpdateDisplay()
    {
        if (currentDisplayedBPM > beatsPerMinute)
        {
            currentDisplayedBPM--;
            timeBetweenBeats = 60f / currentDisplayedBPM;
            ElectricSignalMaterial.SetFloat("_PulsesPerMinute", beatsPerMinute);
        }
        else if (currentDisplayedBPM < beatsPerMinute)
        {
            currentDisplayedBPM++;
            timeBetweenBeats = 60f / currentDisplayedBPM;
            ElectricSignalMaterial.SetFloat("_PulsesPerMinute", beatsPerMinute);
        }
        BPMDisplayText.text = "BPM: " + currentDisplayedBPM;
    }

    void Update()
    {
        if (!HeartAnimator.GetBool("Fibrillation"))
        {
            UpdateAnimationSpeed();
            UpdateHeartTimer();
            UpdateDisplay();
        }
        else
        {
            BPMDisplayText.text = "BPM: -- --"; 
        }
        UpdateDangers();
    }

    void SimulateHeartbeat()
    {
        ekgRenderer.Beat();
            HeartAnimator.Play(HeartbeatAnimation.name);
    }
}
