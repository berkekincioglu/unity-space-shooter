using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour{
    [SerializeField]
    private float _speed = 7.0f;
    private float _speedMultiplier = 2.0f;
    [SerializeField]
    private GameObject _laserPrefabs;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;
    [SerializeField]
    private GameObject _tripleShotPrefabs;
    [SerializeField]
    private bool _isTripleShotActive = false;
    [SerializeField]
    private bool _isSpeedUpActive = false;
    [SerializeField]
    private bool _isShieldActive = false;
    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private int _score;
    [SerializeField]
    private GameObject[] _engines;

    [SerializeField]
    private AudioClip _laserSoundClip;

    private AudioSource _audioSource;

    private UIManager _uimanager;


    // Start is called before the first frame update
    public float fireRate = 0.5f;
    public float nextFire = 0.0f;

    void Start()
    {
        transform.position = new Vector3(0, 2, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _audioSource = GetComponent<AudioSource>();
        _uimanager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if(_spawnManager == null)
        {
            Debug.Log("Spawn Manager is null");
        }

        if (_uimanager == null)
        {
            Debug.Log("UIManager is null");
        }
        if(_audioSource == null)
        {
            Debug.Log("Audio Source is null");
        }

        _audioSource.clip = _laserSoundClip;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        //if (Input.GetKeyDown(KeyCode.Space) && Time.time > nextFire) // cooldown
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireLaser();
        }

        
    }

    void FireLaser()
    {
        Vector3 laserPosition = new Vector3(transform.position.x, transform.position.y + .8f, 0);
        nextFire = Time.time + fireRate;

        if (_isTripleShotActive)
        {
            Instantiate(_tripleShotPrefabs, transform.position, Quaternion.identity);

        } else
        {
            Instantiate(_laserPrefabs, laserPosition, Quaternion.identity);
        }

        _audioSource.Play();

        
    }
    

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        //transform.Translate(Vector3.right * horizontalInput * speed * Time.deltaTime);   
        //transform.Translate(Vector3.up * verticalInput * speed * Time.deltaTime);

        //transform.Translate(new Vector3(horizontalInput, verticalInput, 0) * speed * Time.deltaTime);

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        if (_isSpeedUpActive)
        {
            transform.Translate(direction * _speed * _speedMultiplier*Time.deltaTime);
        }

        transform.Translate(direction * _speed * Time.deltaTime);

        //if (transform.position.y >= 0)
        //{
        //    transform.position = new Vector3(transform.position.x, 0, 0);
        //}else if(transform.position.y < 0)
        //{
        //    transform.position = new Vector3(transform.position.x, 0, 0);
        //}

        //if (transform.position.x >= 11)
        //{
        //    transform.position = new Vector3(-11, transform.position.y, 0);

        //} else
        //{
        //    transform.position = new Vector3(11, transform.position.y, 0);
        //}
        if(transform.position.x < -10) {
            transform.position = new Vector3(10,transform.position.y,0);
        }
        if (transform.position.x > 10) {
            transform.position = new Vector3(-10, transform.position.y, 0);
        }


        if (transform.position.y < 0) {
            transform.position = new Vector3(transform.position.x,0, 0);
        }
        if (transform.position.y >= 10) {

            transform.position = new Vector3(transform.position.x, 10, 0);
        }


    }
    public void AddScore()
    {
        _score += 10;
        _uimanager.UpdateScore(_score);
    }

    public void Damage()
    {

        if (!_isShieldActive)
        {
            _lives--;
            _uimanager.UpdateLives(_lives);
            int ranEngine = Random.Range(0, 2);
            if(_lives == 2)
            {
                _engines[ranEngine].gameObject.SetActive(true);
            }
            if(_lives == 1)
            {
                _engines[0].gameObject.SetActive(true);
                _engines[1].gameObject.SetActive(true);
            }
        }

        if(_lives <1)
        {
            _spawnManager.OnPlayerDead();
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }
    public void SpeedUpActive()
    {
        _isSpeedUpActive = true;
        StartCoroutine(SpeedUpPowerDownRoutine());
    }
    public void ShieldActive()
    {
        _isShieldActive = true;
        _shieldVisualizer.SetActive(true);
        StartCoroutine(ShieldPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }
    IEnumerator SpeedUpPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedUpActive = false;
    }
    IEnumerator ShieldPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _shieldVisualizer.SetActive(false);
        _isShieldActive = false;
    }
}
