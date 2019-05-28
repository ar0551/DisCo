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

public class PartTypeTool : MonoBehaviour
{
    //local variables
    #region
    int offset = 0;

    List<GameObject> templates = new List<GameObject>();
    List<MeshRenderer> renderers = new List<MeshRenderer>();
    static List<bool> activityFlags = new List<bool>();
    static List<string> partNames = new List<string>();

    Vector3 translateOffset = new Vector3(0.1f, 0, 0);
    float scale = 100;

    Material activeMat;
    Material inactiveMat;
    #endregion


    //MonoBehaviour Methods
    #region
    private void Start()
    {
        activeMat = Resources.Load<Material>("Materials/Toolset/Toolset_Font");
        inactiveMat = Resources.Load<Material>("Materials/Toolset/Toolset_Font_Inactive");

        foreach (GameObject go in GlobalReferences.TemplateParts)
        {
            ScaleFactor(go.GetComponent<MeshFilter>().sharedMesh, 0.06f);
        }

        for (int i = 0; i < GlobalReferences.TemplateParts.Count; ++i)
        {
            SpawnAvatar(i);
        }

        ChangeColor();
        ToggleSelected(false);
    }
    #endregion


    //geometry initialization methods
    #region
    private void SpawnAvatar(int i)
    {
        activityFlags.Add(true);
        partNames.Add(GlobalReferences.TemplateParts[i].GetComponent<Part>().Name);

        GameObject go = new GameObject();
        go.transform.SetParent(gameObject.transform);

        renderers.Add(go.AddComponent<MeshRenderer>());

        MeshFilter filter = go.AddComponent<MeshFilter>();
        filter.sharedMesh = GlobalReferences.TemplateParts[i].GetComponent<MeshFilter>().sharedMesh;

        go.transform.localRotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = new Vector3(scale, scale, scale);
        go.transform.localPosition = go.transform.localPosition + (new Vector3(-0.1f * i, -0.03f, 0.035f));

        templates.Add(go);
    }

    private float ScaleFactor(Mesh m, float maxSize)
    {
        float _scale;

        Vector3 b = m.bounds.size;
        _scale = b.x;
        if (b.y > _scale)
            _scale = b.y;
        if (b.z > _scale)
            _scale = b.z;

        _scale = maxSize / _scale;
        if (_scale < scale)
        {
            scale = _scale;
        }
        return _scale;
    }
    #endregion


    //controller handling methods
    #region
    private void ChangeColor()
    {
        for (int i = 0; i < activityFlags.Count; ++i)
        {
            if (activityFlags[i])
            {
                renderers[i].material = activeMat;
            }
            else
            {
                renderers[i].material = inactiveMat;
            }
        }
    }

    public void ToggleSelected(bool selected)
    {
        if (!selected)
        {
            for (int i = 0; i < activityFlags.Count; ++i)
            {
                if (i != offset)
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
            for (int i = 0; i < activityFlags.Count; ++i)
            {
                renderers[i].enabled = true;

            }
        }
    }

    public void ChangeTool(ToolsetControls.InputX input)
    {

        if (input == ToolsetControls.InputX.Enter)
        {
            if (activityFlags[offset])
            {
                activityFlags[offset] = false;
                GlobalReferences.TypeFilter.Remove(partNames[offset]);

                GlobalReferences.ClearAffectedList();
                if (GlobalReferences.PlacementType == PlacementTypeTool.PlaceChoreo.Choreo)
                {
                    GlobalReferences.ChangeAffectedNumber(NumberOfPartsTool.AffectedParts);
                }
            }
            else
            {
                activityFlags[offset] = true;
                GlobalReferences.TypeFilter.Add(partNames[offset]);

                GlobalReferences.ClearAffectedList();
                if (GlobalReferences.PlacementType == PlacementTypeTool.PlaceChoreo.Choreo)
                {
                    GlobalReferences.ChangeAffectedNumber(NumberOfPartsTool.AffectedParts);
                }
            }

            ChangeColor();
        }
        else if (input == ToolsetControls.InputX.Left)
        {
            if (offset > 0)
            {
                --offset;
                foreach (GameObject go in templates)
                {
                    go.transform.localPosition = go.transform.localPosition - translateOffset;
                }
            }
        }
        else if (input == ToolsetControls.InputX.Right)
        {
            if (offset < activityFlags.Count - 1)
            {
                ++offset;
                foreach (GameObject go in templates)
                {
                    go.transform.localPosition = go.transform.localPosition + translateOffset;
                }
            }
        }
    }
    #endregion
}
