using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatToWalk : Trigger
{
    [SerializeField] private GameObject _menuScreen;

    protected override void Action(GameObject player){
        GameManager.Instance.NextGameState();
        player.transform.Find("PlayerCamera").gameObject.SetActive(true);
        _menuScreen.SetActive(false);
        base.Deactivate();
    }
}
