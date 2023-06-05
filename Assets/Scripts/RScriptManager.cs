using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Diagnostics;


public class RScriptManager
{
 string argument = "rScript_velocity.R";
    public IEnumerator runR(List<string> path)
    {
        
        //got from https://stackoverflow.com/questions/40292945/is-it-possible-to-run-r-code-from-unity-c-sharp-in-mono-or-net-on-osx
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        process.StartInfo.FileName = @"C:\Program Files\R\R-4.0.3\bin\Rscript.exe";
        for (int i = 0; i < path.Count; i++)
        {
            argument +=  " " + path[i];
        }
        process.StartInfo.Arguments = argument;
            process.StartInfo.WorkingDirectory = Path.Combine(Application.streamingAssetsPath, "RScript");
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.Start();
            //read the output
            string output = process.StandardOutput.ReadToEnd();
            string err = process.StandardError.ReadToEnd();
            process.WaitForExit();
            UnityEngine.Debug.Log("script end");
            UnityEngine.Debug.Log(output);
            UnityEngine.Debug.LogError(err);
        yield return null;
    }
}
