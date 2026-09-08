using System.Collections.Generic;
using UnityEngine;

public class ProximityIconManager : MonoBehaviour
{
    public static ProximityIconManager Instance;

    [SerializeField] private Transform player;
    [SerializeField] private int checksPerFrame = 10; // opsional: batasi berapa objek dicek per frame (biar makin ringan)

    private List<ProximityIcon> allIcons = new List<ProximityIcon>();
    private int currentIndex = 0;

    private void Awake()
    {
        Instance = this;
        ResolvePlayer();
    }

    private void Update()
    {
        ResolvePlayer();

        if (player == null || allIcons.Count == 0) return;

        for (int i = 0; i < allIcons.Count; i++)
        {
            if (allIcons[i] != null)
                allIcons[i].UpdateVisibility(player.position);
        }
    }

    private void ResolvePlayer()
    {
        if (player != null) return;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    public void Register(ProximityIcon icon)
    {
        if (!allIcons.Contains(icon))
            allIcons.Add(icon);
    }

    public void Unregister(ProximityIcon icon)
    {
        allIcons.Remove(icon);
    }

}