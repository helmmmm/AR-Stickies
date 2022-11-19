using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using System;

public class ARTapToPlace : MonoBehaviour
{

    private ARSessionOrigin _arOrigin;
    private ARRaycastManager _arRaycast;
    private Pose _placementPose;
    private bool _placementPoseIsValid = false;
    public GameObject _placementIndicator;
    public GameObject _placementObject;

    void Start()
    {
        _arOrigin = FindObjectOfType<ARSessionOrigin>();
        _arRaycast = FindObjectOfType<ARRaycastManager>();
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (_placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceObject();
        }
    }

    private void PlaceObject()
    {
        Instantiate(_placementObject, _placementPose.position, _placementPose.rotation);
    }

    private void UpdatePlacementIndicator()
    {
        if (_placementPoseIsValid)
        {
            _placementIndicator.SetActive(true);
            _placementIndicator.transform.SetPositionAndRotation(_placementPose.position, _placementPose.rotation);
        }
        else
        {
            _placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        _arRaycast.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

        _placementPoseIsValid = hits.Count > 0;
        if (_placementPoseIsValid)
        {
            _placementPose = hits[0].pose;


            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            _placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
}
