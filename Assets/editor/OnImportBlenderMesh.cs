using UnityEngine;
using UnityEditor;
using System.Collections;

public class OnImportBlenderMesh : AssetPostprocessor {

    void OnPreprocessModel()
    {
        ModelImporter importer = (ModelImporter) assetImporter;
        importer.globalScale = 1f;
    }

    void OnPostprocessModel (GameObject g) 
    {
        ZeroTransform(g.transform);
    }

    void ZeroTransform(Transform t)
    {
        t.position = Vector3.zero;
        t.rotation = Quaternion.identity;
        t.localScale = Vector3.one;
    }
}