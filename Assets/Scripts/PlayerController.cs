using UnityEngine;

// class that represents the possible animation states of the character
// the variables within are the different animation states 
// and their value is the exact name of the animation state they represent
public static class PlayerStates {
        public static string IDLE = "Idle";
        public static string WALK = "Walk";
        public static string RUN = "Run";
        public static string JUMP = "Jump";
        public static string SPECIAL = "Special Move";
}

// class that handles player input and updates state of the character movement given that input
public class PlayerController : MonoBehaviour
{
    // speed at which character turns
    [Tooltip("Control how quickly the character turns")]    
    public float rotationSpeed = 10f;

    // is the jump movement implemented?
    [Tooltip("Is the jump animation state implemented?")]
    public bool hasJump = true;

    // is the special move implemented?
    [Tooltip("Is the special move animation state implemented?")]
    public bool hasSpecialMove = true;

    // character animator object
    private Animator animator;

    // PlayerInput object that handles controller input
    private PlayerInput input;

    // main camera in scene - assuming using Cinemachine
    private GameObject mainCam;

    // movement vector from controller input
    private Vector2 currentMovement;

    // parameters in the controller input
    private bool movementPressed; // is movement being 'pressed'
    private bool runPressed; // is the run button being pressed
    private bool jumpPressed; // was the jump button pressed?
    private bool specialActionPressed; //was the special action button pressed?

    
    private float jumpDuration; // how long has the jump animation lasted
    private float specialActionDuration; // how long has the special action animation lasted

    /*********************************
    **            KEY               **
    **  Integer           State     **
    **  --------------------------  **
    **      0              idle     **
    **      1              walk     **
    **      2              run      **
    **      3              jump     **
    **      4              special  **
    *********************************/
    
