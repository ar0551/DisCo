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

public class PlacementBehaviour : MonoBehaviour
{
    //local variables
    #region
    [SerializeField]
    GameObject disableSphere;

    static GameObject pickupCarry = null;
    GameObject pickup = null;
    GameObject toDelete = null;

    ConnectionScanningHandler handler = null;

    DisableTrigger trigger = null;

    RaycastHit hit;
    int layerMaskPickup;
    int layerMaskDelete;

    Vector3[] lrPos = new Vector3[2];
    Camera cam = null;

    Vector3 rayFrom = Vector3.zero;
    Vector3 rayTo = Vector3.zero;

    static Material frozenMat = null;
    static Material selectedMat = null;
    static Material unselectedMat = null;
    static Material highlightFrozenMat = null;
    static Material disableSphereMat = null;
    static Material enableSphereMat = null;
    #endregion


    //properties
    #region
    private static GameObject disSphere = null;
    public static GameObject DisableSphere
    {
        get { return disSphere; }
        set { disSphere = value; }
    }

    private static Mode playMode = Mode.VR;
    public static Mode PlayMode
    {
        get { return playMode; }
        set { playMode = value; }
    }

    private static bool aiming = false;
    public static bool Aiming
    {
        get { return aiming; }
        set
        {
            if (PlayMode == Mode.VR && LR != null)
            {
                LR.enabled = value;
            }
            aiming = value;
        }
    }

    static Vector3 viewDirection = Vector3.forward;
    public static Vector3 ViewDirection
    {
        get { return viewDirection; }
        set { viewDirection = value; }
    }

    static LineRenderer lr;
    public static LineRenderer LR
    {
        get { return lr; }
    }

    private static GameObject[] carryGo;
    public static GameObject[] CarryGo
    {
        get { return carryGo; }
    }

    private static int tempID = 0;
    public static int TempID
    {
        get { return tempID; }
    }
    #endregion


    //Monobehaviour methods
    #region
    private void Start()
    {
        if (PlayMode == Mode.VR)
        {
            lr = gameObject.GetComponent<LineRenderer>();
        }

        lrPos[0] = Vector3.zero;
        lrPos[1] = Vector3.forward;

        cam = GetComponentInParent<Camera>();

        Aiming = false;

        carryGo = new GameObject[GlobalReferences.TemplateParts.Count];

        frozenMat = Resources.Load<Material>("Materials/PlacedMaterial");

        selectedMat = Resources.Load<Material>("Materials/affectedMaterial");

        unselectedMat = Resources.Load<Material>("Materials/unaffectedMaterial");

        highlightFrozenMat = Resources.Load<Material>("Materials/highlightPlacedMaterial");

        enableSphereMat = Resources.Load<Material>("Materials/Toolset/enableSphereMat");

        disableSphereMat = Resources.Load<Material>("Materials/Toolset/disableSphereMat");

        trigger = disableSphere.GetComponent<DisableTrigger>();

        for (int i = 0; i < GlobalReferences.TemplateParts.Count; ++i)
        {
            SpawnObject(i);
        }

        carryGo[tempID].SetActive(true);

        layerMaskPickup = 1 << 8;

        layerMaskDelete = 1 << 9;

        disSphere = disableSphere;
        DisableSphere.SetActive(false);
    }

    private void Update()
    {
        CastPickUpRay();

        CastDeleteRay();
    }
    #endregion


    //controller handling methods
    #region
    public void ChangePart(ToolsetControls input)
    {
        if (GlobalReferences.PlacementType == PlacementTypeTool.PlaceChoreo.Place)
        {
            switch (input)
            {
                case ToolsetControls.Left:
                    {
                        if (tempID == 0)
                        {
                            carryGo[tempID].SetActive(false);
                            tempID = carryGo.Length - 1;
                            carryGo[tempID].SetActive(true);
                        }
                        else
                        {
                            carryGo[tempID].SetActive(false);
                            --tempID;
                            carryGo[tempID].SetActive(true);
                        }
                        break;
                    }
                case ToolsetControls.Right:
                    {
                        if (tempID == carryGo.Length - 1)
                        {
                            carryGo[tempID].SetActive(false);
                            tempID = 0;
                            carryGo[tempID].SetActive(true);
                        }
                        else
                        {
                            carryGo[tempID].SetActive(false);
                            ++tempID;
                            carryGo[tempID].SetActive(true);
                        }
                        break;
                    }
            }
        }

        if (GlobalReferences.PlacementType == PlacementTypeTool.PlaceChoreo.DisableScanning)
        {

            if (trigger.AddRem == DisableTrigger.AddRemove.Add)
            {
                trigger.AddRem = DisableTrigger.AddRemove.Remove;
                disableSphere.GetComponent<MeshRenderer>().material = disableSphereMat;
            }
            else
            {
                trigger.AddRem = DisableTrigger.AddRemove.Add;
                disableSphere.GetComponent<MeshRenderer>().material = enableSphereMat;
            }
            
        }
    }


