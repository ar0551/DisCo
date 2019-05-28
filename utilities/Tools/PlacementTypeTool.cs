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

using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlacementTypeTool : MonoBehaviour
{
    //local variables
    #region
    PlaceChoreo offset = 0;
    PlaceChoreo active = PlaceChoreo.Place;

    [SerializeField]
    private List<TextMeshPro> textMeshes = new List<TextMeshPro>();
    private List<MeshRenderer> renderers = new List<MeshRenderer>();

    Vector3 translateOffset = new Vector3(0.1f, 0, 0);
    #endregion


    //Monobehaviour methods
    #region
    private void Start()
    {
        foreach (TextMeshPro tm in textMeshes)
        {
            renderers.Add(tm.gameObject.GetComponent<MeshRenderer>());
        }

        ChangeColor();
        ToggleSelected(false);
    }
    #endregion


    //controller handling methods
    #region
    private void ChangeColor()
    {
        for (int i = 0; i < textMeshes.Count; ++i)
        {
            if (i == (int)active)
            {
                textMeshes[i].faceColor = GlobalReferences.AffectedColor;
            }
            else
            {
                textMeshes[i].faceColor = GlobalReferences.UnaffectedColor;
            }
        }
    }


    public void ToggleSelected(bool selected)
    {
        if (!selected)
        {
            for (int i = 0; i < textMeshes.Count; ++i)
            {
                if (i != (int)offset)
                {
                    renderers[i].enabled = false;
                }
                else
                {
                    renderers[i].enabled = true;
                }
            }
        }
        else
        {
            for (int i = 0; i < textMeshes.Count; ++i)
            {
                renderers[i].enabled = true;

            }
        }
    }


    public void ChangeTool(ToolsetControls.InputX input)
    {
        if (input == ToolsetControls.InputX.Enter)
        {
            if (active != offset)
            {
                active = offset;
                GlobalReferences.PlacementType = (PlaceChoreo)active;
                ToolChangeCaseHandler();
                ChangeColor();
            }
        }
        else if (input == ToolsetControls.InputX.Left)
        {
            if (offset > 0)
            {
                --offset;
                GameObject go = gameObject.transform.GetChild(0).gameObject;
                go.transform.localPosition = go.transform.localPosition - translateOffset;
            }
        }
        else if (input == ToolsetControls.InputX.Right)
        {
            if ((int)offset < textMeshes.Count - 1)
            {
                ++offset;
                GameObject go = gameObject.transform.GetChild(0).gameObject;
                go.transform.localPosition = go.transform.localPosition + translateOffset;
            }
        }
    }


    void ToolChangeCaseHandler()
    {
        switch (active)
        {
            case PlaceChoreo.Place:
                {
                    deactivateAllPlacementTools();
                    PlacementBehaviour.CarryGo[PlacementBehaviour.TempID].SetActive(true);
                    break;
                }
            case PlaceChoreo.Choreo:
                {
                    deactivateAllPlacementTools();
                    GlobalReferences.ChangeAffectedNumber(NumberOfPartsTool.AffectedParts);
                    break;
                }
            case PlaceChoreo.PickNChose:
                {
                    deactivateAllPlacementTools();
                    PlacementBehaviour.Aiming = true;
                    break;
                }
            case PlaceChoreo.Delete:
                {
                    deactivateAllPlacementTools();
                    PlacementBehaviour.Aiming = true;
                    break;
                }
            case PlaceChoreo.DisableScanning:
                {
                    deactivateAllPlacementTools();
                    PlacementBehaviour.DisableSphere.SetActive(true);
                    break;
                }
        }
    }


    void deactivateAllPlacementTools()
    {
        GlobalReferences.ClearAffectedList();
        PlacementBehaviour.CarryGo[PlacementBehaviour.TempID].SetActive(false);
        PlacementBehaviour.ReleasePart();
        PlacementBehaviour.Aiming = false;
        PlacementBehaviour.DisableSphere.SetActive(false);
    }
    #endregion


    //Enumerations
    #region
    public enum PlaceChoreo
    {
        Place = 0,
        Choreo = 1,
        PickNChose = 2,
        Delete = 3,
        DisableScanning = 4
    }
    #endregion
}
