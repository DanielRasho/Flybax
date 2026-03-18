using UnityEngine;

/// <summary>
/// Attach this script to an empty GameObject in the scene.
/// Press Play and the full classroom will be generated automatically.
/// </summary>
public class ClassroomBuilder : MonoBehaviour
{
    // ── Room dimensions ──────────────────────────────────────────────
    [Header("Room Size")]
    public float roomWidth = 12f;
    public float roomLength = 16f;
    public float roomHeight = 4f;

    // ── Desk grid ────────────────────────────────────────────────────
    [Header("Desk Layout")]
    public int deskColumns = 5;
    public int deskRows = 6;          // 30 desks total
    public float deskSpacingX = 1.8f;
    public float deskSpacingZ = 2.2f;

    // ── Player seat (0-indexed, counted from back-left) ──────────────
    [Header("Player Position")]
    public int playerCol = 4;              // rightmost column, back row
    public int playerRow = 5;

    // ── Prefabs ───────────────────────────────────────────────────────
    [Header("Prefabs")]
    public GameObject studentPrefab;   // arrastra Assets/Prefabs/Student aquí

    // ── Materials (auto-created if null) ─────────────────────────────
    private Material matFloor, matWall, matCeiling;
    private Material matDesk, matChair;
    private Material matStudent, matPlayer, matTeacher;
    private Material matBoard, matBoardSurface;

    void Awake()
    {
        CreateMaterials();
        BuildRoom();
        BuildBlackboard();
        BuildDeskGrid();
        PlaceTeacher();
    }

    // ─────────────────────────────────────────────────────────────────
    //  MATERIALS
    // ─────────────────────────────────────────────────────────────────
    void CreateMaterials()
    {
        matFloor = MakeMat(new Color(0.55f, 0.42f, 0.30f));   // wood-ish
        matWall = MakeMat(new Color(0.92f, 0.90f, 0.85f));   // off-white
        matCeiling = MakeMat(new Color(0.97f, 0.97f, 0.95f));   // near-white
        matDesk = MakeMat(new Color(0.70f, 0.55f, 0.35f));   // light wood
        matChair = MakeMat(new Color(0.20f, 0.30f, 0.55f));  // dark blue
        matStudent = MakeMat(new Color(0.30f, 0.55f, 0.30f));   // green uniform
        matPlayer = MakeMat(new Color(0.95f, 0.80f, 0.20f));   // yellow = YOU
        matTeacher = MakeMat(new Color(0.75f, 0.20f, 0.20f));   // red = teacher
        matBoard = MakeMat(new Color(0.25f, 0.20f, 0.15f));   // dark frame
        matBoardSurface = MakeMat(new Color(0.08f, 0.30f, 0.15f));   // chalkboard green
    }

    Material MakeMat(Color color)
    {
        var m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        m.color = color;
        return m;
    }

    // ─────────────────────────────────────────────────────────────────
    //  ROOM (floor, ceiling, 4 walls)
    // ─────────────────────────────────────────────────────────────────
    void BuildRoom()
    {
        float w = roomWidth;
        float l = roomLength;
        float h = roomHeight;

        // Floor
        CreateBox("Floor", new Vector3(0, 0, 0),
                  new Vector3(w, 0.1f, l), matFloor);

        // Ceiling
        CreateBox("Ceiling", new Vector3(0, h, 0),
                  new Vector3(w, 0.1f, l), matCeiling);

        // Front wall (blackboard side)
        CreateBox("Wall_Front", new Vector3(0, h / 2, l / 2),
                  new Vector3(w, h, 0.2f), matWall);

        // Back wall
        CreateBox("Wall_Back", new Vector3(0, h / 2, -l / 2),
                  new Vector3(w, h, 0.2f), matWall);

        // Left wall
        CreateBox("Wall_Left", new Vector3(-w / 2, h / 2, 0),
                  new Vector3(0.2f, h, l), matWall);

        // Right wall
        CreateBox("Wall_Right", new Vector3(w / 2, h / 2, 0),
                  new Vector3(0.2f, h, l), matWall);
    }

    // ─────────────────────────────────────────────────────────────────
    //  BLACKBOARD
    // ─────────────────────────────────────────────────────────────────
    void BuildBlackboard()
    {
        float frontZ = roomLength / 2 - 0.12f;

        // Frame
        CreateBox("Board_Frame", new Vector3(0, 2.0f, frontZ),
                  new Vector3(6f, 1.8f, 0.05f), matBoard);

        // Chalkboard surface
        CreateBox("Board_Surface", new Vector3(0, 2.0f, frontZ - 0.04f),
                  new Vector3(5.6f, 1.4f, 0.05f), matBoardSurface);

        // Chalk tray
        CreateBox("Board_Tray", new Vector3(0, 1.20f, frontZ - 0.05f),
                  new Vector3(5.6f, 0.08f, 0.12f), matBoard);
    }

