using UnityEngine;

public class Debug_GameManager : MonoBehaviour
{
    [SerializeField] private GameState _state;

    [ContextMenu("SetState")]
    void SetState()
    {
        GameManager.Instance.SetState(_state);
    }
    
    [ContextMenu("RemoveState")]
    void RemoveState()
    {
        GameManager.Instance.RemoveState(_state);
    }
    
    [ContextMenu("DebugState")]
    void DebugState()
    {
        Debug.Log($"{_state} : {GameManager.Instance.GetState(_state)}");
    }
}
