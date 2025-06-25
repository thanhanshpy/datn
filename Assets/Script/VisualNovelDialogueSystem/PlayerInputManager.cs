using History;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace Dialouge
{
    public class PlayerInputManager : MonoBehaviour
    {
        private PlayerInput input;
        private List<(InputAction action, Action<InputAction.CallbackContext> command)> actions = new List<(InputAction action, Action<InputAction.CallbackContext> command)> ();
        public void OnNext(InputAction.CallbackContext c)
        {
            DialougeSystem.instance.OnUserPrompt_Next();
        }
        public void OnHistoryBack(InputAction.CallbackContext c)
        {
            HistoryManager.instance.GoBack();
        }
        public void OnHistoryFoward(InputAction.CallbackContext c)
        {
            HistoryManager.instance.GoFoward();
        }
        private void Awake()
        {
            input = GetComponent<PlayerInput>();
            InitializedActions();
        }
        private void InitializedActions()
        {
            actions.Add((input.actions["Next"], OnNext));
            actions.Add((input.actions["HistoryBack"], OnHistoryBack));
            actions.Add((input.actions["HistoryFoward"], OnHistoryFoward));
        }
        private void OnEnable()
        {
            foreach (var inputAction in actions)
            {
                inputAction.action.performed += inputAction.command;
            }
        }

        private void OnDisable()
        {
            foreach (var inputAction in actions)
            {
                inputAction.action.performed -= inputAction.command;
            }
        }
    }
   
}