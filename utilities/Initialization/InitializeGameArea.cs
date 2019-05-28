﻿/*
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
using System;

public class InitializeGameArea : MonoBehaviour
{
    //static variables
    #region
    static float conMinX;
    static float conMaxX;

    static float conMinY;
    static float conMaxY;

    static float conMinZ;
    static float conMaxZ;


    static float collMinX;
    static float collMaxX;

    static float collMinY;
    static float collMaxY;

    static float collMinZ;
    static float collMaxZ;
    #endregion

    //properties
    #region
    public static int ConX
    {
        get { return (int)Math.Ceiling((conMaxX - conMinX) / ConnectionStep); }
    }

    public static int ConY
    {
        get { return (int)Math.Ceiling((conMaxY - conMinY) / ConnectionStep); }
    }

    public static int ConZ
    {
        get { return (int)Math.Ceiling((conMaxZ - conMinZ) / ConnectionStep); }
    }

    public static int ConOffsetX
    {
        get { return (int)Math.Floor(conMinX / ConnectionStep); }
    }

    public static int ConOffsetY
    {
        get { return (int)Math.Floor(conMinY / ConnectionStep); }
    }

    public static int ConOffsetZ
    {
        get { return (int)Math.Floor(conMinZ / ConnectionStep); }
    }

    static float _conStep;
    public static float ConnectionStep
    {
        get { return _conStep; }
    }


    public static int CollX
    {
        get { return (int)Math.Ceiling((collMaxX - collMinX) / CollisionStep); }
    }

    public static int CollY
    {
        get { return (int)Math.Ceiling((collMaxY - collMinY) / CollisionStep); }
    }

    public static int CollZ
    {
        get { return (int)Math.Ceiling((collMaxZ - collMinZ) / CollisionStep); }
    }

    public static int CollOffsetX
    {
        get { return (int)Math.Floor(collMinX / CollisionStep); }
    }

    public static int CollOffsetY
    {
        get { return (int)Math.Floor(collMinY / CollisionStep); }
    }

    public static int CollOffsetZ
    {
        get { return (int)Math.Floor(collMinZ / CollisionStep); }
    }

    static float _collStep;
    public static float CollisionStep
    {
        get { return _collStep; }
    }
    #endregion

    // Start is called before the first frame update
    public void Initialize(float minX, float maxX, float minY, float maxY, float minZ, float maxZ, float connectionStep, float colliderStep)
    {
        conMaxX = (float)(Math.Ceiling(maxX / connectionStep) * connectionStep);
        conMinX = (float)(Math.Floor(minX / connectionStep) * connectionStep);

        conMaxY = (float)(Math.Ceiling(maxY / connectionStep) * connectionStep);
        conMinY = (float)(Math.Floor(minY / connectionStep) * connectionStep);

        conMaxZ = (float)(Math.Ceiling(maxZ / connectionStep) * connectionStep);
        conMinZ = (float)(Math.Floor(minZ / connectionStep) * connectionStep);


        collMaxX = (float)(Math.Ceiling(maxX / colliderStep) * colliderStep);
        collMinX = (float)(Math.Floor(minX / colliderStep) * colliderStep);

        collMaxY = (float)(Math.Ceiling(maxY / colliderStep) * colliderStep);
        collMinY = (float)(Math.Floor(minY / colliderStep) * colliderStep);

        collMaxZ = (float)(Math.Ceiling(maxZ / colliderStep) * colliderStep);
        collMinZ = (float)(Math.Floor(minZ / colliderStep) * colliderStep);


        _conStep = connectionStep;

        _collStep = colliderStep;

        ConnectionVoxelContainer.Initialize(ConX, ConY, ConZ, ConOffsetX, ConOffsetY, ConOffsetZ);

        CollisionVoxelContainer.Initialize(CollX, CollY, CollZ, CollOffsetX, CollOffsetY, CollOffsetZ);
    }
}
