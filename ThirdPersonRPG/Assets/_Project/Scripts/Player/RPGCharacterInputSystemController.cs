using LB.Inventory;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LB
{
    public class RPGCharacterInputSystemController : MonoBehaviour
    {
        private RPGCharacterController rpgCharacterController;
        private RPGCharacterInventoryController rpgCharacterInventoryController;
        private RPGCharacterEquipmentController rpgCharacterEquipmentController;
        private RPGCharacterInteractionController rpgCharacterInteractionController;
        private RPGCharacterMovementController rpgCharacterMovementController;
        private PlayerInput playerInput;

        //InputSystem
        public @RPGInputs rpgInputs;

        // Inputs.
        private bool inputJump;
        private bool inputLightHit;
        private bool inputKnockdown;
        private bool inputAttackL;
        private bool inputAttackR;
        private bool inputRoll;
        private bool inputAim;
        private Vector2 inputMovement;
        private bool inputSprint;
        private bool inputFace;
        private Vector2 inputFacing;
        private bool inputWeaponOne;
        private bool inputWeaponTwo;
        private bool inputInventory;
        private bool inputInteraction;

        // Variables.
        private Vector3 moveInput;
        private Vector3 currentAim;
        private float inputPauseTimeout = 0;
        private bool inputPaused = false;

        private void Awake()
        {
            rpgCharacterController = GetComponent<RPGCharacterController>();
            rpgCharacterInventoryController = GetComponent<RPGCharacterInventoryController>();
            rpgCharacterEquipmentController = GetComponent<RPGCharacterEquipmentController>();
            rpgCharacterInteractionController = GetComponent<RPGCharacterInteractionController>();
            rpgCharacterMovementController = GetComponent<RPGCharacterMovementController>();
            playerInput = GetComponent<PlayerInput>();

            rpgInputs = new @RPGInputs();
            currentAim = Vector3.zero;
        }

        private void OnEnable()
        {
            rpgInputs.Enable();
        }

        private void OnDisable()
        {
            rpgInputs.Disable();
        }

        public bool HasMoveInput() => moveInput.magnitude > 0.1f;
        public bool HasSprintInput() => inputSprint;

        public bool HasAimInput() => inputAim;

        public bool HasFacingInput() => inputFacing != Vector2.zero || inputFace;

        private void Update()
        {
            if (rpgCharacterController == null || rpgCharacterController.rpgCharacterGlobalInputBlocker) return;

            // Pause input for other external input.
            if (inputPaused)
            {
                if (Time.time > inputPauseTimeout)
                {
                    inputPaused = false;
                }
                else
                {
                    return;
                }
            }

            if (!inputPaused)
            {
                Inputs();
            }

            Moving();
            Sprinting();
            Jumping();
            Damage();
            Rolling();
            Attacking();
            Inventory();
            Interacting();
        }

        /// <summary>
        /// Pause input for a number of seconds.
        /// </summary>
        /// <param name="timeout">The amount of time in seconds to ignore input.</param>
        public void PauseInput(float timeout)
        {
            inputPaused = true;
            inputPauseTimeout = Time.time + timeout;
        }

        /// <summary>
        /// Input abstraction for easier asset updates using outside control schemes.
        /// </summary>
        private void Inputs()
        {
            try
            {
                inputAttackL = rpgInputs.RPGCharacter.AttackL.WasPressedThisFrame();
                inputAttackR = rpgInputs.RPGCharacter.AttackR.WasPressedThisFrame();
                inputKnockdown = rpgInputs.RPGCharacter.Knockdown.WasPressedThisFrame();
                inputFace = rpgInputs.RPGCharacter.Face.IsPressed();
                inputFacing = rpgInputs.RPGCharacter.Facing.ReadValue<Vector2>();
                inputJump = rpgInputs.RPGCharacter.Jump.IsPressed();
                inputLightHit = rpgInputs.RPGCharacter.LightHit.WasPressedThisFrame();
                inputMovement = rpgInputs.RPGCharacter.Move.ReadValue<Vector2>();
                inputSprint = rpgInputs.RPGCharacter.Sprint.IsPressed();
                inputRoll = rpgInputs.RPGCharacter.Roll.WasPressedThisFrame();
                inputAim = rpgInputs.RPGCharacter.Aim.IsPressed();
                inputWeaponTwo = rpgInputs.RPGCharacter.WeaponTwo.WasPressedThisFrame();
                inputWeaponOne = rpgInputs.RPGCharacter.WeaponOne.WasPressedThisFrame();
                inputInventory = rpgInputs.RPGCharacter.Inventory.WasPressedThisFrame();
                inputInteraction = rpgInputs.RPGCharacter.Interaction.WasPressedThisFrame();

                // Slow time toggle.
                if (Keyboard.current.tKey.wasPressedThisFrame)
                {
                    if (rpgCharacterController.CanStartAction("SlowTime"))
                    {
                        rpgCharacterController.StartAction("SlowTime", 0.125f);
                    }
                    else if (rpgCharacterController.CanEndAction("SlowTime"))
                    {
                        rpgCharacterController.EndAction("SlowTime");
                    }
                }

                // Pause toggle.
                if (Keyboard.current.pKey.wasPressedThisFrame)
                {
                    if (rpgCharacterController.CanStartAction("SlowTime"))
                    {
                        rpgCharacterController.StartAction("SlowTime", 0f);
                    }
                    else if (rpgCharacterController.CanEndAction("SlowTime"))
                    {
                        rpgCharacterController.EndAction("SlowTime");
                    }
                }
            }
            catch (System.Exception)
            {
                Debug.LogError("Inputs not found!  " +
                               "Make sure your project is using the new InputSystem: Edit>Project Settings>Player>Active Input Handling  - change to 'Input System Package (New)'.");
            }
        }

        public void Moving()
        {
            moveInput = new Vector3(inputMovement.x, inputMovement.y, 0f);
            // Filter the 0.1 threshold of HasMoveInput.
            if (HasMoveInput())
            {
                rpgCharacterController.SetMoveInput(moveInput);
            }
            else
            {
                rpgCharacterController.SetMoveInput(Vector3.zero);
            }
        }

        private void Sprinting()
        {
        }

        private void Jumping()
        {
            // Set the input on the jump axis every frame.
            Vector3 jumpInput = inputJump ? Vector3.up : Vector3.zero;
            rpgCharacterController.SetJumpInput(jumpInput);

            // If we pressed jump button this frame, jump.
            if (inputJump && rpgCharacterController.CanStartAction("Jump"))
            {
                rpgCharacterController.StartAction("Jump");
            }
            else if (inputJump && rpgCharacterController.CanStartAction("DoubleJump"))
            {
                rpgCharacterController.StartAction("DoubleJump");
            }
        }

        public void Rolling()
        {
            if (!inputRoll)
            {
                return;
            }

            if (!rpgCharacterController.CanStartAction("DiveRoll"))
            {
                return;
            }

            rpgCharacterController.StartAction("DiveRoll", 1);
        }

        private void Aiming()
        {
            Strafing();
        }

        private void Strafing()
        {
            if (rpgCharacterController.CanStrafe)
            {
                if (inputAim)
                {
                    if (rpgCharacterController.CanStartAction("Strafe"))
                    {
                        rpgCharacterController.StartAction("Strafe");
                    }
                }
                else
                {
                    if (rpgCharacterController.CanEndAction("Strafe"))
                    {
                        rpgCharacterController.EndAction("Strafe");
                    }
                }
            }
        }

        private void Facing()
        {
            if (rpgCharacterController.CanFace)
            {
                if (HasFacingInput())
                {
                    if (inputFace)
                    {
                        // Get world position from mouse position on screen and convert to direction from character.
                        Plane playerPlane = new Plane(Vector3.up, transform.position);
                        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                        float hitdist = 0.0f;
                        if (playerPlane.Raycast(ray, out hitdist))
                        {
                            Vector3 targetPoint = ray.GetPoint(hitdist);
                            Vector3 lookTarget = new Vector3(targetPoint.x - transform.position.x,
                                transform.position.z - targetPoint.z, 0);
                            rpgCharacterController.SetFaceInput(lookTarget);
                        }
                    }
                    else
                    {
                        rpgCharacterController.SetFaceInput(new Vector3(inputFacing.x, inputFacing.y, 0));
                    }

                    if (rpgCharacterController.CanStartAction("Face"))
                    {
                        rpgCharacterController.StartAction("Face");
                    }
                }
                else
                {
                    if (rpgCharacterController.CanEndAction("Face"))
                    {
                        rpgCharacterController.EndAction("Face");
                    }
                }
            }
        }

        private void Attacking()
        {
            // Check to make sure Attack Action exists.
            if (!rpgCharacterController.HandlerExists(HandlerTypes.Attack))
            {
                return;
            }

            // Check to make character can Attack.
            if (!rpgCharacterController.CanStartAction(HandlerTypes.Attack))
            {
                return;
            }

            if (inputAttackL)
            {
                rpgCharacterController.StartAction(HandlerTypes.Attack,
                    new AttackContext(HandlerTypes.Attack, Side.Left));
            }
            else if (inputAttackR)
            {
                rpgCharacterController.StartAction(HandlerTypes.Attack,
                    new AttackContext(HandlerTypes.Attack, Side.Right));
            }
        }

        private void Damage()
        {
            // Hit.
            if (rpgCharacterController.HandlerExists(HandlerTypes.GetHit))
            {
                if (inputLightHit)
                {
                    rpgCharacterController.StartAction(HandlerTypes.GetHit, new HitContext());
                }
            }

            // Knockdown.
            if (rpgCharacterController.HandlerExists(HandlerTypes.Knockdown))
            {
                if (inputKnockdown && rpgCharacterController.CanStartAction(HandlerTypes.Knockdown))
                {
                    rpgCharacterController.StartAction(HandlerTypes.Knockdown,
                        new HitContext((int)KnockdownType.Knockdown1, Vector3.back));
                }
            }
        }

        /// <summary>
        /// Toggles the visibility of the inventory UI.
        /// Works in combination with the UI StateManager.
        /// </summary>
        private void Inventory()
        {
            if (inputInventory)
            {
                rpgCharacterInventoryController.ToggleInventory();
            }
        }

        /// <summary>
        /// Controls the interaction of the RPGCharacter with the environment like NPCs, Storages, Pickups etc.
        /// </summary>
        private void Interacting()
        {
            if (inputInteraction)
            {
                rpgCharacterInteractionController.Interaction();
            }
        }

        public void OnAttack(InputValue value)
        {
            if (!rpgCharacterController || rpgCharacterController.rpgCharacterGlobalInputBlocker || !value.isPressed) return;

            // Call the constructor that matches the expected signature (string, Side, int)
            // Pass Side.None since Side is no longer used in the simplified attack logic.
            AttackContext attackContext = new AttackContext("Attack", Side.None, -1);
            rpgCharacterController.TryStartAction(HandlerTypes.Attack, attackContext);
        }

        public void OnSwitchWeapon(InputValue value) // Or whatever your input action method is called
        {
             if (!rpgCharacterController || rpgCharacterController.rpgCharacterGlobalInputBlocker || !value.isPressed) return;

            // Removed SwitchWeaponContext creation and usage
            // The context is no longer needed as the handler reads state directly.
            // SwitchWeaponContext context = new SwitchWeaponContext();
            // context.itemTypeSlotOne = equipmentController.weaponSlotOne.weaponType;
            // context.weaponModel = equipmentController.weaponSlotOne.characterDisplay;
            // context.itemTypeSlotTwo = equipmentController.weaponSlotTwo.weaponType;
            // context.slotOnePressed = true; // Or determine based on input?
            
            // Simply try to start the action without context
            rpgCharacterController.TryStartAction(HandlerTypes.SwitchWeapon);
        }
        
        // Example of processing Move input (ensure equippedWeapon is used if needed here)
        public void OnMove(InputValue value)
        {
            if (rpgCharacterController == null || rpgCharacterController.rpgCharacterGlobalInputBlocker) return;

            Vector2 moveInput = value.Get<Vector2>();
            rpgCharacterController.SetMoveInput(new Vector3(moveInput.x, moveInput.y, 0f));

            // Example: Potentially check equippedWeapon state if movement differs when armed
            // if (rpgCharacterController.equippedWeapon != Weapon.Unarmed) { ... }
        }
    }

    /// <summary>
    /// Extension Method to allow checking InputSystem without Action Callbacks.
    /// </summary>
    public static class InputActionExtensions
    {
        public static bool IsPressed(this InputAction inputAction) => inputAction.ReadValue<float>() > 0f;

        public static bool WasPressedThisFrame(this InputAction inputAction) =>
            inputAction.triggered && inputAction.ReadValue<float>() > 0f;

        public static bool WasReleasedThisFrame(this InputAction inputAction) =>
            inputAction.triggered && inputAction.ReadValue<float>() == 0f;
    }
}