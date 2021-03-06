﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Golegos;
using System;

namespace Golegos {

    /*
    * Represents a generic option in the battle menu to be interacted with by the BattleManager
    */
    [RequireComponent(typeof(Text))]
    public class MenuOption : BattleOption {

        //List of battle options that derive from this option
        public MenuOption[] derivedOptions;

        //Indicates whether or not the menu should animate to the left when this option is selected
        public bool increasesDepth = true;
        //The text that this option should display
        public string optionText;

        //The amount of options available after selecting this option
        protected int optionsNum;
        //Keeps track of the current option index
        protected int currentIndex = 0;
        public static int maxOptions = 4;
        //The amount of time until this option hides its derived options
        //public float hideTime = .2f;


        public virtual void Awake() {
            //Check if the number of derived options is 0
            optionsNum = derivedOptions.Length;
            if (optionsNum == 0 && increasesDepth) {
                Debug.LogError("Can't increase depth without sub-options!");
            }
        }

        public override void Start() {
            base.Start();
            //Set this option as the parent of all the derived options
            foreach (MenuOption battleOp in derivedOptions) {
                if (battleOp != null) {
                    battleOp.SetParentOption(this);
                }
            }
        }

        //Called if this option is the first option of the menu
        public override void FirstOption() {
            //Delays the update... because if it doesn't it won't work
            Invoke("FirstUpdate", .005f);
            SetChildrenNewEnable(true);
        }

        public void FirstUpdate() {
            battleUI.UpdateOptionBox(0);
        }

        //Called when this option is selected
        public override BattleOption Select() {
            if (increasesDepth) {
                battleUI.IncreaseDepth();
            }
            battleUI.UpdateOptionBox(0);
            MenuOption battleOp = derivedOptions[currentIndex];
            if (battleOp != null) {
                SetChildrenEnableAtIndex(currentIndex);
                battleOp.SetChildrenNewEnable(true);
                return battleOp;
            }
            else {
                return null;
            }
        }

        //Called when this option is exited from
        public override BattleOption Back() {
            if (parentOption != null) {
                battleUI.DecreaseDepth();
                SetToHideChildren();
                //Invoke("SetToHideChildren", hideTime);

                int newIndex = GetIndexInParent();
                if (newIndex >= 0) {
                    battleUI.UpdateOptionBox(newIndex);
                }
                if (parentOption as MenuOption != null) {
                    (parentOption as MenuOption).SetChildrenNewEnable(true);
                }
                //SetChildrenNewEnable(true);
                return parentOption;
            }
            else {
                return null;
            }
        }

        //Called when the player navigates left
        public override BattleOption LeftNavigate() {
            return Back();
        }

        //Called when the player navigates right
        public override BattleOption RightNavigate() {
            return Select();
        }

        //Called when the player navigates up
        public override void UpNavigate() {
            if (currentIndex <= 0) {
                currentIndex = optionsNum - 1;
            }
            else {
                currentIndex--;
            }
            battleUI.UpdateOptionBox(currentIndex);
        }

        //Called when the player navigates down
        public override void DownNavigate() {
            if (currentIndex < optionsNum - 1) {
                currentIndex++;
            }
            else {
                currentIndex = 0;
            }
            battleUI.UpdateOptionBox(currentIndex);
        }

        //Shows or hides the derived options of this option
        public virtual void SetChildrenNewEnable(bool newEnable) {
            foreach (MenuOption battleOp in derivedOptions) {
                if (battleOp != null) {
                    if (newEnable && battleOp.optionText != null) {
                        battleOp.GetComponent<Text>().text = battleOp.optionText;
                    }
                    else if (!newEnable){
                        battleOp.GetComponent<Text>().text = "";
                    }
                }
            }
        }

        //Hides all the children except for the one at the specified index
        public virtual void SetChildrenEnableAtIndex(int index) {
            for (int i = 0; i < derivedOptions.Length; i++) {
                if (derivedOptions[i] != null) {
                    if (i == index && derivedOptions[i].optionText != null) {
                        derivedOptions[i].GetComponent<Text>().text = derivedOptions[i].optionText;
                    }
                    else if (i != index) {
                        derivedOptions[i].GetComponent<Text>().text = "";
                    }
                }
            }
        }

        //Called when this option gets returned to (so that this option can hide its derived options)
        public void SetToHideChildren() {
            SetChildrenNewEnable(false);
        }
        
        //Returns the index of this option, from the parent's perspective
        public int GetIndexInParent() {
            MenuOption menuOption = parentOption as MenuOption;
            if (menuOption != null) {
                return menuOption.checkChildIndex(this);
            }
            return -1;
        }

        //Returns the index of the requested child from the derivedOptions array
        public virtual int checkChildIndex(MenuOption child) {
            int i = 0;
            foreach (MenuOption menuOption in derivedOptions) {
                if (menuOption.optionText == child.optionText) {
                    return i;
                }
                i++;
            }
            return -1;
        }
    }
}