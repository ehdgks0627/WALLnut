﻿using System;
using System.Collections.Generic;
using System.Management;
using System.Threading;

namespace WALLnutClient
{
    public class DiskInfo
    {
        public bool isWALLNutDevice { get; set; }
        public string Caption { get; set; }
        public string DeviceID { get; set; }
        public string Model { get; set; }
        public UInt64 Partitions { get; set; }
        public UInt64 Size { get; set; }
        public Int64 uuid { get; set; }

        public override string ToString()
        {
            return (isWALLNutDevice ? "[O] " : "[X] ") + Caption + " (" + (Size / 1024 / 1024 / 1024) + "GB)";
        }

        #region [Function] PhysicalDrive의 목록을 반환
        public unsafe static List<DiskInfo> GetDriveList()
        {
            List<DiskInfo> result = new List<DiskInfo>();
            byte[] buffer = new byte[DiskManager.BLOCK_SIZE];
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("", "SELECT * FROM Win32_DiskDrive");
            foreach (ManagementObject queryObj in searcher.Get())
            {
                try
                {
                    DiskInfo diskinfo = new DiskInfo();
                    diskinfo.Caption = queryObj["Caption"].ToString();
                    diskinfo.DeviceID = queryObj["DeviceID"].ToString();
                    diskinfo.Model = queryObj["Model"].ToString();
                    diskinfo.Size = ulong.Parse(queryObj["Size"].ToString());
                    DiskManager.ReadBlock(ref buffer, 0, diskinfo.DeviceID);
                    diskinfo.isWALLNutDevice = true;
                    for (int i = 0; i < 8; i++)
                    {
                        if (buffer[i] != DiskManager.SIGNATURE[i])
                        {
                            diskinfo.isWALLNutDevice = false;
                            break;
                        }
                    }
                    if (diskinfo.isWALLNutDevice)
                    { 
                        fixed (byte* ptr_buffer = &buffer[0])
                        {
                            DiskManager.DISK_HEADER_STRUCTURE* ptr = (DiskManager.DISK_HEADER_STRUCTURE*)ptr_buffer;
                            diskinfo.uuid = ptr->uuid;
                        }
                    }
                    result.Add(diskinfo);
                }
                catch
                {
                    continue;
                }
            }
            return result;
        }
        #endregion
    }
}