    // Awake happens upon compilation, before Start()
    void Awake()
    {
        // create new PlayerInput system and assign to variable
        input = new PlayerInput();

        // for a basic rundown of the callback functions of the input system: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/Actions.html

        // add the following function to the list of callbacks attached to Movement
        // (callbacks = functions called whenever a particular action is performed; here, an action would be a change in the movement input)
        // ctx is the parameter sent to this lambda function; here it is the movement input
        input.CharacterControls.Movement.performed += ctx =>
        {
            // get current movement input from controller
            currentMovement = ctx.ReadValue<Vector2>();
            // if there is non-0 x or y input, then movement has been pressed
            movementPressed = currentMovement.x != 0 || currentMovement.y != 0;
        };

        // add the following function to the list of callbacks attached to Run
        // (callbacks = functions called whenever a particular action is performed; here, an action would be a change in the state of the Run button - pressed or unpressed)
        // ctx is the parameter sent to this lambda function; here it is the run input
        input.CharacterControls.Run.performed += ctx => runPressed = ctx.ReadValueAsButton();

        // add the following function to the list of callbacks attached to Movement being cancelled
        // (callbacks = functions called whenever a particular action is performed; here, an action would be the movement input being cancelled)
        input.CharacterControls.Movement.canceled += ctx => movementPressed = false;

        // add the following function to the list of callbacks attached to Jump
        // (callbacks = functions called whenever a particular action is performed; here, an action would be a change in the state of the Jump button - pressed or unpressed)
        // ctx is the parameter sent to this lambda function; here it is the jump input
        input.CharacterControls.Jump.performed += ctx => jumpPressed = ctx.ReadValueAsButton();
        input.CharacterControls.Jump.canceled += ctx => jumpPressed = false;

        // add the following function to the list of callbacks attached to Special Action
        // (callbacks = functions called whenever a particular action is performed; here, an action would be a change in the state of the Special Action button - pressed or unpressed)
        // ctx is the parameter sent to this lambda function; here it is the special move input
        input.CharacterControls.SpecialAction.performed += ctx => specialActionPressed = ctx.ReadValueAsButton();
        input.CharacterControls.SpecialAction.canceled += ctx => specialActionPressed = false;

        // assign the mainCam field to the game object with the tag "MainCamera"
        mainCam = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Start is called before the first frame update, after GameObject has been instantiated
    void Start()
    {
        // set animator object to the Animator component attached to the Game Object that this script is attached to
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // get current state of animator
        int currentState = animator.GetInteger("currentState");
        switch(currentState) {
            // if current state is jumping
            case 3: {
                // if we have been in the jump state for at least as long as the jump animation, move on (do not interrupt animation)
                if(animator.GetCurrentAnimatorClipInfo(0).Length > 0 && jumpDuration >= animator.GetCurrentAnimatorClipInfo(0)[0].clip.length) {
                    // reset jump duration
                    jumpDuration = 0;
                    // handle rotation of character based on user input
                    HandleRotation();
                    // determine the new state of the character given the user's input
                    SetState();    
                } else {
                    // add the amount of time that has passed since last call to the duration of the current jump state
                    jumpDuration+=Time.deltaTime;
                }
                return;
            }
            // if current state is special
            case 4: {
                // if we have been in the special state for at least as long as the special animation, move on (do not interrupt animation)
                if(animator.GetCurrentAnimatorClipInfo(0).Length > 0 && specialActionDuration >= animator.GetCurrentAnimatorClipInfo(0)[0].clip.length) {
                    // reset special duration
                    specialActionDuration = 0;
                    // handle rotation of character based on user input
                    HandleRotation();
                    // determine the new state of the character given the user's input
                    SetState();    
                } else {
                    // add the amount of time that has passed since last call to the duration of the current special state
                    specialActionDuration+=Time.deltaTime;
                }
                return;
            }
            // otherwise...
            default: {
                // handle rotation of character based on user input
                HandleRotation();
                // determine the new state of the character given the user's input
                SetState(); 
                return;
            }
        }
    }

    // Set the currentState of the animator based on the user's input
    void SetState() {
        int nextState;
        // if user is pressing jump button, set state to jumping
        if(jumpPressed && hasJump) {
            nextState = 3;
            ChangeAnimationState(PlayerStates.JUMP);
        // if user is pressing special button, set state to special
        } else if(specialActionPressed && hasSpecialMove) {
            nextState = 4;
            ChangeAnimationState(PlayerStates.SPECIAL);
        } else {
            if (movementPressed)
            {
                // if user is pressing run button, set state to running
                if (runPressed) {
                    nextState = 2;
                    ChangeAnimationState(PlayerStates.RUN);
                }
                // if user not pressing run button, set state to walking
                else {
                    nextState = 1;
                    ChangeAnimationState(PlayerStates.WALK);
                    // set the speed of the walk based on how hard the user is pressing the move input
                    float speed = currentMovement.magnitude;
                    animator.SetFloat("walkSpeed", speed);
                }
            }
            // user is not initiating movement
            else {
                nextState = 0;
                ChangeAnimationState(PlayerStates.IDLE);
            }
        }
        // update currentState given logic above
        animator.SetInteger("currentState", nextState);
    }

    // handle player rotation input and rotate character
    void HandleRotation()
    {
        // if we are currently moving - we don't want to the character to rotate if we are not moving
        if (movementPressed)
        {
            // create a Vector3 from the inputed movement Vector2
            Vector3 inputDirection = new Vector3(currentMovement.x, 0, currentMovement.y);
            // find angle between the world's forward direction and the inputed direction
            // then rotate the camera's forward vector by that amount
            // this is to translate inputed movement direction from world space to local camera space
            float angleOffForward = Vector3.SignedAngle(Vector3.forward, inputDirection, Vector3.up);
            Vector3 finPos = Quaternion.Euler(0, angleOffForward, 0) * mainCam.transform.forward;
            // find the angle between the character's forward direction and final forward direction
            float angleBetween = Vector3.SignedAngle(transform.forward.normalized, finPos.normalized, Vector3.up);

            // required rotation to make forward direction equal to inputed direction
            Quaternion endRotation = transform.rotation * Quaternion.Euler(new Vector3(0, angleBetween, 0));

            // interoplate between current rotation and end rotation
            // this is to make the character slowly rotate over time, instead of instantly facing new direction
            float singleStep = rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, endRotation, singleStep);
        }
    }

    void OnEnable()
    {
        input.CharacterControls.Enable();
    }


    void OnDisable()
    {
        input.CharacterControls.Disable();
    }

    // change the current state of the animator to the given state if it is not already in that state
    // a method to bypass using transitions in the animator controller
    void ChangeAnimationState(string newState) {
        // if we are not already in that given state, change state to new one
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName(newState) && !animator.GetNextAnimatorStateInfo(0).IsName(newState)) {
            animator.CrossFadeInFixedTime(newState, .25f);
        }
    }
}
