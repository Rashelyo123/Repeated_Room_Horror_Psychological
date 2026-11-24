using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public class CameraLogger : MonoBehaviour
{
    [Header("Referensi")]
    public Camera targetCamera;
    public Transform player;            // objek pemain, untuk hitung kecepatan

    [Header("Ambang kecepatan (m/s), sesuaikan dengan game-mu")]
    public float walkThreshold = 0.1f;  // di bawah ini dianggap idle
    public float runThreshold = 4.5f;   // di atas ini dianggap run

    [Header("Kontrol")]
    public KeyCode toggleKey = KeyCode.F9;   // mulai/berhenti rekam

    bool recording;
    float t0;
    Vector3 lastPos;
    string pendingEvent = "";
    readonly StringBuilder sb = new StringBuilder();

    void Start()
    {
        if (!targetCamera) targetCamera = Camera.main;
        if (player) lastPos = player.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (recording) StopAndSave(); else Begin();
        }
    }

    // Panggil dari event-mu agar muncul penanda di grafik shake,
    // misalnya CameraLogger.Mark("hit_kapak") atau Mark("jumpscare")
    public void Mark(string label) { if (recording) pendingEvent = label; }

    void Begin()
    {
        sb.Clear();
        sb.AppendLine("time,fov,state,speed,pos_x,pos_y,pos_z,rot_x,rot_y,rot_z,event");
        t0 = Time.time;
        lastPos = player.position;
        recording = true;
        Debug.Log("CameraLogger: mulai merekam");
    }

    void LateUpdate()   // LateUpdate supaya nilai kamera sudah final (head bob/shake sudah diterapkan)
    {
        if (!recording) return;
        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        Vector3 d = player.position - lastPos;
        d.y = 0f;
        float speed = d.magnitude / dt;
        lastPos = player.position;

        string state = speed < walkThreshold ? "idle" : (speed < runThreshold ? "walk" : "run");

        Transform c = targetCamera.transform;
        Vector3 p = c.localPosition;
        Vector3 r = c.localEulerAngles;

        sb.AppendLine(string.Join(",",
            F(Time.time - t0), F(targetCamera.fieldOfView), state, F(speed),
            F(p.x), F(p.y), F(p.z),
            F(Wrap(r.x)), F(Wrap(r.y)), F(Wrap(r.z)),
            pendingEvent));
        pendingEvent = "";
    }

   void StopAndSave()
{
    recording = false;

    // Application.dataPath = .../NamaProject/Assets, jadi ".." naik satu level ke root project
    string folder = Path.Combine(Application.dataPath, "..", "CameraLogs");
    Directory.CreateDirectory(folder);   // dibuat otomatis kalau belum ada

    string path = Path.GetFullPath(Path.Combine(folder,
        "camera_log_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv"));

    File.WriteAllText(path, sb.ToString());
    Debug.Log("CameraLogger: tersimpan di " + path);
}
    static string F(float v) => v.ToString("F4", CultureInfo.InvariantCulture);
    static float Wrap(float a) => a > 180f ? a - 360f : a;
}