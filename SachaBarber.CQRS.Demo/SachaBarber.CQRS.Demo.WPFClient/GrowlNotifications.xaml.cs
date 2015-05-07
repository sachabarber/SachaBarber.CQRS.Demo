using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SachaBarber.CQRS.Demo.WPFClient.ViewModels;

namespace SachaBarber.CQRS.Demo.WPFClient
{
    public partial class GrowlNotifications
    {
        private const byte MAX_NOTIFICATIONS = 4;
        private int count;
        private readonly Notifications notifications = new Notifications();
        private readonly Notifications buffer = new Notifications();
        private readonly object syncLock = new object();

        public GrowlNotifications()
        {
            InitializeComponent();

            BindingOperations.EnableCollectionSynchronization(notifications, syncLock);
            BindingOperations.EnableCollectionSynchronization(buffer, syncLock);

            NotificationsControl.DataContext = notifications;
        }

        
        public void AddNotification(Notification notification)
        {
            notification.Id = count++;
            if (notifications.Count + 1 > MAX_NOTIFICATIONS)
                buffer.Add(notification);
            else
                notifications.Add(notification);

            //Show window if there're notifications
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (notifications.Count > 0 && !IsActive)
                    Show();
            }));

        }

        public void RemoveNotification(Notification notification)
        {
            if (notifications.Contains(notification))
                notifications.Remove(notification);

            if (buffer.Count > 0)
            {
                notifications.Add(buffer[0]);
                buffer.RemoveAt(0);
            }

            //Close window if there's nothing to show
            if (notifications.Count < 1)
                Hide();
        }

        private void NotificationWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Height != 0.0)
                return;
            var element = sender as Grid;
            RemoveNotification(notifications.First(n => n.Id == Int32.Parse(element.Tag.ToString())));
        }
    }

}
