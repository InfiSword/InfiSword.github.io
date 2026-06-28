using UnityEngine;
using System.Collections;
using System;

public class Skill_Pong : Skill_BossMain
{
    public enum PongState { Ready, Play, Destroyed }
    private PongState currentState = PongState.Ready;

    [Header("Components")]
    public PongWindow pongWindow;
    public Transform bossHead;

    [Header("Settings")]
    public float patternDuration = 30f;
    private float elapsedTime = 0f;
    private int spawnedBallsCount = 0;

    [Header("Progressive Difficulty")]
    public float speedMultiplierPerHit = 1.05f; // 5% speed increase per hit
    public float damageIncreasePerHit = 25f;   // +25 damage per hit
    public float maxBallSpeed = 15f;            // Maximum speed cap

    public override void StartPattern()
    {
        if (isRunning) return;
        isRunning = true;
        SetState(PongState.Ready);
    }

    private void Update()
    {
        if (isRunning && currentState == PongState.Play)
        {
            UpdatePattern();
        }
    }

    public void SetState(PongState newState)
    {
        currentState = newState;
        switch (currentState)
        {
            case PongState.Ready:
                StartCoroutine(SequenceReady());
                break;
            case PongState.Play:
                elapsedTime = 0f;
                spawnedBallsCount = 0;
                break;
            case PongState.Destroyed:
                FinishPattern();
                Cleanup();
                break;
        }
    }

    private IEnumerator SequenceReady()
    {
        pongWindow.gameObject.SetActive(true);
        pongWindow.Init();

        if (pongWindow.LeftPaddle != null)
            pongWindow.LeftPaddle.increaseAtkAndSpd_HitBall += HandleBallHitPaddle;

        if (pongWindow.RightPaddle != null)
            pongWindow.RightPaddle.increaseAtkAndSpd_HitBall += HandleBallHitPaddle;


        yield return new WaitForSeconds(1.0f);
        SetState(PongState.Play);
    }

    private void HandleBallHitPaddle(PongBall ball)
    {
        if (ball == null) return;

        ball.IncreaseSpeed(speedMultiplierPerHit, maxBallSpeed);
        ball.IncreaseDamage(damageIncreasePerHit);

        Debug.Log($"[Skill_Pong] Ball Hit by Paddle! New Speed: {ball.currentSpeed}, New Atk: {ball.atk}");
    }

    private void UpdatePattern()
    {
        elapsedTime += Time.deltaTime;

        if (spawnedBallsCount == 0)
        {
            pongWindow.SpawnBall();
            spawnedBallsCount = 1;
        }

        if (elapsedTime >= patternDuration)
        {
            SetState(PongState.Destroyed);
        }
    }

    public override void StopPattern()
    {
        base.StopPattern();
        SetState(PongState.Destroyed);
    }

    private void Cleanup()
    {
        if (pongWindow != null)
        {
            if (pongWindow.LeftPaddle != null)
                pongWindow.LeftPaddle.increaseAtkAndSpd_HitBall -= HandleBallHitPaddle;
            if (pongWindow.RightPaddle != null)
                pongWindow.RightPaddle.increaseAtkAndSpd_HitBall -= HandleBallHitPaddle;

            pongWindow.ClearBalls();
            pongWindow.gameObject.SetActive(false);
        }
    }
}
