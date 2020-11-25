using System;
using System.Collections.Generic;
using System.IO;

namespace LibSystemInfo
{
    public static class DiskInfoValue
    {
        /// <summary>
        /// 获取当前驱动使用情况
        /// </summary>
        public static List<DriveInfo> GetDrivesInfo()
        {
            System.IO.DriveInfo[] DriveInfoArr = System.IO.DriveInfo.GetDrives();
            List<DriveInfo> result = null;
            foreach (var drv in DriveInfoArr)
            {
                if (drv != null)
                {
                    DriveInfo driveInfo = new DriveInfo();
                    if (result == null)
                    {
                        result = new List<DriveInfo>();
                    }

                    if (drv.IsReady && drv.DriveType != DriveType.Removable && drv.TotalSize > 0)
                    {
                        driveInfo.Name = drv.Name;
                        driveInfo.IsReady = drv.IsReady;
                        driveInfo.Total = drv.TotalSize;
                        driveInfo.Free = drv.AvailableFreeSpace;
                        driveInfo.Used = drv.TotalSize - drv.AvailableFreeSpace;
                        driveInfo.FreePercent = Math.Round(drv.AvailableFreeSpace * 100.00 / drv.TotalSize, 3);
                        driveInfo.UpdateTime = DateTime.Now;
                        result.Add(driveInfo);
                    }
                }
            }

            return result;
        }
    }
}