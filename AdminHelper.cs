using System.Diagnostics;
using System.Security.Principal;
using System.Windows;

public static class AdminHelper
{

    // These admin privileges request was used for an attempt to override the default Win+V hotkey,
    // sadly that did not work out, replacing the windows clipboard entirely might
    // require a kernel level hack, that seems like a hassle that could break 
    // things and wouldn't be worth it, so at least for now, the main
    // focus is to just build a great app that will overshadow it using a different hotkey :)

    //public static void RelaunchAsAdmin()
    //{
    //    // Check if already running as admin
    //    var currentPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
    //    bool isAdmin = currentPrincipal.IsInRole(WindowsBuiltInRole.Administrator);

    //    if (isAdmin)
    //    {
    //        return; // Already running as admin
    //    }

    //    var currentProcess = Process.GetCurrentProcess();
    //    var mainModule = currentProcess.MainModule;

    //    if (mainModule?.FileName == null)
    //    {
    //        System.Windows.MessageBox.Show("Could not determine the application executable path.",
    //                        "Error",
    //                        MessageBoxButton.OK,
    //                        MessageBoxImage.Error);
    //        return;
    //    }

    //    // Relaunch as admin
    //    var processInfo = new ProcessStartInfo
    //    {
    //        FileName = mainModule.FileName,
    //        UseShellExecute = true,
    //        Verb = "runas" // Requests elevation
    //    };

    //    try
    //    {
    //        Process.Start(processInfo);
    //        Environment.Exit(0); // Close the current instance
    //    }
    //    catch
    //    {
    //        // User declined the elevation request
    //        System.Windows.MessageBox.Show("Administrator privileges are required to change this setting.",
    //                        "Access Denied",
    //                        MessageBoxButton.OK,
    //                        MessageBoxImage.Warning);
    //    }
    //}
}
