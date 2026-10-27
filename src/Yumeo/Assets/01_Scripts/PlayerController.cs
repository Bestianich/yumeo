using System;
using UnityEngine;
using UnityEngine.Serialization;


public class PlayerController : MonoBehaviour
{

    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _checkGroundRange = 1f;
    [SerializeField] private float _jumpForce = 100f;
    
    [Header("GRAVITY DATA")]
    [SerializeField] private Vector3 _gravityDirection = Vector3.down;
    [SerializeField] private float _gravityForce = 2f;
    
    [Header("GLIDE DATA")]
    [SerializeField] private float _glideGravityForce = 0.2f;
    [SerializeField] private ParticleSystem _glideParticles;
    
    
    private Camera _camera;
    private Vector2 _movementInput;
    private bool _isGrounded = true;
    private bool _isGliding = false;
    
    private float _fallVelocity = 0;
    private float _currentGravityForce = 0;


    #region Input Event Registration
    
        private void OnEnable()
        {
            InputController.SetMovementInputAction += SetMovementInput;
            InputController.OnJumpInputAction += Jump;
            InputController.OnGlideInputAction += Glide;
        }
        
        private void OnDisable()
        {
            InputController.SetMovementInputAction -= SetMovementInput;
            InputController.OnJumpInputAction -= Jump;
        }
    #endregion


    private void Awake()
    {
        _camera = Camera.main;
        _glideParticles.Stop();
        _currentGravityForce = _gravityForce;
    }
    
    private void Update()
    {
        
        CheckGround();
        Move();
        
    }
    

    private void SetMovementInput(Vector2 movementInput) => _movementInput = movementInput;

    public void Move()
    {
        Vector3 direction = Vector3.zero;
        if (_movementInput.sqrMagnitude > 0.0001f)
        {
            direction = _camera.transform.right * _movementInput.x + _camera.transform.forward * _movementInput.y;
            if (direction.sqrMagnitude > 1f)
                direction.Normalize();
            direction.y = 0;
        }
        
        transform.position += (direction * _moveSpeed  + Vector3.up * _fallVelocity ) * Time.deltaTime;
        
    }
    
    public void Jump()
    {
        if(!_isGrounded)
            return;
        _fallVelocity = _jumpForce;
    }

    public void Glide(bool isGliding)
    {
        if (isGliding)
        {
            _currentGravityForce = _glideGravityForce;
            _glideParticles.Play();
        }
        else
        {
            _currentGravityForce = _gravityForce;
            _glideParticles.Stop();
        }
    }

    public void CheckGround()
    {
        Ray groundRay = new Ray(transform.position, _gravityDirection);
        if (Physics.Raycast(groundRay, out RaycastHit hit , _checkGroundRange))
        {

            if (!_isGrounded)
            {
                _isGrounded = true;
                _fallVelocity = 0;
                InputController.Instance.ChangeToGroundInput();
            }
            Debug.Log("Toccando");
        }
        else
        {
            if (_isGrounded)
            {
                _isGrounded = false;
                InputController.Instance.ChangeToGlideInput();
            }
            
            Fall();
        }
    }

    public void Fall()
    {
        
        InputController.Instance.ChangeToGlideInput();
        Debug.Log(_gravityForce + " / " + _currentGravityForce);
        _fallVelocity += -_currentGravityForce * Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, _gravityDirection * _checkGroundRange);
    }
}
