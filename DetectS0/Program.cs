using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DetectS0
{
    class Program
    {
        static void Main(string[] args)
        {
            IntPtr registrationHandle = new IntPtr();
            DEVICE_NOTIFY_SUBSCRIBE_PARAMETERS recipient = new DEVICE_NOTIFY_SUBSCRIBE_PARAMETERS();
            recipient.Callback = new DeviceNotifyCallbackRoutine(DeviceNotifyCallback);
            recipient.Context = IntPtr.Zero;

            IntPtr pRecipient = Marshal.AllocHGlobal(Marshal.SizeOf(recipient));
            Marshal.StructureToPtr(recipient, pRecipient, false);

            uint result = PowerRegisterSuspendResumeNotification(DEVICE_NOTIFY_CALLBACK, ref recipient, ref registrationHandle);

            if (result != 0)
                Console.WriteLine("Error registering for power notifications: " + Marshal.GetLastWin32Error());
            else
                Console.WriteLine("Successfully Registered for power notifications!");
            Console.ReadKey();
        }
        private static int DeviceNotifyCallback(IntPtr context, int type, IntPtr setting)
        {
            Console.WriteLine("Device notify callback called: ");

            switch (type)
            {
                case PBT_APMPOWERSTATUSCHANGE:
                    Console.WriteLine("\tPower status has changed.");
                    break;

                case PBT_APMRESUMEAUTOMATIC:
                    Console.WriteLine("\tOperation is resuming automatically from a low-power state.This message is sent every time the system resumes.");
                    break;

                case PBT_APMRESUMESUSPEND:
                    Console.WriteLine("\tOperation is resuming from a low-power state.This message is sent after PBT_APMRESUMEAUTOMATIC if the resume is triggered by user input, such as pressing a key.");
                    break;

                case PBT_APMSUSPEND:
                    Console.WriteLine("\tSystem is suspending operation.");
                    break;
                case PBT_POWERSETTINGCHANGE:
                    Console.WriteLine("\tA power setting change event has been received. ");
                    break;
                default:
                    Console.WriteLine("unknown");
                    break;
            }

            // do something here
            return 0;
        }
        private const int WM_POWERBROADCAST = 536; // (0x218)
        private const int PBT_APMPOWERSTATUSCHANGE = 10; // (0xA) - Power status has changed.
        private const int PBT_APMRESUMEAUTOMATIC = 18; // (0x12) - Operation is resuming automatically from a low-power state.This message is sent every time the system resumes.
        private const int PBT_APMRESUMESUSPEND = 7; // (0x7) - Operation is resuming from a low-power state.This message is sent after PBT_APMRESUMEAUTOMATIC if the resume is triggered by user input, such as pressing a key.
        private const int PBT_APMSUSPEND = 4; // (0x4) - System is suspending operation.
        private const int PBT_POWERSETTINGCHANGE = 32787; // (0x8013) - A power setting change event has been received.
        private const int DEVICE_NOTIFY_CALLBACK = 2;

        /// <summary>
        /// OS callback delegate definition
        /// </summary>
        /// <param name="context">The context for the callback</param>
        /// <param name="type">The type of the callback...for power notifcation it's a PBT_ message</param>
        /// <param name="setting">A structure related to the notification, depends on type parameter</param>
        /// <returns></returns>
        public delegate int DeviceNotifyCallbackRoutine(IntPtr context, int type, IntPtr setting);

        /// <summary>
        /// A callback definition
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct DEVICE_NOTIFY_SUBSCRIBE_PARAMETERS
        {
            public DeviceNotifyCallbackRoutine Callback;
            public IntPtr Context;
        }

        [DllImport("Powrprof.dll", SetLastError = true)]
        static extern uint PowerRegisterSuspendResumeNotification(uint flags, ref DEVICE_NOTIFY_SUBSCRIBE_PARAMETERS receipient, ref IntPtr registrationHandle);

    }

}
