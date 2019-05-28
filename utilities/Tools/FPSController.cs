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

public class FPSController : MonoBehaviour
{
    //local variables
    #region
    ToolsetControls toolSet;
    PlacementBehaviour behaviourTool;
    Texture2D crosshairImage;

    bool toolTrackUp = false;
    bool toolTrackDown = false;
    bool toolTrackLeft = false;
    bool toolTrackRight = false;

    bool toolEnter = false;
    bool toolPressed = false;
    bool toolLeftRightDown = false;

    bool steeringTrackUp = false;
    bool steeringTrackDown = false;

    bool steeringMouseRight = false;
    bool steeringEnter = false;

    bool hidden = false;
    #endregion


    //MonoBehaviour Methods
    #region
    void Start()
    {
        toolSet = gameObject.GetComponentInChildren<ToolsetControls>();
        behaviourTool = gameObject.GetComponentInChildren<PlacementBehaviour>();

        PlacementBehaviour.ViewDirection = new Vector3(0, 0, 1);
        PlacementBehaviour.PlayMode = PlacementBehaviour.Mode.FPS;

        crosshairImage = Resources.Load<Texture2D>("Materials/Toolset/crosshair");
    }

    void Update()
    {
        ToolControllerAction();

        SteeringControllerAction();

        HideAllRenderers();
    }

    private void OnGUI()
    {
        if (PlacementBehaviour.Aiming)
        {
            float xMin = (Screen.width / 2) - (crosshairImage.width / 2);
            float yMin = (Screen.height / 2) - (crosshairImage.height / 2);
            GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width, crosshairImage.height), crosshairImage);
        }
    }
    #endregion


    void ToolControllerAction()
    {
        toolTrackUp = Input.GetKeyDown(KeyCode.UpArrow);
        toolTrackDown = Input.GetKeyDown(KeyCode.DownArrow);
        toolTrackLeft = Input.GetKeyDown(KeyCode.LeftArrow);
        toolTrackRight = Input.GetKeyDown(KeyCode.RightArrow);

        toolEnter = Input.GetKeyDown(KeyCode.Return);

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            toolLeftRightDown = true;
        }
        else
        {
            toolLeftRightDown = false;
        }

        if (toolEnter)
        {
            toolSet.TriggerDown();
        }
        else if (toolTrackLeft)
        {
            toolSet.PadRight(ViveControllerInput.Button.release);
            toolSet.PadLeft(ViveControllerInput.Button.press);
            toolPressed = true;
        }
        else if (toolTrackRight)
        {
            toolSet.PadLeft(ViveControllerInput.Button.release);
            toolSet.PadRight(ViveControllerInput.Button.press);
            toolPressed = true;
        }
        else if (toolTrackDown)
        {
            toolSet.PadDown();
        }
        else if (toolTrackUp)
        {
            toolSet.PadUp();
        }
        

        if (!toolLeftRightDown && toolPressed)
        {
            toolSet.PadLeft(ViveControllerInput.Button.release);
            toolSet.PadRight(ViveControllerInput.Button.release);
            toolPressed = false;
        }
    }


    void SteeringControllerAction()
    {
        steeringEnter = Input.GetKeyDown(KeyCode.Mouse0);
        steeringMouseRight = Input.GetKeyDown(KeyCode.Mouse1);

        steeringTrackUp = false;
        steeringTrackDown = false;

        if (Input.mouseScrollDelta.y > 0)
        {
            steeringTrackUp = true;
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            steeringTrackDown = true;
        }


        if (steeringEnter)
        {
            behaviourTool.Engage();
        }
        else if (steeringTrackDown)
        {
            for (int i = 0; i < 10; ++i)
            {
                behaviourTool.ScrollPart(PlacementBehaviour.ToolsetControls.Down);
            }
        }
        else if (steeringTrackUp)
        {
            for (int i = 0; i < 10; ++i)
            {
                behaviourTool.ScrollPart(PlacementBehaviour.ToolsetControls.Up);
            }
        }
        else if (steeringMouseRight)
        {
            behaviourTool.ChangePart(PlacementBehaviour.ToolsetControls.Left);
        }
        


        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            behaviourTool.DisEngage();
        }
    }

    void HideAllRenderers()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            foreach (MeshRenderer mr in gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                mr.enabled = hidden;
            }

            hidden = !hidden;
        }
    }
}
