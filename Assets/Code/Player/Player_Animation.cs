using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class Player : MonoBehaviour
{
    #region private_members

    private enum PlayerAnimation
    {
        NONE,
        IDLE,
        WALKING,
        USING
    }

    private sealed class PlayerAnimationMapNode
    {
        public PlayerAnimationMapNode(string animationName, PlayerAnimation anim, float animationSpeed)
        {
            this.animationName  = animationName;
            this.anim           = anim;
            this.animationSpeed = animationSpeed;
        }

        public readonly string          animationName;
        public readonly PlayerAnimation anim;    
        public readonly float           animationSpeed;  
    }

    private List<PlayerAnimationMapNode> playerAnimationMap;

    private void fillAnimationMap()
    {
        playerAnimationMap.Add(new PlayerAnimationMapNode("PlayerIdle",  PlayerAnimation.IDLE,    0.75f));
        playerAnimationMap.Add(new PlayerAnimationMapNode("walk",        PlayerAnimation.WALKING, 2.0f));
        playerAnimationMap.Add(new PlayerAnimationMapNode("PlayerUsing", PlayerAnimation.USING,   1.5f));
    }

    private bool isAnimActive(PlayerAnimation anim)
    {
        var _anim = playerAnimationMap.Find(node => node.anim == anim);

        if (_anim != null)
            return animationComponent.IsPlaying(_anim.animationName);
        else
            return false;
    }

    private void playAnim(PlayerAnimation anim)
    {
        var _anim = playerAnimationMap.Find(node => node.anim == anim);

        if (_anim != null)
        {
            animationComponent[_anim.animationName].speed = _anim.animationSpeed;

            animationComponent.CrossFade(_anim.animationName); // TODO: fix!!! maybe rework animation manager
        }
    }

    private void stopAnim(PlayerAnimation anim)
    {
        var _anim = playerAnimationMap.Find(node => node.anim == anim);

        if (_anim != null)
        {
            animationComponent.Stop(_anim.animationName);
        }
    }

    #endregion
}
