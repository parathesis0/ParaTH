using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml;

namespace ParaTH;

public static class DllMap
{
    private static readonly Dictionary<string, string> mapDictionary = [];

    [ModuleInitializer]
    public static void Init()
    {
        // iOS/tvOS statically link all native code into the main binary
        if (!RuntimeFeature.IsDynamicCodeCompiled
            && (OperatingSystem.IsIOS() || OperatingSystem.IsTvOS()))
        {
            NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), LoadStaticLibrary);
            return;
        }

        var os = GetPlatformName();
        var cpu = RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant();
        var wordSize = (IntPtr.Size * 8).ToString();

        var assembly = Assembly.GetExecutingAssembly();

        var xmlPath = Path.Combine(
            AppContext.BaseDirectory,
            assembly.GetName().Name + ".dll.config");
        if (!File.Exists(xmlPath))
            throw new FileNotFoundException(xmlPath);

        var xml = new XmlDocument();
        xml.Load(xmlPath);

        foreach (XmlNode node in xml.GetElementsByTagName("dllmap"))
        {
            var osAttr = node.Attributes?["os"];
            if (osAttr is not null)
            {
                bool containsOs = osAttr.Value.Contains(os);
                bool invert = osAttr.Value.StartsWith('!');
                if ((!containsOs && !invert) || (containsOs && invert))
                    continue;
            }

            var cpuAttr = node.Attributes?["cpu"];
            if (cpuAttr is not null)
            {
                bool containsCpu = cpuAttr.Value.Contains(cpu);
                bool invert = cpuAttr.Value.StartsWith('!');
                if ((!containsCpu && !invert) || (containsCpu && invert))
                    continue;
            }

            var wordsizeAttr = node.Attributes?["wordsize"];
            if (wordsizeAttr is not null)
            {
                bool containsWordSize = wordsizeAttr.Value.Contains(wordSize);
                bool invert = wordsizeAttr.Value.StartsWith('!');
                if ((!containsWordSize && !invert) || (containsWordSize && invert))
                    continue;
            }

            var dllAttr = node.Attributes?["dll"];
            var targetAttr = node.Attributes?["target"];
            if (dllAttr is null || targetAttr is null)
                continue;

            var oldLib = dllAttr.Value;
            var newLib = targetAttr.Value;

            if (string.IsNullOrWhiteSpace(oldLib) || string.IsNullOrWhiteSpace(newLib))
                continue;

            if (mapDictionary.ContainsKey(oldLib))
                continue;

            mapDictionary.Add(oldLib, newLib);
        }

        NativeLibrary.SetDllImportResolver(assembly, MapAndLoad);
    }

    private static string GetPlatformName()
    {
        return true switch
        {
            _ when OperatingSystem.IsWindows() => "windows",
            _ when OperatingSystem.IsMacOS()   => "osx",
            _ when OperatingSystem.IsLinux()   => "linux",
            _ when OperatingSystem.IsFreeBSD() => "freebsd",
            _                                  => "unknown"
        };
    }

    private static IntPtr MapAndLoad(
        string libraryName,
        Assembly assembly,
        DllImportSearchPath? dllImportSearchPath)
    {
        if (!mapDictionary.TryGetValue(libraryName, out var mappedName))
            mappedName = libraryName;

        return NativeLibrary.Load(mappedName, assembly, dllImportSearchPath);
    }

    private static IntPtr LoadStaticLibrary(
        string libraryName,
        Assembly assembly,
        DllImportSearchPath? dllImportSearchPath)
    {
        return NativeLibrary.GetMainProgramHandle();
    }
}
