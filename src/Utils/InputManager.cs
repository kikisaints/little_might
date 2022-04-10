using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Little_Might.Utils
{
    internal class InputManager
    {
        private bool buttonPressed = false;
        private Keys _lastKeyDown = Keys.None;

        private double _timer = 0;
        private double _moveUnlock = 0.15;

        public double MoveSpeed
        {
            get { return _moveUnlock; }
            set { _moveUnlock = value; }
        }

        public bool ButtonToggled(string keyName)
        {
            return false;
        }

        public bool ButtonToggled(Keys keyName)
        {
            if (Keyboard.GetState().IsKeyDown(keyName) && !buttonPressed)
            {
                buttonPressed = true;
                _lastKeyDown = keyName;
                return true;
            }

            return false;
        }

        public void UpdateInput(GameTime time)
        {
            if (buttonPressed)
            {
                _timer += time.ElapsedGameTime.TotalSeconds;
            }

            if (_timer >= _moveUnlock)
            {
                buttonPressed = false;
                _timer = 0;
            }
        }
    }
}
