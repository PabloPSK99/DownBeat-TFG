// GENERATED AUTOMATICALLY FROM 'Assets/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Fight"",
            ""id"": ""00a39836-553f-4a1f-8e1e-b4b607698818"",
            ""actions"": [
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""03e8ae2e-40ee-472e-99ce-58153a202bbe"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Block"",
                    ""type"": ""Button"",
                    ""id"": ""12a34afb-02b2-4381-a9e7-2f8e03dd56fa"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Tech"",
                    ""type"": ""Button"",
                    ""id"": ""1d0c2716-b7e2-4479-87e6-7218146c23f0"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Cut"",
                    ""type"": ""Button"",
                    ""id"": ""53038458-6c51-4b7a-8365-16d64ab3acfb"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Button"",
                    ""id"": ""a5b31fee-ce7b-450d-8e2d-17c52004b2fe"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Debug 1"",
                    ""type"": ""Button"",
                    ""id"": ""df180da2-6c21-40c1-825e-d0c5d7d76c78"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Debug 2"",
                    ""type"": ""Button"",
                    ""id"": ""5ba43d56-826a-432b-99b4-22fb90959161"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Debug 3"",
                    ""type"": ""Button"",
                    ""id"": ""322f3967-cf3c-45bb-9f4b-dfc76b298e82"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Debug 4"",
                    ""type"": ""Button"",
                    ""id"": ""e8599d82-c6e0-4b3d-a67b-706f45d0c8be"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""61a95c2e-44fb-4784-8906-d1ea3669bb76"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""884769aa-05c9-46d6-baba-b7a4b4073ff6"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Block"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""518c4e9a-949c-4889-96a7-34192b01346e"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Tech"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b072471a-a63d-4f1e-95a0-968a81a2827a"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""15224008-2221-41b7-a4da-332caf285404"",
                    ""path"": ""<Gamepad>/dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""efaa3db6-fb96-4705-b95b-ad76ec414218"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cut"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""96a37558-a9c7-4803-86d3-736e996ca6a0"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Debug 1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f70efac9-ab61-471f-9e7d-d9dbce23f700"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Debug 1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2f3a0a09-53db-402a-8dc5-12f6c84d8d20"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Debug 2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""03110b24-756f-4c83-ad10-d44d8579bcb0"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Debug 2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""69b572dd-17e8-4a5e-ae01-6de0ada7b0c4"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Debug 3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e7fee085-074d-499f-90a3-37b098488263"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Debug 4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Fight
        m_Fight = asset.FindActionMap("Fight", throwIfNotFound: true);
        m_Fight_Attack = m_Fight.FindAction("Attack", throwIfNotFound: true);
        m_Fight_Block = m_Fight.FindAction("Block", throwIfNotFound: true);
        m_Fight_Tech = m_Fight.FindAction("Tech", throwIfNotFound: true);
        m_Fight_Cut = m_Fight.FindAction("Cut", throwIfNotFound: true);
        m_Fight_Move = m_Fight.FindAction("Move", throwIfNotFound: true);
        m_Fight_Debug1 = m_Fight.FindAction("Debug 1", throwIfNotFound: true);
        m_Fight_Debug2 = m_Fight.FindAction("Debug 2", throwIfNotFound: true);
        m_Fight_Debug3 = m_Fight.FindAction("Debug 3", throwIfNotFound: true);
        m_Fight_Debug4 = m_Fight.FindAction("Debug 4", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Fight
    private readonly InputActionMap m_Fight;
    private IFightActions m_FightActionsCallbackInterface;
    private readonly InputAction m_Fight_Attack;
    private readonly InputAction m_Fight_Block;
    private readonly InputAction m_Fight_Tech;
    private readonly InputAction m_Fight_Cut;
    private readonly InputAction m_Fight_Move;
    private readonly InputAction m_Fight_Debug1;
    private readonly InputAction m_Fight_Debug2;
    private readonly InputAction m_Fight_Debug3;
    private readonly InputAction m_Fight_Debug4;
    public struct FightActions
    {
        private @PlayerControls m_Wrapper;
        public FightActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Attack => m_Wrapper.m_Fight_Attack;
        public InputAction @Block => m_Wrapper.m_Fight_Block;
        public InputAction @Tech => m_Wrapper.m_Fight_Tech;
        public InputAction @Cut => m_Wrapper.m_Fight_Cut;
        public InputAction @Move => m_Wrapper.m_Fight_Move;
        public InputAction @Debug1 => m_Wrapper.m_Fight_Debug1;
        public InputAction @Debug2 => m_Wrapper.m_Fight_Debug2;
        public InputAction @Debug3 => m_Wrapper.m_Fight_Debug3;
        public InputAction @Debug4 => m_Wrapper.m_Fight_Debug4;
        public InputActionMap Get() { return m_Wrapper.m_Fight; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(FightActions set) { return set.Get(); }
        public void SetCallbacks(IFightActions instance)
        {
            if (m_Wrapper.m_FightActionsCallbackInterface != null)
            {
                @Attack.started -= m_Wrapper.m_FightActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_FightActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_FightActionsCallbackInterface.OnAttack;
                @Block.started -= m_Wrapper.m_FightActionsCallbackInterface.OnBlock;
                @Block.performed -= m_Wrapper.m_FightActionsCallbackInterface.OnBlock;
                @Block.canceled -= m_Wrapper.m_FightActionsCallbackInterface.OnBlock;
                @Tech.started -= m_Wrapper.m_FightActionsCallbackInterface.OnTech;
                @Tech.performed -= m_Wrapper.m_FightActionsCallbackInterface.OnTech;
                @Tech.canceled -= m_Wrapper.m_FightActionsCallbackInterface.OnTech;
                @Cut.started -= m_Wrapper.m_FightActionsCallbackInterface.OnCut;
                @Cut.performed -= m_Wrapper.m_FightActionsCallbackInterface.OnCut;
                @Cut.canceled -= m_Wrapper.m_FightActionsCallbackInterface.OnCut;
                @Move.started -= m_Wrapper.m_FightActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_FightActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_FightActionsCallbackInterface.OnMove;
                @Debug1.started -= m_Wrapper.m_FightActionsCallbackInterface.OnDebug1;
                @Debug1.performed -= m_Wrapper.m_FightActionsCallbackInterface.OnDebug1;
                @Debug1.canceled -= m_Wrapper.m_FightActionsCallbackInterface.OnDebug1;
                @Debug2.started -= m_Wrapper.m_FightActionsCallbackInterface.OnDebug2;
                @Debug2.performed -= m_Wrapper.m_FightActionsCallbackInterface.OnDebug2;
                @Debug2.canceled -= m_Wrapper.m_FightActionsCallbackInterface.OnDebug2;
                @Debug3.started -= m_Wrapper.m_FightActionsCallbackInterface.OnDebug3;
                @Debug3.performed -= m_Wrapper.m_FightActionsCallbackInterface.OnDebug3;
                @Debug3.canceled -= m_Wrapper.m_FightActionsCallbackInterface.OnDebug3;
                @Debug4.started -= m_Wrapper.m_FightActionsCallbackInterface.OnDebug4;
                @Debug4.performed -= m_Wrapper.m_FightActionsCallbackInterface.OnDebug4;
                @Debug4.canceled -= m_Wrapper.m_FightActionsCallbackInterface.OnDebug4;
            }
            m_Wrapper.m_FightActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
                @Block.started += instance.OnBlock;
                @Block.performed += instance.OnBlock;
                @Block.canceled += instance.OnBlock;
                @Tech.started += instance.OnTech;
                @Tech.performed += instance.OnTech;
                @Tech.canceled += instance.OnTech;
                @Cut.started += instance.OnCut;
                @Cut.performed += instance.OnCut;
                @Cut.canceled += instance.OnCut;
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Debug1.started += instance.OnDebug1;
                @Debug1.performed += instance.OnDebug1;
                @Debug1.canceled += instance.OnDebug1;
                @Debug2.started += instance.OnDebug2;
                @Debug2.performed += instance.OnDebug2;
                @Debug2.canceled += instance.OnDebug2;
                @Debug3.started += instance.OnDebug3;
                @Debug3.performed += instance.OnDebug3;
                @Debug3.canceled += instance.OnDebug3;
                @Debug4.started += instance.OnDebug4;
                @Debug4.performed += instance.OnDebug4;
                @Debug4.canceled += instance.OnDebug4;
            }
        }
    }
    public FightActions @Fight => new FightActions(this);
    public interface IFightActions
    {
        void OnAttack(InputAction.CallbackContext context);
        void OnBlock(InputAction.CallbackContext context);
        void OnTech(InputAction.CallbackContext context);
        void OnCut(InputAction.CallbackContext context);
        void OnMove(InputAction.CallbackContext context);
        void OnDebug1(InputAction.CallbackContext context);
        void OnDebug2(InputAction.CallbackContext context);
        void OnDebug3(InputAction.CallbackContext context);
        void OnDebug4(InputAction.CallbackContext context);
    }
}