    // ─────────────────────────────────────────────────────────────────
    //  DESK GRID
    // ─────────────────────────────────────────────────────────────────
    void BuildDeskGrid()
    {
        // Calculate starting position so the grid is centred on X
        // and starts a bit behind the teacher area on Z
        float startX = -((deskColumns - 1) * deskSpacingX) / 2f;
        float startZ = -roomLength / 2f + 2.5f;   // leave room at back wall

        for (int row = 0; row < deskRows; row++)
        {
            for (int col = 0; col < deskColumns; col++)
            {
                float x = startX + col * deskSpacingX;
                float z = startZ + row * deskSpacingZ;

                bool isPlayer = (col == playerCol && row == playerRow);
                BuildDeskAndStudent(x, z, col, row, isPlayer);
            }
        }
    }

    void BuildDeskAndStudent(float x, float z, int col, int row, bool isPlayer)
    {
        string id = $"R{row}C{col}";

        // ── Desk top ─────────────────────────────────────────────────
        CreateBox($"DeskTop_{id}",
                  new Vector3(x, 0.75f, z),
                  new Vector3(0.80f, 0.05f, 0.55f), matDesk);

        // ── Desk legs (4 corners) ─────────────────────────────────────
        float lx = 0.35f; float lz = 0.22f;
        Vector3[] corners = {
            new Vector3(x - lx, 0.375f, z - lz),
            new Vector3(x + lx, 0.375f, z - lz),
            new Vector3(x - lx, 0.375f, z + lz),
            new Vector3(x + lx, 0.375f, z + lz),
        };
        foreach (var c in corners)
            CreateBox($"DeskLeg_{id}", c, new Vector3(0.05f, 0.75f, 0.05f), matDesk);

        // ── Chair seat ───────────────────────────────────────────────
        float chairZ = z - 0.45f;   // chair pulled slightly out behind desk
        CreateBox($"ChairSeat_{id}",
                  new Vector3(x, 0.48f, chairZ),
                  new Vector3(0.42f, 0.04f, 0.40f), matChair);

        // Chair back
        CreateBox($"ChairBack_{id}",
                  new Vector3(x, 0.72f, chairZ - 0.18f),
                  new Vector3(0.42f, 0.48f, 0.04f), matChair);

        // Chair legs
        Vector3[] cLegs = {
            new Vector3(x - 0.17f, 0.24f, chairZ - 0.16f),
            new Vector3(x + 0.17f, 0.24f, chairZ - 0.16f),
            new Vector3(x - 0.17f, 0.24f, chairZ + 0.16f),
            new Vector3(x + 0.17f, 0.24f, chairZ + 0.16f),
        };
        foreach (var cl in cLegs)
            CreateBox($"CLeg_{id}", cl, new Vector3(0.04f, 0.48f, 0.04f), matChair);

        // ── Student (Prefab) ──────────────────────────────────────────
        if (!isPlayer)
        {
            if (studentPrefab != null)
            {
                var student = Instantiate(studentPrefab);
                student.name = $"Student_{id}";
                student.transform.position = new Vector3(x, 1.30f, chairZ);
                // Aplicar color verde al body (primer renderer del prefab)
                var renderers = student.GetComponentsInChildren<Renderer>();
                foreach (var r in renderers)
                    r.material = matStudent;
            }
            else
            {
                // Fallback si no hay prefab asignado
                var student = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                student.name = $"Student_{id}";
                student.transform.position = new Vector3(x, 1.30f, chairZ);
                student.transform.localScale = new Vector3(0.35f, 0.45f, 0.35f);
                student.GetComponent<Renderer>().material = matStudent;

                var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                head.name = $"Head_{id}";
                head.transform.position = new Vector3(x, 1.85f, chairZ);
                head.transform.localScale = Vector3.one * 0.22f;
                head.GetComponent<Renderer>().material = matStudent;
            }
        }
        else
        {
            // Player marker — just a small yellow cube so you can spot the seat
            var marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marker.name = "PlayerSeat_Marker";
            marker.transform.position = new Vector3(x, 0.82f, z);
            marker.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            marker.GetComponent<Renderer>().material = matPlayer;

            // Tag this position so FPSCamera can find it
            marker.tag = "PlayerSeat";
        }
    }

    // ─────────────────────────────────────────────────────────────────
    //  TEACHER
    // ─────────────────────────────────────────────────────────────────
    void PlaceTeacher()
    {
        float frontZ = roomLength / 2f - 1.5f;   // near the blackboard

        // Body
        var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = "Teacher_Body";
        body.transform.position = new Vector3(-1.5f, 1.20f, frontZ);
        body.transform.localScale = new Vector3(0.40f, 0.55f, 0.40f);
        body.GetComponent<Renderer>().material = matTeacher;

        // Head
        var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.name = "Teacher_Head";
        head.transform.position = new Vector3(-1.5f, 2.05f, frontZ);
        head.transform.localScale = Vector3.one * 0.28f;
        head.GetComponent<Renderer>().material = matTeacher;

        // Teacher's desk (at the front)
        CreateBox("TeacherDesk",
                  new Vector3(0, 0.75f, roomLength / 2f - 2.5f),
                  new Vector3(1.6f, 0.06f, 0.7f), matDesk);
    }

    // ─────────────────────────────────────────────────────────────────
    //  HELPERS
    // ─────────────────────────────────────────────────────────────────
    void CreateBox(string name, Vector3 position, Vector3 size, Material mat)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.position = position;
        go.transform.localScale = size;
        go.GetComponent<Renderer>().material = mat;
    }
}