
# Forensic XR

This is a Unity XR application for Meta Quest headsets (Quest 3).  
It allows users to interact with forensic scene evidence in a mixed reality environment.  
Users can load 3D models, scale them using UI buttons, and interact with them using XR controllers.

---

## Features

- Load/unload forensic evidence models at runtime
- Snap-to-marker system for object placement
- Object manipulation and scaling using UI
- Supports `.obj` and `.fbx` 3D model formats
- Built for Meta Quest 3 using the Meta XR All-in-One SDK

---

## Project Structure

```
Assets/
├── Models/              # 3D evidence models
├── Scripts/             # C# scripts for the project
├── UI/                  # World-space UI elements
└── Scenes/MainScene/    # Main scene setup
```

---

## Getting Started

### 1. Cloning the Project (if using Git)

If you're receiving a GitHub repository:

1. Open **Command Prompt** (on Windows):
   - Press `Windows Key + R`, type `cmd`, and press Enter.
   - Or use terminal: right-click within the desired folder and click "Open in Terminal".

2. Navigate to the folder where you want the project:
   ```
   cd path\to\your\desired\folder
   ```

3. Clone the repository:
   ```
   git clone "Link to the repository"
   ```

Ensure you're in the correct folder before cloning to avoid misplaced files.

---

### 2. If You Receive the Project Folder Directly

If the project folder is provided via file sharing:
- Simply copy it to your working directory.
- No Git commands are required.

---

### 3. Opening the Project in Unity

1. Open **Unity Hub**
2. Click **Add Project**
3. Select the folder where the project is located
4. Click **Open**

**Important:** This project uses **Unity 6 (6000.0.42f1)**.  
Make sure you have this version installed to avoid compatibility issues.

---

## Unity Setup Checklist

Unity will usually configure most dependencies automatically.  
However, **check the following settings if the project does not run correctly**:

### Build Platform

- Navigate to `File > Build Profiles`
- Set the platform to **Android**
- Click **Switch Platform** if it's not already selected

### XR Plugin Setup

- Go to `Edit > Project Settings > XR Plug-in Management`
- Ensure **OpenXR** is enabled for Android
- Under OpenXR, confirm both **Meta Quest Feature Group** and **Meta XR Feature Group** are enabled

### Plugin Installation

If Unity did not install the required packages automatically:

1. Go to `Window > Package Manager`
2. Install the following packages:
   - **Meta XR All-in-One SDK**
   - **OpenXR Plugin**

#### Photon Fusion Plugin (if needed)

- First Method: Download from Unity Asset Store  
	- **This Method may not work**
- Alternate Method:
  1. Go to `Edit > Project Settings > Package Manager`
  2. Click the **+** button in the left section and add:

     ```
     Name: Photon
     URL: https://package.photonengine.com/fusion/v2
     ```

  3. Then click the **+** button in the right section and add:

     ```
     Scopes: com.photonengine.fusion.transport.udp
     ```

  4. Navigate to your project folder:
     ```
     /Packages/manifest.json
     ```

  5. Open `manifest.json` in a text editor and add:

     ```json
     "scopedRegistries": [
       {
         "name": "Photon",
         "url": "https://package.photonengine.com/fusion/v2",
         "scopes": [
           "com.photonengine.fusion",
           "com.photonengine.fusion.transport.udp"
         ]
       }
     ],
     ```

     Then add these under `"dependencies": {`:

     ```json
     "com.photonengine.fusion": "2.0.6",
     "com.photonengine.fusion.transport.udp": "2.0.6"
     ```

  6. Save the file

---

## Running the Application in Unity Editor

### If Using Simulator

- Activate the Simulator by clicking the simulator icon (top-middle of the Unity window)
- Icon should turn **blue** when active
- Click **Play** to run the program
- To start with the welcome screen:
  - Go to `Assets/Scenes/Welcome.unity` and double-click

---

## Controls and Interaction

- Use controller ray pointers for UI interaction
- Load/unload objects using the menu
- Grab objects (hold **U** on keyboard in Simulator)
- Scale objects using grab or the UI scale buttons

---

## Running on Meta Quest 3 Headset

- Connect your headset via USB
- Press **Play** to deploy and run the app on the headset
- No need to activate the Simulator

### Optional: Load Welcome Screen First

1. Go to `File > Build Profiles > Android`
2. Click **Open Scene List**
3. Ensure both:
   - `Scenes/WelcomeScene`
   - `Scenes/MainScene`
   are checked

This ensures the welcome screen plays before launching the main scene.
