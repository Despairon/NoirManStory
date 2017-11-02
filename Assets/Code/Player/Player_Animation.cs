using System.Collections.Generic;

public partial class Player
{
    #region private_members

    private enum PlayerAnimation
    {
        NONE,
        IDLE,
        WALKING,
        USING
    }

    private PlayerAnimation currentAnimation;

    private Dictionary<PlayerAnimation, string> playerAnimationMap;

    private void fillAnimationMap()
    {
        playerAnimationMap.Add( PlayerAnimation.IDLE    , "IdleAnimSet"    );
        playerAnimationMap.Add( PlayerAnimation.WALKING , "WalkingAnimSet" );
        playerAnimationMap.Add( PlayerAnimation.USING   , "UsingAnimSet"   );
    }

    private void playAnim(PlayerAnimation animation)
    {
        if (currentAnimation != animation)
        {
            try
            {
                var animControlTrigger = playerAnimationMap[animation];

                if (animControlTrigger != null)
                {
                    animator.SetTrigger(animControlTrigger);

                    currentAnimation = animation;
                }
            }
            catch (KeyNotFoundException)
            {
                // do nothing
            }
        }
    }

    #endregion
}