    public void Engage()
    {
        switch (GlobalReferences.PlacementType)
        {
            case PlacementTypeTool.PlaceChoreo.Place:
                {
                    FreezeObject();
                    break;
                }
            case PlacementTypeTool.PlaceChoreo.Choreo:
                {
                    GlobalReferences.DriftApartFac = 1;
                    break;
                }
            case PlacementTypeTool.PlaceChoreo.PickNChose:
                {
                    if (pickup != null)
                    {
                        PickupPart();
                    }
                    else if (pickupCarry != null && pickup == null)
                    {
                        ReleasePart();
                    }
                    break;
                }
            case PlacementTypeTool.PlaceChoreo.Delete:
                {
                    if (toDelete != null)
                    {
                        GlobalReferences.DeletePart((int)toDelete.GetComponent<Part>().ID);
                    }
                    break;
                }
            case PlacementTypeTool.PlaceChoreo.DisableScanning:
                {
                    trigger.Enable = true;
                    trigger.Triggered = true;
                    break;
                }
        }
    }


    public void DisEngage()
    {
        switch (GlobalReferences.PlacementType)
        {
            case PlacementTypeTool.PlaceChoreo.Choreo:
                {
                    GlobalReferences.DriftApartFac = 0;
                    break;
                }
            case PlacementTypeTool.PlaceChoreo.DisableScanning:
                {
                    trigger.Enable = false;
                    break;
                }
        }
    }


    public void ScrollPart(ToolsetControls input)
    {
        if (GlobalReferences.PlacementType == PlacementTypeTool.PlaceChoreo.Place)
        {
            switch (input)
            {
                case ToolsetControls.Up:
                    {
                        carryGo[tempID].transform.localPosition = carryGo[tempID].transform.localPosition + Vector3.forward * Time.deltaTime;
                        break;
                    }
                case ToolsetControls.Down:
                    {
                        if (carryGo[tempID].transform.localPosition.z > -0.9f)
                        {
                            carryGo[tempID].transform.localPosition = carryGo[tempID].transform.localPosition + Vector3.back * Time.deltaTime;
                        }
                        break;
                    }
            }
        }
        else if (GlobalReferences.PlacementType == PlacementTypeTool.PlaceChoreo.PickNChose && pickupCarry != null)
        {
            switch (input)
            {
                case ToolsetControls.Up:
                    {
                        pickupCarry.transform.localPosition = pickupCarry.transform.localPosition + Vector3.forward * Time.deltaTime;
                        break;
                    }
                case ToolsetControls.Down:
                    {
                        if (carryGo[tempID].transform.localPosition.z > -0.9f)
                        {
                            pickupCarry.transform.localPosition = pickupCarry.transform.localPosition + Vector3.back * Time.deltaTime;
                        }
                        break;
                    }
            }
        }
        else if (GlobalReferences.PlacementType == PlacementTypeTool.PlaceChoreo.DisableScanning)
        {
            switch (input)
            {
                case ToolsetControls.Up:
                    {
                        disableSphere.transform.localScale += Vector3.one * Time.deltaTime;
                        if (PlayMode == Mode.FPS)
                        {
                            disableSphere.transform.position += disableSphere.transform.up * Time.deltaTime / 4;
                        }
                        else
                        {
                            disableSphere.transform.position += disableSphere.transform.forward * Time.deltaTime / 2;
                        }
                        break;
                    }
                case ToolsetControls.Down:
                    {
                        if (disableSphere.transform.localScale.x > 0.1f)
                        {
                            disableSphere.transform.localScale -= Vector3.one * Time.deltaTime;
                            if (PlayMode == Mode.FPS)
                            {
                                disableSphere.transform.position -= disableSphere.transform.up * Time.deltaTime / 4;
                            }
                            else
                            {
                                disableSphere.transform.position -= disableSphere.transform.forward * Time.deltaTime / 2;
                            }
                        }
                            break;
                    }
            }
        }
    }
    #endregion


    //Placement methods
    #region
    private void SpawnObject(int id)
    {
        GameObject go = PartsHolder.SpawnPart(id);
        GlobalReferences.FreeParts.Remove(go);
        carryGo[id] = go;
        go.GetComponent<MeshRenderer>().material = frozenMat;
        go.GetComponent<Rigidbody>().isKinematic = true;
        go.transform.parent = gameObject.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }


