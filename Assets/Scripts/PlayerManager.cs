using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerManager : MonoBehaviour
{
    public static event System.Action OnCoinTaken,OnKeyTaken,OnMazeCompleted,OnDead;
    public static event System.Action<float> OnTakedDamage;

    private float _maxHealth=100f;
    private float _health;
    private float _standardDamage = 10f;
    private bool _isInvincible;
    private PlayerMovement _playerMovement;
    [SerializeField] private float _playerWalkSpeed, _playerBoostSpeed;
    [SerializeField] private int _standardCameraZoom, _boostedCameraZoom;
    [SerializeField] private PixelPerfectCamera _pixelPerfectCamera;

    // Start is called before the first frame update
    void Awake()
    {
        OnCoinTaken = new System.Action(() => { });
        OnKeyTaken = new System.Action(() => { });
        OnMazeCompleted = new System.Action(() => { });
        OnDead = new System.Action(() => { });
    }

    private void Start()
    {
        _health = _maxHealth;
        _playerMovement = GetComponent<PlayerMovement>();
        _playerMovement.PlayerSpeed = _playerWalkSpeed;

        OnDead += () => { _health = _maxHealth; };
        MazeGenerator.OnMazeGenerated += () => { _health = _maxHealth; };
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            collision.gameObject.SetActive(false);
            OnCoinTaken.Invoke();
        }
        else if (collision.CompareTag("Key"))
        {
            collision.gameObject.SetActive(false);
            OnKeyTaken.Invoke();
        }
        else if (collision.CompareTag("Ladder"))
        {
            OnMazeCompleted.Invoke();
        }
        else if (collision.CompareTag("SpeedPowerUp"))
        {
            collision.gameObject.SetActive(false);
            _playerMovement.PlayerSpeed = _playerWalkSpeed;
            StopCoroutine(SpeedUpgradeTimer());
            StartCoroutine(SpeedUpgradeTimer());
        }else if (collision.CompareTag("ViewMazePowerUp"))
        {
            collision.gameObject.SetActive(false);
            _pixelPerfectCamera.assetsPPU = _standardCameraZoom;
            StopCoroutine(CameraZoomTimer());
            StartCoroutine(CameraZoomTimer());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_isInvincible) return;

        if (collision.CompareTag("Enemy"))
        {
            OnTakedDamage.Invoke(_standardDamage);
            _health -= _standardDamage;
            StartCoroutine(InvincibleTimer());

            if (_health <= 0)
            {
                OnDead.Invoke();
            }
        }

    }

    IEnumerator InvincibleTimer()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        _isInvincible = true;

        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(3f);

        _isInvincible = false;
        spriteRenderer.color = Color.white;
    }

    IEnumerator SpeedUpgradeTimer()
    {
        _playerMovement.PlayerSpeed = _playerBoostSpeed;

        yield return new WaitForSeconds(4f);

        _playerMovement.PlayerSpeed = _playerWalkSpeed;
    }

    IEnumerator CameraZoomTimer()
    {
        _pixelPerfectCamera.assetsPPU = _boostedCameraZoom;

        yield return new WaitForSeconds(3f);

        _pixelPerfectCamera.assetsPPU = _standardCameraZoom;
    }
}
