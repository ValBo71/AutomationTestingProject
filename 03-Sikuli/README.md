# 🖼️ GUI Automation with SikuliX

This directory contains GUI automation scripts built with **SikuliX**, which uses image recognition to automate interactions with desktop elements and web browsers.

## 📂 Project Structure

* **[01. ValentinBogdanov.sikuli](file:///e:/Programing/My_project/GitHub/MyProject/03.%20Sikuli/01.%20ValentinBogdanov.sikuli)**: The main SikuliX project folder.
  * **[Exam2.py](file:///e:/Programing/My_project/GitHub/MyProject/03.%20Sikuli/01.%20ValentinBogdanov.sikuli/Exam2.py)**: The Jython (Python for Java) script containing the automation logic.
  * **`*.png`**: Visual patterns (screenshots) representing the GUI targets (buttons, input fields, checkboxes) used by the script for image recognition.

## 💡 Automated Workflow (`Exam2.py`)

The script automates a complete end-to-end integration and bug reporting flow in **Jira Sandbox**:

1. **Launch Browser**: Presses the Windows key, types `firefox`, and launches Firefox.
2. **Retrieve Temporary Email**:
   * Navigates to [temp-mail.org](https://temp-mail.org/en/).
   * Deletes the default email to generate a fresh one and copies it to the clipboard.
3. **Register in Jira Sandbox**:
   * Opens a new browser tab (`Ctrl + T`) and navigates to the Jira Sandbox.
   * Fills out the registration form using the copied temp email and submits it.
4. **Email Verification**:
   * Returns to the temp-mail tab, refreshes, and waits for the confirmation email.
   * Clicks the email, double-clicks the username, and copies it to the clipboard.
5. **Set Password & Login**:
   * Clicks the "Set My Password" button, sets the password to `Penka`, and logs in.
6. **Project Creation**:
   * Creates a new Kanban project with a dynamically generated random ID and uses the username as the project key.
7. **Create Bug Report**:
   * Clicks the "Create" button to open the issue creation dialog.
   * Selects **Bug** as the issue type.
   * Fills in the summary and writes a detailed bug report in the Description field (using rich text formatting and keyboard navigation) regarding a currency conversion issue on `phptravels.net`.
   * Sets the priority to **High** and submits the bug report.
8. **Verify Creation**:
   * Searches for the created bug report using the search bar and verifies its existence via image recognition.
   * Displays a confirmation popup: *"The test performed with Sikuli was successful!"*.
9. **Teardown**: Logs out of Jira cleanly.

## 🛠️ Prerequisites & Setup

To run this project locally, you need the following:

1. **Java Development Kit (JDK)**: Ensure JDK (version 8 or later) is installed and available on your system path.
2. **SikuliX IDE**: Download the SikuliX IDE (e.g., `sikulixide-2.0.5.jar`) from the [SikuliX Official Website](http://sikulix.com/).
3. **Firefox Browser**: The script is configured to launch and automate the Firefox browser.
4. **Active Display / UI**: Since SikuliX relies on visual recognition, the machine running the script must have an active graphical session (cannot run in a headless environment).

## 🚀 How to Run

1. Open the **SikuliX IDE**.
2. Go to **File -> Open** and select the folder `01. ValentinBogdanov.sikuli`.
3. Press **Run** (or `Ctrl + R`) in the IDE to start execution.

> [!WARNING]
> Do not move your mouse or focus away from the active screen during execution, as Sikuli control scripts interact directly with your physical screen and input peripherals.
