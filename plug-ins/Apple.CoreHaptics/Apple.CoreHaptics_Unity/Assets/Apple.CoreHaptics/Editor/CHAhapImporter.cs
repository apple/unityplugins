/*----------------------------------------------------------------------------------------
 *   Copyright (c) Apple. All rights reserved.
 *----------------------------------------------------------------------------------------*/

using Apple.CoreHaptics;
using System;
using System.IO;

#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif

using UnityEngine;

/// <summary>
/// When an AHAP file (&lt;filename&gt;.ahap) is imported to Unity, it is parsed to check for errors
/// and treated as a TextAsset.
/// </summary>
[ScriptedImporter(1, "ahap")]
public class CHAhapImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        var ahapData = File.ReadAllText(ctx.assetPath);

        try
        {
            _ = ParseAhap(ahapData);

            // Use the AHAP as a text asset
            var ahapTextAsset = new TextAsset(ahapData);
            ctx.AddObjectToAsset("main", ahapTextAsset);
            ctx.SetMainObject(ahapTextAsset);
        }
        catch (Exception e)
        {
            Debug.LogError("An error occurred while trying to import the AHAP: " + e);
        }
    }

    /// <summary>
    /// Parses the serialized-string of an AHAP file
    /// </summary>
    /// <param name="jsonString">
    /// The contents of an AHAP file.
    /// </param>
    /// <returns>
    /// An instance of the AhapMaster class
    /// </returns>
    private static CHHapticPattern ParseAhap(string jsonString)
    {
        return CHSerializer.Deserialize(jsonString);
    }
}
