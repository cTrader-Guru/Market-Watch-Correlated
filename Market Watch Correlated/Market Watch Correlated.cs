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

    // --> Estensioni che rendono il codice più leggibile
    #region Extensions

    /// <summary>
    /// Estensione che fornisce metodi aggiuntivi per il simbolo
    /// </summary>
    public static class SymbolExtensions
    {

        /// <summary>
        /// Converte il numero di pips corrente da digits a double
        /// </summary>
        /// <param name="Pips">Il numero di pips nel formato Digits</param>
        /// <returns></returns>
        public static double DigitsToPips(this Symbol MySymbol, double Pips)
        {

            return Math.Round(Pips / MySymbol.PipSize, 2);

        }

        /// <summary>
        /// Converte il numero di pips corrente da double a digits
        /// </summary>
        /// <param name="Pips">Il numero di pips nel formato Double (2)</param>
        /// <returns></returns>
        public static double PipsToDigits(this Symbol MySymbol, double Pips)
        {

            return Math.Round(Pips * MySymbol.PipSize, MySymbol.Digits);

        }

    }

    /// <summary>
    /// Estensione che fornisce metodi aggiuntivi per le Bars
    /// </summary>
    public static class BarsExtensions
    {

        /// <summary>
        /// Converte l'indice di una bar partendo dalla data di apertura
        /// </summary>
        /// <param name="MyTime">La data e l'ora di apertura della candela</param>
        /// <returns></returns>
        public static int GetIndexByDate(this Bars MyBars, DateTime MyTime)
        {

            for (int i = MyBars.ClosePrices.Count - 1; i >= 0; i--)
            {

                if (MyTime == MyBars.OpenTimes[i]) return i;

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
        
        /// <summary>
        /// Nome del prodotto, identificativo, da modificare con il nome della propria creazione
        /// </summary>
        public const string NAME = "Market Watch Correlated";

        /// <summary>
        /// La versione del prodotto, progressivo, utilie per controllare gli aggiornamenti se viene reso disponibile sul sito ctrader.guru
        /// </summary>
        public const string VERSION = "1.0.1";

        #endregion

        #region Params

        /// <summary>
        /// Identità del prodotto nel contesto di ctrader.guru
        /// </summary>
        [Parameter(NAME + " " + VERSION, Group = "Identity", DefaultValue = "https://ctrader.guru/product/market-watch-correlated/")]
        public string ProductInfo { get; set; }

        [Parameter("Symbol", Group = "Params", DefaultValue = "EURUSD")]
        public string MySymbol { get; set; }

        [Parameter("EMA Period", Group = "Params", DefaultValue = 500)]
        public int MyEMAPeriod { get; set; }

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

        /// <summary>
        /// Viene generato all'avvio dell'indicatore, si inizializza l'indicatore
        /// </summary>
        protected override void Initialize()
        {

            // --> Stampo nei log la versione corrente
            Print("{0} : {1}", NAME, VERSION);

            // --> Alcuni controlli di base
            MySymbol = MySymbol.Trim().ToUpper();
            CanDraw = (RunningMode == RunningMode.RealTime || RunningMode == RunningMode.VisualBacktesting);

            // --> L'utente ha inserito un cross valido ?
            if (!Symbols.Exists(MySymbol))
            {

                if (CanDraw) Chart.DrawStaticText("Error", NAME + " : PLEASE SET VALID CROSS LIKE 'EURUSD'", VerticalAlignment.Center, HorizontalAlignment.Center, Color.Red);

                ItsOk = false;

            }

        }

        /// <summary>
        /// Generato ad ogni tick, vengono effettuati i calcoli dell'indicatore
        /// </summary>
        /// <param name="index">L'indice della candela in elaborazione</param>
        public override void Calculate(int index)
        {

            // --> Si esce se non ci sono le condizioni per continuare
            if (!ItsOk) return;


            Symbol CROSS = Symbols.GetSymbol(MySymbol);
            Bars CROSS_Bars = MarketData.GetBars(TimeFrame, CROSS.Name);

            // --> Potrei avere un indice diverso perchè non carico le stesse barre
            int CROSS_Index = CROSS_Bars.GetIndexByDate(Bars.OpenTimes[index]);
            if (CROSS_Index < 0) return;

            ExponentialMovingAverage CROSS_ema = Indicators.ExponentialMovingAverage(CROSS_Bars.ClosePrices, MyEMAPeriod);
            ExponentialMovingAverage Current_CROSS_ema = Indicators.ExponentialMovingAverage(Bars.ClosePrices, MyEMAPeriod);

            double CROSSpips = 0;

            // --> Devo uniformare il numero di pips, i digits saranno di sicuro diversi
            CROSSpips = CROSS.DigitsToPips(Math.Round(CROSS_Bars.ClosePrices[CROSS_Index] - CROSS_ema.Result[CROSS_Index], CROSS.Digits));
            Result[index] = Math.Round(Current_CROSS_ema.Result[index] + Symbol.PipsToDigits(CROSSpips), Symbol.Digits);

            if (!ShowLabel)
                return;

            string CROSStext = string.Format("  ‹ {0} {1:0.00} ( {2:0.00000} )", CROSS.Name, CROSSpips, CROSS_Bars.ClosePrices[CROSS_Index]);

            if (CanDraw) Chart.DrawText(CROSS.Name, CROSStext, index, Result[index], Color.FromName( LabelColor.ToString() ) );

        }

        #endregion

        #region Private Methods



        #endregion

    }

}
