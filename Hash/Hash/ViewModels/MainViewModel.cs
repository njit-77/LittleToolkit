using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using YE.Core.Algorithms;

namespace Hash.ViewModels;

partial class MainViewModel : ObservableObject
{
    #region Property

    [ObservableProperty]
    private string[] fileNames = null;

    [ObservableProperty]
    private string msg;

    [ObservableProperty]
    private int fileValue = 0;

    [ObservableProperty]
    private int totalValue = 0;

    [ObservableProperty]
    private int totalMaximum = 1;

    [ObservableProperty]
    private bool stopEnabled = false;

    [ObservableProperty]
    private bool showVersion = true;

    [ObservableProperty]
    private bool showDate = true;

    [ObservableProperty]
    private bool showMD5 = true;

    [ObservableProperty]
    private bool showSHA1 = true;

    [ObservableProperty]
    private bool showCRC32 = true;

    #endregion


    #region Command

    [RelayCommand]
    private void Browse()
    {
        var openFileDialog = new Microsoft.Win32.OpenFileDialog();
        if (true == openFileDialog.ShowDialog())
        {
            FileNames = new string[] { openFileDialog.FileName };
            ParseFiles(FileNames);
        }
    }

    [RelayCommand]
    private void Clear()
    {
        Msg = string.Empty;
    }

    [RelayCommand]
    private void Copy()
    {
        Clipboard.SetDataObject(Msg);
    }

    [RelayCommand]
    private void Save()
    {
        var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
        saveFileDialog.Filter = "txt files (*.txt)|*.txt";
        if (true == saveFileDialog.ShowDialog())
        {
            File.WriteAllText(saveFileDialog.FileName, Msg);
        }
    }

    [RelayCommand]
    private void Stop() { }

    #endregion

    partial void OnFileNamesChanged(string[] value)
    {
        ParseFiles(value);
    }

    private void ParseFiles(string[] fileNames)
    {
        if (fileNames != null && fileNames.Length > 0)
        {
            TotalMaximum = fileNames.Length;
            TotalValue = 0;

            foreach (var fileName in fileNames)
            {
                ParseFile(fileName);

                TotalValue += 1;
            }
        }
    }

    private void ParseFile(string fileName)
    {
        FileValue = 0;

        var sb = new StringBuilder(1024);
        sb.AppendLine($"File: {fileName}");

        var fileSize = GetFileSize(fileName);
        sb.AppendLine(fileSize);

        if (ShowVersion)
        {
            var fileVersion = GetFileVersion(fileName);
            if (false == (string.IsNullOrEmpty(fileVersion)))
            {
                sb.AppendLine(fileVersion);
            }
        }
        if (ShowDate)
        {
            var fileModifiedTime = GetFileModifiedTime(fileName);
            sb.AppendLine(fileModifiedTime);
        }
        if (ShowMD5)
        {
            var fileMD5 = GetFileMD5(fileName);
            sb.AppendLine(fileMD5);
        }
        if (ShowSHA1)
        {
            var fileSHA1 = GetFileSHA1(fileName);
            sb.AppendLine(fileSHA1);
        }
        if (ShowCRC32)
        {
            var fileCRC32 = GetFileCRC32(fileName);
            sb.AppendLine(fileCRC32);
        }
        sb.AppendLine();

        FileValue = 1;

        Msg += sb.ToString();
        sb.Clear();
    }

    private string GetFileSize(string fileName)
    {
        long length = new System.IO.FileInfo(fileName).Length;
        return $"Size: {length} bytes";
    }

    private string GetFileVersion(string fileName)
    {
        var fileVersion = FileVersionInfo.GetVersionInfo(fileName);
        if (string.IsNullOrEmpty(fileVersion.FileVersion))
        {
            return fileVersion.FileVersion;
        }
        return $"File Version: {fileVersion.FileVersion}";
    }

    private string GetFileModifiedTime(string fileName)
    {
        var fi = new FileInfo(fileName);
        return $"Modified: {fi.LastWriteTime:yyyy年M月d日,H:mm:ss}";
    }

    private string GetFileMD5(string fileName)
    {
        using var md5 = MD5.Create();
        using var stream = System.IO.File.OpenRead(fileName);
        var hash = md5.ComputeHash(stream);

        return $"MD5: {BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant()}";
    }

    private string GetFileSHA1(string fileName)
    {
        using var sha1 = SHA1.Create();
        using var stream = System.IO.File.OpenRead(fileName);
        var hash = sha1.ComputeHash(stream);

        return $"SHA1: {BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant()}";
    }

    private string GetFileCRC32(string fileName)
    {
        using var fs = File.Open(fileName, FileMode.Open, FileAccess.Read);
        using var br = new BinaryReader(fs);
        long numBytes = new FileInfo(fileName).Length;
        var buff = br.ReadBytes((int)numBytes);
        var crc32 = CrcHelper.GetCrc32(data: buff, 0x04C11DB7, true);

        return $"CRC32: {crc32.ToString("x2").ToUpperInvariant()}";
    }
}
