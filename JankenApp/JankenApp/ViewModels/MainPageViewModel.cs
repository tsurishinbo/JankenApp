using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace JankenApp.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        // プロパティ
        public ReactiveProperty<string> Result { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<Color> ResultColor { get; } = new ReactiveProperty<Color>();
        public ReactiveProperty<string> WinCount { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> DrawCount { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> LoseCount { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<ImageSource> ComputerHandImage { get; } = new ReactiveProperty<ImageSource>();
        public ReactiveProperty<int> ComputerHand { get; } = new ReactiveProperty<int>();
        public ReactiveProperty<ImageSource> ButtonImageGu { get; } = new ReactiveProperty<ImageSource>();
        public ReactiveProperty<ImageSource> ButtonImageChoki { get; } = new ReactiveProperty<ImageSource>();
        public ReactiveProperty<ImageSource> ButtonImagePa { get; } = new ReactiveProperty<ImageSource>();
        public ReactiveProperty<int> ButtonParameterGu { get; } = new ReactiveProperty<int>();
        public ReactiveProperty<int> ButtonParameterChoki { get; } = new ReactiveProperty<int>();
        public ReactiveProperty<int> ButtonParameterPa { get; } = new ReactiveProperty<int>();

        // コマンド
        public ReactiveCommand CommandStart { get; } = new ReactiveCommand();
        public ReactiveCommand<int> CommandSelect { get; } = new ReactiveCommand<int>();

        // 定数
        private const int INTERVAL = 80;    // コンピュータの手をぐるぐる回す間隔(msec)
        private const int HAND_GU = 0;      // グー
        private const int HAND_CHOKI = 1;   // チョキ
        private const int HAND_PA = 2;      // パー

        // コンピュータの手がぐるぐる回っているか
        private bool isRunning = true;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="navigationService"></param>
        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            // タイトル
            Title = "じゃんけんアプリ";

            // 勝敗数
            WinCount.Value = "0";
            DrawCount.Value = "0";
            LoseCount.Value = "0";

            // 自分の手ボタンの画像
            ButtonImageGu.Value = ImageSource.FromResource("JankenApp.Images.icon_gu.png");
            ButtonImageChoki.Value = ImageSource.FromResource("JankenApp.Images.icon_choki.png");
            ButtonImagePa.Value = ImageSource.FromResource("JankenApp.Images.icon_pa.png");

            // 自分の手ボタンのパラメータ
            ButtonParameterGu.Value = HAND_GU;
            ButtonParameterChoki.Value = HAND_CHOKI;
            ButtonParameterPa.Value = HAND_PA;

            // コンピュータの手ボタンの処理
            CommandStart.Subscribe(_ => 
            {
                if (!isRunning)
                {
                    Result.Value = string.Empty;    // 結果表示を消す
                    isRunning = true;               // コンピュータの手をぐるぐる回す
                }
            });

            // 自分の手ボタンの処理
            CommandSelect.Subscribe<int>(x =>
            {
                if (isRunning)
                {
                    isRunning = false;      // コンピュータの手をぐるぐる回すのを止める
                    Thread.Sleep(INTERVAL); // ぐるぐるが止まるまでの調整時間
                    Judge(x);               // じゃんけんの結果を判定・表示する
                }
            });

            // コンピュータの手をぐるぐる回すのを非同期処理で実行
            Task.Run(() => ShuffleHand());
        }

        /// <summary>
        /// コンピュータの手をぐるぐる回す
        /// </summary>
        private void ShuffleHand()
        {
            while (true)
            {
                if (isRunning)
                {
                    ComputerHandImage.Value = ImageSource.FromResource("JankenApp.Images.janken_gu.png");
                    ComputerHand.Value = HAND_GU;
                    Thread.Sleep(INTERVAL);
                }
                if (isRunning)
                {
                    ComputerHandImage.Value = ImageSource.FromResource("JankenApp.Images.janken_choki.png");
                    ComputerHand.Value = HAND_CHOKI;
                    Thread.Sleep(INTERVAL);
                }
                if (isRunning)
                {
                    ComputerHandImage.Value = ImageSource.FromResource("JankenApp.Images.janken_pa.png");
                    ComputerHand.Value = HAND_PA;
                    Thread.Sleep(INTERVAL);
                }
            }
        }

        /// <summary>
        /// じゃんけんの結果を判定・表示する
        /// </summary>
        /// <param name="yourHand"></param>
        private void Judge(int yourHand)
        {
            int result = (yourHand - ComputerHand.Value + 3) % 3;
            switch (result)
            {
                case 0:
                    Result.Value = "引き分け";
                    DrawCount.Value = (int.Parse(DrawCount.Value) + 1).ToString();
                    ResultColor.Value = Color.Green;
                    break;
                case 1:
                    Result.Value = "あなたの負け！";
                    LoseCount.Value = (int.Parse(LoseCount.Value) + 1).ToString();
                    ResultColor.Value = Color.Blue;
                    break;
                case 2:
                    Result.Value = "あなたの勝ち！";
                    WinCount.Value = (int.Parse(WinCount.Value) + 1).ToString();
                    ResultColor.Value = Color.Red;
                    break;
            }
        }
    }
}
