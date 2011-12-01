using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace SDRSharp.FUNcube
{
    public class UsbDevice
    {
        public static FileStream Open(string deviceId)
        {
            deviceId = deviceId.ToLower();
            FileStream file = null;
            PspDeviceInterfaceDetailData interfaceDetail;

            Guid guid;
            UsbAPI.HidD_GetHidGuid(out guid);
            var hDevInfo = UsbAPI.SetupDiGetClassDevs(
                ref guid,
                IntPtr.Zero,
                IntPtr.Zero,
                UsbAPI.DigcfInterfaceDevice | UsbAPI.DigcfPresent);

            var result = -1;

            while (result != 0)
            {
                var interfaceData = new SpDeviceInterfaceData();
                interfaceData.cbSize = Marshal.SizeOf(interfaceData);
                result = UsbAPI.SetupDiEnumDeviceInterfaces(
                    hDevInfo,
                    0,
                    ref  guid,
                    0,
                    ref interfaceData);

                int requiredSize;
                UsbAPI.SetupDiGetDeviceInterfaceDetail(
                    hDevInfo,
                    ref interfaceData,
                    IntPtr.Zero,
                    0,
                    out requiredSize,
                    IntPtr.Zero);
                
                interfaceDetail = new PspDeviceInterfaceDetailData();
                interfaceDetail.cbSize = 5;
                UsbAPI.SetupDiGetDeviceInterfaceDetail(
                    hDevInfo,
                    ref interfaceData,
                    ref interfaceDetail,
                    requiredSize,
                    ref requiredSize,
                    IntPtr.Zero);
                
                if (interfaceDetail.DevicePath.ToLower().Contains(deviceId))
                {
                    var unsafeHandle = UsbAPI.CreateFile(
                        interfaceDetail.DevicePath,
                        UsbAPI.GenericRead | UsbAPI.GenericWrite,
                        UsbAPI.FileShareRead | UsbAPI.FileShareWrite,
                        IntPtr.Zero,
                        UsbAPI.OpenExisting,
                        0,
                        0);

                    var hidHandle = new SafeFileHandle(unsafeHandle, true);

                    if (!hidHandle.IsInvalid)
                    {
                        file = new FileStream(hidHandle, FileAccess.ReadWrite);
                    }
                    
                    break;
                }
            }

            UsbAPI.SetupDiDestroyDeviceInfoList(hDevInfo);
            
            return file;
        }
    }
}
