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
        print(f"Logging in as: '{masked_user}' (length: {len(username)}, contains @abv.bg: {'@abv.bg' in username})")
        smtp.login(username, password)
        print("Sending email...")
        smtp.sendmail(username, [to_email], msg.as_string())
        smtp.quit()
        print("Email sent successfully!")
        
    except Exception as e:
        err_msg = f"Error sending email:\n{traceback.format_exc()}"
        print(err_msg)
        with open('smtp_error.txt', 'w', encoding='utf-8') as f:
            f.write(err_msg)
        sys.exit(1)

if __name__ == '__main__':
    main()
