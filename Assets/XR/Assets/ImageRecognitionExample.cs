using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

public class ImageRecognitionExample : MonoBehaviour
{
    public float heightToAdd;
    public GameObject[] trackedObjects;

    private ARTrackedImageManager ARImageManager;
    private Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();

    void Awake()
    {
        ARImageManager = FindObjectOfType<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        ARImageManager.trackedImagesChanged += OnImageChanged;

        foreach (var trackedObject in trackedObjects)
        {
            GameObject spawnedObject = Instantiate(trackedObject, Vector3.zero, Quaternion.identity);
            spawnedObject.name = trackedObject.name;
            spawnedObjects.Add(spawnedObject.name, spawnedObject);

        }
    }

    void OnDisable()
    {
        ARImageManager.trackedImagesChanged -= OnImageChanged;
    }

    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var trackedImage in args.added)
        {
           updateImage(trackedImage);
        }
        foreach (var trackedImage in args.updated)
        {
           updateImage(trackedImage);
        }
        foreach (var trackedImage in args.removed)
        {
           spawnedObjects[trackedImage.name].SetActive(false);
        }
    }

    void updateImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;
        Vector3 position = trackedImage.transform.position;
        GameObject prefab = spawnedObjects[name];

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = touch.position;

            heightToAdd = (touchPos.y / Screen.height) / 2;
            //Debug.Log(heightToAdd);
            float rotationToAdd = (touchPos.x / Screen.width) * 360;

            prefab.transform.rotation = Quaternion.Euler( 0f, -rotationToAdd, 0f);
        }

        Touch[] touches = Input.touches;

        if (Input.touchCount == 2)
        {

            Vector2 touchDistance = touches[Input.touchCount - 2].position - touches[Input.touchCount - 1].position;
            float totalDistance = touchDistance.x + touchDistance.y;
            float scaleToAdd = (Mathf.Abs(totalDistance) / (Screen.width + Screen.height)) / 2;

            prefab.transform.localScale = new Vector3(scaleToAdd + 0.1f, scaleToAdd + 0.1f, scaleToAdd + 0.1f);

            Debug.Log(scaleToAdd);

        }
        prefab.transform.position = position + new Vector3(0f, heightToAdd, 0f);
        prefab.SetActive(true);

        foreach ( GameObject go in spawnedObjects.Values)
        {
            if(go.name != name)
            {
                go.SetActive(false);
            }
        }
    }
}
