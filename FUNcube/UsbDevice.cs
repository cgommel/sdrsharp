using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace SDRSharp.FUNcube
{
    public static class UsbDevice
    {
        public static FileStream Open(string deviceId)
        {
            deviceId = deviceId.ToLower();
            FileStream file = null;

            Guid guid;
            UsbAPI.HidD_GetHidGuid(out guid);
            var hDevInfo = UsbAPI.SetupDiGetClassDevs(
                ref guid,
                IntPtr.Zero,
                IntPtr.Zero,
                UsbAPI.DigcfInterfaceDevice | UsbAPI.DigcfPresent);

            var deviceInfoData = new SpDevInfoData();
            deviceInfoData.cbSize = (uint) Marshal.SizeOf(deviceInfoData);

            for (var i = 0u; UsbAPI.SetupDiEnumDeviceInfo(hDevInfo, i, ref deviceInfoData); i++)
            {
                uint bufferSize = 1024 * 4;
                var sb = new StringBuilder((int) bufferSize);
                uint propertyInfoRegDataType;

                UsbAPI.SetupDiGetDeviceRegistryProperty(
                        hDevInfo,
                        ref deviceInfoData,
                        UsbAPI.SpdrpHardwareId,
                        out propertyInfoRegDataType,
                        sb,
                        bufferSize,
                        out bufferSize);

                if (sb.ToString().ToLower().Contains(deviceId))
                {
                    var interfaceData = new SpDeviceInterfaceData();
                    interfaceData.cbSize = Marshal.SizeOf(interfaceData);
                    UsbAPI.SetupDiEnumDeviceInterfaces(
                        hDevInfo,
                        IntPtr.Zero,
                        ref guid,
                        i,
                        ref interfaceData);

                    int requiredSize;
                    UsbAPI.SetupDiGetDeviceInterfaceDetail(
                        hDevInfo,
                        ref interfaceData,
                        IntPtr.Zero,
                        0,
                        out requiredSize,
                        IntPtr.Zero);

                    var interfaceDetail = new SpDeviceInterfaceDetailData();
                    interfaceDetail.cbSize = IntPtr.Size == 8 ? 8 : 4 + Marshal.SystemDefaultCharSize;
                    UsbAPI.SetupDiGetDeviceInterfaceDetail(
                        hDevInfo,
                        ref interfaceData,
                        ref interfaceDetail,
                        requiredSize,
                        IntPtr.Zero,
                        IntPtr.Zero);

                    var unsafeHandle = UsbAPI.CreateFile(
                        interfaceDetail.DevicePath,
                        UsbAPI.GenericRead | UsbAPI.GenericWrite,
                        UsbAPI.FileShareRead | UsbAPI.FileShareWrite,
                        IntPtr.Zero,
                        UsbAPI.OpenExisting,
                        0,
                        IntPtr.Zero);

                    var hidHandle = new SafeFileHandle(unsafeHandle, true);

                    if (!hidHandle.IsInvalid)
                    {
                        file = new FileStream(hidHandle, FileAccess.ReadWrite);
                    }
                    
                    break;
                }
            }

            UsbAPI.SetupDiDestroyDeviceInfoList(hDevInfo);

            if (file == null)
            {
                throw new ApplicationException("Device not found");
            }
            
            return file;
        }
    }
}
