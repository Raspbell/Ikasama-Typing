using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

namespace TKG.Typing.Demo {

    /// <summary>
    /// 一応のゲーム性を実現するためのスクリプト
    /// キーボードのビジュアル化はこちらで行っています
    /// </summary>
    public class TypingGameDemo : MonoBehaviour {

        #region 使用変数

        private TypingWordController _typingController;

        /// <summary>
        /// タイピングのメインキャンバス
        /// </summary>
        [SerializeField]
        private GameObject _mainCanvas = null;

        /// <summary>
        /// スタートのキャンバス
        /// </summary>
        [SerializeField]
        private GameObject _subCanvas = null;

        /// <summary>
        /// 最終結果の表示用キャンバス
        /// </summary>
        [SerializeField]
        private GameObject _resultCanvas = null;

        [SerializeField]
        private Text _subText = null;

        /// <summary>
        /// キーボードのキャンバス
        /// </summary>
        [SerializeField]
        private GameObject _keyboardCanvas = null;

        /// <summary>
        /// カラーリングの最初(黒)
        /// </summary>
        private string _colorStartBlack = "<color=#000000>";

        /// カラーリングの最初(赤)
        /// </summary>
        private string _colorStartRed = "<color=#ff0000>";

        /// <summary>
        /// カラーリングの最後
        /// </summary>
        private string _colorEnd = "</color>";



        [SerializeField]
        private int _scoreMag = 50;

        /// <summary>
        /// テキスト
        /// </summary>
        [SerializeField]
        private Text _mainText = null;



        /// <summary>
        /// 日本語テキスト
        /// </summary>
        [SerializeField]
        private Text _wordText = null;

        /// <summary>
        /// 時間テキスト
        /// </summary>
        [SerializeField]
        private Text _timeText = null;


        /// <summary>
        /// 正しく入力した回数のテキスト
        /// </summary>
        [SerializeField]
        private Text _inputTrueCountText = null;

        /// <summary>
        /// 最終スコアのテキスト
        /// </summary>
        [SerializeField]
        private Text _scoreText = null;

        /// <summary>
        /// ランク表示
        /// </summary>
        [SerializeField]
        private Image _rankImage = null;

        /// <summary>
        /// もどるよっていうだけのテキスト
        /// </summary>
        //[SerializeField]
        private GameObject _returnText = null;

        int tatwoNum = 0;

        /// <summary>
        /// 正しく入力された文字数
        /// </summary>
        private int _inputTrueCount = 0;

        /// <summary>
        /// ミスした回数
        /// </summary>
        private int _miss = 0;



        /// <summary>
        /// ゲームをクリアしているかどうか
        /// </summary>
        bool isPlaying = false;



        /// <summary>
        /// キーボードキー(^O^)
        /// </summary>
        [SerializeField]
        private List<Image> _keysSpriteList = null;

        /// <summary>
        /// ランク画像集
        /// </summary>
        [SerializeField]
        private List<Sprite> _ranks = null;

        /// <summary>
        /// 制限時間
        /// </summary>
        [SerializeField]
        private const float PLAY_TIME = 90f;

        /// <summary>
        /// 経過時間
        /// </summary>
        private float _time = 0f;

        /// <summary>
        /// mainCanvasの横の長さ
        /// </summary>
        private float _mainCanvasWidth = 0;

        /// <summary>
        /// 田２image
        /// </summary>
        [SerializeField]
        private Image _tatwo = null;

        /// <summary>
        /// タトゥーの見た目
        /// </summary>
        [SerializeField]
        private List<Sprite> _tatwoSprites = null;

        /// <summary>
        /// アルファベット一覧
        /// </summary>
        private string[] _keys = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l",
        "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "-" };


        #endregion

        // Start is called before the first frame update
        void Start()
        {
            if (_mainCanvas == null) _mainCanvas = GameObject.Find("MainCanvas");

            _typingController = GetComponent<TypingWordController>();

            TypingData data = _typingController.GetTypingData();
            
            _time = PLAY_TIME;//制限時間
            StartCoroutine(WaitSpaceKeyDown());//入力待ち
        }

        // Update is called once per frame
        void Update()
        {
            if (!isPlaying) return;

            TypingData data = _typingController.GetTypingData();

            TimeManager();
            DispJapaneseWord(data.Japanese);
            _mainText.text = GetWordDisp(data.NowSelectWord, data.NowWordInputed, data.NowWordNotInput);//現在の入力状況をtextで表示
            SetKeyboardColor(data.Patterns, data.SelectIndexNum);
        }
        #region 時間管理

        /// <summary>
        /// 時間管理
        /// </summary>
        void TimeManager()
        {
            _time -= Time.deltaTime;
            _timeText.text = "Time:" + ((int)_time).ToString();
            bool isEndTime = _time <= 0;
            if (isEndTime) Finish();
        }

        #endregion

        #region 文字色のリセット

