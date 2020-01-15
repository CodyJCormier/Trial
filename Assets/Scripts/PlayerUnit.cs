using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType { physical, elemental } // Using this to signify different attack types doubles as an attack type checker.
public enum UnitState { alive, dead, clinging} // using this for better transition of state to other objects.

// I'm running this code as if there was no turn based approaches, 
// but that would slightly change the order of this stuff, as take damage would be called before checking health etc;
public class PlayerUnit : MonoBehaviour
{

    // Serializing these field for testing purposes.
    [SerializeField] private float jing = 10.0f;
    [SerializeField] private AttackType weakness = AttackType.elemental; 

    // Adjustable in editor for testing
    [SerializeField] private float chanceToCling = 10.0f; 

    // In case you wanted to start out a level with a unit downed/dead through prefab, expose it to the editor.
    [SerializeField] private UnitState state;

    // Meshes to symbolize the different states visually. 
    [SerializeField] private MeshFilter currentMesh; // saving this for quick reference to the players Mesh.
    [SerializeField] private Mesh aliveMesh;
    [SerializeField] private Mesh deadMesh;
    [SerializeField] private Mesh clingingMesh;

    [SerializeField] private SphereCollider detectionArea;

    //Testing Only variables - This would be set in a better mannerism in a turn based game (like you are working on)
    public bool canChangeState = false;

    public void Awake()
    {
        if (currentMesh == null)
        {
            currentMesh = gameObject.GetComponent<MeshFilter>();
        }
    }

    public void Start()
    {
        // if statements here to change chance to cling at game load
        // can be done in other ways if a system is in place already
        // alternatively, could be done by the GameManager at the start of a level/game. 
        // or chanceToCling = GameManager.DifficultyLevel * (whatever value for chance per level)
        chanceToCling = 10.0f;

        detectionArea = gameObject.GetComponent<SphereCollider>();

        SetStartState();
    }

    public void Update()
    {
        if (state != UnitState.alive)
        {
            return;
        } // This is a hard stop to prevent loops, but also because if he is dead/downed there specific functions to use. 
        if(jing <= 0)
        {
            if (ClingRoll())
            {
                EnterCling();
            }
            else
            {
                Death();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == this.gameObject || other.isTrigger)
        {
            return;
        }
        if (other.tag == "PlayerUnit")
        {
            PlayerUnit tmp = other.gameObject.GetComponent<PlayerUnit>();

            if (tmp.state == UnitState.clinging)
            {
                tmp.Revived();
            }
        }
    }

    // Rolls true or false for chance to live
    public bool ClingRoll()
    {
        float tmp = Random.Range(0.0f, 100.0f); // set as a variable for testing purposes.

        if (tmp >= chanceToCling)
        {
            Debug.Log("roll to live was " + tmp);
            return false;
        }
        else return true;
    }

    public void TakeDmg(float dmg, AttackType type)
    {
        if (type == weakness && state == UnitState.clinging)
        {
            Death();
            return;
        }

        jing -= dmg;
    }

    public void EnterCling()
    {
        state = UnitState.clinging;
        currentMesh.mesh = clingingMesh;
        jing = 1f; //Could be changed to something else to signify it can still be destroyed. 
    }

    public void Revived()
    {
        jing = 10f; //could be set to a default variable or any other system or changed based on difficulty. 
        state = UnitState.alive;
        currentMesh.mesh = aliveMesh;
    }

    public void Death()
    {
        state = UnitState.dead;
        currentMesh.mesh = deadMesh;
        // Could go further to set it as a new object
        // Alternatively, keep it there as a reference for selecting unit to view deceased units stats/details/inventory if applicable.
    }

    public UnitState GetState()
    {
        return state;
    }

    private void SetStartState()
    {
        if (state == UnitState.alive)
        {
            state = UnitState.alive;
        }
        else if (state == UnitState.clinging)
        {
            state = UnitState.clinging;
        }
        else if (state == UnitState.dead)
        {
            state = UnitState.dead;
        }
    }
}
