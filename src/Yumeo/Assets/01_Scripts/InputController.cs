using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour , PlayerInputs.IGroundInputActions , PlayerInputs.IGlideInputActions
{
    
    public delegate void SetMovementInput(Vector2 movement);
    public static event SetMovementInput SetMovementInputAction;
    
    public delegate void OnJumpInput();
    public static event OnJumpInput OnJumpInputAction;
    
    public delegate void OnGlideInput(bool isGliding);
    public static event OnGlideInput OnGlideInputAction;
    
    
    
    
    
    public static InputController Instance;
    
    private PlayerInputs _controls;


    private void Awake()
    {
        if(Instance != null)
            Destroy(Instance);
        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void OnEnable()
    {
        if (_controls == null)
        {
            _controls = new PlayerInputs();
            _controls.GroundInput.SetCallbacks(this);
            _controls.GlideInput.SetCallbacks(this);
        }
        _controls.GroundInput.Enable();
        
    }

    private void OnDisable()
    {
        _controls.GroundInput.Disable();
        _controls.GlideInput.Disable();
    }

    public void ChangeToGlideInput()
    {
        Debug.Log("Changing to Glide Input");
        _controls.GroundInput.Disable();
        _controls.GlideInput.Enable();
    }

    
    public void ChangeToGroundInput()
    {
        Debug.Log("Changing to Ground Input");
        _controls.GlideInput.Disable();
        _controls.GroundInput.Enable();
    }

    
    public void OnMovement(InputAction.CallbackContext context)
    {
        if(context.performed)
            SetMovementInputAction?.Invoke(context.ReadValue<Vector2>());
        if(context.canceled)
            SetMovementInputAction?.Invoke(Vector2.zero);
        
    }

    public void OnGlide(InputAction.CallbackContext context)
    {
        if(context.performed)
            OnGlideInputAction?.Invoke(true);
        if(context.canceled)
            OnGlideInputAction?.Invoke(false);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.performed)
            OnJumpInputAction?.Invoke();
            
    }
    
}
