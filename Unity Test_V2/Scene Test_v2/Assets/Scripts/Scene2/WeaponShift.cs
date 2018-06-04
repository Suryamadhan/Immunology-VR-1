using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using TMPro;

public class WeaponShift : MonoBehaviour
{


    public bool isParticle;
    public bool isIngest;
    public bool isNETS;

    public GameObject particleButton;
    public GameObject ingestButton;
    public GameObject netsButton;

    public Material mButtonUnlit;
    public Material mButtonLit;
    public Material mButtonLitNETS;

    //public GameObject displayUI;
    public TextMeshPro weaponText;
    public TextMeshPro header;
    

    public GameObject aimingReticle;


    private ParticleLauncher particle;
    private CapsulatedIngestionV2 ingest;
    private NETSv1 nets;


    private Renderer rendP;
    private Renderer rendI;
    private Renderer rendN;

    public GameObject teleButtonAssembly;
    private Animator animTeleButton;

    public GameObject rightController;
    private AudioManagerS2 AM2Script;
    private AudioSource errorAudio;
    public AudioClip error;

    // Use this for initialization
    void Awake()
    {

        particle = GetComponent<ParticleLauncher>();
        ingest = GetComponent<CapsulatedIngestionV2>();
        nets = GetComponent<NETSv1>();


        particle.enabled = false;
        ingest.enabled = false;
        nets.enabled = false;

        rendP = particleButton.gameObject.GetComponent<Renderer>();
        rendI = ingestButton.gameObject.GetComponent<Renderer>();
        rendN = netsButton.gameObject.GetComponent<Renderer>();

        //weaponText = displayUI.GetComponent<TextMeshPro> ();

        aimingReticle.SetActive(false);

        animTeleButton = teleButtonAssembly.GetComponent<Animator>();

        AM2Script = rightController.GetComponent<AudioManagerS2>();
        errorAudio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        rendP.material = mButtonUnlit;
        rendI.material = mButtonUnlit;
        rendN.material = mButtonUnlit;

        weaponText.text = "Kill pathogens and increase host health to 900";
         
    }

    // Update is called once per frame
    void Update()
    {
        if (isParticle)
        {
            particle.enabled = true;
            aimingReticle.SetActive(true);

        }
        else
        {
            particle.enabled = false;
            aimingReticle.SetActive(false);

        }

        if (isIngest)
        {
            ingest.enabled = true;

        }
        else
        {
            ingest.enabled = false;

        }

        if (isNETS)
        {
            nets.enabled = true;

        }
        else
        {
            nets.enabled = false;

        }


    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Particle Weapon" && AM2Script.isWeapon1Unlock)
        {
            isParticle = true;
            isIngest = false;
            isNETS = false;

            rendP.material = mButtonLit;
            rendI.material = mButtonUnlit;
            rendN.material = mButtonUnlit;

            weaponText.color = new Color32(255, 255, 255, 255);
            weaponText.text = "Degranulation Gun Activated!";
            header.text = "[status]";


            if (animTeleButton != null)
            {
                animTeleButton.SetBool("isButtonOut", false);
            }

        }

        if (other.gameObject.tag == "Particle Weapon" && !AM2Script.isWeapon1Unlock)
        {
            StartCoroutine(WeaponLockWarning());
        }



        if (other.gameObject.tag == "Ingest Weapon" && AM2Script.isWeapon2Unlock)
        {
            isIngest = true;
            isParticle = false;
            isNETS = false;

            rendI.material = mButtonLit;
            rendP.material = mButtonUnlit;
            rendN.material = mButtonUnlit;

            weaponText.color = new Color32(255, 255, 255, 255);
            weaponText.text = "Phagocytosis Reactor Activated!";
            header.text = "[status]";

            if (animTeleButton != null)
            {
                animTeleButton.SetBool("isButtonOut", false);
            }

        }

        if (other.gameObject.tag == "Ingest Weapon" && !AM2Script.isWeapon2Unlock && AM2Script.isWeapon1Unlock)
        {
            StartCoroutine(WeaponLockWarning2());
        }

        if (other.gameObject.tag == "Ingest Weapon" && !AM2Script.isWeapon1Unlock && !AM2Script.isWeapon2Unlock && !AM2Script.isWeapon3Unlock)
        {
            StartCoroutine(WeaponLockWarning());
        }


        if (other.gameObject.tag == "NETS Weapon" && AM2Script.isWeapon3Unlock)
        {
            isNETS = true;
            isIngest = false;
            isParticle = false;

            rendN.material = mButtonLitNETS;
            rendP.material = mButtonUnlit;
            rendI.material = mButtonUnlit;

            weaponText.color = new Color32(255, 255, 255, 255);
            weaponText.text = "Neutrophil Extracellular Traps(NETS) Activated!";
            header.text = "[status]";

            if (animTeleButton != null)
            {
                animTeleButton.SetBool("isButtonOut", true);
            }

        }

        if (other.gameObject.tag == "NETS Weapon" && !AM2Script.isWeapon3Unlock)
        {
            StartCoroutine(WeaponLockWarning());
        }

    }


    IEnumerator WeaponLockWarning()
    {
        errorAudio.PlayOneShot(error, 2f);
        weaponText.color = Color.red;
        weaponText.text = "Weapon is locked now. Please wait till it's been introduced.";
        header.text = "";
        header.color = Color.red;

        yield return new WaitForSeconds(3f);
        weaponText.color = new Color32(255, 255, 255, 255);
        header.color = new Color32(255, 78, 0, 255);

        if(!isParticle && !isIngest && !isNETS)
        {
            weaponText.text = "Kill pathogens and increase host health to 900";
            header.text = "[mission]";
        }

        if (isParticle)
        {
            weaponText.text = "Degranulation Gun Activated!";
            header.text = "[status]";
        }

        if (isIngest)
        {
            weaponText.text = "Phagocytosis Reactor Activated!";
            header.text = "[status]";
        }
    }

    IEnumerator WeaponLockWarning2()
    {
        errorAudio.PlayOneShot(error, 2f);
        weaponText.color = Color.red;
        weaponText.text = "Please kill 1 bacterium with degranulation to unlock this weapon.";
        header.text = "";
        header.color = Color.red;

        yield return new WaitForSeconds(3f);
        weaponText.color = new Color32(255, 255, 255, 255);
        header.color = new Color32(255, 78, 0, 255);

        if (!isParticle && !isIngest && !isNETS)
        {
            weaponText.text = "Kill pathogens and increase host health to 900";
            header.text = "[mission]";
        }

        if (isParticle)
        {
            weaponText.text = "Degranulation Gun Activated!";
            header.text = "[status]";
        }

        if (isIngest)
        {
            weaponText.text = "Phagocytosis Reactor Activated!";
            header.text = "[status]";
        }
    }
}
