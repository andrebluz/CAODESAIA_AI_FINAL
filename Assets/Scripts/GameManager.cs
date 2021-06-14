using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum enemyState
{
    IDLE, ALERT, PATROL, FOLLOW, FURY
}

public class GameManager : MonoBehaviour
{    // pegando player
    public Transform player;
    // ia do npc, variaves de pontos onde o npc vai patrulhar, e a distacia do ataque, tempo de alerta, tempo de ataque pra ataque, e a velocidade que segui olhando o player
    [Header("ENEMY IA")]
    public float slimeIdleWaitTime;
    public Transform[] slimeWayPoints;
    public float slimeDistanceToAttack = 2.3f;
    public float slimeAlertTime = 3f;
    public float slimeAttackDelay = 1f;
    public float slimeLookAtSpeed = 1f;
}
