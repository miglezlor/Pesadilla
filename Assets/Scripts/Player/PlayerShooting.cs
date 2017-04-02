using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    public int damagePerShot = 20;
    public float timeBetweenBullets = 0.15f;
    public float range = 100f;
    public Light pointLight;
    public Slider ammoSlider;
    public int maxBullets = 100;
    public Image enemyImage;
    public Slider enemyHealthSlider;


    float timer;
    Ray shootRay = new Ray();
    RaycastHit shootHit;
    int shootableMask;
    ParticleSystem gunParticles;
    LineRenderer gunLine;
    AudioSource gunAudio;
    Light gunLight;
    float effectsDisplayTime = 0.2f;
    int currentBullets;

    void Awake ()
    {
        shootableMask = LayerMask.GetMask ("Shootable");
        gunParticles = GetComponent<ParticleSystem> ();
        gunLine = GetComponent <LineRenderer> ();
        gunAudio = GetComponent<AudioSource> ();
        gunLight = GetComponent<Light> ();
        currentBullets = maxBullets;
        StartCoroutine(ReloadAmmo());
        enemyImage.enabled = false;
        enemyHealthSlider.gameObject.SetActive(false);
    }

    IEnumerator ReloadAmmo()
    {
        float reloadTime = timeBetweenBullets * 2;
        while (true)
        {
            yield return new WaitForSeconds(reloadTime);
            if (currentBullets < maxBullets)
            {
                currentBullets++;
                ammoSlider.value = currentBullets;
            }
        }
    }

    IEnumerator ScheduleHideEnemyUI()
    {
        yield return new WaitForSeconds(2);
        HideEnemyUI();
    }

    void HideEnemyUI()
    {
        enemyImage.enabled = false;
        enemyHealthSlider.gameObject.SetActive(false);
    }

    void Update ()
    {
        timer += Time.deltaTime;

		if(Input.GetButton ("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0)
        {
            Shoot ();
        }

        if(timer >= timeBetweenBullets * effectsDisplayTime)
        {
            DisableEffects ();
        }
    }


    public void DisableEffects ()
    {
        gunLine.enabled = false;
        gunLight.enabled = false;
        pointLight.enabled = false;
    }


    void Shoot ()
    {
        if (currentBullets <= 0) return;

        currentBullets--;
        ammoSlider.value = currentBullets;

        timer = 0f;

        gunAudio.Play ();

        gunLight.enabled = true;
        pointLight.enabled = true;

        gunParticles.Stop ();
        gunParticles.Play ();

        gunLine.enabled = true;
        gunLine.SetPosition (0, transform.position);

        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        if(Physics.Raycast (shootRay, out shootHit, range, shootableMask))
        {
            EnemyHealth enemyHealth = shootHit.collider.GetComponent <EnemyHealth> ();
            if(enemyHealth != null)
            {
                enemyHealth.TakeDamage (damagePerShot, shootHit.point);
                enemyImage.enabled = true;
                enemyImage.sprite = enemyHealth.icon;
                enemyHealthSlider.gameObject.SetActive(true);
                enemyHealthSlider.value = enemyHealth.GetHealthPercentage();
                StopCoroutine("ScheduleHideEnemyUI");
                StartCoroutine("ScheduleHideEnemyUI");
            }
            else
            {
                enemyImage.enabled = false;
                enemyHealthSlider.gameObject.SetActive(false);

            }
            gunLine.SetPosition (1, shootHit.point);
        }
        else
        {
            gunLine.SetPosition (1, shootRay.origin + shootRay.direction * range);
        }
    }
}
