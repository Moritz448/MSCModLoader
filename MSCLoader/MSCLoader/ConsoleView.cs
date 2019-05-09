﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace MSCLoader
{
#pragma warning disable CS1591
    public class ConsoleView : MonoBehaviour
    {
        public ConsoleController controller;
        public GameObject viewContainer; //Container for console view, should be a child of this GameObject
        public Text logTextArea;
        public InputField inputField;
        private bool wasFocused;
        private int commands, pos;

        void Start()
        {
            if (controller != null)
            {
                controller.logChanged += onLogChanged;
            }
            updateLogStr(controller.log);
        }

        ~ConsoleView()
        {
            controller.logChanged -= onLogChanged;
        }

        public void toggleVisibility()
        {
            if (viewContainer.activeSelf)
                viewContainer.transform.GetChild(4).gameObject.GetComponent<ConsoleUIResizer>().SaveConsoleSize();
            setVisibility(!viewContainer.activeSelf);
            inputField.text = string.Empty;
            if (viewContainer.activeSelf && (bool)ModConsole.typing.GetValue())
            {
                inputField.ActivateInputField();
                inputField.Select();
            }
        }

        public void setVisibility(bool visible)
        {
            viewContainer.SetActive(visible);
        }

        void onLogChanged(string[] newLog)
        {
            updateLogStr(newLog);
        }

        void updateLogStr(string[] newLog)
        {
            if (newLog == null)
            {
                logTextArea.text = "";
            }
            else
            {
                logTextArea.text = string.Join(Environment.NewLine, newLog);
            }
        }

        // Event that should be called by anything wanting to submit the current input to the console.
        public void runCommand()
        {
            controller.RunCommandString(inputField.text);
            inputField.text = string.Empty;
            //keep active input field
            inputField.ActivateInputField();
            inputField.Select();
        }
        private void FixedUpdate()
        {
            if (inputField.text.Length > 0 && Input.GetKey(KeyCode.Return))
            {
                runCommand();
            }
        }
        private void Update()
        {
            if (inputField.isFocused)
            {

                if (!wasFocused)
                {
                    wasFocused = true;
                    commands = controller.commandHistory.Count;
                    pos = commands;
                }
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    //if any command was entered before
                    if (commands != 0)
                    {
                        if (pos != 0)
                            pos--;
                        inputField.text = controller.commandHistory[pos];
                        inputField.MoveTextEnd(false);
                    }
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (commands != 0)
                    {
                        pos++;
                        if (pos != commands)
                        {
                            inputField.text = controller.commandHistory[pos];
                            inputField.MoveTextEnd(false);
                        }
                        else
                        {
                            pos--;
                        }

                    }
                }
            }
            else
            {
                wasFocused = false;
            }
        }

    }
#pragma warning restore CS1591
}