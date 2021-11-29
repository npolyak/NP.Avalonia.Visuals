using Avalonia.Media;
using NP.Utilities;

namespace NP.ThemingPrototype
{
    public class ColorContainer : VMBase
    {
        #region TheColor Property
        private Color _color;
        public Color TheColor
        {
            get
            {
                return this._color;
            }
            set
            {
                if (this._color == value)
                {
                    return;
                }

                this._color = value;
                this.OnPropertyChanged(nameof(TheColor));
            }
        }
        #endregion TheColor Property
    }
}
