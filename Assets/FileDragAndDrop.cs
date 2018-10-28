using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using B83.Win32;
using System.Text;
using System.Windows.Forms;
using System;

public class FileDragAndDrop : MonoBehaviour
{
    UnityDragAndDropHook hook;
    bool isAdministrator = false;
    private void Awake()
    {
        isAdministrator = IsAdministrator();
        UnityEngine.Screen.SetResolution(800,600,false);
    }
    // important to keep the instance alive while the hook is active.
    void Start()
    {
        if (!isAdministrator)
        {
                ReStartAs();
        }
        else
        {
            Debug.Log("管理员");
            // must be created on the main thread to get the right thread id.
            hook = new UnityDragAndDropHook();
            hook.InstallHook();
            hook.OnDroppedFiles += OnFiles;
        }

    }
 
    void OnDisable()
    {
        if (null!=hook)
        {
        hook.UninstallHook();
        }
    }
    private void OnDestroy()
    {
        if (null != hook)
        {
            hook.UninstallHook();
            GC.Collect();
        }
    }
    void OnFiles(List<string> aFiles, POINT aPos)
    {
        // do something with the dropped file names. aPos will contain the 
        // mouse position within the window where the files has been dropped.

        Debug.Log("Dropped " + aFiles.Count + " files at: " + aPos + "\n" +
            aFiles.Aggregate((a, b) => a + "\n" + b));
        StringBuilder sb = new StringBuilder();
        sb.Append("拖拽文件进来了:\n\n");
        foreach (var path in aFiles)
        {
            sb.Append(path);
            sb.Append("\n\n");
        }
        MessageBox.Show(sb.ToString());

    }
    private bool IsAdministrator() // 判断是否有管理员权限
    {
        //获得当前登录的Windows用户标示 
        System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
        System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);
        //判断当前登录用户是否为管理员 
        return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
    }

    private void ReStartAs()
    {
        //创建启动对象 
        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
        //设置运行文件 
        startInfo.FileName = System.Environment.GetCommandLineArgs()[0];
        //设置启动参数 
        startInfo.Arguments = "";
        //设置启动动作,确保以管理员身份运行 
        startInfo.Verb = "runas";
        //如果不是管理员，则启动UAC 
        System.Diagnostics.Process.Start(startInfo);
        //退出 

        UnityEngine.Application.Quit();
    }


}
