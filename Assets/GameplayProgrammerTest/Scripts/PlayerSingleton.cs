using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSingleton : MonoBehaviour
{
    public static PlayerSingleton instance;

    public void Awake()
    {
        instance = this;
    }

    public GameObject player;
    public Transform spawnPoint;
    public Slider HealthBar;
}
