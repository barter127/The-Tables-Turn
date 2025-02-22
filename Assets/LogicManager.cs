using UnityEngine;
using TMPro;

public class LogicManager : MonoBehaviour
{
    public bool enemyIsAttacking = false; //Only lets one enemy attack at a time.

    [SerializeField] private TMP_Text endGameText;

    [SerializeField] private CameraDollyMovement cam;

    //Stores value of amount of enemies in each wave.
    [SerializeField] private int[] enemyWaveNumber;
    public int currentWave;
    private bool waveIncremented;
    public static int alertEnemies; //enemies that aren't stationary.

    public void LoseGame()
    {
        endGameText.text = ("You Suck!!!");
        Time.timeScale = 0;
    }

    public void WinGame()
    {
        endGameText.text = ("K.O");
        Time.timeScale = 0;
    }

    void Update()
    {
        if (alertEnemies == enemyWaveNumber[currentWave])
        {
            cam.lockCamera = true;

            if (!waveIncremented) //Increment currentWave only once per wave
            {
                currentWave++;
                waveIncremented = true;
            }
        }
        else if (alertEnemies == 0)
        {
            cam.lockCamera = false;
            waveIncremented = false; //false when no enemies alert
        }
        if (alertEnemies == 0 && currentWave == 3)
        {
            WinGame();
        }
    }
}
