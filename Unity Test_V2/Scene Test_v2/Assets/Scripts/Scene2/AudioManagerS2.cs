using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AudioManagerS2 : MonoBehaviour {

    AudioSource guideAudioS2;
    public AudioClip[] audioClips;

    public GameObject leftController;
    MagicMoveV4 mm4Script;
    CapsulatedIngestionV2 capsIngestScript;
    public ParticleSystem particleWeapon;
    ParticleCollision pcolScript;
    public float timer = 120f;
    bool isTimerStart;
    public TextMeshPro header;
    public TextMeshPro missionDoneText;
    public TextMeshPro exitText;
    public TextMeshPro deathText;
    public GameObject creditCamera;
    public GameObject deathNoteCamera;
    public GameObject endGameNote;
    public SelfDestructionCountdown destrucScript;
    public PlayerHealth healthScript;
    Animator animEndGameNote;

    Teleportation teleportScriptR;
    Teleportation teleportScriptL;

    bool isC1Played;
    bool isC2Played;
    bool isC3Played;
    bool isC4Played;
    bool isC5Played;
    bool isC6Played;
    bool isC7Played;

    public bool isWeapon1Unlock;
    public bool isWeapon2Unlock;
    public bool isWeapon3Unlock;

    public TimerCountdown countdownScript;
    public PlayerHealth health;

    bool playerDied;

    public GameObject weaponControlPanelIndicator;
    public WeaponShift weaponShiftScript;

    private void Awake()
    {
        guideAudioS2 = GetComponent<AudioSource>();
        mm4Script = GetComponent<MagicMoveV4>();
        capsIngestScript = leftController.GetComponent<CapsulatedIngestionV2>();
        pcolScript = particleWeapon.GetComponent<ParticleCollision>();
        
        teleportScriptR = GetComponent<Teleportation>();
        teleportScriptL = leftController.GetComponent<Teleportation>();

        animEndGameNote = endGameNote.GetComponent<Animator>();

        weaponControlPanelIndicator.SetActive(false);
    }


    // Use this for initialization
    void Start () {
 
        guideAudioS2.clip = audioClips[0]; //intro and movement
        guideAudioS2.PlayDelayed(3f);
        //guideAudioS2.Play();

        if (!countdownScript.isCountDownStart)
        {
            countdownScript.isCountDownStart = true;
        }

        if (!healthScript.isStart)
        {
            healthScript.isStart = true;
        }

    }
	
	// Update is called once per frame
	void Update () {

        if (mm4Script.dist <= 12f && mm4Script.isMoving && !guideAudioS2.isPlaying && !isC1Played)
        {
            isC1Played = true;
            isWeapon1Unlock = true;
            guideAudioS2.PlayOneShot(audioClips[1], 1f); // degranulation weapon

            StartCoroutine(WeaponPanelHighlight());
        }

        if (weaponShiftScript.isParticle)
        {
            weaponControlPanelIndicator.SetActive(false);
        }


        if (ScoreManager.score >= 100 && !guideAudioS2.isPlaying && !isC2Played )
        {
            isC2Played = true;
            isWeapon2Unlock = true;
            guideAudioS2.PlayOneShot(audioClips[2], 1f); //phagocytosis intro
        }


        if(capsIngestScript.isEncapsulated && !guideAudioS2.isPlaying && !isC3Played)
        {
            isC3Played = true;
            guideAudioS2.PlayOneShot(audioClips[3], 1f); // phagocytosis encapsulation
            isTimerStart = true;
        }

        if (isTimerStart)
        {
            timer -= Time.deltaTime;
        }


        if ((pcolScript.isBacteriaBoss && !isC4Played && !guideAudioS2.isPlaying) || (capsIngestScript.isEnemyTooBig && !isC4Played && !guideAudioS2.isPlaying))
        {
            isC4Played = true;
            isWeapon3Unlock = true;
            guideAudioS2.PlayOneShot(audioClips[4], 1f); // NETS weapon
        }


        if(timer < 0f && !isC5Played && !guideAudioS2.isPlaying && !isC4Played)
        {
            isC5Played = true;
            isWeapon3Unlock = true;
            guideAudioS2.PlayOneShot(audioClips[8], 1f); // NETS v2 (if user doesn't apply phagocytosis to big bacteria)
        }


        if((teleportScriptL.isTeleported && !isC6Played) || (teleportScriptR.isTeleported && !isC6Played))
        {
            isC6Played = true;
            guideAudioS2.clip = audioClips[6]; // after teleportation 
            guideAudioS2.PlayDelayed(2f);
        }


        if(((int)PlayerHealth.CurrentHealth + ScoreManager.score) > 900 && !isC7Played && !guideAudioS2.isPlaying)
        {
            isC7Played = true;
            guideAudioS2.PlayOneShot(audioClips[7], 1f); // mission accomplished and exit experience
            missionDoneText.color = Color.green;
            missionDoneText.text = "Congrats, your mission has accomplished!";
            header.text = "";
            StartCoroutine("FadeScreen");
            if (!isC5Played)
            {
                isC5Played = true;
            }
        } 


        if(destrucScript.m_leftTime < 0 && !playerDied || healthScript.healthCombined == 0 && !playerDied)
        {
            playerDied = true;
            StartCoroutine("FadeScreenRestart");
        }

    }


    IEnumerator FadeScreen()
    {
        yield return new WaitForSeconds(15);
        SteamVR_Fade.Start(Color.clear, 0);
        SteamVR_Fade.Start(Color.black, 2);
        yield return new WaitForSeconds(1);
        creditCamera.SetActive(true);
        yield return new WaitForSeconds(5);
        animEndGameNote.SetTrigger("showNote");
    }


    IEnumerator FadeScreenRestart()
    {
        yield return new WaitForSeconds(5);
        deathNoteCamera.SetActive(true);
       
    }


    IEnumerator WeaponPanelHighlight()
    {
        yield return new WaitForSeconds(14f);
        weaponControlPanelIndicator.SetActive(true);
    }
}
