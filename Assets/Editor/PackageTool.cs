using UnityEngine;
using UnityEditor;

public class PackageTool
{
    [MenuItem("Package/Update Package")]
    static void UpdatePackage()
    {
        AssetDatabase.ExportPackage("Assets/Pcx", "Pcx.unitypackage", ExportPackageOptions.Recurse);
    }
}
