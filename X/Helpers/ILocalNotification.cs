using System;
using System.Threading.Tasks;

namespace X.Helpers
{
    public interface ILocalNotification
    {
        void sendLocalNotif(string name);
        void recieveFromLocal(string response);
        void ShareNow();
        void Checker();
    }   
}
