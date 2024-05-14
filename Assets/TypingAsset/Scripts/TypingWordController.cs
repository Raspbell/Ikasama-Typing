using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace TKG.Typing
{
    /// <summary>
    /// タイピングのシステムを実装できるクラス
    /// このスクリプトはオブジェクトにアタッチして利用する
    /// GetTypingDataメソッドから結果を取得することができる
    /// すべてのタイピング方法(xtu　→ "っ" など)に対応しているつもりですが、バグ等あれば改善します
    /// 自分でエクセルシートを作成しインポートすると、インポーターが反応してScriptableObjectを作成します
    /// ↑を[SerializeField]で参照させるかAwake内に書き加えてResourcesフォルダから読み込んでください
    /// 文字列用のファイルのエクセル形式は、Xlsフォルダにはいっている既存のエクセルシートの方式を参考にしてください。
    /// </summary>
    public class TypingWordController : MonoBehaviour
    {

        /// <summary>
        /// 言語データが入っているクラス
        /// </summary>
        [SerializeField]
        RomeLangSheetList _jrll;
        /// <summary>
        /// 上のクラスのシート（言語表）
        /// </summary>
        RomeLangSheetList.Sheet _sheet = new RomeLangSheetList.Sheet();

        /// <summary>
        /// 文字列が入ってるクラス
        /// </summary>
        [SerializeField]
        private WordSheetList _wordsSheetClass;

        /// <summary>
        /// このメソッドを呼び出したフレーム毎でキーボード入力の結果を取得できる
        /// このクラスの本体
        /// </summary>
        /// <returns></returns>
        public TypingData GetTypingData()
        {
            if (InputString() == null)
            {
                _data.TypeResult = ETypeResult.NONE;
                return _data;
            }
            int selectNum = 0;

            bool isBreak = false; //処理を抜けたかどうか？（文字が一致したかどうか）
            string[] wordsJudge = new string[_data.Patterns.Count];


            //入力が一致している文字列を探す
            for (int i = 0; i < _data.Patterns.Count; i++)
            {
                bool isEqualAlphabet = _data.Patterns[i][_data.SelectIndexNum].ToString() == InputString();
                if (isEqualAlphabet && !isBreak)
                {
                    _data.NowSelectWord = _data.Patterns[i];//ローマ字のパターンを変える
                    selectNum = i;
                    _romePatternNum = i;
                    _wordSize = _data.Patterns[i].Length;
                    wordsJudge[i] = _data.Patterns[i].ToString();

                    isBreak = true;
                }
                else if (!isEqualAlphabet)
                {
                    wordsJudge[i] = _removeMark;
                }
            }

            //一致があったら
            if (isBreak)
            {
                //いらない文字列をRemoveする
                for (int i = 0; i < wordsJudge.Length; i++)
                {                   //あったら
                    if (wordsJudge[i] == _removeMark) _data.Patterns[i] = wordsJudge[i];
                }
                _data.Patterns.RemoveAll(IsRemoveStr);

                //文字列を選択している数値がおかしくなってるはずなので調整する
                for (int i = 0; i < _data.Patterns.Count; i++)
                {
                    //一致している文字列があったら
                    if (wordsJudge[selectNum] == _data.Patterns[i])
                    {
                        _romePatternNum = i;
                    }
                }

                //文字の判定先を１進める
                _data.SelectIndexNum++;
                if (IsEqualLength())
                {
                    //次の文字列へ
                    _wordNum++;
                    _wordSize = _data.Patterns[_romePatternNum].Length;
                    _data.SelectIndexNum = 0;
                    _data.Patterns = GetRomePetterns(_wordsList[_wordNum]);
                    _data.NowSelectWord = _data.Patterns[_data.SelectIndexNum];
                    _data.Japanese = _wordsList[_wordNum];
                    _data.TypeResult = ETypeResult.FINISH;
                }
                else
                {
                    _data.TypeResult = ETypeResult.TRUE;
                }
            }
            else
            {
                _data.TypeResult = ETypeResult.FALSE;
            }


            _data.NowSelectWord = _data.Patterns[_romePatternNum];//ローマ字のパターンを変える
            return _data;
        }

        /// <summary>
        /// 文字列格納用リスト
        /// </summary>
        private List<string> _wordsList = new List<string>();

        /// <summary>
        /// アルファベット一覧
        /// </summary>
        private string[] _keys = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l",
        "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "-" };

        /// <summary>
        /// 何番目の文字列が選択されているか
        /// </summary>
        private int _wordNum = 0;

        private int _romePatternNum = 0;

        /// <summary>
        /// 現在選ばれているワードの文字列の大きさ
        /// </summary>
        private int _wordSize;

        /// <summary>
        /// 選択肢除外用文字
        /// </summary>
        private string _removeMark = "×";

        /// <summary>
        /// さんかく
        /// </summary>
        private char _sankaku = '△';

        /// <summary>
        /// タイピングデータ
        /// </summary>
        private TypingData _data;

        private void Awake()
        {
            //ローマ字日本語変換表の取得
            //if (_jrll == null) _jrll = Resources.Load("JaRomeLaList") as RomeLangSheetList;

            ////文章リストの取得部分
            //if (_wordsSheetClass == null) _wordsSheetClass = Resources.Load("WodsList") as WordSheetList;
            for (int i = 0; i < _wordsSheetClass.sheets[0].list.Count; i++)
            {
                _wordsList.Add(_wordsSheetClass.sheets[0].list[i].Words);
            }

            _data = new TypingData();
            ShuffleWordList();
        }

        private void Start()
        {
            //_typingWordsRomePattern[_romePatternNum]
            //初期分のデータキャッシュを作成
            _data = new TypingData()
            {
                SelectIndexNum = 0,
                Patterns = GetRomePetterns(_wordsList[0]),
                Japanese = _wordsList[_wordNum],
                NowSelectWord = GetRomePetterns(_wordsList[0])[0],
                TypeResult = ETypeResult.NONE
            };
        }

        /// <summary>
        /// //Fisher-Yatesアルゴリズムで文章リストをシャッフルする
        /// </summary>
        private void ShuffleWordList()
        {
            System.Random rnd = new System.Random();
            int n = _wordsList.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                string tmp = _wordsList[k];
                _wordsList[k] = _wordsList[n];
                _wordsList[n] = tmp;
            }
        }

        /// <summary>
        /// 日本語一覧取得
        /// </summary>
        /// <returns></returns>
        private string[] GetJapanasePetterns()
        {
            string[] japanese = new string[_sheet.list.Count];
            for (int i = 0; i < japanese.Length; i++)
            {
                japanese[i] = _sheet.list[i].Japanese;
            }
            return japanese;
        }

        /// <summary>
        /// 文字列を組み立て変えて、扱いやすい形にする
        /// </summary>
        private List<string> AssembleStr(string nowWord)
        {
            List<string> strs = new List<string>(); //文字列を分解して格納する用のList
            for (int i = 0; i < nowWord.Length; i++)
            {
                int strsMaxBef = strs.Count - 1;
                //ゃ、ゅ、ょ が現れてしまったら
                if (IsLowercaseStr(nowWord[i]))
                {
                    string toge = strs[strsMaxBef].ToString() + nowWord[i].ToString();
                    strs.RemoveAt(strsMaxBef);
                    strs.Add(toge);
                }
                else
                {
                    strs.Add(nowWord[i].ToString());
                }
            }
            return strs;
        }

        /// <summary>
        /// ゃ、ゅ、ょ　などが現れてしまったときの判定
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool IsLowercaseStr(char str)
        {
            return str == 'ゃ' || str == 'ゅ' || str == 'ょ' || str == 'ぁ' || str == 'ぇ' || str == 'ぉ' || str == 'ぃ' || str == 'ぅ';
        }

        /// <summary>
        /// 現在の考えうるパターンを返す
        /// </summary>
        List<string> GetRomePetterns(string nowWord)
        {
            _data.Patterns = new List<string>();
            _sheet = _jrll.sheets[0]; //言語一覧があるシート
            List<string> typingWordDiv = new List<string>(); //分解した文字列を格納する用のList　例:たらこ　{"た","ら","こ"}
            List<string[]> sourceList = new List<string[]>(typingWordDiv.Count); //組み合わせる文字群(ローマ字変換後)

            string[] japanesePatterns = GetJapanasePetterns();
            
            typingWordDiv = AssembleStr(nowWord);

            //文字数文くりかえす
            for (int i = 0; i < typingWordDiv.Count; i++)
            {
                //日本語と一致するまで繰り返す
                for (int j = 0; j < japanesePatterns.Length; j++)
                {
                    //日本語と一致したら
                    if (japanesePatterns[j] == typingWordDiv[i])
                    {
                        sourceList.Add(GetRomePatternsAll(j));
                    }
                }
            }

            //全ての組み合わせを得て、格納する
            List<string[]> resultList = Combinations<string>.GetCombinations(sourceList);

            //resultListの文字列群を組み合わせて一つの文字列として最終的な判定用のListに格納する。
            for (int i = 0; i < resultList.Count; i++)
            {
                string str2 = "";
                for (int j = 0; j < resultList[i].Length; j++)
                {
                    str2 += resultList[i][j];
                }
                _data.Patterns.Add(str2);
            }
            //かぶりがなぜかあるらしいので消し去ります
            for (int i = 0; i < _data.Patterns.Count; i++)
            {
                string nowRomePatternI = _data.Patterns[i];

                //ついでに"っ"がついてたら連続入力対応させます
                for (int j = 0; j < _data.Patterns[i].Length; j++)
                {
                    if (nowRomePatternI[j] == _sankaku)
                    {
                        string tywrpOne = _data.Patterns[i];
                        //いったん文字型配列に変換。
                        char[] tywrpOneChar = tywrpOne.ToCharArray();
                        tywrpOneChar[j] = _data.Patterns[i][j + 1];
                        tywrpOne = new string(tywrpOneChar);//もどす
                        _data.Patterns[i] = tywrpOne;
                    }
                }

                //最後がn一文字で終わったら消す準備
                bool isEnd_n = nowRomePatternI[nowRomePatternI.Length - 1] == 'n' && nowRomePatternI[nowRomePatternI.Length - 2] != 'n';
                if (isEnd_n)
                {
                    _data.Patterns[i] = _removeMark;//この後消す文字列として設定
                }

                //かぶりは消す準備
                for (int j = i + 1; j < _data.Patterns.Count; j++)
                {
                    //文字列にかぶりがあったら
                    bool isPatternEqual = nowRomePatternI == _data.Patterns[j];
                    if (isPatternEqual)
                    {
                        _data.Patterns[i] = _removeMark;//この後消す文字列として設定
                    }
                }
            }
            //削除
            _data.Patterns.RemoveAll(IsRemoveStr);
            //残ったパターンの返却
            return _data.Patterns;
        }

        /// <summary>
        /// 全ローマ字のパターンを返す
        /// </summary>
        /// <param name="num">配列番号</param>
        /// <returns></returns>
        string[] GetRomePatternsAll(int num)
        {
            List<string> romes = new List<string>();//整理しやすいのでいったんlistで格納

            romes.Add(_sheet.list[num].Rome1);
            romes.Add(_sheet.list[num].Rome2);
            romes.Add(_sheet.list[num].Rome3);
            romes.Add(_sheet.list[num].Rome4);
            romes.Add(_sheet.list[num].Rome5);
            romes.Add(_sheet.list[num].Rome6);
            romes.Add(_sheet.list[num].Rome7);
            romes.RemoveAll(IsRemoveStr);

            //最終的に返す配列
            string[] returnRomes = new string[romes.Count];
            for (int i = 0; i < returnRomes.Length; i++)
            {
                returnRomes[i] = romes[i];
            }

            return returnRomes;
        }

        /// <summary>
        /// 入力キーを文字列として返す
        /// </summary>
        /// <returns></returns>
        string InputString()
        {
            if (Input.anyKeyDown) {
                string inputKey = Input.inputString;
                foreach (string key in _keys)
                {
                    //アルファベット or - の入力があったら
                    if (inputKey == key) return key; //文字を返す }
                }
            }
            return null; //アルファベット or - でない。または無入力ならnull。
        }

        

        /// <summary>
        /// 文字列の文字を入力しきったかどうか
        /// </summary>
        /// <returns></returns>
        private bool IsEqualLength()
        {
            return _wordSize == _data.SelectIndexNum;
        }

        /// <summary>
        /// ×の文字の判定用
        /// </summary>
        private bool IsRemoveStr(string str)
        {
            return str.Contains(_removeMark);
        }
    }

    /// <summary>
    /// １タイプに対する結果の返却値
    /// </summary>
    public enum ETypeResult
    {
        NONE,TRUE,FALSE,FINISH,
    }

    /// <summary>
    ///１タイピングの中でレスポンスされる情報データ
    /// </summary>
    public class TypingData
    {
        /// <summary>
        /// キーボード入力に対する結果
        /// </summary>
        public ETypeResult TypeResult = ETypeResult.NONE;
        /// <summary>
        /// 現在入力中の文字列
        /// </summary>
        public string NowSelectWord = "";
        /// <summary>
        /// 文字列のうち入力済みの部分
        /// </summary>
        public string NowWordInputed 
        { 
            get 
            {
                if (SelectIndexNum == 0)
                {
                    return "";
                }
                else
                {
                    return NowSelectWord.Substring(0,SelectIndexNum);
                }

            }
        }
        /// <summary>
        /// 文字列のうち入力前の
        /// </summary>
        public string NowWordNotInput
        {
            get
            {
                return NowSelectWord.Substring(SelectIndexNum);
            }
        }
        public int SelectIndexNum = 0;

        public List<string> Patterns = new List<string>();
        public string Japanese = "";
    }

    public static class Combinations<T>
    {
        public static List<T[]> GetCombinations(List<T[]> sourceList)
        {
            List<T[]> resultList = new List<T[]>();
            Stack<T> stack = new Stack<T>();
            GetCombinationsCore(stack, resultList, sourceList);
            return resultList;
        }

        private static void GetCombinationsCore(Stack<T> stack, List<T[]> resultList, List<T[]> sourceList)
        {
            int dimension = stack.Count;
            if (sourceList.Count <= dimension)
            {
                T[] array = stack.ToArray();
                Array.Reverse(array);
                resultList.Add(array);
                return;
            }
            else
            {
                foreach (T item in sourceList[dimension])
                {
                    stack.Push(item);
                    GetCombinationsCore(stack, resultList, sourceList);
                    stack.Pop();
                }
            }
        }
    }

}