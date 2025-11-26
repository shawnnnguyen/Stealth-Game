using UnityEngine;

public class GameInput : MonoBehaviour
{
    public Vector3 inputVector;
    
    public Vector3 GetMovementVectorNormalized()
    {
        inputVector = Vector3.zero;
      
        if (Input.GetKey(KeyCode.W))
        {
            inputVector.z = +1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputVector.z = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputVector.x = +1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputVector.x = -1f;
        }
        
        inputVector =  inputVector.normalized;
        
        return inputVector;
    }
}