    private void FreezeObject()
    {
        Vector3 pos = carryGo[tempID].transform.localPosition;
        carryGo[tempID].transform.parent = null;
        carryGo[tempID].GetComponent<Part>().FreezePart();
        carryGo[tempID].GetComponent<Rigidbody>().isKinematic = false;

        SpawnObject(tempID);
        carryGo[tempID].transform.localPosition = pos;
        carryGo[tempID].SetActive(true);
    }
    #endregion


    //PickNChose methods
    #region
    public void PickupPart()
    {
        pickupCarry = pickup;
        pickup = null;

        pickupCarry.transform.SetParent(gameObject.transform, true);
        pickupCarry.GetComponent<Rigidbody>().isKinematic = true;
        pickupCarry.GetComponent<ConnectionScanning>().enabled = true;

        handler = pickupCarry.AddComponent<ConnectionScanningHandler>();
        handler.ConnectionTerminated += new ConnectionScanningHandler.ConnectionHandler(AfterConnection);
        handler.ConnectionFailed += new ConnectionScanningHandler.ConnectionHandler(AfterFailedConnection);

        Aiming = false;
    }


    public static void ReleasePart()
    {
        if (pickupCarry != null)
        {
            pickupCarry.transform.parent = null;
            pickupCarry.GetComponent<ConnectionScanning>().enabled = false;
            Destroy(pickupCarry.GetComponent<ConnectionScanningHandler>());
            pickupCarry.GetComponent<Rigidbody>().isKinematic = false;
            pickupCarry.GetComponent<MeshRenderer>().material = unselectedMat;
            Aiming = true;
            pickupCarry = null;
        }
    }


    private void AfterConnection(GameObject go)
    {
        if (pickupCarry != null)
        {
            pickupCarry.transform.parent = null;
            Destroy(pickupCarry.GetComponent<ConnectionScanningHandler>());
            pickupCarry = null;
            pickup = null;
        }
        Aiming = true;
        handler = null;
    }

    private void AfterFailedConnection(GameObject go)
    {
        pickup = go;
        go.GetComponent<MeshRenderer>().material = selectedMat;
        PickupPart();
    }
    #endregion


    //Raycasting methods
    #region
    private void CastPickUpRay()
    {
        if (GlobalReferences.PlacementType == PlacementTypeTool.PlaceChoreo.PickNChose && pickupCarry == null)
        {
            if (playMode == Mode.VR)
            {
                lrPos[0] = transform.position - transform.parent.TransformDirection(ViewDirection);
                lrPos[1] = transform.parent.position + transform.parent.TransformDirection(ViewDirection) * 100;
                lr.SetPositions(lrPos);

                rayFrom = transform.parent.localPosition;
                rayTo = transform.parent.TransformDirection(ViewDirection);
            }
            else
            {
                rayFrom = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
                rayTo = cam.transform.forward;
            }

            if (Physics.Raycast(rayFrom, rayTo, out hit, Mathf.Infinity, layerMaskPickup))
            {
                if (pickup != null)
                {
                    pickup.GetComponent<MeshRenderer>().material = unselectedMat;
                }
                pickup = hit.collider.gameObject;
                pickup.GetComponent<MeshRenderer>().material = selectedMat;
            }
            else
            {
                if (pickup != null)
                {
                    pickup.GetComponent<MeshRenderer>().material = unselectedMat;
                    pickup = null;
                }
            }
        }
    }

    private void CastDeleteRay()
    {
        if (GlobalReferences.PlacementType == PlacementTypeTool.PlaceChoreo.Delete)
        {
            if (playMode == Mode.VR)
            {
                lrPos[0] = transform.position - transform.parent.TransformDirection(ViewDirection);
                lrPos[1] = transform.parent.position + transform.parent.TransformDirection(ViewDirection) * 100;
                lr.SetPositions(lrPos);

                rayFrom = transform.parent.localPosition;
                rayTo = transform.parent.TransformDirection(ViewDirection);
            }
            else
            {
                rayFrom = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
                rayTo = cam.transform.forward;
            }

            if (Physics.Raycast(rayFrom, rayTo, out hit, Mathf.Infinity, layerMaskDelete))
            {
                if (toDelete != null)
                {
                    toDelete.GetComponent<MeshRenderer>().material = frozenMat;
                }
                toDelete = hit.collider.gameObject;
                frozenMat = toDelete.GetComponent<MeshRenderer>().material;
                toDelete.GetComponent<MeshRenderer>().material = highlightFrozenMat;
            }
            else
            {
                if (toDelete != null)
                {
                    toDelete.GetComponent<MeshRenderer>().material = frozenMat;
                    toDelete = null;
                }
            }
            
        }
    }
    #endregion


    //Enumeration    
    #region
    public enum ToolsetControls
    { 
        Right = 0,
        Left = 1,
        Up = 2,
        Down = 3
    }

    public enum Mode
    {
        VR = 0,
        FPS = 1
    }
    #endregion
}
