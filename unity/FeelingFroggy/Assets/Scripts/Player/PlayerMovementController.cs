using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("Physics")]
    public float speed = 10;
    public float jumpForce = 200;
    public float maxAngularVelocity = 12;

    [Header("Sound")]
    public AudioClip bounceSound;
    public float bounceVolumeMultiplier = 0.1f;
    public float jumpBounceVolumeMultiplier = 0.1f;

    private PlayerController _player;
    private Rigidbody _rigidbody;
    private AudioSource _audioSource;

    private Vector3 _movement;
    private bool _pressingJump;

    private Vector3 _platformContactsNormalSum;
    private List<ContactPoint> _platformContacts;

    private void Awake()
    {
        _player = GetComponent<PlayerController>();
        _rigidbody = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();

        _movement = Vector3.zero;
        _pressingJump = false;

        _platformContacts = new List<ContactPoint>(32);
    }

    private void Start()
    {
        _rigidbody.maxAngularVelocity = maxAngularVelocity;
    }

    public void StopPlayer()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }

    public void FreezePlayer(bool freezeGravity = true)
    {
        // freeze player rigidbody
        var freezeConstraints = RigidbodyConstraints.FreezeAll;

        if (!freezeGravity)
        {
            // Remove freeze position Y flag

            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            freezeConstraints &= ~RigidbodyConstraints.FreezePositionY;
        }

        _rigidbody.constraints = freezeConstraints;
    }

    public void UnfreezePlayer()
    {
        // unfreeze player rigidbody
        _rigidbody.constraints = RigidbodyConstraints.None;
    }

    private void FixedUpdate()
    {
        if (GameController.Instance.IsPlayerInputDisabled)
        {
            return;
        }

        var movementRotated = Quaternion.Euler(0, _player.Camera.camera.transform.eulerAngles.y, 0) * _movement;

        _rigidbody.AddForce(movementRotated * speed);

        if (_pressingJump)
        {
            _platformContactsNormalSum.Normalize();

            if (_platformContactsNormalSum != Vector3.zero)
            {
                _rigidbody.AddForce(_platformContactsNormalSum * jumpForce);
                PlayBounceSound(jumpBounceVolumeMultiplier);
            }
        }

        _platformContactsNormalSum = Vector3.zero;
    }

    [UsedImplicitly]
    private void OnMove(InputValue movementValue)
    {
        var movementVector = movementValue.Get<Vector2>();

        _movement.x = movementVector.x;
        _movement.z = movementVector.y;
    }

    [UsedImplicitly]
    private void OnJump(InputValue jumpValue)
    {
        _pressingJump = jumpValue.isPressed;
    }

    private void OnCollisionStay(Collision collision)
    {
        collision.GetContacts(_platformContacts);

        foreach (var platformContact in _platformContacts)
        {
            _platformContactsNormalSum += platformContact.normal;
        }

        _platformContacts.Clear();
    }

    private void PlayBounceSound(float volume)
    {
        if (bounceSound == null)
        {
            return;
        }

        _audioSource.PlayOneShot(bounceSound, volume);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Don't play bounce sound on initial loading collision
        if (Time.timeSinceLevelLoad < 1)
        {
            return;
        }

        //var audioLevel = collision.relativeVelocity.magnitude * bounceVolumeMultiplier;
        var audioLevel = collision.impulse.magnitude * bounceVolumeMultiplier;
        PlayBounceSound(audioLevel);
    }
}
