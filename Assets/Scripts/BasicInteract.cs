using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicInteract : MonoBehaviour
{

    private Ray g_ray = new Ray();
    public RaycastHit hitObject;
    public LayerMask layerToHit;
    public GraphicRaycaster raycaster;
    public float rayLength = 5f;

    public bool rayHit;

    public Image CrosshairDot;
    private bool nearInteractable;
    private int numberInteractables;
    public bool canInteract;
    public GameObject interactiveObject;

    [SerializeField]
    private AudioClip soundToPlay;
    public AudioSource sourceToPlay; // THIS NEEDS TO BE AN AUDIOSOURCE COMPONENT IN YOUR LEVEL! Maybe 'SFXSytem'
    public float volume;

    // Start is called before the first frame update
    void Start()
    {
        rayHit = false;
        canInteract = false;
        numberInteractables = 0;
        if (sourceToPlay == null)
        {
            sourceToPlay = GameObject.Find("SFXSystem").GetComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        g_ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Raycast From Mouse Position if player is near an ally

        if (Physics.Raycast(g_ray, out hitObject, rayLength, layerToHit)) // If raycast hits collider 
        {
            if (hitObject.collider.tag == "Interact") // if raycast hits collider with tag
            {
                rayHit = true;
                interactiveObject = hitObject.collider.gameObject;
                //Debug.Log("Cursor over =" +interactiveObject);
            }
        }
        else // resets Raycast
        {
            rayHit = false;
            interactiveObject = null;
            canInteract = false;
            //Debug.Log("allyToDirect = null");
        }

        //GameObject.Find("CrosshairDot").GetComponent<Image>();

        if (rayHit == true) //Turns the Crosshair green
        {
            //Debug.Log("RaycastHit is firing");
            CrosshairDot.gameObject.SetActive(true);
            //CrosshairDot.GetComponent<Renderer>();
            //CrosshairDot.color = Color.green;
            canInteract = true;
        }

        if (rayHit == false) // resets colour of crosshair
        {
            CrosshairDot.gameObject.SetActive(false);
            //CrosshairDot.GetComponent<Renderer>();
            //CrosshairDot.color = Color.white;
            canInteract = false;
        }

        if (canInteract == true)
        {
            if (Input.GetKeyDown(KeyCode.E) && (interactiveObject != null))
            {
                //Debug.Log("E key registered!");
                interactiveObject.GetComponent<LightSwitch>().lightSwitchToggle();
                PlaySoundClip();
            }
        }

    }

    public void PlaySoundClip()
    {
        sourceToPlay.PlayOneShot(soundToPlay, volume); //THIS PLAYS IT AT THE PLAYER LOCATION
    }


    public void AddInt()
    {
        numberInteractables++;
    }

    public void SubInt()
    {
        numberInteractables--;
    }
}
