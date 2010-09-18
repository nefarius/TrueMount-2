using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;

namespace TrueMount
{
    static class SystemDevices
    {
        public const string Win32_LogicalDisk = "Win32_LogicalDisk";
        public const string Win32_DiskDrive = "Win32_DiskDrive";
        public const string Win32_DiskPartition = "Win32_DiskPartition";
        public const string Win32_USBControllerDevice = "Win32_USBControllerDevice";

        private static ManagementClass _LogicalDisks = new ManagementClass(Win32_LogicalDisk);
        private static ManagementClass _DiskDrives = new ManagementClass(Win32_DiskDrive);
        private static ManagementClass _DiskPartitions = new ManagementClass(Win32_DiskPartition);
        private static ManagementClass _USBControllerDevices = new ManagementClass(Win32_USBControllerDevice);

        /// <summary>
        /// Check if logical device is online (=plugged in and mounted with drive letter).
        /// </summary>
        /// <param name="letter">Drive letter of the device.</param>
        /// <returns>Returns true if device with given drive letter is online, else false.</returns>
        public static bool IsLogicalDiskOnline(String letter, int type = 2)
        {
            var ldQuery =
                from ManagementObject ldisk in _LogicalDisks.GetInstances()
                where ldisk["Name"].ToString() == letter &&
                int.Parse(ldisk["DriveType"].ToString()) == type
                select ldisk;

            if (ldQuery.Count() > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Get a device with given caption and signature.
        /// </summary>
        /// <param name="caption">Caption (title, name) of the device.</param>
        /// <param name="signature">Signature (10 digit unsigned integer) of the device. May be 0 in some cases.</param>
        /// <returns>Returns the device with the given parameters.</returns>
        public static ManagementObject GetDiskDriveBySignature(String caption, uint signature)
        {
            var hddQuery =
                from ManagementObject hdd in _DiskDrives.GetInstances()
                where (string)hdd["Caption"] == caption
                && (uint)hdd["Signature"] == signature
                select hdd;

            if (hddQuery.Count() > 0)
                return hddQuery.First();
            else
                return null;
        }

        /// <summary>
        /// Contains all disks of the system.
        /// </summary>
        public static ManagementObjectCollection DiskDrives
        {
            get
            {
                return _DiskDrives.GetInstances();
            }
        }

        /// <summary>
        /// Get a partition by its disk index and partition index.
        /// </summary>
        /// <param name="DiskIndex">Disk index (zero based).</param>
        /// <param name="Index">Partition index (zero based).</param>
        /// <returns>Returns a found partition or null.</returns>
        public static ManagementObject GetPartitionByIndex(uint DiskIndex, uint Index)
        {
            var partQuery =
                from ManagementObject partition in DiskPartitions
                where (uint)partition["DiskIndex"] == DiskIndex
                && (uint)partition["Index"] == Index
                select partition;

            if (partQuery.Count() > 0)
                return partQuery.First();
            else
                return null;
        }

        /// <summary>
        /// Contains all partitions of the system.
        /// </summary>
        public static ManagementObjectCollection DiskPartitions
        {
            get { return _DiskPartitions.GetInstances(); }
        }

        /// <summary>
        /// Contains all logical disks of the system.
        /// </summary>
        public static ManagementObjectCollection LogicalDisks
        {
            get { return _LogicalDisks.GetInstances(); }
        }

        /// <summary>
        /// Contains all usb controller devices.
        /// </summary>
        public static ManagementObjectCollection USBControllerDevices
        {
            get { return _USBControllerDevices.GetInstances(); }
        }

        /// <summary>
        /// Get the drive letter of a logical disk identified by its caption, signature and partition number.
        /// </summary>
        /// <param name="caption">Caption (title, name) of the disk.</param>
        /// <param name="signature">Signature (10 digit unsigned integer) of the disk.</param>
        /// <param name="partition_nr">Partition number (zero based).</param>
        /// <returns>Returns the drive letter of the logical disk.</returns>
        public static String GetDriveLetterBySignature(String caption, uint signature, uint partition_nr)
        {
            var devQuery =
                from ManagementObject disk in DiskDrives
                where (string)disk["Caption"] == caption
                && (uint)disk["Signature"] == signature
                from ManagementObject partition in disk.GetRelated(SystemDevices.Win32_DiskPartition)
                where (uint)partition["Index"] == partition_nr
                from ManagementObject ldisk in partition.GetRelated(SystemDevices.Win32_LogicalDisk)
                select (string)ldisk["Name"];

            if (devQuery.Count() > 0)
                return (string)devQuery.First();
            else
                return null;
        }

        /// <summary>
        /// Get a logical disk by a drive letter an disk type.
        /// </summary>
        /// <param name="letter">Drive letter of the disk.</param>
        /// <param name="drive_type">Optional drive type (3 = local, 2 = removable).</param>
        /// <returns>Returns the disk or null.</returns>
        public static ManagementObject GetLogicalDisk(String letter, int drive_type = 3)
        {
            var ldQuery =
                from ManagementObject ldisk in LogicalDisks
                where int.Parse(ldisk["DriveType"].ToString()) == drive_type
                && ldisk["Name"].ToString().StartsWith(letter)
                select ldisk;

            if (ldQuery.Count() > 0)
                return ldQuery.First();
            else
                return null;
        }

        /// <summary>
        /// Get the disk caption, signature and partition number by the drive letter.
        /// </summary>
        /// <param name="letter">Drive letter of the logical disk.</param>
        /// <param name="caption">Caption of the disk.</param>
        /// <param name="signature">Signature of the disk.</param>
        /// <param name="partition_nr">Partition number (zero based).</param>
        public static void GetDiskSignatureFromLetter(String letter,
            ref string caption, ref uint signature, ref uint partition_nr)
        {
            var ldiskQuery =
                from ManagementObject ldisk in LogicalDisks
                where ldisk["Name"].ToString().StartsWith(letter)
                select ldisk;

            var partQuery =
                from ManagementObject part in ldiskQuery.First().GetRelated(SystemDevices.Win32_DiskPartition)
                select part;

            if (partQuery.Count() > 0)
                partition_nr = (uint)partQuery.First()["Index"];

            var diskQuery =
                from ManagementObject disk in partQuery.First().GetRelated(Win32_DiskDrive)
                select disk;

            if (diskQuery.Count() > 0)
            {
                ManagementObject disk_drive = diskQuery.First();

                caption = (string)disk_drive["Caption"];
                signature = (uint)disk_drive["Signature"];
            }
        }

        /// <summary>
        /// Converts the device id into a TrueCrypt compatible path.
        /// </summary>
        /// <param name="deviceid">Device ID</param>
        /// <returns>Returns a string with the device path.</returns>
        public static String GetTCCompatibleName(String deviceid)
        {
            return Regex.Replace(deviceid, "Disk #([0-9]*), Partition #([0-9]*)", ReplaceEvaluator);
        }

        private static string ReplaceEvaluator(Match m)
        {
            return @"\Device\Harddisk" +
                (int.Parse(m.Groups[1].Value)).ToString() +
                @"\Partition" +
                (int.Parse(m.Groups[2].Value) + 1).ToString();
        }

        /// <summary>
        /// Contains all available drive letters (gets refreshed every call).
        /// </summary>
        public static List<string> FreeDriveLetters
        {
            get
            {
                List<string> alphabet = AllDriveLetters;

                foreach (DriveInfo drive in DriveInfo.GetDrives())
                    alphabet.Remove(drive.Name.Substring(0, 1).ToUpper());

                if (alphabet.Count > 0)
                    return alphabet;
                else
                    return null;
            }
        }

        /// <summary>
        /// Contains all drive letters.
        /// </summary>
        public static List<string> AllDriveLetters
        {
            get
            {
                List<string> alphabet = new List<string>();
                int lowerBound = Convert.ToInt16('C');
                int upperBound = Convert.ToInt16('Z');
                int index = 0;

                for (index = lowerBound; index < upperBound; index++)
                {
                    char driveLetter = (char)index;
                    alphabet.Add(driveLetter.ToString());
                }

                return alphabet;
            }
        }
    }
}
