﻿using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public struct Pos
    {
        public int Y;
        public int X;

        public Pos(int y, int x)
        {
            Y = y;
            X = x;
        }
    }

    public struct PQNode : IComparable<PQNode>
    {
        public int F;
        public int G;
        public int Y;
        public int X;

        public int CompareTo(PQNode other)
        {
            if (F == other.F)
                return 0;
            return F < other.F ? 1 : -1;
        }
    }

    public struct Vector2Int
    {
        public int x;
        public int y;

        public Vector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2Int up { get { return new Vector2Int(0, 1); } }
        public static Vector2Int down { get { return new Vector2Int(0, -1); } }
        public static Vector2Int left { get { return new Vector2Int(-1, 0); } }
        public static Vector2Int right { get { return new Vector2Int(1, 0); } }

        public static Vector2Int operator +(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x + b.x, a.y + b.y);
        }
    }

    public class Map
    {
        public int MinX { get; set; }
        public int MaxX { get; set; }
        public int MinY { get; set; }
        public int MaxY { get; set; }

        public int SizeX { get { return MaxX - MinX + 1; } }
        public int SizeY { get { return MaxY - MinY + 1; } }

        bool[,] collisionDatas;
        Player[,] players;

        public bool CanGo(Vector2Int cellPos, bool checkObjects = true)
        {
            if (cellPos.x < MinX || cellPos.x > MaxX)
                return false;
            if (cellPos.y < MinY || cellPos.y > MaxY)
                return false;

            int x = cellPos.x - MinX;
            int y = MaxY - cellPos.y;
            return !collisionDatas[y, x] && (!checkObjects || players[y, x] == null);
        }

        public Player Find(Vector2Int cellPos)
        {
            if (cellPos.x < MinX || cellPos.x > MaxX)
                return null;
            if (cellPos.y < MinY || cellPos.y > MaxY)
                return null;

            int x = cellPos.x - MinX;
            int y = MaxY - cellPos.y;
            return players[y, x];
        }

        public bool ApplyMove(Player player, Vector2Int dest)
        {
            PositionInfo posInfo = player.Info.PosInfo;
            if (posInfo.PosX < MinX || posInfo.PosX > MaxX)
                return false;
            if (posInfo.PosY < MinY || posInfo.PosY > MaxY)
                return false;
            if (CanGo(dest, true) == false)
                return false;

            {
                int x = posInfo.PosX - MinX;
                int y = MaxY - posInfo.PosY;
                if (players[y, x] == player)
                    players[y, x] = null;
            }
            {
                int x = dest.x - MinX;
                int y = MaxY - dest.y;
                players[y, x] = player;
            }

            posInfo.PosX = dest.x;
            posInfo.PosY = dest.y;

            return true;
        }

        public void LoadMap(int mapId, string pathPrefix = "../../../../../Common/MapData")
        {
            string mapName = $"Map_{mapId:000}";

            string text = File.ReadAllText($"{pathPrefix}/{mapName}.txt");
            StringReader reader = new StringReader(text);

            MinX = int.Parse(reader.ReadLine());
            MaxX = int.Parse(reader.ReadLine());
            MinY = int.Parse(reader.ReadLine());
            MaxY = int.Parse(reader.ReadLine());

            int xCount = MaxX - MinX + 1;
            int yCount = MaxY - MinY + 1;
            collisionDatas = new bool[yCount, xCount];
            players = new Player[yCount, xCount];

            for (int y = 0; y < yCount; ++y)
            {
                string line = reader.ReadLine();
                for (int x = 0; x < xCount; ++x)
                {
                    collisionDatas[y, x] = (line[x] == '1' ? true : false);
                }
            }
        }

        #region A* PathFinding

        // U D L R
        int[] deltaY = new int[] { 1, -1, 0, 0 };
        int[] deltaX = new int[] { 0, 0, -1, 1 };
        int[] cost = new int[] { 10, 10, 10, 10 };

        public List<Vector2Int> FindPath(Vector2Int startCellPos, Vector2Int destCellPos, bool ignoreDestCollision = false)
        {
            List<Pos> path = new List<Pos>();

            // F = G + H
            // F = 최종 점수 (작을수록 좋음, 경로에 따라 달라짐)
            // G = 시작점에서 해당 좌표까지 이동하는데 드는 비용 (작을수록 좋음, 경로에 따라 달라짐)
            // H = 목적지에서 얼마나 가까운지 (작을수록 좋음, 고정)

            bool[,] closed = new bool[SizeY, SizeX];

            int[,] open = new int[SizeY, SizeX];
            for (int y = 0; y < SizeY; ++y)
                for (int x = 0; x < SizeX; ++x)
                    open[y, x] = Int32.MaxValue;

            Pos[,] parent = new Pos[SizeY, SizeX];

            PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

            Pos pos = Cell2Pos(startCellPos);
            Pos dest = Cell2Pos(destCellPos);

            open[pos.Y, pos.X] = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X));
            pq.Push(new PQNode() { F = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X)), G = 0, Y = pos.Y, X = pos.X });
            parent[pos.Y, pos.X] = new Pos(pos.Y, pos.X);

            while (pq.Count > 0)
            {
                PQNode node = pq.Pop();
                if (closed[node.Y, node.X])
                    continue;

                closed[node.Y, node.X] = true;

                if (node.Y == dest.Y && node.X == dest.X)
                    break;

                for (int i = 0; i < deltaY.Length; ++i)
                {
                    Pos next = new Pos(node.Y + deltaY[i], node.X + deltaX[i]);

                    if (!ignoreDestCollision || next.Y != dest.Y || next.X != dest.X)
                    {
                        if (CanGo(Pos2Cell(next)) == false)
                            continue;
                    }

                    if (closed[next.Y, next.X])
                    {
                        continue;
                    }

                    int g = 0;// node.G + cost[i];
                    int h = 10 * ((dest.Y - next.Y) * (dest.Y - next.Y) + (dest.X - next.X) * (dest.X - next.X));
                    if (open[next.Y, next.X] < g + h)
                        continue;

                    open[next.Y, next.X] = g + h;
                    pq.Push(new PQNode() { F = g + h, G = g, Y = next.Y, X = next.X });
                    parent[next.Y, next.X] = new Pos(node.Y, node.X);
                }
            }

            return CalcPathFromParent(parent, dest);
        }

        List<Vector2Int> CalcPathFromParent(Pos[,] parent, Pos dest)
        {
            List<Vector2Int> cells = new List<Vector2Int>();

            int y = dest.Y;
            int x = dest.X;
            while (parent[y, x].Y != y || parent[y, x].X != x)
            {
                cells.Add(Pos2Cell(new Pos(y, x)));
                Pos pos = parent[y, x];
                y = pos.Y;
                x = pos.X;
            }
            cells.Add(Pos2Cell(new Pos(y, x)));
            cells.Reverse();

            return cells;
        }

        Pos Cell2Pos(Vector2Int cell)
        {
            return new Pos(MaxY - cell.y, cell.x - MinX);
        }

        Vector2Int Pos2Cell(Pos pos)
        {
            return new Vector2Int(pos.X + MinX, MaxY - pos.Y);
        }

        #endregion
    }

}
