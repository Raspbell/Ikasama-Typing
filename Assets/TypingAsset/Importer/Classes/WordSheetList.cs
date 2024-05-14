using System.Collections.Generic;
using UnityEngine;

namespace TKG.Typing
{
	/// <summary>
	/// Inspecter��ŊǗ��ł��郊�X�g
	/// Assets/Create/MyScriptable/Create WordData�@�̃o�i�[����쐬���ł���
	/// </summary>
	[CreateAssetMenu(menuName = "MyScriptable/Create WordData")]
	public class WordSheetList : ScriptableObject
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
			public string Words;
		}
	}
}