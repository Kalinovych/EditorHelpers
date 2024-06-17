#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using Application = UnityEngine.Application;
#endif


public class CloudBuildSettingsHelper
{
#if UNITY_EDITOR
	[MenuItem( "Tools/Unity Cloud Diagnostic/Disable", false, 300 )]
    public static void DisableCloudDiagnostic()
    {
        _UpdateCloudDiagnosticEnable( false );
    }
	
    [MenuItem( "Tools/Unity Cloud Diagnostic/Enable", false, 301 )]
    public static void EnableCloudDiagnostic()
    {
        _UpdateCloudDiagnosticEnable( true );
    }
    
    private static void _UpdateCloudDiagnosticEnable( bool enabled )
    {
        var projectDir = Path.GetDirectoryName( Application.dataPath) ?? "";
        var assetFile  = Path.Combine( projectDir, "ProjectSettings", "UnityConnectSettings.asset" );
        if( !File.Exists( assetFile ) )
        {
            Debug.LogError( $"CloudBuildSettingsHelper._UpdateCloudDiagnosticEnable({enabled}) -> file {assetFile} does not exists" );
            return;
        }
     
        var lines            = File.ReadAllLines( assetFile );
        var isCrashReporting = false;
        for( var i=0; i<lines.Length; i++ )
        {
            var line = lines[i];
            if( isCrashReporting && line is "    m_Enabled: 1" or "    m_Enabled: 0" )
            {
                var isEnabled = line == "    m_Enabled: 1";
                if( isEnabled != enabled )
                {
                    lines[i] = enabled ? "    m_Enabled: 1" : "    m_Enabled: 0";
                    File.WriteAllLines( assetFile, lines );
                }
                break;
            }
         
            if( line == "  CrashReportingSettings:" )
            {
                isCrashReporting = true;
            }
        }
    }
#endif
}