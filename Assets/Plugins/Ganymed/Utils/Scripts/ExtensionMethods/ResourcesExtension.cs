using System.IO;
using UnityEngine;

public class ResourcesExtension
{
    private static readonly string ResourcesPath = Application.dataPath +"/Resources";

    public static Object Load(string resourceName, System.Type systemTypeInstance) 
    {
        var directories = Directory.GetDirectories(ResourcesPath,"*",SearchOption.AllDirectories);
        foreach (var item in directories)
        {
            var itemPath = item.Substring(ResourcesPath.Length+1);
            var result = Resources.Load(itemPath+"\\"+resourceName,systemTypeInstance);
            if(result!=null)
                return result;
        }
        return null;
    }
}