// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.Windows.Threading;
using System.Windows;

namespace NoteboardWPFUI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) { /*...*/ }
    private void OnExit(object sender, ExitEventArgs e) { /*...*/ }
    private void OnStartup(object sender, StartupEventArgs e)
    {
        var mainWindow = new MainWindow();
    }
    private void NotifyIcon_FocusOnLeftClick(object sender, RoutedEventArgs e)
    {
        if (Current.MainWindow is MainWindow mainWindow)
        {
            mainWindow.Show();
            mainWindow.WindowState = WindowState.Normal;
            mainWindow.Activate();
        }
    }
}
