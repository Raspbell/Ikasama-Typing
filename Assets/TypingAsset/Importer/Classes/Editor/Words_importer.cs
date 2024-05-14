using UnityEngine;
using System.IO;
using UnityEditor;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

/// <summary>
/// エクセルシートを自動でScriptableObjectの形式でImportするクラスS
/// Editor上で機能
/// </summary>
namespace TKG.Typing
{
	public class Words_importer : AssetPostprocessor
	{
		private static readonly string filePath = "Assets/Xls/Words.xls";
		private static readonly string exportPath = "Assets/Xls/Words.asset";
		private static readonly string[] sheetNames = { "Sheet1", };

		static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			foreach (string asset in importedAssets)
			{
				if (!filePath.Equals(asset))
					continue;

				WordSheetList data = (WordSheetList)AssetDatabase.LoadAssetAtPath(exportPath, typeof(WordSheetList));
				if (data == null)
				{
					data = ScriptableObject.CreateInstance<WordSheetList>();
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

						WordSheetList.Sheet s = new WordSheetList.Sheet();
						s.name = sheetName;

						for (int i = 1; i <= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow(i);
							ICell cell = null;

							WordSheetList.Param p = new WordSheetList.Param();

							cell = row.GetCell(0); p.Words = (cell == null ? "" : cell.StringCellValue);
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