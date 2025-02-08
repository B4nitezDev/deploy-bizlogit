using System.Diagnostics;

public static class FrontUtil
{
    public static void BuildFrontMobile(string projectPath)
    {
        if (!Directory.Exists(projectPath))
            throw new DirectoryNotFoundException($"Error: La ruta '{projectPath}' no existe.");
    
        Console.WriteLine("Modificando archivos antes del build...");
        ModifyIndexHtmlMobile(projectPath);

        RunCommand("npm", "run watch", projectPath);

        Console.WriteLine("✅ Build del frontend completado exitosamente.");
    }

    public static void BuildFromBizion(string projectPath)
    {
        if (!Directory.Exists(projectPath))
            throw new DirectoryNotFoundException($"Error: La ruta '{projectPath}' no existe.");

        Console.WriteLine("Modificando archivos antes del build...");
        ModifyIndexHtmlBizion(projectPath);
        ModifyEnviromentBizion(projectPath);

        // Ejecuta el build
        RunCommand("npm", "run build", projectPath);

        Console.WriteLine("✅ Build del frontend completado exitosamente.");
    }

    private static void ModifyIndexHtmlMobile(string projectPath)
    {
        string indexPath = Path.Combine($"{projectPath}/src/", "index.html");

        if (!File.Exists(indexPath))
        {
            Console.WriteLine($"⚠ No se encontró {indexPath}, omitiendo modificación.");
            return;
        }

        string content = File.ReadAllText(indexPath);

        content = content.Replace("<base href=\"/\" />", "<base href=\"/mobile\" />");
        content = content.Replace("    \"watch\": \"ng build --watch --configuration development\",\r\n", "    \"watch\": \"ng build --configuration production\",\r\n");

        File.WriteAllText(indexPath, content);
        Console.WriteLine($"✔ Archivo modificado: {indexPath}");
    }

    private static void ModifyIndexHtmlBizion(string projectPath)
    {
        string indexPath = Path.Combine($"{projectPath}/src/", "index.html");

        if (!File.Exists(indexPath))
        {
            Console.WriteLine($"⚠ No se encontró {indexPath}, omitiendo modificación.");
            return;
        }

        string content = File.ReadAllText(indexPath);

        content = content.Replace("<base href=\"/\">\r\n", "<base href=\"/bizion\" />");

        File.WriteAllText(indexPath, content);
        Console.WriteLine($"✔ Archivo modificado: {indexPath}");
    }

    private static void ModifyEnviromentBizion(string projectPath)
    {
        string enviromentPath = Path.Combine($"{projectPath}/src/environments/", "environment.prod.ts");

        if (!File.Exists(enviromentPath))
            throw new InvalidOperationException($"⚠ No se encontró {enviromentPath}, omitiendo modificación.");
        
        string content = File.ReadAllText(enviromentPath);

        content = content.Replace("    //baseUrl: '../apigateway/administration'\r\n", "    baseUrl: '../apigateway/administration'\r\n");
        content = content.Replace("    baseUrl: '../api'", "    //baseUrl: '../api'");

        File.WriteAllText(enviromentPath, content);
        Console.WriteLine($"✔ Archivo modificado: {enviromentPath}");
    }

    private static void RunCommand(string command, string arguments, string workingDirectory = "")
    {
        string npmPath = @"C:\Program Files\nodejs\npm.cmd";
        ProcessStartInfo psi = new()
        {
            FileName = npmPath,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using Process? process = Process.Start(psi) ?? 
            throw new InvalidDataException($"Error: No se pudo iniciar el proceso '{command}'.");
        
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (!string.IsNullOrEmpty(output)) 
            Console.WriteLine(output);

        if (!string.IsNullOrEmpty(error)) 
            Console.WriteLine($"Error:\n{error}");

        if (process.ExitCode != 0)
            throw new InvalidOperationException($"Error: El comando '{command} {arguments}' falló con el código de salida {process.ExitCode}.");
    }
}
