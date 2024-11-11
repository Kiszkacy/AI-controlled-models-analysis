
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class PythonManager : Singleton<PythonManager>
{
    private Process process = null;
    private static readonly string defaultPythonConfigurationPath = "./src/pythonConfiguration.yaml";

    public bool IsRunning => this.process != null;

    public void Start()
    {
        // TODO move this to a config or load via OS
        string pythonPath = @"C:\Users\Karol\AppData\Local\Programs\Python\Python311\python.exe";
        string scriptPath = @"./core/src/main.py --pipe_name godot-pipe-name";
        string workingDirectory = @"C:\Users\Karol\Documents\Projects-Github\inzynierka";
        string pythonPathEnv  = @"C:\Users\Karol\Documents\Projects-Github\inzynierka";
        
        ProcessStartInfo processStartInfo = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = scriptPath,
//            RedirectStandardOutput = true,
//            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = false,
            WorkingDirectory = workingDirectory,
        };
        
        processStartInfo.Environment["PYTHONPATH"] = pythonPathEnv;
        
        this.process = new();
        process.StartInfo = processStartInfo;
        process.Start();
        
//        string output = process.StandardOutput.ReadToEnd();
//        string error = process.StandardError.ReadToEnd();
////        process.WaitForExit();
//
//        if (!string.IsNullOrEmpty(output))
//        {
//            Console.WriteLine("Output: " + output);
//        }
//        else
//        {
//            Console.WriteLine("No output from Python script.");
//        }
//
//        if (!string.IsNullOrEmpty(error))
//        {
//            Console.WriteLine("Error: " + error);
//        }
//        else
//        {
//            Console.WriteLine("No errors from Python script.");
//        }
    }
    
    public void Stop()
    {
        if (this.process != null && !this.process.HasExited)
        {
            this.process.Kill();
            this.process.Dispose();
        }
        this.process = null;
    }

    public PythonConfigurationData GetDefaultConfigurationData()
    {
        return PythonConfigurationData.Load(defaultPythonConfigurationPath);
    }
}
