using Godot;
using System;
using System.Collections.Generic;

namespace Mars_Seal_Crimson
{
    public class SpriteAdvocate
    {
        //typedef List<Sprite3D> ListSprites;
        private Dictionary<string, List<Sprite3D>> spriteOrganisationMap = new Dictionary<string, List<Sprite3D>>();

        public SpriteAdvocate()
        {
        }

        public void AddGroup(string groupDescriptor, List<Sprite3D> spritesList)
        {
            if ( (groupDescriptor != null) && (spritesList != null))
            {

            }
        }

        //List<AnimationAction> -- geometric operations, colour adjustment ...
        public void SetActionsOnGroup(string groupDescriptor)
        {
            if ( (spriteOrganisationMap != null) && (spriteOrganisationMap.ContainsKey(groupDescriptor)))
            {

            }
        }

        public void AnimatePopupSprite(string groupDescriptor, string spriteName)
        {
            if ((spriteOrganisationMap != null) && (spriteOrganisationMap.ContainsKey(groupDescriptor)))
            {

            }
        }
    }
}
