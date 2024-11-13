# Sample Unity Character Controller
_3D Unity character controller using new input system_

This repository includes a Unity package that can be imported into your Unity URP project as well as the individual files included in that package. The package allows you to attach the provided content to a pre-existing 3D character in order to control it and the main camera in a 3rd person point of view using a game controller.

## Instructions for Use

### 1. Prepare the Project
You should be working in a Unity Universal Render Pipeline (URP) project. First, you must install the `Cinemachine` package and the `Input System` package from the Unity Registry. The `Input System` package will prompt you to restart your project after installation; you should accept.

### 2. Import the Package
Import the Unity package included in this repository by dragging and dropping it into the Assets folder of your project within the Unity editor. When the popup appears prompting you to select which items to import, import all items listed.

### 3. Set Up the Camera
1. While in the scene you are working in, select the 3 items in the Camera folder in the Project panel and drag them onto the character you're using in the Hierarchy. If done correctly, the 3 camera objects should be direct children of the character object in the hierarchy.
2. If the Main Camera you just imported is missing a script in the Inspector panel, ensure that the `Universal RP` package is installed in your project.
3. Select the Main Camera that was originally in your scene (**not** the one you just added) and either disable it by unchecking the checkbox next to it in the Inspector panel, or delete it using the `delete` key.
4. It is likely that your character is a different size than the one in the Example Scene included in this package. Because of this, you will need to adjust the TopRig, MiddleRig, and BottomRig attributes of the CamController object in the Inspector panel (located towards the bottom, under the Orbit section). There is no exact science to this, so you will need to experiment with the values for height and radius for each until you are happy with the results. The best way to test all values is to edit them while in play mode so you can move the camera around while changing the values. Once you are done adjusting the values, mark the numbers down somewhere and stop the preview; the values will reset to the way they were before hitting play, so you will need to re-enter the numbers you wrote down.
5. If you wish to invert the camera controls, you can check off the Invert checkbox in CamController next to Input Axis Value in the Y Axis and/or X Axis subsections of the Axis Control Section.

### 4. Set Up the Animator Controller
In order for the character controller to work, you need to have an animator controller attached to your character that is set up in the exact manner described here.

![image](https://github.com/user-attachments/assets/af268bc1-691a-457d-b3f1-246c40e90b5c)
1. As in the image above, you will need to create 5 states: Idle, Walk, Run, Jump, and Special Movement. The Idle state needs to be the default state, but none of the other states should be connected to anything.
    - The names for these states needs to be exactly as written here, with the same capitalization. If you wish to use different names, you will need to edit the Player Controller Script, which will be covered in the next section.
4. Each state should have the appropriate character animation assigned to its motion.
5. Create two parameters in your animator controller: `currentState` (int) and `walkSpeed` (float). These need to be named exactly as they are named here, with the same capitalization.
6. In the Walk state, check the Parameter checkbox underneath the speed field. Then, in the Multiplier drop down, select walkSpeed. It should look like the image below.
7. Attach the animator controller to your character by selecting your character in the scene and dragging the controller into the Inspector panel.

![image](https://github.com/user-attachments/assets/54f08425-dc57-4251-bac9-82e92c8af0d9)

### 5. Attach the Player Controller Script
1. Attach the `Player Controller` script to your character by selecting your character in the scene and dragging the script into the Inspector panel.
2. The script has 3 fields that can be edited from the Inspector panel: `Rotation Speed`, `Has Jump`, and `Has Special Move`
    - `Rotation Speed` controls how quickly the character can turn. The default is 10, but if your character is much larger or much smaller than the sample character (Remy), you will likely need to adjust this value. The larger the number, the less time it will take for the character to turn to face the inputted direction.
    - `Has Jump` is used to control whether or not you have the Jump state implemented in your controller. If you do not have this state, or the animation associated with it, uncheck this field.
    - `Has Special Move` is used to control whether or not you have the Special Move state implemented in your controller. If you do not have this state, or the animation associated with it, uncheck this field.
3. If you wish to use different names for your states in the animator controller, you will need to adjust them in the script. To do this, open the script in your preferred code editor and find the following code at the top of the file:

    ```
   public static class PlayerStates {
        public static string IDLE = "Idle";
        public static string WALK = "Walk";
        public static string RUN = "Run";
        public static string JUMP = "Jump";
        public static string SPECIAL = "Special Move";
    }
  The text in the quotation marks are the names of the states in the animator controller. If you used different names, find the variable that represents the state you want to change, then update the text in the quotation marks. **Do not** change the variable names, which are the words in all caps.

## Using the Controller

The controls set up in this package are as follows:
- Left Joystick: Control character movement. The less force used to move it, the slower the character will move.
- Right Joystick: Control camera movement.
- Left Shoulder (L1): Hold while moving to run.
- X (Bottom Button/Button South): Jump. Cannot cancel jump or move until after the jump animation completes.
- Square (Left Button/Button West): Special Move. Cannot cancel special move or move until after the special move animation completes.

### Changing the Controls
If you wish to change the bindings of the controller, you can do so by double clicking on the PlayerInput object in the Input folder in the Project Window. You do not want the script, you want the object with the dropdown arrow next to it. In the window that opens, you can change the key bindings by clicking on the dropdown next to the move you wish to change, clicking on the name of the button in the dropdown, and selecting a new button from the editor in the right-hand window. Do not switch the type of input; if the original binding was a button, do not change it to a joystick, or vice versa.

### Troubleshooting the Controller
If the controller does not seem to be doing anything, check to see if it is a Playstation controller. There is a known issue with non-Playstation controllers.
