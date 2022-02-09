using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private const string VERTICAL_AXIS = "Vertical";
    private const string HORIZONTAL_AXIS = "Horizontal";
    public Text ScoreText;
    public Text WinText;
    public Text LivesText;
    public float Speed = 5F;
    public float TableFriction = 5F;
    public float FrictionRampSpeed = 1F;
    public float FrictionRamp = 2.5F;

    private Rigidbody _player;

    private int _score;
    private int _lives;
    private Vector3 _playerStart;
    private GameObject[] _allPickups;
    private Color _winTextColor;
    private bool _gameLost;

    private void Start()
    {
        _lives = 2;
        _player = GetComponent<Rigidbody>();
        _playerStart = _player.position;
        Log("Player created");
        _allPickups = GameObject.FindGameObjectsWithTag("pickup");
        _winTextColor = WinText.color;
        HideWinText();
        UpdateLivesDisplay();
    }

    private void HideWinText()
    {
        WinText.color = new Color(0, 0, 0, 0);
        WinText.text = "YOU LOST!";
    }

    private void ShowWinText()
    {
        WinText.color = _winTextColor;
        WinText.text = "YOU WON!";
    }
    private void ShowLoseText()
    {
        WinText.color = _winTextColor;
        WinText.text = "YOU LOST!\nPress R to play again";
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            Reset();
        }
    }

    private void Reset()
    {
        GameObject.FindGameObjectWithTag("menusound").GetComponent<AudioSource>().Play();

        ShowPlayer();

        _score = 0;
        _lives = 2;
        _player.velocity = new Vector3(0, 0, 0);
        _player.position = _playerStart;
        foreach (var pickup in _allPickups)
        {
            pickup.SetActive(true);
        }

        var shells = GameObject.FindGameObjectsWithTag("tankshell");
        foreach (var shell in shells)
        {
            Destroy(shell);
        }

        UpdateLivesDisplay();
        UpdateScoreDisplay();
        HideWinText();
    }

    void OnTriggerEnter(Collider other)
    {
        var gameObject = other.gameObject;
        Log("Entering trigger: {0}", gameObject.tag);
        if (gameObject.CompareTag("pickup"))
        {
            if (!_gameLost)
            {
                IncrementScore();
                gameObject.SetActive(false);
                GameObject.FindGameObjectWithTag("pickupsound").GetComponent<AudioSource>().Play();
            }
        }

        if (gameObject.CompareTag("tankshell"))
        {
            Log("We reached here");
            Destroy(gameObject);
            DoOnHitStuff();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        var gameObject = collision.gameObject;
        Log("Colliding with: {0}", gameObject.tag);

        if (gameObject.CompareTag("tank"))
        {
            Log("We reached here -- hit a tank");
            DoOnHitStuff();
        }
    }

    private void DoOnHitStuff()
    {
        if (!_gameLost)
        {
            GameObject.FindGameObjectWithTag("onhitsound").GetComponent<AudioSource>().Play();
            DecrementLives();

            if (_lives <= 0)
            {
                HidePlayer();
                ShowLoseText();
            }
        }
    }
    private void ShowPlayer()
    {
        _player.position = _playerStart;
        _player.velocity = new Vector3(0, 0, 0);

        _player.GetComponent<MeshRenderer>().enabled = true;
        _gameLost = false;
    }

    private void HidePlayer()
    {
        //_player.position = new Vector3(0, -10, 0);
        //_player.velocity = new Vector3(0, 0, 0);

        _player.GetComponent<MeshRenderer>().enabled = false;
        _gameLost = true;
    }

    private void IncrementScore()
    {
        _score++;
        UpdateScoreDisplay();
    }

    private void DecrementLives()
    {
        if (_lives > 0)
        {
            _lives--;
        }
        UpdateLivesDisplay();
    }

    private void UpdateScoreDisplay()
    {
        if (ScoreText)
        {
            ScoreText.text = string.Format("Score: {0}", _score);
            if (_score == _allPickups.Length)
            {
                if (Globals.level == 7)
                {
                    ShowWinText();
                }
                else
                {
                    GameObject.FindGameObjectWithTag("menusound").GetComponent<AudioSource>().Play();
                    
                    Invoke("GoToNextLevel", 1.5f);
                }
            }
        }
    }

    private void GoToNextLevel()
    {
        SceneManager.LoadScene("Level" + ++Globals.level);
    }

    private void UpdateLivesDisplay()
    {
        if (LivesText)
        {
            LivesText.text = string.Format("Lives: {0}", _lives);
            if (_lives == 0)
            {
                ShowLoseText();
            }
        }
    }

    public void FixedUpdate()
    {
        UpdatePlayerForceFromInput();
        //ApplyTableSurfaceFriction();
    }

    private void UpdatePlayerForceFromInput()
    {
        if (!_gameLost)
        {
            var moveX = Input.GetAxis(HORIZONTAL_AXIS);
            var moveY = Input.GetAxis(VERTICAL_AXIS);

            var force = new Vector3(moveX, 0, moveY);
            _player.AddForce(Speed * force);
        }
    }

    private void ApplyTableSurfaceFriction()
    {
        var velocity = _player.velocity;
        var decelX = GetDecellerationFor(velocity.x);
        var decelZ = GetDecellerationFor(velocity.z);
        _player.AddForce(new Vector3(decelX, 0, decelZ));
    }

    private void Log(string message, params object[] args)
    {
        Debug.Log(string.Format(message, args));
    }

    private float GetDecellerationFor(float f)
    {
        var absoluteForce = Math.Abs(f);
        var situationMultiplier = absoluteForce < FrictionRampSpeed ? FrictionRamp : 1;   // the slower you're going, the more friction of the table matters
        var direction = f < 0 ? 1 : -1; // should oppose the current movement
        var max = (float)(TableFriction * direction * situationMultiplier * 0.1);
        return Math.Abs(max) < absoluteForce ? -1 * absoluteForce : max;
    }
}
