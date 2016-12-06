using UnityEngine;
using System.Collections;

public interface EnemyState {
    void Execute();
    void Begin(Enemy enemy);
    void Leave();
    void OnTriggerEnter2D(Collider2D other);
}
