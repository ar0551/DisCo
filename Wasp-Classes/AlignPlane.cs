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
using System;
using UnityEngine;

public class AlignPlane : ICloneable
    {

    //properties (readOnly)
    #region
    //World Plane
    private Transform parent;
    public Transform Parent
    {
        get {return parent;}
        set { parent = value; }
    }

    private Vector3 origin = Vector3.zero;
    public Vector3 Origin
    {
        get
        {
            if (parent != null)
            {
                return parent.position + (Quaternion.Euler(parent.eulerAngles) * origin);
            }
            else
            {
                throw new Exception("You are trying to access a destroyed GameObject via one of its AlignPlanes.");
            }
        }
    }

    private Vector3 xVector= Vector3.right;
    public Vector3 XVector
    {
        get { return Quaternion.Euler(parent.eulerAngles) * xVector; }
    }

    private Vector3 yVector = Vector3.up;
    public Vector3 YVector
    {
        get { return Quaternion.Euler(parent.eulerAngles) * yVector; }
    }

    public Vector3 ZVector
    {
        get { return Quaternion.Euler(parent.eulerAngles) * ConstructZ(); }
    }

    public Vector3 InvertedZVector
    {
        get { return Quaternion.Euler(parent.eulerAngles) * (ConstructZ() * -1);  }
    }

    //Local Plane
    public Vector3 LocalOrigin
    {
        get { return origin; }
    }

    public Vector3 LocalXVector
    {
        get { return xVector; }
    }

    public Vector3 LocalYVector
    {
        get { return yVector; }
    }

    public Vector3 LocalZVector
    {
        get { return ConstructZ(); }
    }
    #endregion


    //constructors
    #region
    public AlignPlane(Vector3 orig, Transform par)
    {
        origin = orig;
        parent = par;
    }

    public AlignPlane(Vector3 orig, Vector3 xVec, Vector3 yVec, Transform par)
    {
        origin = orig;
        parent = par;
        xVector = xVec;
        yVector = yVec;
    }
    #endregion


    //methods
    #region
    public void MoveOrigin(Vector3 orig)
    {
        origin = orig;
    }

    public void ReAlign(Vector3 orig, Vector3 xVec, Vector3 yVec)
    {
        origin = orig;
        xVector = xVec.normalized;

        Vector3 tempZ = Vector3.Cross(xVec, yVec);
        yVector = Quaternion.AngleAxis(90,tempZ) * xVec;
    }

    public void Flip()
    {
        yVector = yVector * -1;
    }

    private Vector3 ConstructZ()
    {
        return Vector3.Cross(XVector, YVector);
    }

    public override string ToString()
    {
        string temp = "O " + Origin.ToString("F2") + " Z " + ZVector.ToString("F2");
        return temp;
    }

    public Quaternion GetEulerQuaternion()
    {
        Quaternion xRot = Quaternion.AngleAxis(Vector3.Angle(Vector3.right, XVector), Vector3.Cross(Vector3.right, XVector));
        Vector3 tempX = xRot * Vector3.right;

        Quaternion yRot = Quaternion.AngleAxis(Vector3.Angle(Vector3.up, YVector), Vector3.Cross(Vector3.up, YVector));
        return Quaternion.LookRotation( YVector,- ConstructZ());
    }

    public object Clone()
    {
        AlignPlane cloneAlign = new AlignPlane(origin, xVector, yVector, parent);
        return cloneAlign;
    }
    #endregion


    //static methods
    #region
    public static void Orient(AlignPlane from, AlignPlane to, GameObject geo)
    {
        Vector3 posMove = to.Origin - from.Origin;
        geo.transform.position += posMove;

        while (Vector3.Angle(from.XVector, to.XVector) > 0.1)
        {
            geo.transform.RotateAround(to.Origin, Vector3.Cross(from.XVector, to.XVector), Vector3.Angle(from.XVector, to.XVector));
        }

        while (Vector3.Angle(from.YVector, to.YVector) > 0.1)
        {
            geo.transform.RotateAround(to.Origin, to.XVector, Vector3.Angle(from.YVector, to.YVector));
        }


        geo.transform.RotateAround(to.Origin, to.XVector, 180);
    }

    public static AlignPlane FlipCopy(AlignPlane pln)
    {
        AlignPlane pl = new AlignPlane(pln.origin, pln.xVector, pln.yVector * -1, pln.parent);
        return pl;
    }

    public static float BuildAngle(Vector3 from, Vector3 to, bool flip)
    {
        Vector3 too;

        if (flip)
        {
            too = new Vector3(to.x * -1, to.y * -1, to.z * -1);
        }
        else
        {
            too = to;
        }

        double toppart = 0;
        for (int d = 0; d < 3; d++) toppart += from[d] * too[d];

        double u2 = 0; //u squared
        double v2 = 0; //v squared
        for (int d = 0; d < 3; d++)
        {
            u2 += from[d] * from[d];
            v2 += too[d] * too[d];
        }

        double bottompart = 0;
        bottompart = Math.Sqrt(u2 * v2);


        double rtnval = Math.Acos(toppart / bottompart);
        rtnval *= 360.0 / (2 * Math.PI);

        return (float)rtnval;
    }
    #endregion
}
