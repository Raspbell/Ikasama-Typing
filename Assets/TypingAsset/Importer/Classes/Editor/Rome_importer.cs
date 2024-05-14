using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace TKG.Typing
{
	/// <summary>
	/// ローマ字表をエクセルからScriptableObjectにImortするクラス
	/// </summary>
	public class Rome_importer : AssetPostprocessor
	{
		private static readonly string filePath = "Assets/Xls/Rome.xls";
		private static readonly string exportPath = "Assets/Xls/Rome.asset";
		private static readonly string[] sheetNames = { "Sheet1", };

		static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			foreach (string asset in importedAssets)
			{
				if (!filePath.Equals(asset))
					continue;

				RomeLangSheetList data = (RomeLangSheetList)AssetDatabase.LoadAssetAtPath(exportPath, typeof(RomeLangSheetList));
				if (data == null)
				{
					data = ScriptableObject.CreateInstance<RomeLangSheetList>();
					AssetDatabase.CreateAsset((ScriptableObject)data, exportPath);
					data.hideFlags = HideFlags.NotEditable;
				}

				data.sheets.Clear();
				using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					IWorkbook book = null;
					if (Path.GetExtension(filePath) == ".xls")
					{
						book = new HSSFWorkbook(stream);
					}
					else
					{
						book = new XSSFWorkbook(stream);
					}

					foreach (string sheetName in sheetNames)
					{
						ISheet sheet = book.GetSheet(sheetName);
						if (sheet == null)
						{
							Debug.LogError("[QuestData] sheet not found:" + sheetName);
							continue;
						}

						RomeLangSheetList.Sheet s = new RomeLangSheetList.Sheet();
						s.name = sheetName;

						for (int i = 1; i <= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow(i);
							ICell cell = null;

							RomeLangSheetList.Param p = new RomeLangSheetList.Param();

							cell = row.GetCell(0); p.Japanese = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(1); p.Rome1 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(2); p.Rome2 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(3); p.Rome3 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4); p.Rome4 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5); p.Rome5 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(6); p.Rome6 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(7); p.Rome7 = (cell == null ? "" : cell.StringCellValue);
							s.list.Add(p);
						}
						data.sheets.Add(s);
					}
				}

				ScriptableObject obj = AssetDatabase.LoadAssetAtPath(exportPath, typeof(ScriptableObject)) as ScriptableObject;
				EditorUtility.SetDirty(obj);
			}
		}
	}
}