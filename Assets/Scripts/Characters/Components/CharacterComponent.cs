using UnityEngine;

public class CharacterComponent : MonoBehaviour
{
    private CharacterLocomotion locomotion;

    private void Start() {
        locomotion = GetComponent<CharacterLocomotion>();
        locomotion.InitializePatrolPoints();
    }

    private void Update() {
        locomotion.IdleRandomWalk();
    }
}
