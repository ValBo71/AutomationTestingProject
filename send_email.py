# Verification run trigger for email verification v2
import smtplib


import os
import sys
import traceback
from email.mime.text import MIMEText
from email.mime.multipart import MIMEMultipart

def main():
    try:
        server = os.environ.get('MAIL_SERVER')
        port_str = os.environ.get('MAIL_PORT', '465')
        username = os.environ.get('MAIL_USERNAME')
        password = os.environ.get('MAIL_PASSWORD')
        to_email = 'vbogdanov@abv.bg'
        
        if not server or not username or not password:
            raise ValueError(f"Missing required SMTP configuration secrets. Server: {server}, Username: {username}")
            
        try:
            port = int(port_str)
        except ValueError:
            port = 465

        # Read email body from specified path or fallback paths
        body_path = os.environ.get('EMAIL_BODY_PATH')
        body_content = None
        
        paths_to_try = []
        if body_path:
            paths_to_try.append(body_path)
        # Add fallback locations
        paths_to_try.extend([
            'email_body.txt',
            '../../email_body.txt',
            '../email_body.txt',
            '01.Playwright/email_body.txt'
        ])
        
        for p in paths_to_try:
            if os.path.exists(p):
                print(f"Reading email body from {p}...")
                with open(p, 'r', encoding='utf-8') as f:
                    body_content = f.read()
                break
                
        if body_content is None:
            body_content = "Test results file not found (searched paths: " + ", ".join(paths_to_try) + ")."

        msg = MIMEMultipart()
        msg['From'] = f"GitHub Actions <{username}>"
        msg['To'] = to_email
        msg['Subject'] = f"Test Report: {os.environ.get('WORKFLOW_NAME', 'Playwright Tests')} - {os.environ.get('JOB_STATUS', 'Done')}"
        msg.attach(MIMEText(body_content, 'plain', 'utf-8'))

        print(f"Connecting to {server}:{port}...")
        if port == 465:
            smtp = smtplib.SMTP_SSL(server, port, timeout=30)
        else:
            smtp = smtplib.SMTP(server, port, timeout=30)
            smtp.ehlo()
            smtp.starttls()
            smtp.ehlo()
        
        masked_user = username if len(username) <= 4 else f"{username[:2]}***{username[-2:]}"
        pw_len = len(password) if password else 0
        pw_starts_space = password.startswith(' ') if password else False
        pw_ends_space = password.endswith(' ') if password else False
        pw_has_quotes = ((password.startswith('"') and password.endswith('"')) or 
                         (password.startswith("'") and password.endswith("'"))) if password else False
        diag_info = (f"Diag Info: User='{masked_user}', Length={len(username)}, Has_Domain={'@abv.bg' in username}, "
                     f"PW_Len={pw_len}, PW_StartsSpace={pw_starts_space}, PW_EndsSpace={pw_ends_space}, PW_HasQuotes={pw_has_quotes}")
        print(diag_info)
        smtp.login(username, password)
        print("Sending email...")
        smtp.sendmail(username, [to_email], msg.as_string())
        smtp.quit()
        print("Email sent successfully!")
        
    except Exception as e:
        # Include diag_info in error log if username is defined
        diag_log = ""
        try:
            diag_log = f"Diagnostics: {diag_info}\n"
        except NameError:
            try:
                if 'username' in locals():
                    masked = username if len(username) <= 4 else f"{username[:2]}***{username[-2:]}"
                    diag_log = f"Diagnostics: User='{masked}', Length={len(username)}, Has_Domain={'@abv.bg' in username}\n"
            except:
                pass
        err_msg = f"{diag_log}Error sending email:\n{traceback.format_exc()}"
        print(err_msg)
        with open('smtp_error.txt', 'w', encoding='utf-8') as f:
            f.write(err_msg)
        sys.exit(1)

if __name__ == '__main__':
    main()
