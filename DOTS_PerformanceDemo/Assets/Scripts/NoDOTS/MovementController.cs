using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public Vector3 direction;
    public float movementSpeed;
    private bool doPseudoCalculations;
    
    void Start()
    {
        GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
        doPseudoCalculations = gameManager.doPseudoCalculations;
        direction = new Vector3(UnityEngine.Random.Range(0, 2) * 2 - 1, 0f, 0f);
        movementSpeed = UnityEngine.Random.Range(1.0f, 6.0f);
    }

    void Update()
    {
        transform.position += direction * movementSpeed * Time.deltaTime;
        // Pseudo calculations
        if (doPseudoCalculations)
        {
            // TODO
        }
    }
}
