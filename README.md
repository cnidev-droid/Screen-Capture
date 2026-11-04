Screen Capture 3
A lightweight screen capture utility written in C# using Windows Presentation Foundation (WPF).

Developed with Visual Studio 2019.

Screen Capture 3 allows you to capture selected areas of your screen, with or without the mouse cursor. Captures can be saved as JPEG or BMP images and are also copied to the Windows clipboard.

The application also includes automatic interval capture, mouse position tracking and RGB color inspection.
Features
- Capture any selected rectangular area of the screen.
- Save captures as JPEG or BMP files.
- Optionally include the mouse cursor in captured images.
- Copy captured images to the Windows clipboard.
- Display captured images in an image window.
- Automatic screen capture at one-second intervals.
- Display the current mouse coordinates.
- Read RGB color values at the mouse cursor position.
- Freeze and copy an RGB color value using a keyboard shortcut.
- Specify a fixed capture width and height.
- Automatically number sequential captures.
- Open the current save directory in Windows Explorer.
How to use
Taking a screen capture
1. Click Take Cap.
2. The main window is hidden and a semi-transparent blue selection window appears.
3. Drag the blue window to the desired screen position.
4. Resize the selection by dragging its lower-right corner.
5. Right-click inside the blue window to capture the selected screen area.
The capture is saved in the directory specified in the Save screen cap to dir text box.

The destination directory can be changed by either:
- Dragging a folder from Windows Explorer into the text box.
- Typing a directory path and pressing Enter.
Mouse cursor
Enable include mouse to include the mouse cursor in the captured image.
Fixed capture size
If you want to take several captures with exactly the same dimensions, enter the desired width and height in the Cap size text box.

Use the format:

width*height

For example:

100*50

If the field is empty or contains w*h, the capture dimensions are taken from the blue selection window.
Automatic capture
Screen Capture 3 can capture the same screen area repeatedly at one-second intervals.
1. Click Take Cap and position the selection window.
2. Enable Auto cap active.
3. The application captures the selected area once every second.
4. Disable Auto cap active to stop capturing.
RGB color inspection
The application displays the current mouse coordinates and the RGB color value at the mouse position.

To freeze an RGB value so it can be copied:
1. Move the mouse pointer over the desired screen pixel.
2. Press Alt+L (lowercase L).
3. The displayed RGB value is frozen and can be copied from the application.
Naming captured images
Enter a valid file name in the Give name text box to assign a common name to sequential captures.

Images are automatically numbered using a three-digit suffix.

For example:
img_000.jpg
img_001.jpg
img_002.jpg
Open save directory
Click Open dir to open the current save directory in Windows Explorer.
Requirements
- Windows
- Visual Studio 2019
- .NET Framework / .NET version specified in the project settings
Building the project
1. Clone or download this repository.
2. Open ScreenCapture3.sln in Visual Studio 2019.
3. Build the solution.
4. Run the application.
License
This project is licensed under the MIT License. See the LICENSE file for details.
