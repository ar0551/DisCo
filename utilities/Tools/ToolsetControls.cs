/*
Project DisCo (Discrete Choreography) (GPL) initiated by Jan Philipp Drude

This file is part of Project DisCo.

Copyright (c) 2019, Jan Philipp Drude <jpdrude@gmail.com>

A full build of Project DisCo is available at <http://www.project-disco.com>

Project DisCo's underlaying Source Code is free to use; you can 
redistribute it and/or modify it under the terms of the GNU 
General Public License as published by the Free Software Foundation; 
either version 3 of the License, or (at your option) any later version. 

The Project DisCo source code is distributed in the hope that it will 
be useful, but WITHOUT ANY WARRANTY; without even the implied warranty 
of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
with the DisCo-Classes repository; 
If not, see <http://www.gnu.org/licenses/>.

@license GPL-3.0 <https://www.gnu.org/licenses/gpl.html>

The Project DisCo base classes build on Wasp developed by Andrea Rossi.
You can find Wasp at: <https://github.com/ar0551/Wasp>

Significant parts of Project DisCo have been developed by Jan Philipp Drude
as part of research on virtual reality, digital materials and 
discrete design at: 
dMA - digital Methods in Architecture - Prof. Mirco Becker
Leibniz University Hannover
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;

public class ToolsetControls : MonoBehaviour
{
    //local variables
    #region
    [SerializeField]
    MeshRenderer txtNumParts = null;

    [SerializeField]
    MeshRenderer txtPartType = null;

    [SerializeField]
    MeshRenderer txtRuleFilter = null;

    [SerializeField]
    MeshRenderer txtPlaceType = null;

    [SerializeField]
    MeshRenderer txtSaveLoad = null;

    [SerializeField]
    MeshRenderer txtSimStat = null;

    NumberOfPartsTool NumberOfParts;
    PartTypeTool PartType;
    RuleFilterTool RuleFilter;
    PlacementTypeTool PlacementType;
    SaveLoadTool SaveLoad;
    SimStaticsTool SimulateStatics;

    Coroutine holdLeft;
    Coroutine holdRight;

    Material activeFont;
    Material inactiveFont;

    bool routineActive = true;
    #endregion


    //properties
    #region
    static ToolHeader activeToolHeader = ToolHeader.numParts;
    public static ToolHeader ActiveToolHeader
    {
        get { return activeToolHeader; }
        set
        {
            if (value < ToolHeader.numParts)
            {
                activeToolHeader = ToolHeader.simStat;
            }
            else if (value > ToolHeader.simStat)
            {
                activeToolHeader = ToolHeader.numParts;
            }
            else
            {
                activeToolHeader = value;
            }
        }
    }
    #endregion


    //MonoBehaviour methods
    #region
    void Start()
    {
        activeFont = Resources.Load<Material>("Materials/Toolset/Toolset_Font");
        inactiveFont = Resources.Load<Material>("Materials/Toolset/Toolset_Font_Inactive");

        NumberOfParts = txtNumParts.gameObject.GetComponent<NumberOfPartsTool>();
        PartType = txtPartType.gameObject.GetComponent<PartTypeTool>();
        RuleFilter = txtRuleFilter.gameObject.GetComponent<RuleFilterTool>();
        PlacementType = txtPlaceType.gameObject.GetComponent<PlacementTypeTool>();
        SaveLoad = txtSaveLoad.gameObject.GetComponent<SaveLoadTool>();
        SimulateStatics = txtSimStat.gameObject.GetComponent<SimStaticsTool>();
    }
    #endregion


    //controller methods
    #region
    public void PadUp()
    {
        ChangeToolHeader(InputY.Up);
    }

    public void PadDown()
    {
        ChangeToolHeader(InputY.Down);
    }

    public void PadRight(ViveControllerInput.Button type)
    {
        if (type == ViveControllerInput.Button.press)
        {
            if (routineActive)
            {
                holdRight = StartCoroutine(HoldKey(InputX.Right));
            }
            else
            {
                ChangeOrActivateTool(InputX.Right);
            }
        }
        else
        {
            if (holdRight != null)
            {
                StopCoroutine(holdRight);
            }
        }
    }

    public void PadLeft(ViveControllerInput.Button type)
    {
        if (type == ViveControllerInput.Button.press)
        {
            if (routineActive)
            {
                holdLeft = StartCoroutine(HoldKey(InputX.Left));
            }
            else
            {
                ChangeOrActivateTool(InputX.Left);
            }
        }
        else
        {
            if (holdLeft != null)
            {
                StopCoroutine(holdLeft);
            }
        }
    }

    public void TriggerDown()
    {
        ChangeOrActivateTool(InputX.Enter);
    }

    IEnumerator HoldKey(InputX input)
    {
        while(routineActive)
        {
            ChangeOrActivateTool(input);
            yield return new WaitForSeconds(0.03f);
        }
    }
    #endregion


    //methods
    #region
    private void ChangeOrActivateTool(InputX input)
    {
        switch (ActiveToolHeader)
        {
            case ToolHeader.numParts:
                {
                    NumberOfParts.ChangeTool(input);
                    break;
                }
            case ToolHeader.partType:
                {
                    PartType.ChangeTool(input);
                    break;
                }
            case ToolHeader.ruleFilter:
                {
                    RuleFilter.ChangeTool(input);
                    break;
                }
            case ToolHeader.placeType:
                {
                    PlacementType.ChangeTool(input);
                    break;
                }
            case ToolHeader.saveLoad:
                {
                    SaveLoad.ChangeTool(input);
                    break;
                }
            case ToolHeader.simStat:
                {
                    SimulateStatics.ChangeTool(input);
                    break;
                }
        }
    }

    private void ChangeToolHeader(InputY input)
    {
        routineActive = false;

        if (input == InputY.Up)
        {
            --ActiveToolHeader;
            setToolActivityMaterials();
        }
        else
        {
            ++ActiveToolHeader;
            setToolActivityMaterials();
        }

        void setToolActivityMaterials()
        {
            switch (ActiveToolHeader)
            {
                case ToolHeader.numParts:
                    {
                        routineActive = true;
                        AllTxtInactive();
                        txtNumParts.material = activeFont;
                        break;
                    }
                case ToolHeader.partType:
                    {
                        AllTxtInactive();
                        txtPartType.material = activeFont;
                        PartType.ToggleSelected(true);
                        break;
                    }
                case ToolHeader.ruleFilter:
                    {
                        AllTxtInactive();
                        txtRuleFilter.material = activeFont;
                        RuleFilter.ToggleSelected(true);
                        break;
                    }
                case ToolHeader.placeType:
                    {
                        AllTxtInactive();
                        txtPlaceType.material = activeFont;
                        PlacementType.ToggleSelected(true);
                        break;
                    }
                case ToolHeader.saveLoad:
                    {
                        AllTxtInactive();
                        txtSaveLoad.material = activeFont;
                        SaveLoad.ToggleSelected(true);
                        break;
                    }
                case ToolHeader.simStat:
                    {
                        AllTxtInactive();
                        txtSimStat.material = activeFont;
                        break;
                    }
            }

        }

        void AllTxtInactive()
        {
            txtNumParts.material = inactiveFont;
            txtPartType.material = inactiveFont;
            txtRuleFilter.material = inactiveFont;
            txtPlaceType.material = inactiveFont;
            txtSaveLoad.material = inactiveFont;
            txtSimStat.material = inactiveFont;
            PartType.ToggleSelected(false);
            RuleFilter.ToggleSelected(false);
            PlacementType.ToggleSelected(false);
            SaveLoad.ToggleSelected(false);
            SaveLoad.Initialize();
        }
    }
    #endregion


    //enumerations
    #region
    public enum ToolHeader
    {
        numParts = 0,
        partType = 1,
        ruleFilter = 2,
        placeType = 3,
        saveLoad = 4,
        simStat = 5
    }

    private enum InputY
    {
        Up = 0,
        Down = 1
    }

    public enum InputX
    {
        Enter = 0,
        Left = 1,
        Right =2
    }
    #endregion
}

