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
using System.Text;

public class RuleFilterTool : MonoBehaviour
{
    //local variables
    #region
    int offset = 0;

    [SerializeField]
    TextMeshPro templateTM = null;

    private List<TextMeshPro> textMeshes = new List<TextMeshPro>();
    private List<MeshRenderer> renderers = new List<MeshRenderer>();

    static List<bool> activityFlags = new List<bool>();

    Vector3 translateOffset = new Vector3(0.1f, 0, 0);
    #endregion


    //MonoBehaviour methods
    #region
    private void Start()
    {
        List<GameObject> gos = new List<GameObject>();

        int i = 0;
        foreach (string s in GlobalReferences.RulesGrammer)
        {
            string grammer = FormatRule(s);

            GameObject go = BuildTextFields(i, grammer);

            gos.Add(go);

            activityFlags.Add(true);
            renderers.Add(go.GetComponent<MeshRenderer>());
            ++i;
        }

        i = 0;
        foreach (GameObject go in gos)
        {
            go.transform.localPosition = go.transform.localPosition - translateOffset * i;
            ++i;
        }

        ChangeColor();
        ToggleSelected(false);
    }
    #endregion


    //geometry and List initialization methods
    #region
    private string FormatRule(string s)
    {
        string grammer = s;

        if (grammer.Length > 12)
        {
            StringBuilder sb = new StringBuilder();
            int saveLength = 0;

            foreach (char c in grammer)
            {
                sb.Append(c);
                if (c == '>' || c == '<')
                {
                    if (sb.Length > 12)
                    {
                        translateOffset.x = 12 * 0.1f / sb.Length;
                    }
                    sb.Append("\n");
                    saveLength = sb.Length;
                }
            }

            if (sb.Length - saveLength > saveLength)
            {
                translateOffset.x = 12 * 0.1f / (sb.Length - saveLength);
            }
            else if (saveLength <= 12 && sb.Length - saveLength > 12)
            {
                translateOffset.x = 12 * 0.1f / (sb.Length - saveLength);
            }

            grammer = sb.ToString();
        }

        return grammer;
    }

    private GameObject BuildTextFields(int i, string grammer)
    {
        GameObject go = null;

        if (i == 0)
        {
            go = templateTM.gameObject;
            textMeshes.Add(go.GetComponent<TextMeshPro>());
            textMeshes[i].text = grammer;
        }
        else
        {
            go = Instantiate(templateTM.gameObject, templateTM.transform.parent);
            textMeshes.Add(go.GetComponent<TextMeshPro>());
            textMeshes[i].text = grammer;
        }

        return go;
    }
    #endregion


    //controller handling methods
    #region
    private void ChangeColor()
    {
        for (int i = 0; i < textMeshes.Count; ++i)
        {
            if (activityFlags[i])
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
            if (activityFlags[offset])
            {
                activityFlags[offset] = false;

                int delIdx = -1;
                for (int i = 0; i < GlobalReferences.ActiveRulesGrammer.Count; ++i)
                {
                    if (string.Compare(GlobalReferences.ActiveRulesGrammer[i], GlobalReferences.RulesGrammer[offset]) == 0)
                    {
                        delIdx = i;
                    }
                }

                if (delIdx != -1)
                {
                    GlobalReferences.ActiveRulesGrammer.RemoveAt(delIdx);
                }
            }
            else
            {
                activityFlags[offset] = true;
                if (!GlobalReferences.ActiveRulesGrammer.Contains(GlobalReferences.RulesGrammer[offset]))
                {
                    GlobalReferences.ActiveRulesGrammer.Add(string.Copy(GlobalReferences.RulesGrammer[offset]));
                }
            }

            ChangeColor();
        }
        else if (input == ToolsetControls.InputX.Left)
        {
            if (offset > 0)
            {
                --offset;
                GameObject go = gameObject.transform.GetChild(0).gameObject;
                go.transform.localPosition = go.transform.localPosition - translateOffset;
                ChangeColor();
            }
        }
        else if (input == ToolsetControls.InputX.Right)
        {
            if (offset <= textMeshes.Count - 2)
            {
                ++offset;
                GameObject go = gameObject.transform.GetChild(0).gameObject;
                go.transform.localPosition = go.transform.localPosition + translateOffset;
                ChangeColor();
            }
        }
    }
    #endregion
}
