using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PacmanGame.Utils
{
    abstract class GameData
    {
        /// <summary>
        /// Pacman and ghost speed table
        /// </summary>
        static float[,] tableSpeed = new float[,]
        {   /***************PACMAN****************|************GHOST*********/
            /*Round 1 */{0.80f, 0.71f, 0.90f, 0.79f, 0.75f, 0.50f, 0.40f, 0.40f, 0.80f, 0.85f},
            /*Round 2 */{0.90f, 0.79f, 0.95f, 0.83f, 0.85f, 0.55f, 0.45f, 0.40f, 0.90f, 0.95f},
            /*Round 3 */{0.90f, 0.79f, 0.95f, 0.83f, 0.85f, 0.55f, 0.45f, 0.40f, 0.90f, 0.95f},
            /*Round 4 */{0.90f, 0.79f, 0.95f, 0.83f, 0.85f, 0.55f, 0.45f, 0.40f, 0.90f, 0.95f},
            /*Round 5 */{1.00f, 0.87f, 1.00f, 0.87f, 0.95f, 0.60f, 0.50f, 0.40f, 1.00f, 1.05f},
            /*Round 6 */{1.00f, 0.87f, 1.00f, 0.87f, 0.95f, 0.60f, 0.50f, 0.40f, 1.00f, 1.05f},
            /*Round 7 */{1.00f, 0.87f, 1.00f, 0.87f, 0.95f, 0.60f, 0.50f, 0.40f, 1.00f, 1.05f},
            /*Round 8 */{1.00f, 0.87f, 1.00f, 0.87f, 0.95f, 0.60f, 0.50f, 0.40f, 1.00f, 1.05f},
            /*Round 9 */{1.00f, 0.87f, 1.00f, 0.87f, 0.95f, 0.60f, 0.50f, 0.40f, 1.00f, 1.05f},
            /*Round 10*/{1.00f, 0.87f, 1.00f, 0.87f, 0.95f, 0.60f, 0.50f, 0.40f, 1.00f, 1.05f},
            /*Round 11*/{1.00f, 0.87f, 1.00f, 0.87f, 0.95f, 0.60f, 0.50f, 0.40f, 1.00f, 1.05f},
            /*Round 12*/{1.00f, 0.87f, 1.00f, 0.87f, 0.95f, 0.60f, 0.50f, 0.40f, 1.00f, 1.05f},
            /*Round 13*/{1.00f, 0.87f, 1.00f, 0.87f, 0.95f, 0.60f, 0.50f, 0.40f, 1.00f, 1.05f},
            /*Round 14*/{1.00f, 0.87f, 1.00f, 0.87f, 0.95f, 0.60f, 0.50f, 0.40f, 1.00f, 1.05f},
            /*Round 15*/{1.00f, 0.87f, 1.00f, 0.87f, 0.95f, 0.60f, 0.50f, 0.40f, 1.00f, 1.05f},
            /*Round 16*/{1.00f, 0.87f, 1.00f, 0.87f, 0.95f, 0.60f, 0.50f, 0.40f, 1.00f, 1.05f},
            /*Round 17*/{1.00f, 0.87f, 1.00f, 0.87f, 0.95f, 0.60f, 0.50f, 0.40f, 1.00f, 1.05f},
            /*Round 18*/{1.00f, 0.87f, 1.00f, 0.87f, 0.95f, 0.60f, 0.50f, 0.40f, 1.00f, 1.05f},
            /*Round 19*/{1.00f, 0.87f, 1.00f, 0.87f, 0.95f, 0.60f, 0.50f, 0.40f, 1.00f, 1.05f},
            /*Round 20*/{1.00f, 0.87f, 1.00f, 0.87f, 0.95f, 0.60f, 0.50f, 0.40f, 1.00f, 1.05f},
            /*Round 21*/{0.90f, 0.79f, 1.00f, 0.87f, 0.95f, 0.60f, 0.50f, 0.40f, 1.00f, 1.05f},
            /*...*/
        };

        public enum HeaderSpeed {PAC_NORM, PAC_NORM_DOTS, PAC_FRIGHT, PAC_FRIGHT_DOTS, GHOST_NORM, GHOST_FRIGHT, GHOST_TUNNEL, GHOST_PEN, ELROY_1, ELROY_2};

        public static float getSpeed(int round, HeaderSpeed header)
        {
            round = Math.Max(round - 1, 0);
            round = Math.Min(round, 20);

            return tableSpeed[round, (int)header];
        }

        /// <summary>
        /// Time in second for each scatter and chase of each round
        /// </summary>
        static float[,] tableMode = new float[,]
        {   
            /*Round 1 */ {7,      20,     7,      20,     5,      20,         5,          -1, 6},
            /*Round 2 */ {7,      20,     7,      20,     5,      1033,       1/60f,      -1, 5},
            /*Round 3 */ {7,      20,     7,      20,     5,      1033,       1/60f,      -1, 4},
            /*Round 4 */ {7,      20,     7,      20,     5,      1033,       1/60f,      -1, 3},
            /*Round 5 */ {5,      20,     5,      20,     5,      1037,       1/60f,      -1, 2},
            /*Round 6 */ {5,      20,     5,      20,     5,      1037,       1/60f,      -1, 5},
            /*Round 7 */ {5,      20,     5,      20,     5,      1037,       1/60f,      -1, 2},
            /*Round 8 */ {5,      20,     5,      20,     5,      1037,       1/60f,      -1, 2},
            /*Round 9 */ {5,      20,     5,      20,     5,      1037,       1/60f,      -1, 1},
            /*Round 10*/ {5,      20,     5,      20,     5,      1037,       1/60f,      -1, 5},
            /*Round 11*/ {5,      20,     5,      20,     5,      1037,       1/60f,      -1, 2},
            /*Round 12*/ {5,      20,     5,      20,     5,      1037,       1/60f,      -1, 1},
            /*Round 13*/ {5,      20,     5,      20,     5,      1037,       1/60f,      -1, 1},
            /*Round 14*/ {5,      20,     5,      20,     5,      1037,       1/60f,      -1, 3},
            /*Round 15*/ {5,      20,     5,      20,     5,      1037,       1/60f,      -1, 1},
            /*Round 16*/ {5,      20,     5,      20,     5,      1037,       1/60f,      -1, 1},
            /*Round 17*/ {5,      20,     5,      20,     5,      1037,       1/60f,      -1, 1/60f},
            /*Round 18*/ {5,      20,     5,      20,     5,      1037,       1/60f,      -1, 1/60f},
            /*Round 19*/ {5,      20,     5,      20,     5,      1037,       1/60f,      -1, 1/60f},
            /*Round 20*/ {5,      20,     5,      20,     5,      1037,       1/60f,      -1, 1/60f},
            /*Round 21*/ {5,      20,     5,      20,     5,      1037,       1/60f,      -1, 1/60f},
        };

        public static int getModeTime(int round, int index)
        {
            round = Math.Max(round - 1, 0);
            round = Math.Min(round, 20);

            index = Math.Min(tableMode.GetUpperBound(1), index);

            return (int)(tableMode[round, index] * 1000);
        }

        /// <summary>
        /// Points for each bonus per round and the corresponding tile id
        /// </summary>
        static int[,] tableBonus = new int[,]
        {
            /*Round 1 */ {100, 23},
            /*Round 2 */ {300, 24},
            /*Round 3 */ {500, 25},
            /*Round 4 */ {500, 25},
            /*Round 5 */ {700, 26},
            /*Round 6 */ {700, 26},
            /*Round 7 */ {1000, 27},
            /*Round 8 */ {1000, 27},
            /*Round 9 */ {2000, 28},
            /*Round 10*/ {2000, 28},
            /*Round 11*/ {3000, 29},
            /*Round 12*/ {3000, 29},
            /*Round 13*/ {5000, 30},
            /*Round 14*/ {5000, 30},
            /*Round 15*/ {5000, 30},
            /*Round 16*/ {5000, 30},
            /*Round 17*/ {5000, 30},
            /*Round 18*/ {5000, 30},
            /*Round 19*/ {5000, 30},
            /*Round 20*/ {5000, 30},
            /*Round 21*/ {5000, 30}
        };

        public static int getBonusPoint(int round)
        {
            round = Math.Max(round - 1, 0);
            round = Math.Min(round, 20);

            return tableBonus[round, 0];
        }

        public static int getBonusTileId(int round)
        {
            round = Math.Max(round - 1, 0);
            round = Math.Min(round, 20);

            return tableBonus[round, 1];
        }

        /// <summary>
        /// Dots left percent necassary for Blinky to turn to Elroy
        /// </summary>
        static int[,] tableElroy = new int[,]
        {
            /*Round 1 */ {8, 4},
            /*Round 2 */ {12, 6},
            /*Round 3 */ {16, 8},
            /*Round 4 */ {16, 8},
            /*Round 5 */ {16, 8},
            /*Round 6 */ {20, 10},
            /*Round 7 */ {20, 10},
            /*Round 8 */ {20, 10},
            /*Round 9 */ {24, 12},
            /*Round 10*/ {24, 12},
            /*Round 11*/ {24, 12},
            /*Round 12*/ {32, 16},
            /*Round 13*/ {32, 16},
            /*Round 14*/ {32, 16},
            /*Round 15*/ {40, 20},
            /*Round 16*/ {40, 20},
            /*Round 17*/ {40, 20},
            /*Round 18*/ {40, 20},
            /*Round 19*/ {49, 24},
            /*Round 20*/ {49, 24},
            /*Round 21*/ {49, 24}
        };

        public enum ElroyHeader {ELROY_1, ELROY_2};

        public static int getElroyPercent(int round, ElroyHeader header)
        {
            round = Math.Max(round - 1, 0);
            round = Math.Min(round, 20);

            return tableElroy[round, (int)header];
        }
    }
}
