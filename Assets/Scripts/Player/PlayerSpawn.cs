using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Centralized player spawn handler.
/// Attach one PlayerSpawn object per scene (or a persistent manager) and set
/// the `spawnPoint` in the Inspector. On scene load, this script will place
/// the Player at the configured spawn unless the load was an explicit 'Load Game'.
/// </summary>
public class PlayerSpawn : MonoBehaviour
{
    [Tooltip("Optional transform to use as player spawn location. If null, script will try to find a GameObject tagged 'PlayerSpawnPoint' or named 'PlayerSpawn'.")]
    public Transform spawnPoint;

    /// <summary>
    /// Set the spawn position programmatically. If no spawnPoint Transform exists,
    /// this creates one as a child of this GameObject.
    /// </summary>
    public void SetSpawnPosition(Vector3 pos)
    {
        if (spawnPoint == null)
        {
            var go = new GameObject("PlayerSpawnPoint");
            go.transform.SetParent(this.transform);
            spawnPoint = go.transform;
        }
        spawnPoint.position = pos;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Defer by a couple frames so DataPersistenceManager and other initializers run first
        StartCoroutine(PlacePlayerNextFrame());
    }

    private IEnumerator PlacePlayerNextFrame()
    {
        // wait a frame or two to allow other sceneLoaded handlers to run (DataPersistenceManager, etc.)
        yield return null;
        yield return null;

        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogWarning("PlayerSpawn: no GameObject with tag 'Player' found in scene. Skipping spawn.");
            yield break;
        }

        var dpm = DataPersistenceManager.instance;
        bool lastWasLoad = dpm != null && dpm.LastLoadFromMenu;

        if (lastWasLoad)
        {
            // If the last scene load was a manual Load Game, assume Player.LoadData
            // already restored the saved position. Clear the flag and skip.
            if (dpm != null)
                dpm.LastLoadFromMenu = false;
            yield break;
        }

        Transform spawn = spawnPoint;
        if (spawn == null)
        {
            var go = GameObject.FindWithTag("PlayerSpawnPoint");
            if (go != null) spawn = go.transform;
            else
            {
                var go2 = GameObject.Find("PlayerSpawn");
                if (go2 != null) spawn = go2.transform;
            }
        }

        if (spawn != null)
        {
            playerObj.transform.position = spawn.position;
            playerObj.transform.rotation = spawn.rotation;
        }
        else
        {
            // Fallback: if the DataPersistenceManager has a currentGameData with a position,
            // we can use it as a default spawn for new games (but skip if it was a Load Game).
            if (dpm != null && dpm.CurrentGameData != null)
            {
                playerObj.transform.position = dpm.CurrentGameData.playerPosition;
            }
            else
            {
                Debug.Log("PlayerSpawn: no spawn configured; player left at current location.");
            }
        }
    }
}
