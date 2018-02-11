using System;

namespace LoLa.Compiler
{
    public sealed class Label
    {
        private int location;

        public event EventHandler LocationChanged;

        public int Location
        {
            get { return this.location; }
            set
            {
                if (this.location == value)
                    return;
                this.location = value;
                this.LocationChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}