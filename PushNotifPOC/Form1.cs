using PushSharp;
using PushSharp.Apple;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PushNotifPOC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (txtMessage.Text.Length > 0)
            {
                try
                {
                    string deviceId = "4ead049e00cc26141740ca902c0a0e7e6d11b9b52b6c215ce9654cf3a60c2225";
                    string appLocation = System.AppDomain.CurrentDomain.BaseDirectory;
                    var appleCert = File.ReadAllBytes(string.Concat(appLocation, @"..\..\certs\INT.p12"));
                    //IMPORTANT: If you are using a Development provisioning Profile, you must use
                    // the Sandbox push notification server 
                    //  (so you would leave the first arg in the ctor of ApplePushChannelSettings as
                    // 'false')
                    //  If you are using an AdHoc or AppStore provisioning profile, you must use the 
                    //Production push notification server
                    //  (so you would change the first arg in the ctor of ApplePushChannelSettings to 
                    //'true')
                    push.RegisterAppleService(new ApplePushChannelSettings(false, appleCert, ""));
                    //Extension method
                    //Fluent construction of an iOS notification
                    //IMPORTANT: For iOS you MUST MUST MUST use your own DeviceToken here that gets
                    // generated within your iOS app itself when the Application Delegate
                    //  for registered for remote notifications is called, 
                    // and the device token is passed back to you
                    push.QueueNotification(new AppleNotification()
                                                .ForDeviceToken(deviceId)//the recipient device id
                                                .WithAlert(txtMessage.Text)//the message
                                                .WithBadge(1)
                                                .WithSound("sound.caf")
                                                );

                    stopActions();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                MessageBox.Show("Enter a message to send notification");
            }
        }

        void push_OnChannelDestroyed(object sender)
        {
            //stopActions();
            MessageBox.Show("push_OnChannelDestroyed");
        }

        void push_OnChannelCreated(object sender, PushSharp.Core.IPushChannel pushChannel)
        {
            MessageBox.Show("push_OnChannelCreated");
        }

        void push_OnDeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, PushSharp.Core.INotification notification)
        {
            //stopActions();
            MessageBox.Show("push_OnDeviceSubscriptionChanged");
        }

        void push_OnDeviceSubscriptionExpired(object sender, string expiredSubscriptionId, DateTime expirationDateUtc, PushSharp.Core.INotification notification)
        {
            //stopActions();
            MessageBox.Show("OnDeviceSubscriptionExpired : " + expiredSubscriptionId);
        }

        void push_OnNotificationFailed(object sender, PushSharp.Core.INotification notification, Exception error)
        {
            //stopActions();
            MessageBox.Show("OnNotificationFailed : " + error);
        }

        void push_OnServiceException(object sender, Exception error)
        {
            //stopActions();
            MessageBox.Show("OnServiceException : " + error);
        }

        void push_OnChannelException(object sender, PushSharp.Core.IPushChannel pushChannel, Exception error)
        {
            //stopActions();
            MessageBox.Show("OnChannelException : " + error);
        }

        void push_OnNotificationSent(object sender, PushSharp.Core.INotification notification)
        {
            MessageBox.Show("push_OnNotificationSent");
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            push = new PushBroker();
            push.OnNotificationSent += push_OnNotificationSent;
            push.OnChannelException += push_OnChannelException;
            push.OnServiceException += push_OnServiceException;
            push.OnNotificationFailed += push_OnNotificationFailed;
            push.OnDeviceSubscriptionExpired += push_OnDeviceSubscriptionExpired;
            push.OnDeviceSubscriptionChanged += push_OnDeviceSubscriptionChanged;
            push.OnChannelCreated += push_OnChannelCreated;
            push.OnChannelDestroyed += push_OnChannelDestroyed;
            startActions();
        }

        private void stopActions()
        {
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            btnSend.Enabled = false;
            push.StopAllServices(waitForQueuesToFinish: true);
        }

        private void startActions()
        {
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            btnSend.Enabled = true;
        }

        PushBroker push;
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            btnSend.Enabled = false;
            push.StopAllServices(waitForQueuesToFinish: true);
        }
    }
}
