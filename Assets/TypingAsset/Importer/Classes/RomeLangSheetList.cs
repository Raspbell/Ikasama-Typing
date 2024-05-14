using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TKG.Typing
{
	/// <summary>
	/// 各ひらがなのローマ字入力パターンが各音につき最大七通りまですべて網羅したエクセルシートをUnity上で反映するためのクラス
	/// Inspecter上で確認が可能
	/// </summary>
	public class RomeLangSheetList : ScriptableObject
	{
		public List<Sheet> sheets = new List<Sheet>();

		[System.SerializableAttribute]
		public class Sheet
		{
			public string name = string.Empty;
			public List<Param> list = new List<Param>();
		}

		[System.SerializableAttribute]
		public class Param
		{
			public string Japanese;
			public string Rome1;
			public string Rome2;
			public string Rome3;
			public string Rome4;
			public string Rome5;
			public string Rome6;
			public string Rome7;
		}
	}

}