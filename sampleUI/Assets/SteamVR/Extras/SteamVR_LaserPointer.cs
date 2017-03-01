using UnityEngine;
using System.Collections;

public struct PointerEventArgs
{
    public uint controllerIndex;
    public uint flags;
    public float distance;
    public Transform target;
}

public delegate void PointerEventHandler(object sender, PointerEventArgs e);


public class SteamVR_LaserPointer : MonoBehaviour
{
    public GameObject rig;
    public GameObject head;
    public bool active = true;
    public Color color;
    public float thickness = 0.002f;
    public GameObject holder;
    public GameObject pointer;
    bool isActive = false;
    public bool addRigidBody = false;
    public Transform reference;
    public event PointerEventHandler PointerIn;
    public event PointerEventHandler PointerOut;
    private Vector3 destination;
    private bool pressed = false;

    Transform previousContact = null;

    //variables for pad moving
    private bool clicked = false;
    private bool held = false;
    private float xAxis;
    private float yAxis;


	// Use this for initialization
	void Start ()
    {
        holder = new GameObject();
        holder.transform.parent = this.transform;
        holder.transform.localPosition = Vector3.zero;

        pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pointer.transform.parent = holder.transform;
        pointer.transform.localScale = new Vector3(thickness, thickness, 100f);
        pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
        
        BoxCollider collider = pointer.GetComponent<BoxCollider>();
        if (addRigidBody)
        {
            if (collider)
            {
                collider.isTrigger = true;
            }
            Rigidbody rigidBody = pointer.AddComponent<Rigidbody>();
            rigidBody.isKinematic = true;
        }
        else
        {
            if(collider)
            {
                Object.Destroy(collider);
            }
        }
        Material newMaterial = new Material(Shader.Find("Unlit/Color"));
        newMaterial.SetColor("_Color", color);
        pointer.GetComponent<MeshRenderer>().material = newMaterial;
	}

    public virtual void OnPointerIn(PointerEventArgs e)
    {
        if (PointerIn != null)
            PointerIn(this, e);
    }

    public virtual void OnPointerOut(PointerEventArgs e)
    {
        if (PointerOut != null)
            PointerOut(this, e);
    }


    // Update is called once per frame
	void Update ()
    {
        if (!isActive)
        {
            isActive = true;
            this.transform.GetChild(0).gameObject.SetActive(true);
        }

        float dist = 100f;

        SteamVR_TrackedController controller = GetComponent<SteamVR_TrackedController>();

        Ray raycast = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        bool bHit = Physics.Raycast(raycast, out hit);

        if(previousContact && previousContact != hit.transform)
        {
            PointerEventArgs args = new PointerEventArgs();
            if (controller != null)
            {
                args.controllerIndex = controller.controllerIndex;
            }
            args.distance = 0f;
            args.flags = 0;
            args.target = previousContact;
            OnPointerOut(args);
            previousContact = null;
        }
        if(bHit && previousContact != hit.transform)
        {
            PointerEventArgs argsIn = new PointerEventArgs();
            if (controller != null)
            {
                argsIn.controllerIndex = controller.controllerIndex;
            }
            argsIn.distance = hit.distance;
            argsIn.flags = 0;
            argsIn.target = hit.transform;
            OnPointerIn(argsIn);
            previousContact = hit.transform;
            
        }

        destination = hit.point;

        if (!bHit)
        {
            previousContact = null;
            color = new Color(255.0f, 0.0f, 0.0f);
        }
        if (bHit && hit.distance < 100f)
        {
            dist = hit.distance;
        }

        if (controller != null && controller.triggerPressed)
        {
            pointer.transform.localScale = new Vector3(thickness, thickness, dist);
            
            Material newMaterial = new Material(Shader.Find("Unlit/Color"));

            if (bHit)
            {
                color = new Color(0.0f, 255.0f, 0.0f);
                newMaterial.SetColor("_Color", color);
                pointer.GetComponent<MeshRenderer>().material = newMaterial;
            }
            else
            {
                color = new Color(255.0f, 0.0f, 0.0f);
                newMaterial.SetColor("_Color", color);
                pointer.GetComponent<MeshRenderer>().material = newMaterial;
            }

            if (controller.menuPressed && bHit)
            {
                if (pressed == false)
                {
                    rig.transform.position = destination;
                    pressed = true;
                }
            }
        }
        else
        {
            pointer.transform.localScale = new Vector3(0f, 0f, 0f);
        }
        pointer.transform.localPosition = new Vector3(0f, 0f, dist/2f);

        if (controller.menuPressed == false)
        {
            pressed = false;
        }

        if (held)
        {
            yAxis = controller.yPos;
            xAxis = controller.xPos;

            //forward backward movements
            if (yAxis > 0)
            {
                rig.transform.position += head.transform.forward * 0.1f;
            }
            else
            {
                rig.transform.position -= head.transform.forward * 0.1f;
            }


            //left right strafe movements
            if (xAxis > 0.5f)
            {
                rig.transform.position += head.transform.right * 0.1f;
            }
            else if (xAxis < -0.5f)
            {
                rig.transform.position -= head.transform.right * 0.1f;
            }
        }

    }

    void OnEnable()
    {
        SteamVR_TrackedController controller = GetComponent<SteamVR_TrackedController>();

        if (!clicked)
        {
            controller.PadClicked += OnPadClicked;
            controller.PadUnclicked += OnPadUnClicked;
            clicked = true;
        }

        if (!controller.padPressed)
        {
            clicked = false;
        }

    }

    void OnDisable()
    {
        SteamVR_TrackedController controller = GetComponent<SteamVR_TrackedController>();
        controller.PadClicked -= OnPadClicked;
        controller.PadUnclicked -= OnPadUnClicked;
    }

    void OnPadClicked(object sender, ClickedEventArgs e)
    {
        xAxis = e.padX;
        yAxis = e.padY;
        held = true;
    }

    void OnPadUnClicked (object sender, ClickedEventArgs e)
    {
        held = false;
    }


}
