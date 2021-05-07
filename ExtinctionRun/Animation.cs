using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ExtinctionRun
{
    public class Animation
    {
        int _index = 0; // the current index into the frames array
        int _delay; // the number of game frames per animation frame, a value of 0 stops the animation
        int _clock = 0; // keeps track of elapsed frames
        Texture2D[] _frames; // the textures of the animation

        public int Delay { 
            get { return _delay; } 
            set { _delay = value; }
        }

        public bool IsFinished { get; set; } = false; // is true if animation has looped at least once

        public Animation(ContentManager content, string[] filenames, int delay)
        {
            _frames = new Texture2D[filenames.Length];
            for (int i = 0; i < filenames.Length; i++)
            {
                _frames[i] = content.Load<Texture2D>(filenames[i]);
            }
            _delay = delay;
        }

        /// <summary>
        /// Tracks the state of the animation and returns the texture of the next frame
        /// </summary>
        /// <returns></returns>
        public Texture2D Animate()
        {
            if(_delay > 0)
            {
                if (_clock >= _delay)
                {
                    _index++;
                    _clock = 0;

                    if (_index > _frames.Length - 1) { 
                        _index = 0;
                        IsFinished = true;
                    }
                }

                _clock++;
            }
            return _frames[_index];
        }

        /// <summary>
        /// Restarts the animation loop
        /// </summary>
        public void Reset()
        {
            _index = 0;
            _clock = 0;
        }
    }
}
