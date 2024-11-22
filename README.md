# Introduction
This is a sample app that demonstrates a custom WPF control. The control allows a user to zoom in and out of a raster image, in this case a fractal, while maintaining accuracy within each pixel of the image. See 
[PanAndZoomViewer.cs](https://github.com/rsmart8452/fractals/blob/main/FractalFlair/Views/Controls/PanAndZoomViewer.cs)

To view the control in action:
- Install and run the app
- Hit the Start button to generate an image
- Zoom in on a single pixel within the image and pan to a point within the pixel you wish to zoom further in on.
- Hit Start to generate a new image centered on the point you selected

The project can also be used as a practical example of a custom WiX installer using a WPF UI. Refer to https://github.com/rsmart8452/Wix4BurnTutorial for a tutorial.

# Getting Started
1. Software prerequisites:
   - **Heatwave** (Wix v4) - found on the Visual Studio Marketplace
   - **IsWix** (External app) - download and install from GitHub https://github.com/iswix-llc/iswix/releases
1. Software dependencies:
   - **.NET 8**, specifically `TargetFramework=net8.0-windows`
   - **WPF** (which requires targetting Windows)

# Build
1. Open SLN file in Visual Studio
1. Build the solution

# Integration Test
1. Open a command terminal of your choice.
   - In the Visual Studio menus, _View\Terminal_
   - Either Powershell or Command Prompt will work.
1. Using the Terminal, run `.\sandbox.wsb`
   - This starts a Windows sandbox.
1. Wait for it to open a couple of File Explorer windows.
   - One of these will show `C:\Fractal_Source`
     - You can test the installer from `C:\Fractal_Source\FractalFlair_Setup\bin\x64\Release`.
   - The other will show where FractalFlair will be installed.
     - Use this to verify the installer runs correctly.
1. Verify that the installer created a Start Menu item for FractalFlair.
1. Test the app by using this shortcut to run it.