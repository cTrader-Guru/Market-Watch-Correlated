/*  CTRADER GURU --> Indicator Template 1.0.8

    Homepage    : https://ctrader.guru/
    Telegram    : https://t.me/ctraderguru
    Twitter     : https://twitter.com/cTraderGURU/
    Facebook    : https://www.facebook.com/ctrader.guru/
    YouTube     : https://www.youtube.com/channel/UCKkgbw09Fifj65W5t5lHeCQ
    GitHub      : https://github.com/ctrader-guru

*/

using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;

namespace cAlgo
{


    #region Extensions

    public static class SymbolExtensions
    {

        public static double DigitsToPips(this Symbol MySymbol, double Pips)
        {

            return Math.Round(Pips / MySymbol.PipSize, 2);

        }

        public static double PipsToDigits(this Symbol MySymbol, double Pips)
        {

            return Math.Round(Pips * MySymbol.PipSize, MySymbol.Digits);

        }

    }

    public static class BarsExtensions
    {

        public static int GetIndexByDate(this Bars MyBars, DateTime MyTime)
        {

            for (int i = MyBars.ClosePrices.Count - 1; i >= 0; i--)
            {

                if (MyTime == MyBars.OpenTimes[i])
                    return i;

            }

            return -1;

        }

    }

    #endregion

    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class MarketWatchCorrelated : Indicator
    {

        #region Enums

        public enum MyColors
        {

            AliceBlue,
            AntiqueWhite,
            Aqua,
            Aquamarine,
            Azure,
            Beige,
            Bisque,
            Black,
            BlanchedAlmond,
            Blue,
            BlueViolet,
            Brown,
            BurlyWood,
            CadetBlue,
            Chartreuse,
            Chocolate,
            Coral,
            CornflowerBlue,
            Cornsilk,
            Crimson,
            Cyan,
            DarkBlue,
            DarkCyan,
            DarkGoldenrod,
            DarkGray,
            DarkGreen,
            DarkKhaki,
            DarkMagenta,
            DarkOliveGreen,
            DarkOrange,
            DarkOrchid,
            DarkRed,
            DarkSalmon,
            DarkSeaGreen,
            DarkSlateBlue,
            DarkSlateGray,
            DarkTurquoise,
            DarkViolet,
            DeepPink,
            DeepSkyBlue,
            DimGray,
            DodgerBlue,
            Firebrick,
            FloralWhite,
            ForestGreen,
            Fuchsia,
            Gainsboro,
            GhostWhite,
            Gold,
            Goldenrod,
            Gray,
            Green,
            GreenYellow,
            Honeydew,
            HotPink,
            IndianRed,
            Indigo,
            Ivory,
            Khaki,
            Lavender,
            LavenderBlush,
            LawnGreen,
            LemonChiffon,
            LightBlue,
            LightCoral,
            LightCyan,
            LightGoldenrodYellow,
            LightGray,
            LightGreen,
            LightPink,
            LightSalmon,
            LightSeaGreen,
            LightSkyBlue,
            LightSlateGray,
            LightSteelBlue,
            LightYellow,
            Lime,
            LimeGreen,
            Linen,
            Magenta,
            Maroon,
            MediumAquamarine,
            MediumBlue,
            MediumOrchid,
            MediumPurple,
            MediumSeaGreen,
            MediumSlateBlue,
            MediumSpringGreen,
            MediumTurquoise,
            MediumVioletRed,
            MidnightBlue,
            MintCream,
            MistyRose,
            Moccasin,
            NavajoWhite,
            Navy,
            OldLace,
            Olive,
            OliveDrab,
            Orange,
            OrangeRed,
            Orchid,
            PaleGoldenrod,
            PaleGreen,
            PaleTurquoise,
            PaleVioletRed,
            PapayaWhip,
            PeachPuff,
            Peru,
            Pink,
            Plum,
            PowderBlue,
            Purple,
            Red,
            RosyBrown,
            RoyalBlue,
            SaddleBrown,
            Salmon,
            SandyBrown,
            SeaGreen,
            SeaShell,
            Sienna,
            Silver,
            SkyBlue,
            SlateBlue,
            SlateGray,
            Snow,
            SpringGreen,
            SteelBlue,
            Tan,
            Teal,
            Thistle,
            Tomato,
            Transparent,
            Turquoise,
            Violet,
            Wheat,
            White,
            WhiteSmoke,
            Yellow,
            YellowGreen

        }

        #endregion

        #region Identity

        public const string NAME = "Market Watch Correlated";

        public const string VERSION = "1.0.2";

        #endregion

        #region Params

        [Parameter(NAME + " " + VERSION, Group = "Identity", DefaultValue = "https://www.google.com/search?q=ctrader+guru+market+watch+correlated")]
        public string ProductInfo { get; set; }

        [Parameter("Correlated", Group = "Symbol", DefaultValue = "EURUSD")]
        public string CorrelatedSymbol { get; set; }

        [Parameter("Period", Group = "EMA", DefaultValue = 500)]
        public int EMAPeriod { get; set; }

        [Parameter("Show Labels ?", Group = "Styles", DefaultValue = true)]
        public bool ShowLabel { get; set; }

        [Parameter("Label Color", Group = "Styles", DefaultValue = MyColors.Gray)]
        public MyColors LabelColor { get; set; }

        [Output("Symbol", LineColor = "Gray")]
        public IndicatorDataSeries Result { get; set; }

        #endregion

        #region Property

        bool CanDraw;
        bool ItsOk = true;

        #endregion

        #region Indicator Events

        protected override void Initialize()
        {

            Print("{0} : {1}", NAME, VERSION);

            CorrelatedSymbol = CorrelatedSymbol.Trim().ToUpper();
            CanDraw = (RunningMode == RunningMode.RealTime || RunningMode == RunningMode.VisualBacktesting);

            if (!Symbols.Exists(CorrelatedSymbol))
            {

                if (CanDraw)
                    Chart.DrawStaticText("Error", NAME + " : PLEASE SET VALID CROSS LIKE 'EURUSD'", VerticalAlignment.Center, HorizontalAlignment.Center, Color.Red);

                ItsOk = false;

            }

        }

        public override void Calculate(int index)
        {

            if (!ItsOk)
                return;


            Symbol CROSS = Symbols.GetSymbol(CorrelatedSymbol);
            Bars CROSS_Bars = MarketData.GetBars(TimeFrame, CROSS.Name);

            int CROSS_Index = CROSS_Bars.GetIndexByDate(Bars.OpenTimes[index]);
            if (CROSS_Index < 0)
                return;

            ExponentialMovingAverage CROSS_ema = Indicators.ExponentialMovingAverage(CROSS_Bars.ClosePrices, EMAPeriod);
            ExponentialMovingAverage Current_CROSS_ema = Indicators.ExponentialMovingAverage(Bars.ClosePrices, EMAPeriod);

            double CROSSpips = CROSS.DigitsToPips(Math.Round(CROSS_Bars.ClosePrices[CROSS_Index] - CROSS_ema.Result[CROSS_Index], CROSS.Digits));
            Result[index] = Math.Round(Current_CROSS_ema.Result[index] + Symbol.PipsToDigits(CROSSpips), Symbol.Digits);

            if (!ShowLabel)
                return;

            string CROSStext = string.Format("  ‹ {0} {1:0.00} ( {2:0.00000} )", CROSS.Name, CROSSpips, CROSS_Bars.ClosePrices[CROSS_Index]);

            if (CanDraw)
            {

                ChartText ThisLabel = Chart.DrawText(CROSS.Name, CROSStext, index, Result[index], Color.FromName(LabelColor.ToString()));
                ThisLabel.VerticalAlignment = VerticalAlignment.Center;

            }
        }

        #endregion

    }

}