        /// <summary>
        /// 文字列を黒色にリセット
        /// </summary>
        string ResetStringBlack(string selectWord)
        {
            return _colorStartBlack + selectWord + _colorEnd;
        }
        #endregion

        #region 文字列の返却(カラー情報付き）

        /// <summary>
        /// 文字列をカラー情報付きで返す
        /// </summary>
        /// <returns></returns>
        string GetWordDisp(string selectWord,string wordRed,string wordBlack)
        {
            if (!isPlaying) {
                return ResetStringBlack(selectWord); ;
            }
            return GetWordRed(wordRed) + GetWordBlack(wordBlack);
        }

        /// <summary>
        /// 黒色部分
        /// </summary>
        /// <returns>黒色部分</returns>
        string GetWordBlack(string word)
        {
            return _colorStartBlack + word + _colorEnd;
        }

        /// <summary>
        /// 赤色部分
        /// </summary>
        /// <returns>赤色部分</returns>
        string GetWordRed(string word)
        {
            return _colorStartRed + word + _colorEnd;
        }

        #endregion

        /// <summary>
        /// 日本語を表示
        /// </summary>
        void DispJapaneseWord(string word)
        {
            _wordText.text = word;
        }

        #region キーボードの表示

        /// <summary>
        /// キーボードの色をリセット
        /// </summary>
        void ResetKeyboardColor()
        {
            for (int i = 0; i < _keysSpriteList.Count; i++)
            {
                _keysSpriteList[i].color = Color.white;
            }
        }
        /// <summary>
        /// キーボードの色をセット
        /// </summary>
        void SetKeyboardColor(List<string> typePetterns,int select)
        {
            ResetKeyboardColor();
            for (int i = 0; i < typePetterns.Count; i++)
            {
                for (int j = 0; j < _keys.Length; j++)
                {
                    if (_keys[j] == typePetterns[i][select].ToString())
                    {
                        _keysSpriteList[j].color = Color.yellow;
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// スペースが押されるまで待つ
        /// </summary>
        /// <returns></returns>
        IEnumerator WaitSpaceKeyDown()
        {
            _keyboardCanvas.SetActive(false);
            _subCanvas.SetActive(true);
            _mainCanvas.SetActive(false);
            _resultCanvas.SetActive(false);
            while (true)
            {

                if (isPlaying) yield break;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    yield return WaitStart();
                    isPlaying = PlayStart();
                }
                yield return null;
            }
        }

        /// <summary>
        /// 3.2.1.スタート！ってやつ
        /// </summary>
        /// <returns></returns>
        IEnumerator WaitStart()
        {
            for (int i = 3; i > 0; i--)
            {
                _subText.text = i.ToString();
                yield return new WaitForSeconds(1f);
            }
            _subText.text = "スタート！！！";
            yield return new WaitForSeconds(1f);
            _subCanvas.SetActive(false);
        }

        /// <summary>
        /// 終わり際のスコア表示等々
        /// </summary>
        /// <returns></returns>
        IEnumerator FinishPerformance()
        {
            _subCanvas.SetActive(true);
            _subText.text = "終わりー！！";
            yield return new WaitForSeconds(1f);
            _subCanvas.SetActive(false);

            yield return WaitForSecondsCanSkip(0.3f);
            _resultCanvas.SetActive(true);
            yield return WaitForSecondsCanSkip(1f);
            _inputTrueCountText.gameObject.SetActive(true);
            yield return WaitForSecondsCanSkip(1f);
            _scoreText.gameObject.SetActive(true);
            yield return WaitForSecondsCanSkip(2f);
            _rankImage.gameObject.SetActive(true);
            yield return WaitForSecondsCanSkip(1f);
            _returnText.SetActive(true);

            while (true)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                     SceneManager.LoadScene("GameScene");
                }
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Application.Quit();
                }
                yield return null;
            }

        }

        /// <summary>
        /// WaitForSecondsのパクリ
        /// </summary>
        /// <param name="tt"></param>
        /// <returns></returns>
        IEnumerator WaitForSecondsCanSkip(float tt)
        {
            float t = 0;
            while (true)
            {
                yield return null;
                t += Time.deltaTime;
                if (Input.GetKeyDown(KeyCode.Space)) yield break;
                if (t >= tt) yield break;
            }
        }

        /// <summary>
        /// タイピングをスタート
        /// </summary>
        bool PlayStart()
        {
            _mainCanvas.SetActive(true);
            _keyboardCanvas.SetActive(true);
            return true;
        }

        /// <summary>
        /// ゲームをクリアした
        /// </summary>
        void Finish()
        {
            isPlaying = false;
            _mainCanvas.SetActive(false);
            _keyboardCanvas.SetActive(false);
            _inputTrueCountText.text += _inputTrueCount.ToString() + "m";
            GetScore();
            StartCoroutine(FinishPerformance());
        }

        /// <summary>
        /// スコアの画像をゲットだ！
        /// </summary>
        void GetScore()
        {
            int score = _inputTrueCount;
            _rankImage.sprite = _ranks[score];
        }
    }
}