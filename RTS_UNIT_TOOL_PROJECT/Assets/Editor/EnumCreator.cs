using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EnumCreator : Editor
{
    private const string UnitTypeName ="UnitType";
    private const string UnitTypePath ="Assets/Scripts/Enum/UnitType.cs";
    
    public static void WriteUnitType(List<string> data)
    {
        WriteToEnum(UnitTypePath, UnitTypeName, data);
    }

    private const string PlayerTypeName ="PlayerName";
    private const string PlayerTypePath ="Assets/Scripts/Enum/PlayerName.cs";
    
    public static void WritePlayerType(List<string> data)
    {
        WriteToEnum(PlayerTypePath,PlayerTypeName, data);
    }
    
    public static void WriteToEnum(string path, string name, List<string> data)
        {
            using (StreamWriter file = new StreamWriter(path))
            {
                file.WriteLine("public enum " + name + " \n{");
                int i = 0;
                foreach (var line in data)
                {
                    file.WriteLine(string.Format("\t{0} = {1},",
                            line, i));
                        i++;
                    }
                file.WriteLine("\n}");
            }

            AssetDatabase.ImportAsset(path);
        }
}
