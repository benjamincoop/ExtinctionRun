

namespace ExtinctionRun.Screens
{
    // The options screen is brought up over the top of the main menu
    // screen, and gives the user a chance to configure the game
    // in various hopefully useful ways.
    public class OptionsMenuScreen : MenuScreen
    {

        private readonly MenuEntry _musicVolEntry;
        private readonly MenuEntry _SFXVolEntry;

        private static int _music = 50;
        private static int _sfx = 50;

        public OptionsMenuScreen() : base("Options")
        {
            _musicVolEntry = new MenuEntry(string.Empty);
            _SFXVolEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            var back = new MenuEntry("Back");

            _musicVolEntry.Selected += MusicEntrySelected;
            _SFXVolEntry.Selected += SFXEntrySelected;
            back.Selected += OnCancel;

            MenuEntries.Add(_musicVolEntry);
            MenuEntries.Add(_SFXVolEntry);

            MenuEntries.Add(back);
        }

        // Fills in the latest values for the options screen menu text.
        private void SetMenuEntryText()
        {
            _musicVolEntry.Text = $"Music Volume: {_music.ToString()}";
            _SFXVolEntry.Text = $"Music Volume: {_sfx.ToString()}";
        }

        private void MusicEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            _music += 5;

            if (_music > 100)
                _music = 0;

            SetMenuEntryText();
        }
        private void SFXEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            _sfx += 5;

            if (_sfx > 100)
                _sfx = 0;

            SetMenuEntryText();
        }
    }
}
