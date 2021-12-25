using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace DidacticalEnigma.Core.Models.LanguageService;

public class DisclaimersGetter
{
    private readonly string disclaimersPath;

    public DisclaimersGetter(string disclaimersPath)
    {
        this.disclaimersPath = disclaimersPath;
    }
    
    public string GetDisclaimers()
    {
        return File.ReadAllText(disclaimersPath, Encoding.UTF8);
    }

    public string GetVersion()
    {
        return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location)
            .ProductVersion;
    }
}