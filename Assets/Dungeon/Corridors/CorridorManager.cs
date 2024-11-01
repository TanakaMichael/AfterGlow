using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorManager : MonoBehaviour
{
    [Header("Corridor Settings")]
    public int corridorWidth = 1; // 廊下の幅
    [Range(0f, 1f)]
    public float extraConnectionProbability = 0.1f; // 追加接続の確率
    [Range(1, 3)]
    public int maxBendCount = 2; // 廊下が曲がる最大回数

    [Header("Connection Constraints")]
    public float maxConnectionDistance = 20f; // 最大接続距離

    public List<Room> rooms; // 接続する部屋のリスト
    public List<Corridor> corridors = new List<Corridor>(); // 生成された廊下のリスト

    public void GenerateCorridors(List<Room> rooms)
    {
        this.rooms = rooms;
        if (rooms == null || rooms.Count == 0)
        {
            Debug.LogError("No rooms available for corridor generation.");
            return;
        }

        // 部屋間のエッジを計算し、最大距離を超えるエッジを除外
        List<Edge> edges = CalculateEdges(rooms);

        // 最大接続距離を超えるエッジを除外
        edges = edges.Where(edge => edge.weight <= maxConnectionDistance).ToList();

        // 有効なエッジがない場合はエラーを出力
        if (edges.Count == 0)
        {
            Debug.LogError("No valid edges within the maximum connection distance.");
            return;
        }

        // 最小全域木を構築
        List<Edge> mst = KruskalMST(edges, rooms.Count);

        // 最小全域木に基づいて廊下を生成
        foreach (Edge edge in mst)
        {
            // SecretRoom への接続は秘密の通路で行う
            if (edge.roomA.roomType == RoomType.Secret || edge.roomB.roomType == RoomType.Secret)
            {
                CreateCorridor(edge.roomA, edge.roomB, isSecret: true);
            }
            else
            {
                CreateCorridor(edge.roomA, edge.roomB);
            }
        }

        // 追加のエッジをランダムに選択して廊下を生成
        foreach (Edge edge in edges)
        {
            if (!mst.Contains(edge) && Random.value < extraConnectionProbability)
            {
                // SecretRoom への接続は秘密の通路で行う
                if (edge.roomA.roomType == RoomType.Secret || edge.roomB.roomType == RoomType.Secret)
                {
                    CreateCorridor(edge.roomA, edge.roomB, isSecret: true);
                }
                else
                {
                    CreateCorridor(edge.roomA, edge.roomB);
                }
            }
        }

        // Entrance と Exit 部屋の接続を確認・強制接続
        EnsureEntranceExitConnection();
    }

    // エッジの計算
    private List<Edge> CalculateEdges(List<Room> rooms)
    {
        List<Edge> edges = new List<Edge>();

        for (int i = 0; i < rooms.Count; i++)
        {
            for (int j = i + 1; j < rooms.Count; j++)
            {
                Edge edge = new Edge(rooms[i], rooms[j]);

                // 最大接続距離を超える場合はエッジを追加しない
                if (edge.weight <= maxConnectionDistance)
                {
                    edges.Add(edge);
                }
                else
                {
                    // Entrance と Exit、または SecretRoom への接続は最大距離を無視
                    if ((rooms[i].roomType == RoomType.Entrance && rooms[j].roomType == RoomType.Exit) ||
                        (rooms[i].roomType == RoomType.Secret || rooms[j].roomType == RoomType.Secret))
                    {
                        edges.Add(edge);
                    }
                }
            }
        }
        return edges;
    }

    // クラスカル法による MST の構築
    private List<Edge> KruskalMST(List<Edge> edges, int roomCount)
    {
        List<Edge> mst = new List<Edge>();
        DisjointSet ds = new DisjointSet(roomCount);

        edges = edges.OrderBy(edge => edge.weight).ToList();

        foreach (Edge edge in edges)
        {
            int idA = edge.roomA.id;
            int idB = edge.roomB.id;

            if (ds.Find(idA) != ds.Find(idB))
            {
                ds.Union(idA, idB);
                mst.Add(edge);
            }

            if (mst.Count >= roomCount - 1)
                break;
        }

        return mst;
    }

    // 廊下の生成
    private void CreateCorridor(Room roomA, Room roomB, bool ignoreConstraints = false, bool isSecret = false)
    {
        // 部屋の境界ボックスを取得
        RectInt rectA = new RectInt(roomA.x, roomA.y, roomA.width, roomA.height);
        RectInt rectB = new RectInt(roomB.x, roomB.y, roomB.width, roomB.height);

        // 部屋が隣接または重なっている場合
        if (rectA.Overlaps(rectB) || IsAdjacent(rectA, rectB))
        {
            Vector2Int connectionPoint = GetAdjacentPoint(rectA, rectB);
            Corridor corridor = new Corridor(connectionPoint, connectionPoint, corridorWidth, 0, isSecret); // 曲がり角なし
            corridor.roomA = roomA;
            corridor.roomB = roomB;
            corridors.Add(corridor);
            corridor.GenerateTiles();
            return;
        }

        // 部屋間の距離を計算
        float distance = Vector2.Distance(roomA.GetCenterFloat(), roomB.GetCenterFloat());

        // 最大接続距離を超える場合は接続しない（制約を無視する場合を除く）
        if (!ignoreConstraints && distance > maxConnectionDistance)
        {
            Debug.Log($"Skipping connection between Room {roomA.id} and Room {roomB.id} due to distance.");
            return;
        }

        // 出入り口の候補を取得
        List<Vector2Int> entrancesA = roomA.GetEntrancePositions();
        List<Vector2Int> entrancesB = roomB.GetEntrancePositions();

        // 最短距離の組み合わせを探す
        float minDistance = float.MaxValue;
        Vector2Int bestEntranceA = Vector2Int.zero;
        Vector2Int bestEntranceB = Vector2Int.zero;

        foreach (var entranceA in entrancesA)
        {
            foreach (var entranceB in entrancesB)
            {
                float currentDistance = Vector2Int.Distance(entranceA, entranceB);
                if (currentDistance < minDistance)
                {
                    minDistance = currentDistance;
                    bestEntranceA = entranceA;
                    bestEntranceB = entranceB;
                }
            }
        }

        // 出入り口が見つからない場合、部屋の中心を使用
        if (minDistance == float.MaxValue)
        {
            bestEntranceA = roomA.GetCenterInt();
            bestEntranceB = roomB.GetCenterInt();
            minDistance = Vector2Int.Distance(bestEntranceA, bestEntranceB);
        }

        // 曲がり角の数を計算
        int bendCount = CalculateBendCount(bestEntranceA, bestEntranceB);

        // 最大曲がり角数を超える場合は接続しない（制約を無視する場合を除く）
        if (!ignoreConstraints && bendCount > maxBendCount)
        {
            Debug.Log($"Skipping connection between Room {roomA.id} and Room {roomB.id} due to bend count.");
            return;
        }

        // 廊下を生成
        Corridor newCorridor = new Corridor(bestEntranceA, bestEntranceB, corridorWidth, bendCount, isSecret);
        newCorridor.roomA = roomA;
        newCorridor.roomB = roomB;
        corridors.Add(newCorridor);

        // 廊下のタイルデータを生成
        newCorridor.GenerateTiles();
    }

    // Entrance と Exit 部屋の接続を確認し、接続されていなければ接続する
    private void EnsureEntranceExitConnection()
    {
        // Entrance と Exit 部屋を取得
        Room entranceRoom = rooms.FirstOrDefault(room => room.roomType == RoomType.Entrance);
        Room exitRoom = rooms.FirstOrDefault(room => room.roomType == RoomType.Exit);

        if (entranceRoom == null || exitRoom == null)
        {
            Debug.LogError("Entrance or Exit room not found.");
            return;
        }

        // 既に接続されているかを確認
        if (AreRoomsConnected(entranceRoom, exitRoom))
        {
            Debug.Log("Entrance and Exit rooms are already connected.");
            return;
        }

        // 最大接続距離を無視してエッジを作成
        Edge entranceExitEdge = new Edge(entranceRoom, exitRoom);

        // 廊下を生成
        CreateCorridor(entranceRoom, exitRoom, ignoreConstraints: true);
        Debug.Log("Entrance and Exit rooms have been connected.");
    }

    // 部屋間の接続を確認する（深さ優先探索）
    private bool AreRoomsConnected(Room roomA, Room roomB)
    {
        HashSet<Room> visited = new HashSet<Room>();
        Stack<Room> stack = new Stack<Room>();
        stack.Push(roomA);

        while (stack.Count > 0)
        {
            Room current = stack.Pop();
            if (current == roomB)
                return true;

            visited.Add(current);

            List<Room> connectedRooms = GetConnectedRooms(current);
            foreach (Room neighbor in connectedRooms)
            {
                if (!visited.Contains(neighbor))
                {
                    stack.Push(neighbor);
                }
            }
        }

        return false;
    }

    // ある部屋に直接接続している部屋を取得
    private List<Room> GetConnectedRooms(Room room)
    {
        List<Room> connectedRooms = new List<Room>();

        foreach (Corridor corridor in corridors)
        {
            if (corridor.roomA == room)
                connectedRooms.Add(corridor.roomB);
            else if (corridor.roomB == room)
                connectedRooms.Add(corridor.roomA);
        }

        return connectedRooms;
    }

    // 曲がり角の数を計算
    private int CalculateBendCount(Vector2Int start, Vector2Int end)
    {
        // シンプルな L 字型の廊下では曲がり角は 1 つ
        // 始点と終点が同じ X または Y 座標を持つ場合、曲がり角は 0
        if (start.x == end.x || start.y == end.y)
            return 0;
        else
            return 1;
    }

    // 部屋が隣接しているかを判定
    private bool IsAdjacent(RectInt rectA, RectInt rectB)
    {
        // 上下左右に隣接しているかを判定
        return (rectA.xMax == rectB.xMin || rectA.xMin == rectB.xMax) && (rectA.yMin < rectB.yMax && rectA.yMax > rectB.yMin) ||
               (rectA.yMax == rectB.yMin || rectA.yMin == rectB.yMax) && (rectA.xMin < rectB.xMax && rectA.xMax > rectB.xMin);
    }

    // 隣接している辺の中点を取得
    private Vector2Int GetAdjacentPoint(RectInt rectA, RectInt rectB)
    {
        int x = Mathf.Max(rectA.xMin, rectB.xMin);
        x = Mathf.Min(x, rectA.xMax - 1, rectB.xMax - 1);

        int y = Mathf.Max(rectA.yMin, rectB.yMin);
        y = Mathf.Min(y, rectA.yMax - 1, rectB.yMax - 1);

        return new Vector2Int(x, y);
    }
}