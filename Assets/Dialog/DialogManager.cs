using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }
    
    public bool isTalk = false; // 会話中かどうか
    public Text dialogText; // 会話表示用のText UI
    public Image speakerImage; // 話し手の画像用のImage UI
    public Text speakerNameText; // 話し手の名前表示用のText UI

    public AudioSource audioSource; // 効果音再生用のAudioSource

    // デフォルトのサウンド
    public AudioClip defaultTypingSound;
    public AudioClip defaultDialogStartSound;
    public AudioClip defaultDialogEndSound;

    // 選択肢UI
    public GameObject choicesPanel; // 選択肢を表示するパネル
    public Button choiceButtonPrefab; // 選択肢用のボタンプレハブ

    private Queue<Dialog> dialogQueue = new Queue<Dialog>(); // 会話のキュー
    private bool isTyping = false;
    private bool fullTextDisplayed = false; // 全文が表示されているかどうか
    private bool skipConversation = false; // 会話をスキップするかどうか

    private void Awake()
    {
        // シングルトンの初期化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいで存在させる
        }
        else
        {
            Destroy(gameObject);
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (choicesPanel != null)
        {
            choicesPanel.SetActive(false);
        }
    }

    private void Update()
    {
        // スペースキー、エンターキー、またはマウスクリックで次のダイアログに進む
        if (isTalk)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
            {
                if (isTyping)
                {
                    // タイピング中なら全文表示に切り替える
                    fullTextDisplayed = true;
                }
                else
                {
                    // 全文が表示されているなら次のダイアログに進む
                    TriggerNextDialog();
                }
            }

            // Escapeキーで会話をスキップ
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SkipConversation();
            }
        }
    }

    public async void Talk(List<Dialog> dialogs)
    {
        if (isTalk) return; // 既に会話中なら無視

        isTalk = true;
        dialogQueue.Clear();

        foreach (var dialog in dialogs)
        {
            dialogQueue.Enqueue(dialog);
        }

        // 会話開始イベントを発行
        EventManager.Instance.TriggerEvent("OnConversationStart");

        await ProcessNextDialog();
    }

    private async Task ProcessNextDialog()
    {
        if (dialogQueue.Count == 0 || skipConversation)
        {
            EndConversation();
            return;
        }

        var dialog = dialogQueue.Dequeue();
        ShowDialog(dialog);
        
        // ダイアログ開始時の効果音を再生（デフォルトを使用）
        if (dialog.dialogStartSound != null)
        {
            audioSource.PlayOneShot(dialog.dialogStartSound);
        }
        else if (defaultDialogStartSound != null)
        {
            audioSource.PlayOneShot(defaultDialogStartSound);
        }

        // タイピングエフェクト
        await TypeText(dialog.dialogue, dialog.speed, dialog.typingSound, dialog.playSpecialSound, dialog.dialogEndSound);
        
        // ダイアログ終了時の効果音を再生（特別な効果音が指定されている場合）
        if (dialog.dialogEndSound != null)
        {
            audioSource.PlayOneShot(dialog.dialogEndSound);
        }
        else if (defaultDialogEndSound != null)
        {
            audioSource.PlayOneShot(defaultDialogEndSound);
        }

        await Task.Delay((int)(dialog.waitTime * 1000));

        await ProcessNextDialog();
    }

    private void ShowDialog(Dialog dialog)
    {
        if (speakerImage != null && dialog.image != null)
        {
            speakerImage.sprite = dialog.image.sprite;
            speakerImage.enabled = true;
        }

        if (speakerNameText != null)
        {
            speakerNameText.text = dialog.speaker.ToString();
        }

        // テキストの色とスタイルを設定
        if (dialogText != null)
        {
            dialogText.color = dialog.textColor;
            dialogText.fontStyle = dialog.fontStyle;
        }

        // 選択肢がある場合は表示
        if (dialog.choices != null && dialog.choices.Count > 0)
        {
            ShowChoices(dialog.choices);
        }
        else
        {
            if (choicesPanel != null)
            {
                choicesPanel.SetActive(false);
            }
        }
    }

    private async Task TypeText(string text, float speed, AudioClip typingSound, bool playSpecialSound, AudioClip dialogEndSound)
    {
        dialogText.text = "";
        isTyping = true;
        fullTextDisplayed = false;

        foreach (char letter in text.ToCharArray())
        {
            if (fullTextDisplayed || skipConversation)
            {
                // 全文表示フラグが立っていたら残りを一気に表示
                dialogText.text = text;
                break;
            }

            dialogText.text += letter;

            // タイピング音を再生（デフォルトを使用）
            if (typingSound != null)
            {
                audioSource.PlayOneShot(typingSound);
            }
            else if (defaultTypingSound != null)
            {
                audioSource.PlayOneShot(defaultTypingSound);
            }

            await Task.Delay((int)(1000 / speed));
        }

        isTyping = false;
        fullTextDisplayed = true;
    }

    private void TriggerNextDialog()
    {
        // 非同期メソッドを適切に呼び出すために async void メソッドを使用
        TriggerNextDialogAsync();
    }

    private async void TriggerNextDialogAsync()
    {
        await ProcessNextDialog();
    }

    private void SkipConversation()
    {
        skipConversation = true;
        dialogQueue.Clear();
        EndConversation();
    }

    private void EndConversation()
    {
        isTalk = false;
        dialogText.text = "";
        speakerNameText.text = "";
        if (speakerImage != null) speakerImage.enabled = false;

        // 選択肢パネルを非表示
        if (choicesPanel != null)
        {
            choicesPanel.SetActive(false);
        }

        // 会話終了イベントを発行
        EventManager.Instance.TriggerEvent("OnConversationEnd");
    }

    #region 選択肢機能

    private void ShowChoices(List<Choice> choices)
    {
        if (choicesPanel == null || choiceButtonPrefab == null)
            return;

        choicesPanel.SetActive(true);

        // 既存のボタンをすべて削除
        foreach (Transform child in choicesPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var choice in choices)
        {
            Button button = Instantiate(choiceButtonPrefab, choicesPanel.transform);
            button.GetComponentInChildren<Text>().text = choice.text;
            button.onClick.AddListener(() => OnChoiceSelected(choice));
        }
    }

    private void OnChoiceSelected(Choice choice)
    {
        // 選択肢に応じて会話を分岐
        if (choice.nextDialogs != null && choice.nextDialogs.Count > 0)
        {
            // 現在のキューをクリアして選択した会話をキューに追加
            dialogQueue.Clear();
            foreach (var dialog in choice.nextDialogs)
            {
                dialogQueue.Enqueue(dialog);
            }

            // 選択肢パネルを非表示
            if (choicesPanel != null)
            {
                choicesPanel.SetActive(false);
            }

            // 選択肢選択イベントを発行
            EventManager.Instance.TriggerEvent("OnChoiceSelected", choice.text);

            // 次のダイアログを処理
            TriggerNextDialog();
        }
    }

    #endregion
}
    