# -*- coding: utf-8 -*-
import random
import time

# ==============================================================================
# Centralized Configuration & Settings
# ==============================================================================
TEMP_MAIL_URL = "https://temp-mail.org/en/"
JIRA_DASHBOARD_URL = "https://sandbox.xpand-it.com/secure/Dashboard.jspa"
DEFAULT_PASSWORD = "Penka"

# Timeouts (in seconds)
DEFAULT_TIMEOUT = 30
EMAIL_CHECK_TIMEOUT = 120
LONG_TIMEOUT = 3600

# ==============================================================================
# Safe Automation Wrappers (Improves reliability and error reporting)
# ==============================================================================
def safe_wait(pattern, timeout=DEFAULT_TIMEOUT, msg=None):
    """Waits for a pattern to appear. Raises Exception with clear context if not found."""
    if not exists(pattern, timeout):
        error_msg = "Could not find element: %s" % pattern
        if msg:
            error_msg += " (%s)" % msg
        raise Exception(error_msg)
    return True

def safe_click(pattern, timeout=DEFAULT_TIMEOUT, msg=None):
    """Waits for a pattern to appear and clicks it."""
    safe_wait(pattern, timeout, msg)
    click(pattern)

def safe_double_click(pattern, timeout=DEFAULT_TIMEOUT, msg=None):
    """Waits for a pattern to appear and double-clicks it."""
    safe_wait(pattern, timeout, msg)
    doubleClick(pattern)

def safe_type(pattern, text, timeout=DEFAULT_TIMEOUT, msg=None):
    """Waits for a pattern to appear, clicks it, and types text."""
    safe_wait(pattern, timeout, msg)
    type(pattern, text)

# ==============================================================================
# Workflow Steps
# ==============================================================================
def launch_browser():
    print("Step 1: Launching Firefox browser...")
    type(Key.WIN)
    wait(2)
    type("firefox" + Key.ENTER)
    wait(5)

def get_temp_email():
    print("Step 2: Retrieving temporary email from temp-mail.org...")
    type(TEMP_MAIL_URL + Key.ENTER)
    
    # Wait for delete button to refresh and generate a new address
    safe_click("Delete_button.png", DEFAULT_TIMEOUT, "Delete button on temp-mail.org")
    
    # Wait for the email to generate and copy it
    safe_click("copy_emal_address.png", DEFAULT_TIMEOUT, "Copy email address button")
    time.sleep(2)
    
    email = Env.getClipboard().strip()
    print("Temporary email retrieved: %s" % email)
    return email

def register_jira(email):
    print("Step 3: Registering on Jira Sandbox...")
    type("t", KeyModifier.CTRL)  # Open new tab
    time.sleep(2)
    type(JIRA_DASHBOARD_URL + Key.ENTER)
    
    # Fill in registration form details
    safe_type("FN_regform_Jira.png", "Pencho", DEFAULT_TIMEOUT, "First Name field")
    safe_type("CN_regForm_Jira.png", "Penchev LTD", 10, "Company Name field")
    safe_type("LN_regForm_Jira.png", "Penchev", 10, "Last Name field")
    safe_type("EA_regForm_Jira.png", email, 10, "Email Address field")
    
    # Accept GDPR / Terms check boxes
    safe_click("check_box_regForm_Jira.png", 10, "GDPR checkbox 1")
    time.sleep(1)
    safe_click("check_box_regForm_Jira.png", 10, "GDPR checkbox 2")
    time.sleep(1)
    
    # Submit Registration
    safe_click("Submit_Botton_regForm_Jira.png", 10, "Submit Registration button")

def verify_email_and_extract_username():
    print("Step 4: Waiting for verification email and extracting username...")
    safe_click("TM_tab.png", DEFAULT_TIMEOUT, "Temp-mail tab")
    type(Key.UP, KeyModifier.WIN)  # Maximize browser
    time.sleep(2)
    
    safe_click("refresh_button_TM.png", DEFAULT_TIMEOUT, "Refresh temp-mail inbox")
    
    # Wait for email to arrive and open it
    print("Waiting for verification email...")
    safe_click("sender_click.png", EMAIL_CHECK_TIMEOUT, "Verification email sender entry")
    
    # Wait for email body details
    safe_wait("label_jira_email.png", LONG_TIMEOUT, "Jira email label in body")
    safe_click("email_jira.png", 10, "Jira confirmation email link")
    
    # Extract username from the email content
    safe_wait("label_username.png", 60, "Username label")
    safe_double_click("label_user.png", 10, "Username value area")
    
    type("c", KeyModifier.CTRL)
    time.sleep(1)
    username = Env.getClipboard().strip()
    print("Extracted Username: %s" % username)
    return username

