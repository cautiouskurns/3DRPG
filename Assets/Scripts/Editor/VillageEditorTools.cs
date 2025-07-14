using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(BasicVillageLayout))]
public class BasicVillageLayoutEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw default inspector
        DrawDefaultInspector();
        
        BasicVillageLayout villageLayout = (BasicVillageLayout)target;
        
        GUILayout.Space(10);
        GUILayout.Label("Village Generation Tools", EditorStyles.boldLabel);
        
        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Generate Village", GUILayout.Height(30)))
        {
            villageLayout.BuildVillage();
            EditorUtility.SetDirty(villageLayout);
        }
        
        if (GUILayout.Button("Clear Village", GUILayout.Height(30)))
        {
            villageLayout.RebuildVillage();
            EditorUtility.SetDirty(villageLayout);
        }
        
        GUILayout.EndHorizontal();
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("Rebuild All Village Systems", GUILayout.Height(25)))
        {
            // Find and trigger all village systems
            BasicGroundSetup groundSetup = villageLayout.GetComponent<BasicGroundSetup>();
            BasicCollisionSetup collisionSetup = villageLayout.GetComponent<BasicCollisionSetup>();
            
            if (groundSetup != null) groundSetup.SetupVillageGround();
            villageLayout.RebuildVillage();
            if (collisionSetup != null) collisionSetup.SetupVillageCollision();
            
            EditorUtility.SetDirty(villageLayout);
            Debug.Log("Rebuilt all village systems in edit mode");
        }
    }
}

[CustomEditor(typeof(BasicGroundSetup))]
public class BasicGroundSetupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw default inspector
        DrawDefaultInspector();
        
        BasicGroundSetup groundSetup = (BasicGroundSetup)target;
        
        GUILayout.Space(10);
        GUILayout.Label("Ground Generation Tools", EditorStyles.boldLabel);
        
        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Generate Ground", GUILayout.Height(30)))
        {
            groundSetup.SetupVillageGround();
            EditorUtility.SetDirty(groundSetup);
        }
        
        if (GUILayout.Button("Clear Ground", GUILayout.Height(30)))
        {
            groundSetup.RecreateGround();
            EditorUtility.SetDirty(groundSetup);
        }
        
        GUILayout.EndHorizontal();
    }
}

[CustomEditor(typeof(BasicCollisionSetup))]
public class BasicCollisionSetupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw default inspector
        DrawDefaultInspector();
        
        BasicCollisionSetup collisionSetup = (BasicCollisionSetup)target;
        
        GUILayout.Space(10);
        GUILayout.Label("Collision Generation Tools", EditorStyles.boldLabel);
        
        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Generate Collision", GUILayout.Height(30)))
        {
            collisionSetup.SetupVillageCollision();
            EditorUtility.SetDirty(collisionSetup);
        }
        
        if (GUILayout.Button("Clear Collision", GUILayout.Height(30)))
        {
            collisionSetup.RecreateCollision();
            EditorUtility.SetDirty(collisionSetup);
        }
        
        GUILayout.EndHorizontal();
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("Validate Collision Setup", GUILayout.Height(25)))
        {
            bool isValid = collisionSetup.ValidateCollisionSetup();
            string message = isValid ? "Collision validation PASSED" : "Collision validation FAILED";
            EditorUtility.DisplayDialog("Collision Validation", message, "OK");
        }
    }
}
#endif