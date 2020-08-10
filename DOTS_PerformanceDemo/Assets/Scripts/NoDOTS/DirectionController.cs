using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionController : MonoBehaviour
{
    public Vector3 direction;
    private void Start()
    {
        direction = new Vector3(UnityEngine.Random.Range(0, 2) * 2 - 1, 0f, 0f);
    }

    void Update()
    {
        var position = transform.position;
        if ((direction.x == 1 && position.x > 10f)
        || (direction.x == -1 && position.x < -10f))
        {
            direction = direction * -1;
        }
    }
}
