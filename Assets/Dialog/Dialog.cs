using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Dialog
{
    public SpeakerName speaker; // 話し手の名前
    public Image image; // 話し手の画像
    public string dialogue; // 発言内容

    // 表示オプション
    public float speed = 50.0f; // 表示速度（文字/秒）
    public float waitTime = 1.0f; // 待機時間 (自動モードの場合)
    public Color textColor = Color.white; // テキストの色
    public FontStyle fontStyle = FontStyle.Normal; // テキストのフォントスタイル
    public bool autoAdvance = false; // 自動的に次のセリフに進むかどうか

    // 音声オプション
    public AudioClip typingSound; // タイピング音
    public AudioClip dialogStartSound; // ダイアログ開始時の音
    public AudioClip dialogEndSound; // ダイアログ終了時の音
    public bool playSpecialSound = false; // 特別な効果音を再生するかどうか

    // 選択肢
    public List<Choice> choices; // 選択肢リスト

    // 経過時間と状態
    public float elapsedTime = 0.0f; // 経過時間
    public bool isFinished = false; // 発言が完了したか

    // コンストラクタ
    public Dialog(
        SpeakerName speaker, 
        string dialogue, 
        float speed = 50.0f, 
        bool autoAdvance = false, 
        Color? textColor = null, 
        FontStyle fontStyle = FontStyle.Normal, 
        AudioClip typingSound = null, 
        AudioClip dialogStartSound = null, 
        AudioClip dialogEndSound = null, 
        bool playSpecialSound = false, 
        List<Choice> choices = null
    )
    {
        this.speaker = speaker;
        this.dialogue = dialogue;
        this.speed = speed;
        this.autoAdvance = autoAdvance;
        this.textColor = textColor ?? Color.white;
        this.fontStyle = fontStyle;
        this.typingSound = typingSound;
        this.dialogStartSound = dialogStartSound;
        this.dialogEndSound = dialogEndSound;
        this.playSpecialSound = playSpecialSound;
        this.choices = choices;
    }
}

[System.Serializable]
public class Choice
{
    public string text; // 選択肢のテキスト
    public List<Dialog> nextDialogs; // 選択肢が選ばれた際に続くダイアログのリスト

    public Choice(string text, List<Dialog> nextDialogs)
    {
        this.text = text;
        this.nextDialogs = nextDialogs;
    }
}
