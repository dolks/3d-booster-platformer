using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    Rigidbody rocketRigidbody;
    AudioSource rocketAudioSource;
    [SerializeField] AudioClip thrustSFX;
    [SerializeField] AudioClip crashSFX;
    [SerializeField] AudioClip finishSFX;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem explosionParticles;
    [SerializeField] ParticleSystem thrustParticleMain;
    [SerializeField] ParticleSystem thrustParticleRight;
    [SerializeField] ParticleSystem thrustParticleLeft;
    [SerializeField] float thrust = 10;
    [SerializeField] float rotationThrust = 200;
    bool isTransitioning = false;
    // Start is called before the first frame update
    void Start()
    {
        rocketRigidbody = GetComponent<Rigidbody>();
        rocketAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessRotation();
        ProcessThrust();
    }

    void ProcessThrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (!rocketAudioSource.isPlaying)
            {
                rocketAudioSource.PlayOneShot(thrustSFX);
            }
            thrustParticleMain.Play();
            rocketRigidbody.AddRelativeForce(Vector3.up * thrust * Time.deltaTime);
        }
        else
        {
            rocketAudioSource.Stop();
            thrustParticleMain.Stop();
        }
    }

    void ProcessRotation()
    {
        rocketRigidbody.freezeRotation = true;
        if (Input.GetKey(KeyCode.A))
        {
            thrustParticleRight.Play();
            transform.Rotate(Vector3.forward * rotationThrust * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            thrustParticleLeft.Play();
            transform.Rotate(Vector3.back * rotationThrust * Time.deltaTime);
        } else
        {
            if (thrustParticleLeft.isPlaying)
            {
                thrustParticleLeft.Stop();
            }
            if (thrustParticleRight.isPlaying)
            {
                thrustParticleRight.Stop();
            }
        }

        rocketRigidbody.freezeRotation = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                Debug.Log("Touched Friendly");
                break;
            case "Fuel":
                Debug.Log("Touched Fuel");
                break;
            case "Finish":
                FinishLevel();
                break;
            default:
                Crash();
                break;
        }
    }

    private void FinishLevel()
    {
        successParticles.Play();
        if (!isTransitioning)
        {
            AudioSource.PlayClipAtPoint(finishSFX, Camera.main.transform.position);
            isTransitioning = true;
        }
        Invoke("LoadNextLevel", 1);
    }


    private void Crash()
    {
        GetComponent<Movement>().enabled = false;
        explosionParticles.Play();
        if (!isTransitioning)
        {
            rocketAudioSource.PlayOneShot(crashSFX);
            isTransitioning = true;
        }
        Invoke("ReloadLevel", 1);
    }

    private void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
