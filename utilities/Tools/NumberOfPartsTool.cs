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
using TMPro;

public class NumberOfPartsTool : MonoBehaviour
{
    //properties
    #region
    private TextMeshPro textMesh;
    public TextMeshPro TextMesh
    {
        get { return textMesh; }
    }

    private static bool active = false;
    public static bool Active
    {
        get { return active; }
    }

    private static int affectedParts = 10;
    public static int AffectedParts
    {
        get { return affectedParts; }
    }
    #endregion


    //MonoBehaviour methods
    #region
    private void Start()
    {
        textMesh = gameObject.transform.GetComponentInChildren<TextMeshPro>();
        textMesh.faceColor = GlobalReferences.AffectedColor;
        textMesh.text = affectedParts.ToString();
    }
    #endregion

    //methods
    #region
    public void ChangeTool(ToolsetControls.InputX input)
    {
        if (input == ToolsetControls.InputX.Enter)
        {
            return;
        }
        else if (input == ToolsetControls.InputX.Left)
        {
            if (affectedParts > 0)
            {
                --affectedParts;
                if (GlobalReferences.PlacementType == PlacementTypeTool.PlaceChoreo.Choreo)
                {
                    GlobalReferences.ChangeAffectedNumber(affectedParts);
                }
            }
            textMesh.text = affectedParts.ToString();
        }
        else if (input == ToolsetControls.InputX.Right)
        {
            if (affectedParts < 999 && affectedParts <= GlobalReferences.FreeParts.Count + GlobalReferences.AffectedParts.Count)
            {
                ++affectedParts;
                if (GlobalReferences.PlacementType == PlacementTypeTool.PlaceChoreo.Choreo)
                {
                    GlobalReferences.ChangeAffectedNumber(affectedParts);
                }
            }
            textMesh.text = affectedParts.ToString();
        }
    }
    #endregion
}
