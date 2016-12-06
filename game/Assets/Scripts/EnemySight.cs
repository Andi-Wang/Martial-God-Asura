﻿using UnityEngine;
using System.Collections;

public class EnemySight : MonoBehaviour
{

    [SerializeField]
    private Enemy enemy;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            enemy.player = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            enemy.player = null;
        }
    }
}