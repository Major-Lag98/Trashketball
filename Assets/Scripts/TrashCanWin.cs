using UnityEngine;

public class TrashCanWin : MonoBehaviour
{
    ParticleSystem ps;

    private void Start()
    {
        // get reference to particle system
        ps = GetComponent<ParticleSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            // play the particle system
            ps.Play();
            // set game state to ending
            FindAnyObjectByType<GameStateMachine>().ChangeState(GameStateMachine.GameState.Ending);
        }
    }
}
