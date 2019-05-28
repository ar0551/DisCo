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
using Valve.VR;

public class ViveControllerInput : MonoBehaviour
{
    //local variables
    #region
    ToolsetControls toolSet;
    PlacementBehaviour behaviourTool;

    string trackPadClick_tool = "TrackPadClick_Left";
    string trackPadClick_steering = "TrackPadClick_Right";

    string trackPadPos_tool = "TrackPadPos_Left";
    string trackPadPos_steering = "TrackPadPos_Right";

    string triggerClick_tool = "TriggerClick_Left";
    string triggerClick_steering = "TriggerClick_Right";

    Vector2 toolTrackPos = Vector2.zero;
    bool toolTrackDown = false;
    bool toolTrack = false;
    bool toolPressed = false;

    Vector2 steeringTrackPos = Vector2.zero;
    bool steeringTrackDown = false;
    bool steeringTrack = false;
    bool steeringTriggerDown = false;
    #endregion


    //MonoBehaviour methods
    #region
    void Start()
    {
        toolSet = gameObject.GetComponentInChildren<ToolsetControls>();
        behaviourTool = gameObject.GetComponentInChildren<PlacementBehaviour>();
    }
    

    void Update()
    {
        ToolControllerAction();

        SteeringControllerAction();
    }
    #endregion


    //Controller actions
    #region
    void ToolControllerAction()
    {
        toolTrackPos = SteamVR_Input.GetVector2(trackPadPos_tool, SteamVR_Input_Sources.Any, true);
        toolTrackDown = SteamVR_Input.GetStateDown(trackPadClick_tool, SteamVR_Input_Sources.Any, true);
        toolTrack = SteamVR_Input.GetState(trackPadClick_tool, SteamVR_Input_Sources.Any, true);

        if (SteamVR_Input.GetStateDown(triggerClick_tool, SteamVR_Input_Sources.Any, true))
        {
            toolSet.TriggerDown();
        }
        else if (toolTrackDown && toolTrackPos.y < 0.5f && toolTrackPos.y > -0.5f)
        {
            if (toolTrackPos.x < -0.5f)
            {
                toolSet.PadRight(Button.release);
                toolSet.PadLeft(Button.press);
                toolPressed = true;
            }
            else if (toolTrackPos.x > 0.5f)
            {
                toolSet.PadLeft(Button.release);
                toolSet.PadRight(Button.press);
                toolPressed = true;
            }
        }
        else if (toolTrackDown && toolTrackPos.x < 0.5f && toolTrackPos.x > -0.5f)
        {
            if (toolTrackPos.y < -0.5f)
            {
                toolSet.PadDown();
            }
            else if (toolTrackPos.y > 0.5f)
            {
                toolSet.PadUp();
            }
        }

        if (!toolTrack && toolPressed)
        {
            toolSet.PadLeft(Button.release);
            toolSet.PadRight(Button.release);
            toolPressed = false;
        }
    }


    void SteeringControllerAction()
    {
        steeringTrackPos = SteamVR_Input.GetVector2(trackPadPos_steering, SteamVR_Input_Sources.Any, true);
        steeringTrackDown = SteamVR_Input.GetStateDown(trackPadClick_steering, SteamVR_Input_Sources.Any, true);
        steeringTrack = SteamVR_Input.GetState(trackPadClick_steering, SteamVR_Input_Sources.Any, true);

        if (SteamVR_Input.GetStateDown(triggerClick_steering, SteamVR_Input_Sources.Any, true))
        {
            behaviourTool.Engage();
            steeringTriggerDown = true;
        }
        else if (steeringTrackDown && steeringTrackPos.y < 0.5f && steeringTrackPos.y > -0.5f)
        {
            if (steeringTrackPos.x < -0.5f)
            {
                behaviourTool.ChangePart(PlacementBehaviour.ToolsetControls.Left);
            }
            else if (steeringTrackPos.x > 0.5f)
            {
                behaviourTool.ChangePart(PlacementBehaviour.ToolsetControls.Right);
            }
        }
        else if (steeringTrack && steeringTrackPos.x < 0.5f && steeringTrackPos.x > -0.5f)
        {
            if (steeringTrackPos.y < -0.5f)
            {
                behaviourTool.ScrollPart(PlacementBehaviour.ToolsetControls.Down);
            }
            else if (steeringTrackPos.y > 0.5f)
            {
                behaviourTool.ScrollPart(PlacementBehaviour.ToolsetControls.Up);
            }
        }


        if (!SteamVR_Input.GetState(triggerClick_steering, SteamVR_Input_Sources.Any, true) && steeringTriggerDown)
        {
            behaviourTool.DisEngage();
            steeringTriggerDown = false;
        }
    }
    #endregion


    //Enumerations
    #region
    public enum Button
    {
        press = 0,
        release = 1
    }
    #endregion
}
