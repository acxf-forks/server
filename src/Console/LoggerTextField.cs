﻿using System;

namespace GameServer.Logging
{
    public class LoggerTextField
    {
        public string m_Input = "";
        public int m_Column;
        public int m_Row;

        /// <summary>
        /// Redraw the text field
        /// </summary>
        public void Redraw()
        {
            var prevCursorLeft = Console.CursorLeft;
            Clear(false);

            Console.WriteLine(m_Input);
            Console.CursorLeft = prevCursorLeft;
        }

        /// <summary>
        /// Move the text field down
        /// </summary>
        public void MoveDown()
        {
            var prevCursorLeft = Console.CursorLeft;
            Clear(false);
            Console.CursorTop++;

            Console.WriteLine(m_Input);

            Console.CursorLeft = prevCursorLeft;
        }

        /// <summary>
        /// Clear the text field display and optionally clear the input
        /// </summary>
        public void Clear(bool clearInput)
        {
            if (clearInput)
                m_Input = "";

            Console.CursorLeft = 0;
            Console.Write(new string(' ', Console.WindowWidth));
            Console.CursorTop--;
        }
    }
}
