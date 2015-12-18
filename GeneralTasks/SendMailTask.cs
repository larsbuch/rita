using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using BasicTasks;
using RukisIntegrationTaskhandlerInterface;
using RukisIntegrationTaskhandlerInterface.Constants;
using RukisIntegrationTaskhandlerInterface.Enumerations;
using RukisIntegrationTaskhandlerInterface.Exceptions;

namespace GeneralTasks
{
    public class SendMailTask:AbstractTask
    {
        protected static readonly string Recipients = "Recipients";
        protected static readonly string Sender = "Sender";
        protected static readonly string Subject = "Subject";
        protected static readonly string Body = "Body";
        protected static readonly string SMTPServer = "SMTPServer";
        protected static readonly string SMTPPort = "SMTPPort";
        protected static readonly string SMTPUser = "SMTPUser";
        protected static readonly string SMTPPassword = "SMTPPassword";
        protected static readonly string Attachments = "Attachments";

        protected override void preExecuteTaskInternally(ITaskExecutionContext taskExecutionContext)
        {
            // Do nothing
        }

        protected override void executeTaskInternally(ITaskExecutionContext taskExecutionContext)
        {
            try
            {
                string recipients = getVariable(Recipients).getStringValue();
                string sender = getVariable(Sender).getStringValue();
                string subject = getVariable(Subject).getStringValue();
                string body = getVariable(Body).getStringValue();
                string smtpServer = getVariable(SMTPServer).getStringValue();
                int smtpPort = getVariable(SMTPPort).getIntegerValue();
                string smtpUser = getVariable(SMTPUser).getStringValue();
                string smtpPassword = getVariable(SMTPPassword).getStringValue();
                List<string> attachments = getVariable(Attachments).getStringListValue();

                // Create message
                MailMessage msg = new MailMessage();
                msg.To.Add(recipients);
                msg.Sender = new MailAddress(sender);
                msg.Subject = subject;
                msg.Body = body;

                // Attach attachments
                foreach (string filePlacement in attachments)
                {
                    Attachment attachment = new Attachment(filePlacement);
                    msg.Attachments.Add(attachment);
                }

                // Create smtp connection

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Host = smtpServer;
                smtpClient.Port = smtpPort;
                if (!smtpUser.Equals(string.Empty))
                {
                    smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPassword);
                }

                smtpClient.Send(msg);
            }
            catch(Exception e)
            {
                throw new TaskExecutionException(TaskName,"executeTaskInternally","Error occured while executing",e);
            }
        }

        protected override void postExecuteTaskInternally(ITaskExecutionContext taskExecutionContext)
        {
            // Do nothing
        }

        public override void resetTask()
        {
            // Do nothing
        }

        public override void initializeTask()
        {
            createNewEmptyVariable(Recipients,VariableType.String,false,"Recipients separated with ;");
            createNewEmptyVariable(Sender,VariableType.String,false,"The sender of the message");
            createNewEmptyVariable(Subject, VariableType.String, false, "Subject of the message");
            createNewEmptyVariable(Body, VariableType.String, false, "The message body.");
            createNewEmptyVariable(SMTPServer, VariableType.String, false, "Mail server with either URL or IP.");
            createNewEmptyVariable(SMTPPort, VariableType.Integer, false, "Port of the server (normally 25).");
            createNewEmptyVariable(SMTPUser, VariableType.String, false, "User account of the mail server.");
            createNewEmptyVariable(SMTPPassword, VariableType.String, false, "Password of the user account");
            createNewEmptyVariable(Attachments, VariableType.ListOfString, false, "List of attachments locations.");

            base.initializeTask();
        }
    }
}