def set_password_and_login(username, password):
    print("Step 5: Setting password and logging in...")
    safe_click("button_SetMyPass.png", DEFAULT_TIMEOUT, "Set My Password button")
    safe_type("New_Pass_Jira_restForm.png", password, DEFAULT_TIMEOUT, "New Password field")
    safe_type("Confirm_resetForm_Jira.png", password, 10, "Confirm Password field")
    time.sleep(1)
    
    safe_click("Reset_Button_resFormPass_Jira.png", 10, "Reset Password Submit button")
    safe_click("LogIn_link_Jira.png", DEFAULT_TIMEOUT, "Login page link")
    
    # Log in with extracted username and set password
    safe_type("UserName_LogIn_Form.png", username, DEFAULT_TIMEOUT, "Username input field")
    safe_click("WDE_text.png", 10, "WDE field/text to shift focus")
    safe_type("Pass_LogInForm_Jira.png", password, 10, "Password input field")
    safe_click("Button_logIn_Jira.png", 10, "Log In submit button")

def create_kanban_project(username):
    print("Step 6: Creating Kanban project...")
    safe_click("Project_button_jira.png", 60, "Projects dropdown/button")
    safe_click("CreateProject_button_Jira.png", DEFAULT_TIMEOUT, "Create Project button")
    safe_click("Kanban_project.png", DEFAULT_TIMEOUT, "Kanban Project template")
    safe_click("Button_Next.png", DEFAULT_TIMEOUT, "Next button")
    safe_click("Select_button_CreateProject.png", DEFAULT_TIMEOUT, "Select Template button")
    
    # Generate unique random number for project name
    num = random.randrange(10000000000, 10000000000000)
    project_name = str(num)
    print("Generating project with name: %s and key: %s" % (project_name, username))
    
    safe_type("Name_CreatePojectForm_Jira.png", project_name, DEFAULT_TIMEOUT, "Project Name field")
    safe_type("Key_CreateProjectForm_Jira.png", username, DEFAULT_TIMEOUT, "Project Key field")
    safe_click("Submit_project.png", 10, "Submit Project button")

def create_bug_report():
    print("Step 7: Creating bug report issue...")
    safe_click("Create_Issue.png", 60, "Create Issue button")
    
    # Handle bug vs story type selection
    if exists("bug-2.png", 10):
        # Fill issue details
        type("1622896080922.png", "When you change the language, the currencies used in the specific countries do not change automatically")
    
    if exists("story.png", 5):
        click("story.png")
        
    safe_click("bug.png", 10, "Bug issue type option")
    
    # Fill Bug details
    safe_type("Summary_Bug_Report.png", "When you change the language, the currencies used in the specific countries do not change automatically", 20, "Bug Summary field")
    time.sleep(1)
    
    # Description Text (Using paste to bypass keyboard layout issues and improve speed)
    description_text = (
        "*Prerequisites:*\n\n"
        "Go to url: https://phptravels.net/\n\n"
        "*Steps to reproduce:*\n\n"
        "1. Click on the dropdown menu *English* on the top right corner of the page;\n\n"
        "2. Select any of the language lists;\n\n"
        "*Expected result:*\n\n"
        "The currency should automatically change to the one used in the country\n\n"
        "*Actual result:*\n\n"
        "When choosing the Russian language, the currency remains usd\n\n"
        "*Priority:*\n\n"
        "High\n\n"
        "*Additional info:*\n\n"
        "Mozilla Firefox 88.0 (64-bit), Windows 10 version 20H2\n"
    )
    
    safe_click("Description.png", 20, "Bug Description field")
    paste(description_text)
    time.sleep(1)
    
    # Set Priority to High
    safe_click("Priority.png", 10, "Priority dropdown")
    safe_click("High_.png", 10, "High priority option")
    
    # Submit Bug Report
    safe_click("Create_BugReport.png", 10, "Create Bug Report submit button")

def verify_and_logout():
    print("Step 8: Verifying bug report creation and logging out...")
    # Click search or search results
    safe_click("1622910819622.png", DEFAULT_TIMEOUT, "Search/Navigation trigger")
    safe_click("SearchFieldButton.png", 10, "Search field button")
    safe_type("1622910865745.png", "When you change the language", 10, "Search input field")
    safe_click("SearchButton.png", 10, "Search execute button")
    
    if exists("test_messege-1.png", DEFAULT_TIMEOUT):
        popup("The test performed with Sikuli was successful!")
        
    time.sleep(5)
    if exists("pop_up-1.png", 10):
        type(Key.ENTER)
        
    time.sleep(5)
    # Logout
    safe_click("LogOut_Button_Jira.png", 20, "User profile/logout menu")
    safe_click("logOut_Jira.png", 15, "Logout confirmation option")
    print("Successfully logged out. Automation complete.")

def main():
    try:
        launch_browser()
        email = get_temp_email()
        register_jira(email)
        username = verify_email_and_extract_username()
        set_password_and_login(username, DEFAULT_PASSWORD)
        create_kanban_project(username)
        create_bug_report()
        verify_and_logout()
    except Exception as e:
        print("AUTOMATION FAILED: %s" % str(e))
        popup("Automation failed:\n%s" % str(e))

if __name__ == "__main__":
    main()
