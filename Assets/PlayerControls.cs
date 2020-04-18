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
                    ""name"": ""Ataque"",
                    ""type"": ""Button"",
                    ""id"": ""03e8ae2e-40ee-472e-99ce-58153a202bbe"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Bloqueo"",
                    ""type"": ""Button"",
                    ""id"": ""12a34afb-02b2-4381-a9e7-2f8e03dd56fa"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Técnica"",
                    ""type"": ""Button"",
                    ""id"": ""1d0c2716-b7e2-4479-87e6-7218146c23f0"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Corte"",
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
                    ""action"": ""Ataque"",
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
                    ""action"": ""Bloqueo"",
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
                    ""action"": ""Técnica"",
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
                    ""id"": ""efaa3db6-fb96-4705-b95b-ad76ec414218"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Corte"",
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
        m_Fight_Ataque = m_Fight.FindAction("Ataque", throwIfNotFound: true);
        m_Fight_Bloqueo = m_Fight.FindAction("Bloqueo", throwIfNotFound: true);
        m_Fight_Técnica = m_Fight.FindAction("Técnica", throwIfNotFound: true);
        m_Fight_Corte = m_Fight.FindAction("Corte", throwIfNotFound: true);
        m_Fight_Move = m_Fight.FindAction("Move", throwIfNotFound: true);
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
    private readonly InputAction m_Fight_Ataque;
    private readonly InputAction m_Fight_Bloqueo;
    private readonly InputAction m_Fight_Técnica;
    private readonly InputAction m_Fight_Corte;
    private readonly InputAction m_Fight_Move;
    public struct FightActions
    {
        private @PlayerControls m_Wrapper;
        public FightActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Ataque => m_Wrapper.m_Fight_Ataque;
        public InputAction @Bloqueo => m_Wrapper.m_Fight_Bloqueo;
        public InputAction @Técnica => m_Wrapper.m_Fight_Técnica;
        public InputAction @Corte => m_Wrapper.m_Fight_Corte;
        public InputAction @Move => m_Wrapper.m_Fight_Move;
        public InputActionMap Get() { return m_Wrapper.m_Fight; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(FightActions set) { return set.Get(); }
        public void SetCallbacks(IFightActions instance)
        {
            if (m_Wrapper.m_FightActionsCallbackInterface != null)
            {
                @Ataque.started -= m_Wrapper.m_FightActionsCallbackInterface.OnAtaque;
                @Ataque.performed -= m_Wrapper.m_FightActionsCallbackInterface.OnAtaque;
                @Ataque.canceled -= m_Wrapper.m_FightActionsCallbackInterface.OnAtaque;
                @Bloqueo.started -= m_Wrapper.m_FightActionsCallbackInterface.OnBloqueo;
                @Bloqueo.performed -= m_Wrapper.m_FightActionsCallbackInterface.OnBloqueo;
                @Bloqueo.canceled -= m_Wrapper.m_FightActionsCallbackInterface.OnBloqueo;
                @Técnica.started -= m_Wrapper.m_FightActionsCallbackInterface.OnTécnica;
                @Técnica.performed -= m_Wrapper.m_FightActionsCallbackInterface.OnTécnica;
                @Técnica.canceled -= m_Wrapper.m_FightActionsCallbackInterface.OnTécnica;
                @Corte.started -= m_Wrapper.m_FightActionsCallbackInterface.OnCorte;
                @Corte.performed -= m_Wrapper.m_FightActionsCallbackInterface.OnCorte;
                @Corte.canceled -= m_Wrapper.m_FightActionsCallbackInterface.OnCorte;
                @Move.started -= m_Wrapper.m_FightActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_FightActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_FightActionsCallbackInterface.OnMove;
            }
            m_Wrapper.m_FightActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Ataque.started += instance.OnAtaque;
                @Ataque.performed += instance.OnAtaque;
                @Ataque.canceled += instance.OnAtaque;
                @Bloqueo.started += instance.OnBloqueo;
                @Bloqueo.performed += instance.OnBloqueo;
                @Bloqueo.canceled += instance.OnBloqueo;
                @Técnica.started += instance.OnTécnica;
                @Técnica.performed += instance.OnTécnica;
                @Técnica.canceled += instance.OnTécnica;
                @Corte.started += instance.OnCorte;
                @Corte.performed += instance.OnCorte;
                @Corte.canceled += instance.OnCorte;
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
            }
        }
    }
    public FightActions @Fight => new FightActions(this);
    public interface IFightActions
    {
        void OnAtaque(InputAction.CallbackContext context);
        void OnBloqueo(InputAction.CallbackContext context);
        void OnTécnica(InputAction.CallbackContext context);
        void OnCorte(InputAction.CallbackContext context);
        void OnMove(InputAction.CallbackContext context);
    }
}
